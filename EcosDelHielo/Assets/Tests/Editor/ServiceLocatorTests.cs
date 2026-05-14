using System;
using NUnit.Framework;
using Core.Services;

namespace Core.Tests
{
    public class ServiceLocatorTests
    {
        private interface ITestService : IService { }
        private class TestService : ITestService { }

        [SetUp]
        public void SetUp() => ServiceLocator.Clear();

        [TearDown]
        public void TearDown() => ServiceLocator.Clear();

        [Test]
        public void Get_UnregisteredService_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => ServiceLocator.Get<ITestService>());
        }

        [Test]
        public void Register_ThenGet_ReturnsSameInstance()
        {
            var service = new TestService();
            ServiceLocator.Register<ITestService>(service);
            Assert.AreSame(service, ServiceLocator.Get<ITestService>());
        }

        [Test]
        public void Register_Twice_OverwritesFirst()
        {
            var first = new TestService();
            var second = new TestService();
            ServiceLocator.Register<ITestService>(first);
            ServiceLocator.Register<ITestService>(second);
            Assert.AreSame(second, ServiceLocator.Get<ITestService>());
        }

        [Test]
        public void TryGet_UnregisteredService_ReturnsFalse()
        {
            Assert.IsFalse(ServiceLocator.TryGet<ITestService>(out _));
        }

        [Test]
        public void TryGet_RegisteredService_ReturnsTrueAndService()
        {
            var service = new TestService();
            ServiceLocator.Register<ITestService>(service);
            var found = ServiceLocator.TryGet<ITestService>(out var result);
            Assert.IsTrue(found);
            Assert.AreSame(service, result);
        }

        [Test]
        public void Unregister_ThenGet_ThrowsInvalidOperationException()
        {
            ServiceLocator.Register<ITestService>(new TestService());
            ServiceLocator.Unregister<ITestService>();
            Assert.Throws<InvalidOperationException>(() => ServiceLocator.Get<ITestService>());
        }

        [Test]
        public void Clear_RemovesAllServices()
        {
            ServiceLocator.Register<ITestService>(new TestService());
            ServiceLocator.Clear();
            Assert.Throws<InvalidOperationException>(() => ServiceLocator.Get<ITestService>());
        }
    }
}
