using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Panels.Funnel
{
    public class BottleConveyor : MonoBehaviour
    {
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite fullSprite;
        [SerializeField] private float  enterDuration = 0.6f;
        [SerializeField] private float  exitDuration  = 0.5f;

        [SerializeField] private Image image;

        private RectTransform _rt;

        public bool         IsReadyToFill { get; private set; }
        public Action       OnArrived;
        public Action<bool> OnDeparted;

        private void Awake() => _rt = GetComponent<RectTransform>();

        public void BeginCycle(Vector3 spawnPos, Vector3 fillPos)
        {
            IsReadyToFill        = false;
            image.sprite         = emptySprite;
            _rt.anchoredPosition = new Vector2(spawnPos.x, _rt.anchoredPosition.y);

            Debug.Log("[Bottle] Entering fill position");

            DOTween.To(
                    () => _rt.anchoredPosition.x,
                    x  => _rt.anchoredPosition = new Vector2(x, _rt.anchoredPosition.y),
                    fillPos.x, enterDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    IsReadyToFill = true;
                    Debug.Log("[Bottle] Ready to fill");
                    OnArrived?.Invoke();
                });
        }

        public void Depart(Vector3 exitPos, bool isPure)
        {
            IsReadyToFill = false;
            image.sprite  = fullSprite;

            DOTween.To(
                    () => _rt.anchoredPosition.x,
                    x  => _rt.anchoredPosition = new Vector2(x, _rt.anchoredPosition.y),
                    exitPos.x, exitDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    GameEventBus.RaiseBottleReady(isPure);
                    OnDeparted?.Invoke(isPure);
                });
        }
    }
}
