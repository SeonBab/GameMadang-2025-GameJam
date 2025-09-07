using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interact
{
    public class RopeGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject segmentPrefab;
        [SerializeField] private Rigidbody2D anchor;
        [SerializeField, Range(2, 20)] private int segmentCount;
        [SerializeField] private float segmentLength;
        [SerializeField] private float spriteLength;

        private readonly List<GameObject> segments = new ();
        public int Count { get; private set; }

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
                var sr = newSegment.GetComponent<SpriteRenderer>();

                sr.size = new Vector2(1, spriteLength);
                col.size = new Vector2(1, segmentLength);

                joint.connectedBody = prevBody;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = new Vector2(0, segmentLength / 2);
                joint.connectedAnchor = prevBody != null ? new Vector2(0, -segmentLength / 2) : Vector2.zero;
                joint.useLimits = true;

                prevBody = rb;
                segments.Add(newSegment);
            }
        }

        public Bounds GetRopeBounds()
        {
            var cols = GetComponentsInChildren<Collider2D>(false)
                .Where(c => c && c.enabled)
                .ToArray();

            if (cols.Length == 0)
                return new Bounds(transform.position, Vector3.zero);

            var b = cols[0].bounds;
            for (var i = 1; i < cols.Length; i++)
                b.Encapsulate(cols[i].bounds);

            return b;
        }

        public void NotifyRopeEnter()
        {
            Count++;
        }

        public void NotifyRopeExit()
        {
            Count--;
        }

        public void ResetCount()
        {
            Count = 0;
        }
    }
}
