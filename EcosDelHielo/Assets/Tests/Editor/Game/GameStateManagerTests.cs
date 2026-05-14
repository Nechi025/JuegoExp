using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class GameStateManagerTests
    {
        private GameObject _go;
        private GameStateManager _manager;

        [SetUp]
        public void Setup()
        {
            GameEventBus.ClearAll();
            _go = new GameObject();
            _manager = _go.AddComponent<GameStateManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
            GameEventBus.ClearAll();
        }

        [Test]
        public void InitialState_IsPlaying()
        {
            Assert.AreEqual(GameState.Playing, _manager.CurrentState);
        }

        [Test]
        public void RaiseGameOver_TransitionsToGameOverState()
        {
            GameEventBus.RaiseGameOver();
            Assert.AreEqual(GameState.GameOver, _manager.CurrentState);
        }

        [Test]
        public void RaiseGameOver_FiresOnStateChangedWithGameOver()
        {
            GameState? received = null;
            GameEventBus.OnStateChanged += s => received = s;
            GameEventBus.RaiseGameOver();
            Assert.AreEqual(GameState.GameOver, received);
        }

        [Test]
        public void RaiseGameOver_CalledTwice_OnStateChangedFiresOnce()
        {
            int count = 0;
            GameEventBus.OnStateChanged += _ => count++;
            GameEventBus.RaiseGameOver();
            GameEventBus.RaiseGameOver();
            Assert.AreEqual(1, count);
        }
    }
}
