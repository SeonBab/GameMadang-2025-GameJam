using Interact;
using Save;
using Unity.VisualScripting;
using UnityEngine;

public class SavePointInteractable : BaseInteractable
{
    [SerializeField] private int savePointNumber;
    public int SavePointNumber
    {
        get => savePointNumber;
        set => savePointNumber = value;
    }

    public override void Interact(PlayerController player)
    {
        Debug.Log("세이브포인트 상호작용 시작");

        // 세이브포인트 갱신을 위해 호출하는 함수
        SaveManager.instance.Save(this);
    }
}