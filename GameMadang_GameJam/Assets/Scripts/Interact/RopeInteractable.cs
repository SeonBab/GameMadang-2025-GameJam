using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RopeInteractable : BaseInteractable
{
    [SerializeField, Range(0f, 1f)] private float forceMultiplier = 0.1f;
    [SerializeField, Range(0f, 60f)] private float maxXSpeed = 10f;
    [SerializeField, Range(0f, 60f)] private float maxYSpeed = 10f;

    private Rigidbody2D rb2D;
    private CapsuleCollider2D capsuleCollider2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject playerGameObject = collision.transform.parent.gameObject;
            PlayerController playerController = playerGameObject.GetComponent<PlayerController>();
            if (playerController == null)
            {
                return;
            }

            playerController.OnFixedUpdateEnd -= RopeAddForce;
            playerController.OnFixedUpdateEnd -= HandleClimbHinge;
        }
    }

    public override void Interact(GameObject InteractCharacter)
    {
        Debug.Log("로프 상호작용 시작");

        if (InteractCharacter == null || InteractCharacter.CompareTag("Player") == false)
        {
            return;
        }

        PlayerController playerController = InteractCharacter.GetComponent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        if (InteractCharacter.GetComponent<HingeJoint2D>() == null)
        {
            playerController.OnFixedUpdateEnd += RopeAddForce;
            playerController.OnFixedUpdateEnd += HandleClimbHinge;

            // 플레이어 캐릭터에 HingeJoint2D 추가 및 연결
            HingeJoint2D playerHingeJoint2D = InteractCharacter.AddComponent<HingeJoint2D>();
            playerHingeJoint2D.connectedBody = transform.GetComponent<Rigidbody2D>();
            playerHingeJoint2D.autoConfigureConnectedAnchor = false;

            JointAngleLimits2D limits = new JointAngleLimits2D();
            limits.min = -20f;
            limits.max = 20f;
            playerHingeJoint2D.limits = limits;
            playerHingeJoint2D.useLimits = true;

            Vector2 localAnchor = InteractCharacter.transform.InverseTransformPoint(rb2D.position);
            localAnchor.x = 0f;
            playerHingeJoint2D.anchor = Vector2.zero;
            playerHingeJoint2D.connectedAnchor = localAnchor;
        }
    }

    private void RopeAddForce(Vector2 Direction, GameObject MovementObejct)
    {
        Vector2 forceToAdd = new Vector2(Mathf.Sign(Direction.x) * forceMultiplier, 0f);
        rb2D.AddForce(forceToAdd, ForceMode2D.Impulse);

        Vector2 currentVelocity = rb2D.linearVelocity;

        // X축 속도 제한
        if (Mathf.Abs(currentVelocity.x) > maxXSpeed)
        {
            currentVelocity.x = Mathf.Sign(currentVelocity.x) * maxXSpeed;
        }

        // Y축 속도 제한
        if (Mathf.Abs(currentVelocity.x) > maxXSpeed)
        {
            currentVelocity.y = Mathf.Sign(currentVelocity.y) * maxYSpeed;
        }

        // 속도 제한 적용
        rb2D.linearVelocity = currentVelocity;
    }

    // Y축 이동 함수
    private void HandleClimbHinge(Vector2 Direction, GameObject MovementObejct)
    {
        HingeJoint2D playerHingeJoint = MovementObejct.GetComponent<HingeJoint2D>();
        PlayerController playerController = MovementObejct.GetComponent<PlayerController>();
        if (playerHingeJoint == null || playerController == null)
        {
            return;
        }

        // Y축 이동
        Vector2 Connected = playerHingeJoint.connectedAnchor;
        Connected.y += Direction.y * playerController.ClimbSpeed * Time.fixedDeltaTime;
        playerHingeJoint.connectedAnchor = Connected;

        // 가장 가까운 로프 확인
        UpdateHingeConnection(playerHingeJoint);

        // 모든 로프에서 벗어났는지 확인
        OutOfAllRopes(MovementObejct);
    }

    // 가장 가까운 로프를 찾아 연결을 변경하는 함수
    private void UpdateHingeConnection(HingeJoint2D PlayerHingeJoint)
    {
        Rigidbody2D closestSegment = FindClosestRopeSegment(PlayerHingeJoint.transform.position);

        if (closestSegment != null && closestSegment != PlayerHingeJoint.connectedBody)
        {
            PlayerHingeJoint.connectedBody = closestSegment;

            Vector2 localAnchor = PlayerHingeJoint.transform.InverseTransformPoint(closestSegment.position);
            localAnchor.x = 0f;
            PlayerHingeJoint.anchor = Vector2.zero;
            PlayerHingeJoint.connectedAnchor = localAnchor;

            PlayerController playerController = PlayerHingeJoint.GetComponent<PlayerController>();
            if (playerController == null)
            {
                return;
            }

            playerController.OnFixedUpdateEnd -= RopeAddForce;
            playerController.OnFixedUpdateEnd -= HandleClimbHinge;

            playerController.OnFixedUpdateEnd += closestSegment.GetComponent<RopeInteractable>().RopeAddForce;
            playerController.OnFixedUpdateEnd += closestSegment.GetComponent<RopeInteractable>().HandleClimbHinge;
        }
    }

    // 가장 가까운 로프를 반환하는 함수
    private Rigidbody2D FindClosestRopeSegment(Vector2 playerPos)
    {
        Rigidbody2D closestRb = null;

        float minDistance = float.MaxValue;

        RopeGenerator ropeGenerator = transform.parent.GetComponent<RopeGenerator>();
        List<GameObject> ropeSegments = ropeGenerator.Segments;

        foreach (var segment in ropeSegments)
        {
            Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();
            float distance = Vector2.Distance(playerPos, rb.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestRb = rb;
            }
        }

        return closestRb;
    }

    // 모든 로프에서 벗어났는지 확인하는 함수
    private void OutOfAllRopes(GameObject MovementObejct)
    {
        RopeGenerator ropeGenerator = transform.parent.GetComponent<RopeGenerator>();
        List<GameObject> ropeSegments = ropeGenerator.Segments;

        Transform childTransform = MovementObejct.transform.Find("Interact");
        InteractionHandler interactionHandler = childTransform.GetComponentInChildren<InteractionHandler>();
        if (interactionHandler == null)
        {
            return;
        }

        Vector2 playerPos = MovementObejct.transform.position;

        // 범위 안에 있는 로프를 찾는다.
        foreach (var segment in ropeSegments)
        {
            Vector2 closestPoint = segment.GetComponent<CapsuleCollider2D>().ClosestPoint(playerPos);

            float distance = Vector2.Distance(playerPos, closestPoint);

            if (distance <= interactionHandler.CenterDistanceX)
            {
                return;
            }
        }

        // 모든 로프와 멀리 떨어진 경우
        DetachFromRope(MovementObejct);
    }

    // 로프들과의 연결을 제거하는 함수
    private void DetachFromRope(GameObject MovementObejct)
    {
        PlayerController playerController = MovementObejct.GetComponent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        playerController.OnFixedUpdateEnd -= RopeAddForce;
        playerController.OnFixedUpdateEnd -= HandleClimbHinge;

        Destroy(MovementObejct.GetComponent<HingeJoint2D>());
    }
}