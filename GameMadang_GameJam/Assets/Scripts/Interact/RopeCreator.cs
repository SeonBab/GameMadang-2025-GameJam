using UnityEngine;
using System.Collections.Generic;

public class RopeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField, Range(2, 20)] int segmentCount;
    [SerializeField] private float segmentLength;

    [SerializeField] private GameObject anchor;

    [HideInInspector][SerializeField] private List<GameObject> segments = new List<GameObject>();

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
        Rigidbody2D prevBody = anchor.GetComponent<Rigidbody2D>();

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject newSegment = Instantiate(segmentPrefab, transform);

            newSegment.name = "RopeSegment_" + i;
            newSegment.tag = "ClimbObject";
            newSegment.transform.position = anchor.transform.position - new Vector3(0, (i + 1) * segmentLength, 0);

            CapsuleCollider2D capsuleCollider2D = newSegment.GetComponent<CapsuleCollider2D>();
            Rigidbody2D rigidbody2D = newSegment.GetComponent<Rigidbody2D>();
            HingeJoint2D hingeJoint2D = newSegment.GetComponent<HingeJoint2D>();

            capsuleCollider2D.size = new Vector2(1, 3);

            hingeJoint2D.connectedBody = prevBody;
            hingeJoint2D.autoConfigureConnectedAnchor = false;
            hingeJoint2D.anchor = new Vector2(0, segmentLength / 2);
            hingeJoint2D.connectedAnchor = prevBody != null ? new Vector2(0, -segmentLength / 2) : Vector2.zero;
            hingeJoint2D.useLimits = true;

            JointAngleLimits2D ropeLimis = new JointAngleLimits2D();
            ropeLimis.min = -45f;
            ropeLimis.max = 45f;

            hingeJoint2D.limits = ropeLimis;

            prevBody = rigidbody2D;
            segments.Add(newSegment);
        }
    }
}
