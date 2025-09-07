using System;
using Player;
using UnityEngine;

namespace Interact
{
    public class RopeInteractable : BaseInteractable
    {
        private RopeGenerator ropeGenerator;
        private PlayerClimb climber;

        private void Awake()
        {
            ropeGenerator = GetComponentInParent<RopeGenerator>();
        }

        public override void Interact(PlayerController player)
        {
            if (!player) return;

            climber = player.GetComponentInParent<PlayerClimb>();
            if (climber) { climber.currentClimbable = this; climber.ropeSeg = this; }

            Debug.Log("루프 상호작용 시작");
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            ropeGenerator.NotifyRopeEnter();
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            ropeGenerator.NotifyRopeExit();

            if (ropeGenerator.Count == 0)
            {
                climber.currentClimbable = null;
                climber.ropeSeg = null;
            }
        }

        public override float GetBottom => ropeGenerator.GetRopeBounds().min.y;
        public override float GetTop => ropeGenerator.GetRopeBounds().max.y;
    }
}