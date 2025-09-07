using System;
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

        public bool IsClimbing {get; private set;}
        public BaseInteractable currentClimbable;

        private PlayerController player;
        private InputHandler inputHandler;
        private Rigidbody2D rb;
        private Collider2D col;

        public int entryKey;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            inputHandler = GetComponent<InputHandler>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
        }

        private void Start()
        {
            inputHandler.Input.Player.Move.performed += StartClimb;
            inputHandler.Input.Player.Jump.performed += JumpOnClimb;
        }

        private void OnDestroy()
        {
            inputHandler.Input.Player.Move.performed -= StartClimb;
            inputHandler.Input.Player.Jump.performed -= JumpOnClimb;
        }

        private void FixedUpdate()
        {
            HandleClimbing();
        }

        private int CheckEntryKey()
        {
            if (!currentClimbable) return 0;

            if (Mathf.Abs(footPoint.position.y - currentClimbable.GetTop) < 0.01f)
            {
                print($"on top");
                return -1;
            }
            else if (Mathf.Abs(footPoint.position.y - currentClimbable.GetBottom) < 0.01f)
            {
                print($"on bottom");
                return 1;
            }

            return 0;
        }

        private void JumpOnClimb(InputAction.CallbackContext ctx)
        {
            if (!IsClimbing) return;

            var x = inputHandler.MoveInput.x;
            var dir = Vector2.up + Vector2.right * Mathf.Sign(x);

            rb.AddForce(dir.normalized * climbJumpForce, ForceMode2D.Impulse);

            EndClimb();
        }

        private void StartClimb(InputAction.CallbackContext ctx)
        {
            if (!Mathf.Approximately(ctx.ReadValue<Vector2>().y, CheckEntryKey())) return;
            if (IsClimbing) return;
            if (!currentClimbable) return;

            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            col.isTrigger = true;
            IsClimbing = true;

            StartCoroutine(BeginClimbCo());
        }

        internal void EndClimb()
        {
            if (!currentClimbable) return;

            rb.gravityScale = 1.5f;
            col.isTrigger = false;
            IsClimbing = false;

            transform.rotation = Quaternion.identity;

            currentClimbable = null;
        }

        private IEnumerator BeginClimbCo()
        {
            while (currentClimbable &&
                   Mathf.Abs(currentClimbable.transform.InverseTransformPoint(rb.position).x) > 0.1f)
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
            /*if (CheckEntryKey() == 1)
            {
                EndClimb();
                return;
            }*/

            entryKey = CheckEntryKey();

            var v = inputHandler.MoveInput.y;

            Vector2 up = !currentClimbable ? transform.up : currentClimbable.transform.up;

            var targetDeg = Mathf.Atan2(up.y, up.x) * Mathf.Rad2Deg - 90f;
            var next = Mathf.LerpAngle(rb.rotation, targetDeg, Time.fixedDeltaTime * 100f);
            rb.MoveRotation(next);

            var pos = rb.position + up * (v * climbSpeed * Time.fixedDeltaTime);
            var local = currentClimbable.transform.InverseTransformPoint(pos);
            local.x = Mathf.Lerp(local.x, 0f, Time.fixedDeltaTime * climbObjectSnapSpeed);
            var finalPos = currentClimbable.transform.TransformPoint(local);
            rb.MovePosition(finalPos);
        }
    }
}