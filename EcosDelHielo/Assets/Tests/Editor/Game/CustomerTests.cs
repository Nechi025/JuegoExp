using NUnit.Framework;
using UnityEngine;
using Core.Services;
using Game.Panels.Customers;

namespace Game.Tests
{
    public class CustomerTests
    {
        private GameObject _go;
        private Customer   _customer;
        private GameConfig _config;

        [SetUp]
        public void Setup()
        {
            ServiceLocator.Clear();
            _config = ScriptableObject.CreateInstance<GameConfig>();
            _config.customerPatienceTime = 5f;
            ServiceLocator.Register<GameConfig>(_config);
            _go       = new GameObject();
            _customer = _go.AddComponent<Customer>();
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
        public void NewCustomer_IsActive()
        {
            Assert.IsTrue(_customer.IsActive);
        }

        [Test]
        public void Serve_DeactivatesCustomer()
        {
            _customer.Serve();
            Assert.IsFalse(_customer.IsActive);
        }

        [Test]
        public void Tick_PatienceExpires_FiresMistake()
        {
            bool mistake = false;
            GameEventBus.OnMistake += () => mistake = true;
            _customer.Tick(5.1f);
            Assert.IsTrue(mistake);
        }

        [Test]
        public void Tick_PatienceExpires_DeactivatesCustomer()
        {
            _customer.Tick(5.1f);
            Assert.IsFalse(_customer.IsActive);
        }

        [Test]
        public void Tick_AfterServed_DoesNotFireMistake()
        {
            _customer.Serve();
            bool mistake = false;
            GameEventBus.OnMistake += () => mistake = true;
            _customer.Tick(10f);
            Assert.IsFalse(mistake);
        }

        [Test]
        public void PatienceNormalized_StartsAtOne()
        {
            Assert.AreEqual(1f, _customer.PatienceNormalized, 0.001f);
        }

        [Test]
        public void PatienceNormalized_DropsWithTick()
        {
            _customer.Tick(2.5f);
            Assert.AreEqual(0.5f, _customer.PatienceNormalized, 0.001f);
        }
    }
}
