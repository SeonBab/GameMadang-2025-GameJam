using UnityEngine;

public class LeverInteractable : BaseInteractable
{
    // ���� ������ ����� �Ǵ� ������Ʈ
    [SerializeField] private GameObject targetObject;

    public override void Interact()
    {
        Debug.Log("���� ��ȣ�ۿ� ����");

        //TODO
        //Ÿ�� ������Ʈ�� �Լ� ����
    }
}