using NUnit.Framework;
using UnityEngine;
using Core.Services;

namespace Game.Tests
{
    public class ScoreManagerTests
    {
        private GameObject _go;
        private ScoreManager _manager;
        private GameConfig _config;

        [SetUp]
        public void Setup()
        {
            ServiceLocator.Clear();
            _config = ScriptableObject.CreateInstance<GameConfig>();
            _config.scorePerDelivery = 100;
            _config.scorePerSecond   = 10f;
            ServiceLocator.Register<GameConfig>(_config);
            _go = new GameObject();
            _manager = _go.AddComponent<ScoreManager>();
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
        public void OnDeliverySuccess_AddsScorePerDelivery()
        {
            GameEventBus.RaiseDeliverySuccess();
            Assert.AreEqual(100, _manager.Score);
        }

        [Test]
        public void OnDeliverySuccess_IncrementsDeliveries()
        {
            GameEventBus.RaiseDeliverySuccess();
            GameEventBus.RaiseDeliverySuccess();
            Assert.AreEqual(2, _manager.DeliveriesCompleted);
        }

        [Test]
        public void Tick_AccumulatesTimeScore()
        {
            _manager.Tick(1f);
            Assert.AreEqual(10, _manager.Score);
        }

        [Test]
        public void Tick_AccumulatesTimeAlive()
        {
            _manager.Tick(2.5f);
            Assert.AreEqual(2.5f, _manager.TimeAlive, 0.001f);
        }

        [Test]
        public void AfterGameOver_Tick_DoesNotAccumulate()
        {
            GameEventBus.RaiseStateChanged(GameState.GameOver);
            _manager.Tick(5f);
            Assert.AreEqual(0f, _manager.TimeAlive, 0.001f);
        }

        [Test]
        public void AfterGameOver_OnDeliverySuccess_DoesNotScore()
        {
            GameEventBus.RaiseStateChanged(GameState.GameOver);
            GameEventBus.RaiseDeliverySuccess();
            Assert.AreEqual(0, _manager.Score);
        }
    }
}
