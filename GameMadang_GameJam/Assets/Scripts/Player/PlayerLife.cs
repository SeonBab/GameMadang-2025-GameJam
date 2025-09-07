using System.Collections;
using Save;
using UnityEngine;

namespace Player
{
    public class PlayerLife : MonoBehaviour
    {
        [SerializeField] private float returnTime = 3f;
        private static readonly int IsDeath = Animator.StringToHash("IsDead");
        public bool IsDead { get; private set; }

        private Animator animator;
        private Rigidbody2D rb;

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
            print("Death");

            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            IsDead = true;

            rb.linearVelocity = Vector2.zero;

            animator.SetTrigger(IsDeath);

            print("Death Animation Start");

            yield return new WaitForSeconds(returnTime);

            print("Return to Checkpoint");

            IsDead = false;

            animator.Rebind();

            // GameManager.RestartGame();
        }
    }
}