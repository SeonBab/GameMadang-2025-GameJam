using System.Collections.Generic;
using UnityEngine;

namespace Interact
{
    public class RopeGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject segmentPrefab;
        [SerializeField, Range(2, 20)] private int segmentCount;
        [SerializeField] private float segmentLength;
        [SerializeField] private Rigidbody2D anchor;

        [HideInInspector][SerializeField] private List<GameObject> segments = new List<GameObject>();
        public List<GameObject> Segments => segments;

        [ContextMenu("Generate Rope")]
        public void GenerateRope()
        {
            // 기존 세그먼트 제거
            foreach (var segment in segments)
            {
                DestroyImmediate(segment);
            }

            segments.Clear();

            anchor.transform.position = transform.position;

            // 시작점에 바디가 없어도 허용
            var prevBody = anchor;

            for (var i = 0; i < segmentCount; i++)
            {
                var newSegment = Instantiate(segmentPrefab, transform);

                newSegment.name = "RopeSegment_" + i;
                newSegment.transform.position = anchor.transform.position - new Vector3(0, (i + 1) * segmentLength, 0);

                var col = newSegment.GetComponent<CapsuleCollider2D>();
                var rb = newSegment.GetComponent<Rigidbody2D>();
                var joint = newSegment.GetComponent<HingeJoint2D>();

                col.size = new Vector2(1, 1 * segmentLength);

                joint.connectedBody = prevBody;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = new Vector2(0, segmentLength / 2);
                joint.connectedAnchor = prevBody != null ? new Vector2(0, -segmentLength / 2) : Vector2.zero;
                joint.useLimits = true;

                prevBody = rb;
                segments.Add(newSegment);
            }
        }
    }
}
