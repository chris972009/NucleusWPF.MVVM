namespace NucleusWPF.MVVM
{
    public class DependencyInjection
    {
        private static DependencyInjection? _instance;
        public static DependencyInjection Instance
        {
            get
            {
                if (_instance == null) _instance = new();
                return _instance;
            }
        }

        private readonly Dictionary<Type, Type> _interfacesMap = [];
        private readonly Dictionary<Type, object> _singletonsMap = [];

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _interfacesMap[typeof(TInterface)] = typeof(TImplementation);
        }

        public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface, new()
        {
            var instance = new TImplementation();
            _singletonsMap[typeof(TInterface)] = instance;
        }

        public void RegisterSingleton<TInterface>(TInterface instance)
        {
            _singletonsMap[typeof(TInterface)] = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public TInterface Resolve<TInterface>()
        {
            return (TInterface)Resolve(typeof(TInterface));
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

            if (constructor == null)
                throw new InvalidOperationException($"No public constructors found for {type}.");

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
