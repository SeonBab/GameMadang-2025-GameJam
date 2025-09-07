using System.Linq.Expressions;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class StageTrigger : MonoBehaviour
{
    [SerializeField] AudioClip targetAudioClip;

    private BoxCollider2D stageCollider;

    private void Awake()
    {
        if (targetAudioClip != null)
        {
            targetAudioClip.LoadAudioData();
        }
    }

    void Start()
    {
        stageCollider = GetComponent<BoxCollider2D>();

        if (stageCollider.isTrigger == false)
        {
            Debug.LogWarning("콜라이더 트리거 미설정, 코드로 설정 됐습니다.");
            stageCollider.isTrigger = true;
        }
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
