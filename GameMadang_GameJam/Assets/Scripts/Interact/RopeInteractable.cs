using Player;
using UnityEngine;

namespace Interact
{
    public class RopeInteractable : BaseInteractable
    {
        public override void Interact(PlayerController player)
        {
            if (!player) return;

            var climber = player.GetComponent<PlayerClimb>();
            climber.currentClimbable = this;

            Debug.Log("로프 상호작용 시작");
        }
    }
}