using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact
{
    public class LeverInteractable : BaseInteractable
    {
        // 레버 동작의 대상이 되는 오브젝트
        [SerializeField] private List<GameObject> targetObjects = new();
        [SerializeField] private List<Sprite> sprites = new();
        [SerializeField] private float animationSpeed = 0.1f;

        private SpriteRenderer sr;
        private bool isActive;
        private bool isBusy;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();

            foreach (var obj in targetObjects)
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

        private void StartAnimation()
        {
            if (isBusy) return;
            StartCoroutine(UpdateSprite());
        }

        private IEnumerator UpdateSprite()
        {
            isBusy = true;

            foreach (var sprite in sprites)
            {
                sr.sprite = sprite;

                yield return new WaitForSeconds(animationSpeed);
            }

            sprites.Reverse();

            isBusy = false;
        }

        public override void Interact(PlayerController player)
        {
            Debug.Log("레버 상호작용 시작");

            if (isActive == true)
            {
                return;
            }

            isActive = !isActive;

            StartAnimation();

            foreach (var obj in targetObjects)
            {
                var targetSwitch = obj.GetComponent<ISwitch>();

                if (targetSwitch == null) continue;

                if (isActive) targetSwitch.OnSwitch();
                else targetSwitch.OffSwitch();
            }
        }
    }
}