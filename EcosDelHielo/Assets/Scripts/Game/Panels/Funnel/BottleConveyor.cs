using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Panels.Funnel
{
    public class BottleConveyor : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite purifiedBottleSprite;
        [SerializeField] private Sprite contaminatedBottleSprite;

        [Header("Waypoints")]
        [SerializeField] private RectTransform spawnPoint;
        [SerializeField] private RectTransform fillPoint;
        [SerializeField] private RectTransform leavePoint;

        [Header("Timing")]
        [SerializeField] private float enterDuration = 0.6f;
        [SerializeField] private float exitDuration  = 0.5f;

        [SerializeField] private Image image;

        private RectTransform _rt;
        private Tween         _cycleDelay;

        public bool         IsReadyToFill { get; private set; }
        public Action       OnArrived;
        public Action<bool> OnDeparted;

        private void Awake() => _rt = GetComponent<RectTransform>();

        private void KillAll()
        {
            _cycleDelay?.Kill();
            _cycleDelay = null;
            _rt.DOKill();
        }

        public void BeginCycle()
        {
            KillAll();
            IsReadyToFill        = false;
            image.sprite         = emptySprite;
            _rt.anchoredPosition = new Vector2(spawnPoint.anchoredPosition.x, _rt.anchoredPosition.y);
            gameObject.SetActive(true);

            _cycleDelay = DOVirtual.DelayedCall(UnityEngine.Random.Range(3f, 5f), () =>
            {
                _cycleDelay = null;
                Debug.Log("[Bottle] Entering fill position");
                DOTween.To(
                        () => _rt.anchoredPosition.x,
                        x  => _rt.anchoredPosition = new Vector2(x, _rt.anchoredPosition.y),
                        fillPoint.anchoredPosition.x, enterDuration)
                    .SetEase(Ease.OutQuad)
                    .SetTarget(_rt)
                    .OnComplete(() =>
                    {
                        IsReadyToFill = true;
                        Debug.Log("[Bottle] Ready to fill");
                        OnArrived?.Invoke();
                    });
            });
        }

        public void Depart(bool isPure)
        {
            KillAll();
            IsReadyToFill = false;
            image.sprite  = isPure ? purifiedBottleSprite : contaminatedBottleSprite;
            image.transform.DOPunchScale(Vector3.one * 0.25f, 0.3f, 8, 1f);

            DOTween.To(
                    () => _rt.anchoredPosition.x,
                    x  => _rt.anchoredPosition = new Vector2(x, _rt.anchoredPosition.y),
                    leavePoint.anchoredPosition.x, exitDuration)
                .SetEase(Ease.InQuad)
                .SetTarget(_rt)
                .OnComplete(() =>
                {
                    ResetAndDeactivate();
                    GameEventBus.RaiseBottleReady(isPure);
                    OnDeparted?.Invoke(isPure);
                });
        }

        private void ResetAndDeactivate()
        {
            KillAll();
            IsReadyToFill        = false;
            image.sprite         = emptySprite;
            _rt.anchoredPosition = new Vector2(spawnPoint.anchoredPosition.x, _rt.anchoredPosition.y);
            gameObject.SetActive(false);
        }
    }
}
