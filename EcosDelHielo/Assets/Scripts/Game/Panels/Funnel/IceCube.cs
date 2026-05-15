using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Funnel
{
    public class IceCube : MonoBehaviour
    {
        [SerializeField] private Sprite[]       dirtySprites;
        [SerializeField] private Sprite[]       cleanSprites;
        [SerializeField] private Sprite         contaminatedSprite;
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
                _sr.sprite = IsPure ? PickRandom(cleanSprites, _sr.sprite) : contaminatedSprite;
            Debug.Log($"[IceCube] Reached funnel — IsPure: {IsPure}");
            if (!IsPure) GameEventBus.RaiseMistake();
        }

        private void UpdateSprite()
        {
            if (_sr == null) return;
            _sr.sprite = IsPure
                ? PickRandom(cleanSprites, _sr.sprite)
                : PickRandom(dirtySprites,  _sr.sprite);
        }

        private static Sprite PickRandom(Sprite[] pool, Sprite current)
        {
            if (pool == null || pool.Length == 0) return null;
            if (pool.Length == 1) return pool[0];

            int idx;
            int attempts = 0;
            do { idx = Random.Range(0, pool.Length); }
            while (pool[idx] == current && ++attempts < 10);
            return pool[idx];
        }
    }
}
