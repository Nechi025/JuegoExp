using NUnit.Framework;
using Game.Panels.Funnel;

namespace Game.Tests
{
    public class BottleTests
    {
        [Test]
        public void NewBottle_IsNotFull()
        {
            var bottle = new Bottle(3);
            Assert.IsFalse(bottle.IsFull);
        }

        [Test]
        public void AddPureCube_IncreasesFillCount()
        {
            var bottle = new Bottle(3);
            bottle.AddCube(isPure: true);
            Assert.AreEqual(1, bottle.FillCount);
        }

        [Test]
        public void AddDirtyCube_TaintsBottle()
        {
            var bottle = new Bottle(3);
            bottle.AddCube(isPure: false);
            Assert.IsTrue(bottle.IsTainted);
        }

        [Test]
        public void AddOnlyPureCubes_BottleIsNotTainted()
        {
            var bottle = new Bottle(3);
            bottle.AddCube(isPure: true);
            bottle.AddCube(isPure: true);
            Assert.IsFalse(bottle.IsTainted);
        }

        [Test]
        public void AddCubesUpToCapacity_BottleIsFull()
        {
            var bottle = new Bottle(2);
            bottle.AddCube(isPure: true);
            bottle.AddCube(isPure: true);
            Assert.IsTrue(bottle.IsFull);
        }

        [Test]
        public void AddCubeWhenFull_DoesNotIncreaseFillCount()
        {
            var bottle = new Bottle(1);
            bottle.AddCube(isPure: true);
            bottle.AddCube(isPure: true);
            Assert.AreEqual(1, bottle.FillCount);
        }
    }
}
