using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Glaciers
{
    public enum GlacierState { Idle, Regenerating }

    [ExecuteAlways]
    public class Glacier : MonoBehaviour
    {
        private GameConfig _config;

        public GlacierState State         { get; private set; } = GlacierState.Idle;
        public int          ClickCount    { get; private set; }
        public float        PassiveHealth { get; private set; } = 1f;
        public float        RegenProgress { get; private set; }

        public bool IsClickable => State == GlacierState.Idle;

        private void Awake() => ServiceLocator.TryGet<GameConfig>(out _config);

        public void OnClick()
        {
            if (!IsClickable || _config == null) return;
            ClickCount++;
            if (ClickCount >= _config.glacierClicksToBreak)
                CalveIceCube();
        }

        private void CalveIceCube()
        {
            ClickCount    = 0;
            PassiveHealth = 0f;
            GameEventBus.RaiseIceCubeSpawned();
            EnterRegen();
        }

        private void EnterRegen()
        {
            State         = GlacierState.Regenerating;
            RegenProgress = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (State == GlacierState.Idle)
                TickDecay(deltaTime);
            else
                TickRegen(deltaTime);
        }

        private void TickDecay(float deltaTime)
        {
            if (_config == null) return;
            PassiveHealth -= _config.glacierPassiveDecayRate * deltaTime;
            if (PassiveHealth > 0f) return;
            PassiveHealth = 0f;
            GameEventBus.RaiseMistake();
            EnterRegen();
        }

        private void TickRegen(float deltaTime)
        {
            if (_config == null) return;
            RegenProgress += deltaTime / _config.glacierRegenTime;
            if (RegenProgress < 1f) return;
            RegenProgress = 1f;
            PassiveHealth = 1f;
            State         = GlacierState.Idle;
        }

        private void Update() => Tick(Time.deltaTime);
    }
}
