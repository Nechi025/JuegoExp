using NUnit.Framework;
using UnityEngine;
using Core.Services;
using Game.Panels.Glaciers;

namespace Game.Tests
{
    public class GlacierTests
    {
        private GameObject _go;
        private Glacier _glacier;
        private GameConfig _config;

        [SetUp]
        public void Setup()
        {
            ServiceLocator.Clear();
            _config = ScriptableObject.CreateInstance<GameConfig>();
            _config.glacierClicksToBreak     = 3;
            _config.glacierPassiveDecayRate  = 1f;  // loses all health in 1 second
            _config.glacierRegenTime         = 2f;
            ServiceLocator.Register<GameConfig>(_config);
            _go     = new GameObject();
            _glacier = _go.AddComponent<Glacier>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
            Object.DestroyImmediate(_config);
            ServiceLocator.Clear();
            GameEventBus.ClearAll();
        }

        [Test]
        public void InitialState_IsIdle_FullHealth()
        {
            Assert.AreEqual(GlacierState.Idle, _glacier.State);
            Assert.AreEqual(1f, _glacier.PassiveHealth, 0.001f);
        }

        [Test]
        public void OnClick_IncrementsClickCount()
        {
            _glacier.OnClick();
            Assert.AreEqual(1, _glacier.ClickCount);
        }

        [Test]
        public void OnClick_AtThreshold_SpawnsIceCube_AndEntersRegen()
        {
            bool spawned = false;
            GameEventBus.OnIceCubeSpawned += () => spawned = true;
            _glacier.OnClick();
            _glacier.OnClick();
            _glacier.OnClick();
            Assert.IsTrue(spawned);
            Assert.AreEqual(GlacierState.Regenerating, _glacier.State);
        }

        [Test]
        public void OnClick_WhenRegenerating_DoesNothing()
        {
            _glacier.OnClick();
            _glacier.OnClick();
            _glacier.OnClick(); // enters regen
            _glacier.OnClick();
            Assert.AreEqual(0, _glacier.ClickCount); // reset, not incremented further
        }

        [Test]
        public void Tick_PassiveDecay_FiresMistakeWhenHealthZero()
        {
            bool mistake = false;
            GameEventBus.OnMistake += () => mistake = true;
            _glacier.Tick(1.1f); // decay rate 1f → health 0 after 1s
            Assert.IsTrue(mistake);
        }

        [Test]
        public void Tick_PassiveDecay_EntersRegenAfterMistake()
        {
            _glacier.Tick(1.1f);
            Assert.AreEqual(GlacierState.Regenerating, _glacier.State);
        }

        [Test]
        public void Tick_Regen_ReturnsToIdleWhenComplete()
        {
            _glacier.OnClick();
            _glacier.OnClick();
            _glacier.OnClick(); // enters regen
            _glacier.Tick(2.1f); // regenTime = 2f
            Assert.AreEqual(GlacierState.Idle, _glacier.State);
            Assert.AreEqual(1f, _glacier.PassiveHealth, 0.001f);
        }
    }
}
