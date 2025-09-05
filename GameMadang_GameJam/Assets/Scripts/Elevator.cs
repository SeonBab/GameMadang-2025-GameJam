using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Rigidbody2D place;
    [SerializeField] private Transform topStop;
    [SerializeField] private Transform bottomStop;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopEpsilon = 0.01f;

    public void GoUp()
    {
        StartCoroutine(MoveTo(topStop.position));
    }

    public void GoDown()
    {
        StartCoroutine(MoveTo(bottomStop.position));
    }

    private IEnumerator MoveTo(Vector2 target)
    {
        print("Start Move");

        yield return new WaitForSeconds(0.5f);

        while ((place.position - target).sqrMagnitude > stopEpsilon * stopEpsilon)
        {
            var next = Vector2.MoveTowards(place.position, target, speed * Time.fixedDeltaTime);
            place.MovePosition(next);
            yield return new WaitForFixedUpdate();
        }

        place.MovePosition(target);
    }

    [ContextMenu("Go Up")]
    private void CtxGoUp() => GoUp();

    [ContextMenu("Go Down")]
    private void CtxGoDown() => GoDown();
}