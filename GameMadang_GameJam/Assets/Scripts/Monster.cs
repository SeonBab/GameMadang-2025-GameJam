using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour, ISwitch
{
    public enum MonsterMode
    {
        Normal,
        Patrol,
        Chase
    }

    private const float Eps = 0.01f;

    [SerializeField] private MonsterMode mode;
    [SerializeField] private MonsterMode targetMode;
    [SerializeField] private Transform chaseTarget;
    [SerializeField] private float range = 2f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float loseRange = 10f;
    [SerializeField] private float waitAtEnds = 0.2f;

    public bool toRight = true;
    private readonly WaitForFixedUpdate wait = new();

    private Coroutine patrolCo, chaseCo;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private float leftX;
    private float rightX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        sr.flipX = toRight;

        animator.SetBool(Animator.StringToHash("Moving"), !(Mathf.Abs(rb.linearVelocityX) < 0.1f));
    }

    public void OnSwitch()
    {
        switch (targetMode)
        {
            case MonsterMode.Normal:
                break;
            case MonsterMode.Patrol:
                StartPatrol();
                break;
            case MonsterMode.Chase:
                StartChase();
                break;
        }
    }

    public void OffSwitch()
    {
        // 아무 것도 변경하지 않는다.
    }

    private void SetChaseTarget(Transform target)
    {
        chaseTarget = target;
        StartChase();
    }

    [ContextMenu("Set Target X")]
    private void SetTargetX()
    {
        leftX = rb.position.x - Mathf.Abs(range);
        rightX = rb.position.x + Mathf.Abs(range);
    }

    [ContextMenu("Start Chase")]
    private void StartChase()
    {
        mode = MonsterMode.Chase;
        if (patrolCo != null)
        {
            StopCoroutine(patrolCo);
            patrolCo = null;
        }

        chaseCo ??= StartCoroutine(ChaseCo());
    }

    [ContextMenu("Start Patrol")]
    private void StartPatrol()
    {
        mode = MonsterMode.Patrol;
        if (chaseCo != null)
        {
            StopCoroutine(chaseCo);
            chaseCo = null;
        }

        patrolCo ??= StartCoroutine(PatrolCo());
    }

    private IEnumerator PatrolCo()
    {
        SetTargetX();

        toRight = true;

        while (mode == MonsterMode.Patrol)
        {
            var targetX = toRight ? rightX : leftX;
            var x = toRight ? 1f : -1f;

            while (Mathf.Abs(rb.position.x - targetX) > Eps)
            {
                var targetVx = Mathf.Sign(x) * patrolSpeed;
                rb.linearVelocity = new Vector2(targetVx, rb.linearVelocity.y);

                yield return wait;
            }

            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            yield return new WaitForSeconds(waitAtEnds);

            toRight = !toRight;
            yield return wait;
        }
    }

    private IEnumerator ChaseCo()
    {
        while (mode == MonsterMode.Chase && chaseTarget)
        {
            var dir = Mathf.Sign(chaseTarget.position.x - rb.position.x);
            var desiredVx = dir * chaseSpeed;
            rb.linearVelocity = new Vector2(desiredVx, rb.linearVelocity.y);

            if (Vector2.Distance(rb.position, chaseTarget.position) > loseRange)
            {
                yield break;
            }

            yield return wait;
        }
    }
}