using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SequenceInfo
{
    public Sprite cutSprite;
    public float cutTime;
}

public class EndingSequenceController : MonoBehaviour
{
    [SerializeField] private SequenceInfo[] endingSequenceInfoList;

    static public Action OnSequenceFinished;
    private Image sequenceImage;
    private int endingSpriteIndex = 0;
    

    void Start()
    {
        Transform endingCanvas = transform.Find("EndingCanvas");
        Transform endingSequence = endingCanvas.transform.Find("EndingSequence");
        sequenceImage = endingSequence.GetComponent<Image>();
    }

    public IEnumerator PlayEndingSequence()
    {
        if (!sequenceImage.enabled)
        {
            sequenceImage.enabled = true;
        }

        while (endingSequenceInfoList.Length > endingSpriteIndex)
        {
            SequenceInfo currentSequenceInfo = endingSequenceInfoList[endingSpriteIndex];

            Sprite currentSprite = currentSequenceInfo.cutSprite;

            sequenceImage.sprite = currentSprite;

            ++endingSpriteIndex;

            yield return new WaitForSecondsRealtime(currentSequenceInfo.cutTime);
        }

        OnSequenceFinished?.Invoke();
    }
}
