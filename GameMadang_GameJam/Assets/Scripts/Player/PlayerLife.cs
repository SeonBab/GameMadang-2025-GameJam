using System.Collections;
using Save;
using UnityEngine;

namespace Player
{
    public class PlayerLife : MonoBehaviour
    {
        [SerializeField] private float returnTime = 3f;

        private static readonly int IsDeath = Animator.StringToHash("IsDead");
        private Animator animator;
        private Rigidbody2D rb;

        public bool IsDead { get; private set; }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Monster"))
            {
                Debug.Log("Monster");

                Death();
            }
        }

        [ContextMenu("Test/Death")]
        private void Death()
        {
            if (IsDead) return;

            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            IsDead = true;

            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;

            animator.SetTrigger(IsDeath);

            GameManager.RestartGame(returnTime);

            yield return new WaitForSeconds(returnTime);

            IsDead = false;
            rb.bodyType = RigidbodyType2D.Dynamic;

            animator.Rebind();
        }
    }
}