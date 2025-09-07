using UnityEngine;

namespace Interact
{
    public class RopeGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject segmentPrefab;
        [SerializeField, Range(2, 20)] private int segmentCount;
        [SerializeField] private float segmentLength;
        [SerializeField] private Rigidbody2D anchor;

        [ContextMenu("Generate Rope")]
        public void GenerateRope()
        {
            foreach (Transform segment in transform)
            {
                if (!segment.TryGetComponent<RopeInteractable>(out _)) continue;

                DestroyImmediate(segment.gameObject);
            }

            anchor.transform.position = transform.position;

            var prevBody = anchor;

            for (var i = 0; i < segmentCount; i++)
            {
                var newSegment = Instantiate(segmentPrefab, transform);

                newSegment.name = "RopeSegment_" + i;
                newSegment.transform.position = anchor.transform.position - new Vector3(0, (i + 1) * segmentLength, 0);

                var rb = newSegment.GetComponent<Rigidbody2D>();
                var joint = newSegment.GetComponent<HingeJoint2D>();

                joint.connectedBody = prevBody;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = new Vector2(0, segmentLength / 2);
                joint.connectedAnchor = prevBody != null ? new Vector2(0, -segmentLength / 2) : Vector2.zero;
                joint.useLimits = true;

                var ropeLimits = new JointAngleLimits2D
                {
                    min = -45f,
                    max = 45f
                };

                joint.limits = ropeLimits;

                prevBody = rb;
            }
        }
    }
}
