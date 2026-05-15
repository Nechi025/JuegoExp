using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Glaciers
{
    public enum GlacierState { Active, Broken }

    [ExecuteAlways]
    public class Glacier : MonoBehaviour
    {
        [SerializeField] private Sprite[]       healthSprites;
        [SerializeField] private SpriteRenderer sr;

        private GameConfig   _config;

        public GlacierState State  { get; private set; } = GlacierState.Active;
        public float         Health { get; private set; } = 1f;

        private int _clicksThisCycle;
        private int _targetThisCycle;

        public bool IsClickable => State == GlacierState.Active;

        private void Awake()
        {
            if (ServiceLocator.TryGet<GameManager>(out var gm)) _config = gm.Config;
            RollNewTarget();
            UpdateSprite();
        }

        private void OnMouseDown() => OnClick();

        public void OnClick()
        {
            if (!IsClickable || _config == null) return;

            _clicksThisCycle++;
            Health = Mathf.Max(0f, Health - _config.glacierClickDamage);
            Debug.Log($"[Glacier] Click {_clicksThisCycle}/{_targetThisCycle}, health: {Health:F2}");

            if (_clicksThisCycle >= _targetThisCycle)
            {
                Debug.Log("[Glacier] Ice ball spawned");
                GameEventBus.RaiseIceCubeSpawned();
                RollNewTarget();
            }

            if (Health <= 0f)
                Break();
            else
                UpdateSprite();
        }

        private void RollNewTarget()
        {
            _clicksThisCycle = 0;
            _targetThisCycle = _config != null
                ? Random.Range(_config.glacierClicksMin, _config.glacierClicksMax + 1)
                : 10;
            Debug.Log($"[Glacier] Next ice ball in {_targetThisCycle} clicks");
        }

        private void Break()
        {
            State  = GlacierState.Broken;
            Health = 0f;
            Debug.Log("[Glacier] Permanently broken!");
            GameEventBus.RaiseMistake();
            UpdateSprite();
        }

        public void Tick(float deltaTime)
        {
            if (_config == null || State == GlacierState.Broken) return;
            Health -= _config.glacierPassiveDecayRate * deltaTime;
            if (Health <= 0f)
            {
                Health = 0f;
                Break();
                return;
            }
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (sr == null || healthSprites.Length == 0) return;
            int idx = Mathf.Clamp(
                Mathf.RoundToInt(Health * (healthSprites.Length - 1)),
                0, healthSprites.Length - 1);
            sr.sprite = healthSprites[idx];
        }

        private void Update()
        {
            if (Application.isPlaying) Tick(Time.deltaTime);
        }
    }
}
