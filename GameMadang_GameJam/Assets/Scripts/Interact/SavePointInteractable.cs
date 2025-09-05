using Save;
using UnityEngine;

public class SavePointInteractable : BaseInteractable
{
    [SerializeField] private int savePointNumber = -1;
    public int SavePointNumber => savePointNumber;

    private void Awake()
    {
        if (savePointNumber == -1)
        {
            Debug.LogError(gameObject.name + " �� savePointNumber �� �̼���");
        }
    }

    public override void Interact()
    {
        Debug.Log("���̺�����Ʈ ��ȣ�ۿ� ����");

        // ���̺�����Ʈ ������ ���� ȣ���ϴ� �Լ�
        SaveManager.Instance.Save(this);
    }
}