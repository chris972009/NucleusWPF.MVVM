﻿namespace NucleusWPF.MVVM
{
    /// <summary>
    /// Provides a dependency injection container for transient and singleton servies and models.
    /// </summary>
    public class DependencyInjection
    {
        private DependencyInjection()
        {

        }

        private static DependencyInjection? _instance;
        /// <summary>
        /// Gets the singleton instance of the DependencyInjection class.
        /// </summary>
        public static DependencyInjection Instance =>
            _instance ??= new DependencyInjection();

        private readonly Dictionary<Type, Type> _interfacesMap = new()
        {
            { typeof(IMessageService), typeof(MessageService) },
        };

        private readonly Dictionary<Type, object> _singletonsMap = new()
        {
            { typeof(IWindowService), WindowService.Instance },
        };

        /// <summary>
        /// Register an interface to its implementation type.
        /// </summary>
        /// <typeparam name="TInterface">Interface to be registered.</typeparam>
        /// <typeparam name="TImplementation">Implementation of interface</typeparam>
        public void Register<TInterface, TImplementation>() where TImplementation : TInterface =>
            _interfacesMap[typeof(TInterface)] = typeof(TImplementation);

        /// <summary>
        /// Registers an interface its implementation type as a singleton.
        /// </summary>
        /// <typeparam name="TInterface">Interface to be registered.</typeparam>
        /// <typeparam name="TImplementation">Implementation of interface</typeparam>
        public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface, new()
        {
            var instance = new TImplementation();
            _singletonsMap[typeof(TInterface)] = instance;
        }

        /// <summary>
        /// REgister an instance of an interface as a singleton.
        /// </summary>
        /// <typeparam name="TInterface">Interface to be registered</typeparam>
        /// <param name="instance">Implementation of interface</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterSingleton<TInterface>(TInterface instance) =>
            _singletonsMap[typeof(TInterface)] = instance ?? throw new ArgumentNullException(nameof(instance));

        /// <summary>
        /// Resolves an intance of the specified interface type.
        /// </summary>
        /// <typeparam name="TInterface">Interface to resolve</typeparam>
        /// <returns>Returns the resolved implementation.</returns>
        public TInterface Resolve<TInterface>() =>
            (TInterface)Resolve(typeof(TInterface));

        /// <summary>
        /// Reset all registered services and models.
        /// </summary>
        public void Clear()
        {
            _interfacesMap.Clear();
            Register<IMessageService, MessageService>();
            _singletonsMap.Clear();
            RegisterSingleton(WindowService.Instance);
        }

        private object Resolve(Type type)
        {
            // Check for singleton
            if (_singletonsMap.TryGetValue(type, out var singleton))
            {
                return singleton;
            }

            // Check for mapped interface
            if (_interfacesMap.TryGetValue(type, out var implementationType))
            {
                type = implementationType;
            }

            // Get the constructor with the most parameters
            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            _ = constructor ?? throw new InvalidOperationException($"No public constructors for for {type}.");

            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
                return Activator.CreateInstance(type)!;

            var parameterInstances = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                parameterInstances[i] = Resolve(parameters[i].ParameterType);

            return constructor.Invoke(parameterInstances);
        }
    }
}
