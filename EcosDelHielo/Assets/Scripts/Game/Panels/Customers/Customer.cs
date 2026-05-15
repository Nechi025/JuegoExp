using Core.Services;
using DG.Tweening;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Panels.Customers
{
    // GameObject must start ACTIVE in the scene so Awake runs at load.
    // CustomerPanel.Start() deactivates it after initialization.
    public class Customer : MonoBehaviour
    {
        private GameConfig _config;
        private GameConfig Config => _config ??= ServiceLocator.Get<GameManager>().Config;

        [SerializeField] private Image          sr;
        [SerializeField] private ParticleSystem deliveryParticles;

        private static readonly Color PureColor    = new Color(0.3f,  1f,    0.3f);
        private static readonly Color TaintedColor = new Color(0.29f, 0.16f, 0f);

        private RectTransform _rt;
        private CanvasGroup   _cg;
        private float         _patience;
        private bool          _isServed;
        private bool          _hasLeft;

        public bool  IsActive          => gameObject.activeSelf && !_isServed && !_hasLeft;
        public float PatienceNormalized => IsActive ? _patience / Config.customerPatienceTime : 0f;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }

        public void Show()
        {
            _patience = Config.customerPatienceTime;
            _isServed = false;
            _hasLeft  = false;
            _cg.alpha = 0f;
            gameObject.SetActive(true);
            DOTween.To(() => _cg.alpha, x => _cg.alpha = x, 1f, 0.3f);
        }

        private void Hide()
        {
            DOTween.To(() => _cg.alpha, x => _cg.alpha = x, 0f, 0.3f)
                   .OnComplete(() => gameObject.SetActive(false));
        }

        public void Serve(bool isPure)
        {
            if (!IsActive) return;
            _isServed = true;
            PlayDeliveryEffect(isPure);
        }

        private void PlayDeliveryEffect(bool isPure)
        {
            if (_rt != null)
            {
                Vector2 origin = _rt.anchoredPosition;
                DOTween.Sequence()
                    .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2( 8f, 0f), 0.04f))
                    .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2(-8f, 0f), 0.04f))
                    .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin + new Vector2( 5f, 0f), 0.04f))
                    .Append(DOTween.To(() => _rt.anchoredPosition, p => _rt.anchoredPosition = p, origin,                        0.04f))
                    .OnComplete(Hide);
            }
            else
            {
                Hide();
            }

            if (sr != null)
            {
                Color flash = isPure ? PureColor : TaintedColor;
                DOTween.To(() => sr.color, x => sr.color = x, flash, 0.15f)
                       .OnComplete(() => DOTween.To(() => sr.color, x => sr.color = x, Color.white, 0.15f));
            }
            else
            {
                Debug.LogWarning("[Customer] sr (Image) is not assigned — tint effect skipped.", this);
            }

            if (deliveryParticles != null)
                deliveryParticles.Play();
            else
                Debug.LogWarning("[Customer] deliveryParticles is not assigned — particle effect skipped.", this);
        }

        private void Tick(float deltaTime)
        {
            if (!IsActive) return;
            _patience -= deltaTime;
            if (_patience <= 0f) Leave();
        }

        private void Leave()
        {
            _hasLeft = true;
            GameEventBus.RaiseMistake();
            Hide();
        }

        private void Update() => Tick(Time.deltaTime);
    }
}
