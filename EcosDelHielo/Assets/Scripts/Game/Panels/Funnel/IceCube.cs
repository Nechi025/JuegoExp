using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Funnel
{
    public class IceCube : MonoBehaviour
    {
        [SerializeField] private Sprite[]       purifySprites;
        [SerializeField] private Sprite         purifiedSprite;
        [SerializeField] private Sprite         contaminatedSprite;
        [SerializeField] private SpriteRenderer dirtOverlay;
        [SerializeField] private ParticleSystem hitParticles;

        private SpriteRenderer _sr;
        private GameConfig     _config;
        private GameConfig     Config => _config ??= ServiceLocator.Get<GameManager>().Config;

        private int  _purifyClicks;
        private int  _purifyClicksRequired;
        private bool _hasReachedFunnel;

        public bool IsPure           => _purifyClicks >= _purifyClicksRequired;
        public bool HasReachedFunnel => _hasReachedFunnel;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _purifyClicksRequired = Config.purifyClicksRequired;
            UpdateSprite();
        }

        private void OnMouseDown() => OnClick();

        public void OnClick()
        {
            if (_hasReachedFunnel || IsPure) return;
            _purifyClicks++;
            Debug.Log($"[IceCube] Purify click {_purifyClicks}/{_purifyClicksRequired}");
            if (hitParticles != null)
                hitParticles.Play();
            else
                Debug.LogWarning("[IceCube] hitParticles is not assigned — particle effect skipped.", this);
            UpdateSprite();
        }

        public void ReachFunnel()
        {
            if (_hasReachedFunnel) return;
            _hasReachedFunnel = true;
            if (_sr != null)
                _sr.sprite = IsPure ? purifiedSprite : contaminatedSprite;
            Debug.Log($"[IceCube] Reached funnel — IsPure: {IsPure}");
            if (!IsPure) GameEventBus.RaiseMistake();
        }

        private void UpdateSprite()
        {
            float t = _purifyClicksRequired > 0
                      ? Mathf.Clamp01((float)_purifyClicks / _purifyClicksRequired)
                      : 1f;

            if (_sr != null && purifySprites.Length > 0)
            {
                int idx = Mathf.Clamp(Mathf.RoundToInt(t * (purifySprites.Length - 1)), 0, purifySprites.Length - 1);
                _sr.sprite = purifySprites[idx];
            }

            if (dirtOverlay != null)
            {
                Color c = dirtOverlay.color;
                c.a = 1f - t;
                dirtOverlay.color = c;
            }
        }
    }
}
