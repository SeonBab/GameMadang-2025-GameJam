using UnityEngine;

namespace Save
{
    public class SavePoint : MonoBehaviour
    {
        [SerializeField] internal int number;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) SaveManager.Instance.Save(this);
        }
    }
}