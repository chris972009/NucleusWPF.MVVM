using NucleusWPF.MVVM;

namespace TestApp
{
    [TestClass]
    public class DependencyInjectionTest
    {
        [TestInitialize]
        public void Setup()
        {
            DependencyInjection.Instance.Clear();
        }

        [TestMethod]
        public void ResolveTransientTest()
        {
            // IMessageService is registered as transient by default.
            var service = DependencyInjection.Instance.Resolve<IMessageService>();
            Assert.IsInstanceOfType(service, typeof(IMessageService));
            var service2 = DependencyInjection.Instance.Resolve<IMessageService>();
            Assert.AreNotSame(service, service2, "Transient services should not be the same instance.");
        }

        [TestMethod]
        public void ResolveSingletonTest()
        {
            // IWindowService is registered as a singleton by default.
            var service = DependencyInjection.Instance.Resolve<IWindowService>();
            Assert.IsInstanceOfType(service, typeof(IWindowService));
            var service2 = DependencyInjection.Instance.Resolve<IWindowService>();
            Assert.AreSame(service, service2, "Singleton services should be the same instance.");
        }

        [TestMethod]
        public void ResolveViewModelTest()
        {
            var viewModel = DependencyInjection.Instance.Resolve<TestViewModel>();
            Assert.IsInstanceOfType(viewModel, typeof(TestViewModel));
            Assert.IsInstanceOfType(viewModel.MessageService, typeof(IMessageService), "ViewModel should have IMessageService injected.");
        }

        [TestMethod]
        public void RegisterTransientTest()
        {
            DependencyInjection.Instance.Register<ITransientService, TransientService>();
            var service = DependencyInjection.Instance.Resolve<ITransientService>();
            Assert.IsInstanceOfType(service, typeof(TransientService));
            var service2 = DependencyInjection.Instance.Resolve<ITransientService>();
            Assert.AreNotSame(service, service2, "Service was registed as Singleton");
        }

        [TestMethod]
        public void RegisterSingletonTest()
        {
            DependencyInjection.Instance.RegisterSingleton<ISingletonService, SingletonService>();
            var service = DependencyInjection.Instance.Resolve<ISingletonService>();
            Assert.IsInstanceOfType(service, typeof(SingletonService));
            var service2 = DependencyInjection.Instance.Resolve<ISingletonService>();
            Assert.AreSame(service, service2, "Service was registered as transient.");
        }

        private class TestViewModel(IMessageService messageService)
        {
            public IMessageService MessageService { get; } = messageService;
        }

        private interface ITransientService { }

        private class TransientService : ITransientService { }

        private interface ISingletonService { }

        private class SingletonService : ISingletonService { }
    }
}
