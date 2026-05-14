using NUnit.Framework;

namespace Game.Tests
{
    public class GameEventBusTests
    {
        [TearDown]
        public void TearDown() => GameEventBus.ClearAll();

        [Test]
        public void OnMistake_FiresSubscribedHandler()
        {
            bool fired = false;
            GameEventBus.OnMistake += () => fired = true;
            GameEventBus.RaiseMistake();
            Assert.IsTrue(fired);
        }

        [Test]
        public void OnBottleReady_PassesIsPureValue()
        {
            bool? received = null;
            GameEventBus.OnBottleReady += isPure => received = isPure;
            GameEventBus.RaiseBottleReady(false);
            Assert.AreEqual(false, received);
        }

        [Test]
        public void OnStateChanged_PassesState()
        {
            GameState? received = null;
            GameEventBus.OnStateChanged += s => received = s;
            GameEventBus.RaiseStateChanged(GameState.GameOver);
            Assert.AreEqual(GameState.GameOver, received);
        }

        [Test]
        public void ClearAll_PreventsSubsequentFiring()
        {
            int count = 0;
            GameEventBus.OnMistake += () => count++;
            GameEventBus.ClearAll();
            GameEventBus.RaiseMistake();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void OnDeliverySuccess_FiresSubscribedHandler()
        {
            bool fired = false;
            GameEventBus.OnDeliverySuccess += () => fired = true;
            GameEventBus.RaiseDeliverySuccess();
            Assert.IsTrue(fired);
        }
    }
}
