using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class Parkour : MonoBehaviour
    {
        [SerializeField] private LayerMask parkourLayer;

        [SerializeField] private float parkourSpeed =5f;
        [SerializeField] private float rayLength = 0.8f;
        [SerializeField] private float epsilon = 0.001f;

        [SerializeField] private float stepForwardDist  = 0.35f;
        [SerializeField] private float stepForwardSpeed = 6f;

        private Rigidbody2D rb;
        private Collider2D col;
        private SpriteRenderer sr;
        private bool busy;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        public Collider2D IsParkour()
        {
            var dir = Vector2.up + Vector2.right * (sr.flipX ? 1 : -1);
            var hit = Physics2D.Raycast(transform.position, dir.normalized, rayLength, parkourLayer);
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

            var savedG = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;

            var targetTopY = wall.bounds.max.y;

            while (col.bounds.min.y < targetTopY - epsilon)
            {
                var need = targetTopY - col.bounds.min.y;
                var step = Mathf.Min(need, parkourSpeed * Time.fixedDeltaTime);
                rb.MovePosition(rb.position + Vector2.up * step);

                yield return new WaitForFixedUpdate();
            }

            var finalCenterY = targetTopY + col.bounds.extents.y;
            rb.MovePosition(new Vector2(rb.position.x, finalCenterY));

            var sideDir = Mathf.Sign(wall.bounds.center.x - rb.position.x);
            var targetX = rb.position.x + sideDir * stepForwardDist;

            while (Mathf.Abs(rb.position.x - targetX) > epsilon)
            {
                var dx = sideDir * stepForwardSpeed * Time.fixedDeltaTime;
                var nextX = Mathf.MoveTowards(rb.position.x, targetX, Mathf.Abs(dx));
                rb.MovePosition(new Vector2(nextX, rb.position.y));
                yield return new WaitForFixedUpdate();
            }

            rb.MovePosition(new Vector2(targetX, rb.position.y));
            rb.gravityScale = savedG;
            busy = false;
        }

        public bool IsBusy() => busy;
    }
}