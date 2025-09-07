using System.Collections;
using Save;
using UnityEngine;

namespace Player
{
    public class PlayerLife : MonoBehaviour
    {
        public bool IsDead { get; private set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Monster"))
            {
                Debug.Log("Monster");

                // Death();
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

            // death animation

            print("Death Animation Start");

            GameManager.RestartGame(3f);

            yield return new WaitForSeconds(3f);

            print("Return to Checkpoint");

            IsDead = false;
        }
    }
}