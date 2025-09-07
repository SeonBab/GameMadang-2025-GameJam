using System.Linq.Expressions;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class StageTrigger : MonoBehaviour
{
    [SerializeField] AudioClip targetAudioClip;

    private BoxCollider2D stageCollider;

    void Start()
    {
        stageCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 브금 재생
            AudioManager.Instance.PlayBGM(targetAudioClip, 1f, true, 0.3f);
        }
    }
}
