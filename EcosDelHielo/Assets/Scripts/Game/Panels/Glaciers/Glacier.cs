using Core.Services;
using DG.Tweening;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Panels.Glaciers
{
    public enum GlacierState { Active, Broken }

    [ExecuteAlways]
    public class Glacier : MonoBehaviour
    {
        [SerializeField] private Sprite[]       healthSprites;
        [SerializeField] private Image          sr;
        [SerializeField] private Image          fillImage;
        [SerializeField] private Image          fillImage2;
        [SerializeField] private Image          skyImage;
        [SerializeField] private ParticleSystem iceParticles;

        private static readonly Color IceBlue = new Color(0.5f, 0.8f, 1f);

        private RectTransform _rt;
        private GameConfig    _config;
        private int           _totalClicks;
        private int           _clicksThisCycle;
        private int           _targetThisCycle;

        public GlacierState State     { get; private set; } = GlacierState.Active;
        public bool         IsClickable => State == GlacierState.Active;

        private float HealthNormalized =>
            _config != null
                ? 1f - Mathf.Clamp01((float)_totalClicks / _config.glacierMaxClicks)
                : 1f;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            if (ServiceLocator.TryGet<GameManager>(out var gm)) _config = gm.Config;
            RollNewTarget();
            UpdateSprite();
        }

        private void OnMouseDown() => OnClick();

        public void OnClick()
        {
            if (!IsClickable || _config == null) return;

            _totalClicks++;
            _clicksThisCycle++;
            Debug.Log($"[Glacier] Click {_clicksThisCycle}/{_targetThisCycle}, total: {_totalClicks}/{_config.glacierMaxClicks}");

            PlayHitEffects();
            UpdateSprite();

            if (_clicksThisCycle >= _targetThisCycle)
            {
                Debug.Log("[Glacier] Ice ball spawned");
                GameEventBus.RaiseIceCubeSpawned();
                RollNewTarget();
            }

            if (_totalClicks >= _config.glacierMaxClicks)
                Break();
        }

        private void PlayHitEffects()
        {
            if (_rt != null)
            {
                Vector2 origin = _rt.anchoredPosition;
                DOTween.Sequence()
                    .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2( 6f, 0f), 0.04f))
                    .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2(-6f, 0f), 0.04f))
                    .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2( 4f, 0f), 0.04f))
                    .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin,                        0.04f));
            }
            if (sr != null)
                DOTween.To(() => sr.color, x => sr.color = x, IceBlue, 0.1f)
                       .OnComplete(() => DOTween.To(() => sr.color, x => sr.color = x, Color.white, 0.15f));
            else
                Debug.LogWarning("[Glacier] sr (Image) is not assigned — tint effect skipped.", this);

            if (iceParticles != null)
                iceParticles.Play();
            else
                Debug.LogWarning("[Glacier] iceParticles is not assigned — particle effect skipped.", this);
        }

        private void RollNewTarget()
        {
            _clicksThisCycle = 0;
            _targetThisCycle = _config != null ? _config.glacierClicksPerIceBall : 1;
            Debug.Log($"[Glacier] Next ice ball in {_targetThisCycle} clicks");
        }

        private void Break()
        {
            State = GlacierState.Broken;
            Debug.Log("[Glacier] Permanently broken!");
            PlayBreakEffect();
            GameEventBus.RaiseGameOver();
        }

        private void PlayBreakEffect()
        {
            if (_rt == null || sr == null) return;

            Vector2 origin   = _rt.anchoredPosition;
            float   duration = 1.2f;

            DOTween.Sequence()
                .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2( 8f, 0f), 0.04f))
                .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2(-8f, 0f), 0.04f))
                .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2( 6f, 0f), 0.04f))
                .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2(-6f, 0f), 0.04f))
                .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin,                        0.04f))
                .Append(DOTween.To(
                    () => _rt.anchoredPosition,
                    p  => _rt.anchoredPosition = p,
                    origin + new Vector2(0f, -120f),
                    duration).SetEase(Ease.InQuad))
                .OnComplete(() => gameObject.SetActive(false));

            DOTween.To(() => sr.color, x => sr.color = x, new Color(1f, 1f, 1f, 0f), duration)
                   .SetEase(Ease.InQuad)
                   .SetDelay(0.2f);
        }

        private void UpdateSprite()
        {
            float h = HealthNormalized;

            if (sr != null && healthSprites.Length > 0)
            {
                int idx = Mathf.Clamp(
                    Mathf.RoundToInt(h * (healthSprites.Length - 1)),
                    0, healthSprites.Length - 1);
                sr.sprite = healthSprites[idx];
            }

            if (fillImage != null)
                fillImage.fillAmount = 1f - h;
            if (fillImage2 != null)
                fillImage2.fillAmount = 1f - h;
            if (skyImage != null)
            {
                Color c = skyImage.color;
                c.a = (192f / 255f) * (1f - h);
                skyImage.color = c;
            }
        }
    }
}
