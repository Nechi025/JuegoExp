using NUnit.Framework;
using UnityEngine;
using Core.Services;

namespace Game.Tests
{
    public class MistakeManagerTests
    {
        private GameObject _go;
        private MistakeManager _manager;
        private GameConfig _config;

        [SetUp]
        public void Setup()
        {
            GameEventBus.ClearAll();
            ServiceLocator.Clear();
            _config = ScriptableObject.CreateInstance<GameConfig>();
            _config.maxMistakes = 3;
            ServiceLocator.Register<GameConfig>(_config);
            _go = new GameObject();
            _manager = _go.AddComponent<MistakeManager>();
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
        public void RaiseMistake_IncrementsCount()
        {
            GameEventBus.RaiseMistake();
            Assert.AreEqual(1, _manager.CurrentMistakes);
        }

        [Test]
        public void RaiseMistake_AtMaxMistakes_FiresOnGameOver()
        {
            bool fired = false;
            GameEventBus.OnGameOver += () => fired = true;
            GameEventBus.RaiseMistake();
            GameEventBus.RaiseMistake();
            GameEventBus.RaiseMistake();
            Assert.IsTrue(fired);
        }

        [Test]
        public void AfterGameOver_RaiseMistake_DoesNotIncrementCount()
        {
            GameEventBus.RaiseStateChanged(GameState.GameOver);
            GameEventBus.RaiseMistake();
            Assert.AreEqual(0, _manager.CurrentMistakes);
        }

        [Test]
        public void RaiseMistake_BelowMax_DoesNotFireOnGameOver()
        {
            bool fired = false;
            GameEventBus.OnGameOver += () => fired = true;
            GameEventBus.RaiseMistake();
            GameEventBus.RaiseMistake();
            Assert.IsFalse(fired);
        }
    }
}
