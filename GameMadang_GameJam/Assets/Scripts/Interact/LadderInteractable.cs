using Player;
using UnityEngine;

namespace Interact
{
    public class LadderInteractable : BaseInteractable
    {
        private PlayerClimb climber;

        public override void Interact(PlayerController player)
        {
            if (!player) return;

            climber = player.GetComponent<PlayerClimb>();
            climber.currentClimbable = this;

            Debug.Log("사다리 상호작용 시작");
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            climber.EndClimb();
        }
    }
}