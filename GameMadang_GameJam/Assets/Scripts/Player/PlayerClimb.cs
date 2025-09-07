using System.Collections;
using Interact;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerClimb : MonoBehaviour
    {
        [SerializeField] private LayerMask interact;
        [SerializeField] private Transform footPoint;
        [SerializeField] private float climbJumpForce = 5f;
        [SerializeField] private float climbSpeed = 10f;
        [SerializeField] private float climbObjectSnapSpeed = 30f;

        [SerializeField] private float entryMargin = 0.2f;
        [SerializeField] private float endMargin = 0.1f;
        [SerializeField] private float exitNudge = 0.05f;
        [SerializeField] private float endGraceTime = 0.15f;
        [SerializeField] private float edgeSafety = 0.05f;

        [SerializeField] private float pumpImpulse = 4f; // 좌/우 펌핑 임펄스(질량 스케일)
        [SerializeField] private float maxSwingXSpeed = 14f; // 속도 캡
        [SerializeField] private float maxSwingYSpeed = 18f;

        [SerializeField] private string ladderClimbState = "Climb";
        [SerializeField] private AnimationClip ladderClimbClip;
        [SerializeField] private string ropeClimbState = "Rope";
        [SerializeField] private AnimationClip ropeClimbClip;
        [SerializeField] private string ropeSwingState = "Swing";

        [SerializeField] private float frameStepInterval = 0.08f;

        public BaseInteractable currentClimbable;
        public RopeInteractable ropeSeg;

        private AnimationClip activeClip;
        private int activeStateHash;
        private Animator animator;
        private Collider2D col;

        private bool endArmed;
        private float endBlockUntil;
        private int entryKey;

        private InputHandler inputHandler;
        private float nextStepAt;
        private float originalGravity;

        private Rigidbody2D rb;
        private HingeJoint2D ropeJoint;

        private PlayerLife playerLife;

        public bool IsClimbing { get; private set; }

        private void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            ropeJoint = GetComponent<HingeJoint2D>();
            animator = GetComponent<Animator>();

            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();

            playerLife = GetComponent<PlayerLife>();

            originalGravity = rb.gravityScale;
        }

        private void Start()
        {
            ropeJoint.enabled = false;

            inputHandler.Input.Player.Move.performed += StartClimb;
            inputHandler.Input.Player.Jump.performed += JumpOnClimb;
        }

        private void FixedUpdate()
        {
            if (ropeJoint && ropeJoint.connectedBody)
            {
                RopeTick();
                return;
            }

            // ★ 축-타기 중 + 대상이 로프이면, 좌/우 입력이 들어올 때 스윙으로 전환(자동)
            if (IsClimbing && currentClimbable is RopeInteractable rope
                           && Mathf.Abs(inputHandler.MoveInput.x) > 0.01f)
            {
                AttachToRopeSegment(rope);
                return;
            }

            HandleClimbing();
        }

        private void OnDestroy()
        {
            inputHandler.Input.Player.Move.performed -= StartClimb;
            inputHandler.Input.Player.Jump.performed -= JumpOnClimb;
        }

        private int CheckEntryKey()
        {
            if (!currentClimbable) return 0;

            var feetY = footPoint ? footPoint.position.y : col.bounds.min.y;
            var top = currentClimbable.GetTop;
            var bottom = currentClimbable.GetBottom;

            if (feetY >= top - entryMargin) return -1; // Top 근접 → ↑만 시작
            if (feetY <= bottom + entryMargin) return 1; // Bottom 근접 → ↓만 시작
            return 0;
        }

        private void JumpOnClimb(InputAction.CallbackContext ctx)
        {
            if (playerLife.IsDead) return;
            if (!IsClimbing) return;

            EndClimb();
            DetachFromRope();
            var x = inputHandler.MoveInput.x;
            var dir = (Vector2.up + Vector2.right * Mathf.Sign(x)).normalized;
            rb.AddForce(dir * climbJumpForce, ForceMode2D.Impulse);
        }

        private void StartClimb(InputAction.CallbackContext ctx)
        {
            if (playerLife.IsDead) return;
            if (IsClimbing) return;
            if (!currentClimbable) return;

            var vY = ctx.ReadValue<Vector2>().y;

            entryKey = CheckEntryKey();

            if (entryKey == -1 && vY >= 0f) return;
            if (entryKey == 1 && vY <= 0f) return;

            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            col.isTrigger = true;
            IsClimbing = true;

            var p = rb.position;
            p.y = Mathf.Clamp(
                p.y,
                currentClimbable.GetBottom + edgeSafety,
                currentClimbable.GetTop - edgeSafety
            );
            rb.position = p;

            endBlockUntil = Time.time + endGraceTime;
            endArmed = false;

            nextStepAt = Time.time;
            SelectClimbAnimSet(currentClimbable is RopeInteractable);

            StartCoroutine(BeginClimbCo());
        }

        internal void EndClimb()
        {
            rb.gravityScale = originalGravity;
            col.isTrigger = false;
            IsClimbing = false;

            transform.rotation = Quaternion.identity;

            currentClimbable = null;

            animator.speed = 1f;
        }

        private void DetachFromRope()
        {
            if (!ropeJoint && !ropeJoint.connectedBody) return;

            ropeJoint.connectedBody = null;
            ropeSeg = null;
            IsClimbing = false;
            ropeJoint.enabled = false;
        }

        private IEnumerator BeginClimbCo()
        {
            while (currentClimbable &&
                   Mathf.Abs(currentClimbable.transform.InverseTransformPoint(rb.position).x) >
                   0.1f)
            {
                var local = currentClimbable.transform.InverseTransformPoint(rb.position);
                local.x = Mathf.Lerp(local.x, 0f, climbObjectSnapSpeed * Time.fixedDeltaTime);
                var next = currentClimbable.transform.TransformPoint(local);
                rb.MovePosition(next);
                yield return new WaitForFixedUpdate();
            }
        }

        private void HandleClimbing()
        {
            if (!IsClimbing) return;
            if (!currentClimbable) return;

            var v = inputHandler.MoveInput.y;

            if (Mathf.Abs(v) <= 0.01f)
            {
                nextStepAt = Time.time;
            }
            else
            {
                if (Time.time >= nextStepAt)
                {
                    StepClimbFrame(v > 0 ? +1 : -1);
                    nextStepAt = Time.time + frameStepInterval;
                }
            }

            Vector2 up = !currentClimbable ? transform.up : currentClimbable.transform.up;

            var targetDeg = Mathf.Atan2(up.y, up.x) * Mathf.Rad2Deg - 90f;
            var next = Mathf.LerpAngle(rb.rotation, targetDeg, Time.fixedDeltaTime * 100f);
            rb.MoveRotation(next);

            var pos = rb.position + up * (v * climbSpeed * Time.fixedDeltaTime);
            var local = currentClimbable.transform.InverseTransformPoint(pos);
            local.x = Mathf.Lerp(local.x, 0f, Time.fixedDeltaTime * climbObjectSnapSpeed);
            var finalPos = currentClimbable.transform.TransformPoint(local);
            rb.MovePosition(finalPos);

            var feetY = footPoint ? footPoint.position.y : col.bounds.min.y;
            var atTop = Mathf.Abs(feetY - currentClimbable.GetTop) <= endMargin;
            var atBottom = Mathf.Abs(feetY - currentClimbable.GetBottom) <= endMargin;

            if (Time.time >= endBlockUntil && !atTop && !atBottom)
                endArmed = true;

            if (!endArmed) return;

            if (atTop && v > 0.01f)
            {
                rb.position += up * exitNudge;
                EndClimb();
                return;
            }

            if (atBottom && v < -0.01f)
            {
                rb.position -= up * exitNudge;
                EndClimb();
            }
        }

        public void AttachToRopeSegment(RopeInteractable seg)
        {
            if (!seg) return;

            transform.rotation = Quaternion.identity;

            ropeSeg = seg;
            currentClimbable = null;

            ropeJoint.enabled = true;
            ropeJoint.connectedBody = seg.GetComponent<Rigidbody2D>();
            ropeJoint.autoConfigureConnectedAnchor = false;
            ropeJoint.enableCollision = false;
            ropeJoint.useLimits = false;

            ropeJoint.anchor = Vector2.zero;
            ropeJoint.connectedAnchor = seg.transform.InverseTransformPoint(rb.position);

            IsClimbing = true;
            rb.gravityScale = originalGravity;
            col.isTrigger = false;

            if (animator && !string.IsNullOrEmpty(ropeSwingState))
            {
                animator.speed = 1f; // 루프 재생
                animator.Play(Animator.StringToHash(ropeSwingState), 0, 0f);
            }
        }

        private void RopeTick()
        {
            if (!ropeSeg || !ropeJoint || !ropeJoint.connectedBody) return;

            var x = inputHandler.MoveInput.x;

            if (Mathf.Abs(x) > 0.01f)
            {
                Vector2 pivot = ropeSeg.transform.TransformPoint(ropeJoint.connectedAnchor);
                var r = rb.worldCenterOfMass - pivot;
                if (r.sqrMagnitude > 1e-6f)
                {
                    var tangent = new Vector2(-r.y, r.x).normalized * Mathf.Sign(x);
                    rb.AddForceAtPosition(tangent * (pumpImpulse * rb.mass),
                        rb.worldCenterOfMass, ForceMode2D.Impulse);
                }
            }

            var v = rb.linearVelocity;
            if (Mathf.Abs(v.x) > maxSwingXSpeed) v.x = Mathf.Sign(v.x) * maxSwingXSpeed;
            if (Mathf.Abs(v.y) > maxSwingYSpeed) v.y = Mathf.Sign(v.y) * maxSwingYSpeed;
            rb.linearVelocity = v;
        }

        private void SelectClimbAnimSet(bool isRope)
        {
            if (!animator) return;

            // 활성 세트 결정
            if (isRope)
            {
                activeClip = ropeClimbClip;
                activeStateHash = Animator.StringToHash(ropeClimbState);
            }
            else
            {
                activeClip = ladderClimbClip;
                activeStateHash = Animator.StringToHash(ladderClimbState);
            }

            // 정지(수동 스텝 모드) + 상태 진입
            animator.speed = 0f;
            animator.Play(activeStateHash, 0, 0f);
            animator.Update(0f);
        }

        private void StepClimbFrame(int dir)
        {
            if (!animator || !activeClip) return;

            var totalFrames = Mathf.Max(1, Mathf.RoundToInt(activeClip.frameRate * activeClip.length));
            var step = 1f / totalFrames;
            animator.speed = 0f;
            var st = animator.GetCurrentAnimatorStateInfo(0);

            if (st.shortNameHash != activeStateHash)
            {
                animator.Play(activeStateHash, 0, dir > 0 ? 0f : 1f);
                animator.Update(0f);
                return;
            }

            var t = st.normalizedTime % 1f;
            var next = Mathf.Repeat(t + dir * step, 1f);
            animator.Play(activeStateHash, 0, next);
            animator.Update(0f);
        }
    }
}