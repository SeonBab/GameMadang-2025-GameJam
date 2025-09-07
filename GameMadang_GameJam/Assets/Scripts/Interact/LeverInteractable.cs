using Interact;
using UnityEngine;
using System.Collections.Generic;

public class LeverInteractable : BaseInteractable
{
    // 레버 동작의 대상이 되는 오브젝트
    [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();
    private bool isActive = false;

    private void Awake()
    {
        foreach (GameObject obj in targetObjects)
        {
            if (obj == null)
            {
                Debug.LogError(gameObject.name + " 의 동작 대상 미설정");
            }
            else if (obj.GetComponent<ISwitch>() == null)
            {
                Debug.LogError(gameObject.name + " 의 잘못된 동작 대상 설정");
            }
        }
    }

    public override void Interact(PlayerController player)
    {
        Debug.Log("레버 상호작용 시작");

        isActive = !isActive;

        //타겟 오브젝트들의 함수 실행
        foreach (GameObject obj in targetObjects)
        {
            ISwitch targetSwitch = obj.GetComponent<ISwitch>();
            if (targetSwitch != null)
            {
                if (isActive == true)
                {
                    targetSwitch.OnSwitch();
                }
                else
                {
                    targetSwitch.OffSwitch();
                }
            }
        }
    }
}