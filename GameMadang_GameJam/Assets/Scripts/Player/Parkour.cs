using System.Collections;
using UnityEngine;

namespace Player
{
    public class Parkour : MonoBehaviour
    {
        private static readonly int IsParkour1 = Animator.StringToHash("IsParkour");
        [SerializeField] private LayerMask parkourLayer;

        [SerializeField] private float parkourSpeed = 5f;
        [SerializeField] private float rayLength = 0.8f;
        [SerializeField] private float epsilon = 0.001f;

        [SerializeField] private float stepForwardDist = 0.35f;
        [SerializeField] private float stepForwardSpeed = 6f;
        private Animator animator;
        private bool busy;
        private Collider2D col;

        private Rigidbody2D rb;
        private SpriteRenderer sr;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        public Collider2D IsParkour()
        {
            var dir = Vector2.up + Vector2.right * (sr.flipX ? 1 : -1);
            var hit = Physics2D.Raycast(transform.position, dir.normalized, rayLength,
                parkourLayer);
            Debug.DrawRay(rb.position, dir.normalized * rayLength, Color.red);

            return hit.collider;
        }

        public void StartParkour(Collider2D wall)
        {
            if (busy) return;

            if (wall) StartCoroutine(ParkourCo(wall));
        }

        private IEnumerator ParkourCo(Collider2D wall)
        {
            busy = true;

            col.isTrigger = true;

            var savedG = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;

            var targetTopY = wall.bounds.max.y;

            animator.SetTrigger(IsParkour1);

            // 앞으로 나갈 방향(+1: 오른쪽, -1: 왼쪽)
            var sideDir = Mathf.Sign(wall.bounds.center.x - rb.position.x);
            if (sideDir == 0) sideDir = 1f; // 동점 방지

            // 시작 X, 최종 X, 전체 올라가야 할 높이
            var startX = rb.position.x;
            var upDistance = Mathf.Max(0f, targetTopY - col.bounds.min.y);
            var targetX = startX + sideDir * stepForwardDist;

            // 올라가면서 X를 분배할 비율 (총 높이 대비 총 X이동)
            var xPerY = upDistance > 0f ? stepForwardDist / upDistance : 0f;

            // ⬆️ 올라가면서 ➡️ X를 함께 이동
            while (col.bounds.min.y < targetTopY - epsilon)
            {
                var needY = targetTopY - col.bounds.min.y;
                var stepY = Mathf.Min(needY, parkourSpeed * Time.fixedDeltaTime);

                var stepX = sideDir * xPerY * stepY; // 이번 프레임 X 분배

                rb.MovePosition(new Vector2(rb.position.x + stepX, rb.position.y + stepY));
                yield return new WaitForFixedUpdate();
            }

            // 최종 스냅(잔오차 보정: Y는 꼭대기 중앙, X는 목표)
            var finalCenterY = targetTopY + col.bounds.extents.y;
            rb.MovePosition(new Vector2(targetX, finalCenterY));

            rb.gravityScale = savedG;
            busy = false;
            col.isTrigger = false;
        }

        public bool IsBusy()
        {
            return busy;
        }
    }
}