using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class EndingTrigger : MonoBehaviour
{
    [SerializeField] float fadeOutDelay;
    
    BoxCollider2D boxTrigger;

    void Start()
    {
        boxTrigger = GetComponent<BoxCollider2D>();

        if (boxTrigger.isTrigger == false)
        {
            Debug.LogWarning("콜라이더 트리거 미설정, 코드로 설정 됐습니다.");
            boxTrigger.isTrigger = true;
        }

        if (fadeOutDelay == 0f)
        {
            Debug.LogError("페이드 아웃 시간 미설정");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 엔딩 호출
            GameManager.EndingGame(fadeOutDelay);
        }
    }
}
