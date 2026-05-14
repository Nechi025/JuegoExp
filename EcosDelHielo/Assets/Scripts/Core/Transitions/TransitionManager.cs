using System;
using Core.Services;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Transitions
{
    public class TransitionManager : MonoBehaviour, ITransitionManager
    {
        [SerializeField] private CanvasGroup overlay;
        [SerializeField] private Image overlayImage;
        [SerializeField] private TransitionSettings settings = new TransitionSettings();

        private void Awake()
        {
            overlayImage.color = settings.overlayColor;
            overlay.alpha = 1f;
            overlay.blocksRaycasts = true;
            ServiceLocator.Register<ITransitionManager>(this);
        }

        private void OnValidate()
        {
            if (overlay == null)
                Debug.LogError("[TransitionManager] 'overlay' CanvasGroup is not assigned.", this);
            if (overlayImage == null)
                Debug.LogError("[TransitionManager] 'overlayImage' Image is not assigned.", this);
        }

        public void FadeToBlack(Action onComplete = null)
        {
            overlay.DOKill();
            overlay.blocksRaycasts = true;

            // Already black — skip tween and fire callback immediately.
            // This covers the initial Bootstrap load where overlay starts opaque.
            if (overlay.alpha >= 1f)
            {
                onComplete?.Invoke();
                return;
            }

            DOTween.To(() => overlay.alpha, x => overlay.alpha = x, 1f, settings.fadeOutDuration)
                .SetTarget(overlay)
                .SetEase(settings.fadeOutEase)
                .SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void FadeFromBlack(Action onComplete = null)
        {
            overlay.DOKill();

            if (overlay.alpha <= 0f)
            {
                overlay.blocksRaycasts = false;
                onComplete?.Invoke();
                return;
            }

            DOTween.To(() => overlay.alpha, x => overlay.alpha = x, 0f, settings.fadeInDuration)
                .SetTarget(overlay)
                .SetEase(settings.fadeInEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    overlay.blocksRaycasts = false;
                    onComplete?.Invoke();
                });
        }

        public void Transition(Action onMidpoint = null, Action onComplete = null)
        {
            FadeToBlack(() =>
            {
                onMidpoint?.Invoke();
                FadeFromBlack(onComplete);
            });
        }

        private void OnDestroy()
        {
            overlay.DOKill();
            ServiceLocator.Unregister<ITransitionManager>();
        }
    }
}
