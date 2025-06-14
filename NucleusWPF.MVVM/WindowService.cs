using System.Windows;

namespace NucleusWPF.MVVM
{
    public class WindowService : IWindowService
    {
        private static IWindowService? _instance;
        public static IWindowService Instance =>
            _instance ??= new WindowService();

        private readonly Dictionary<Type, Type> _mappedWindows = [];

        private string defaultViewSuffix = "Window";
        public string DefaultViewSuffix
        {
            get => defaultViewSuffix;
            set => defaultViewSuffix = value;
        }

        private const string _viewModelSuffix = "ViewModel";

        public void Register<TViewModel, TWindow>()
            where TViewModel : class
            where TWindow : Window =>
            _mappedWindows[typeof(TViewModel)] = typeof(TWindow);

        private static Window InitializeWindow(object viewModel, Type windowType)
        {
            var window = Activator.CreateInstance(windowType) as Window;
            _ = window ?? throw new InvalidOperationException($"Could not create instance of {windowType}");
            window.DataContext = viewModel;
            return window;
        }

        private Window GetWindow(object viewModel, string? suffix = null)
        {
            var viewModelType = viewModel.GetType();
            if (_mappedWindows.TryGetValue(viewModelType, out var windowType))
                return InitializeWindow(viewModelType, windowType);

            var viewModelName = viewModelType.Name;

            if (!viewModelName.EndsWith(_viewModelSuffix, StringComparison.Ordinal))
                throw new ArgumentException($"'{viewModel}' must end with '{_viewModelSuffix}'");

            var viewSuffix = suffix ?? defaultViewSuffix;
            var viewName = viewModelName.AsSpan(0, _viewModelSuffix.Length).ToString() + viewSuffix;
            viewName = viewName.Replace(".ViewModels.", ".Views.");
            var viewType = Type.GetType(viewName);
            _ = viewType ?? throw new InvalidOperationException($"Could not find a view for '{viewModel}'");
            return InitializeWindow(viewModel, viewType);
        }

        public void Show(object viewModel, string? suffix = null)
        {
            var w = GetWindow(viewModel);
            w.Show();
        }

        public bool? ShowDialog(object viewModel, string? suffix = null)
        {
            var w = GetWindow(viewModel);
            return w.ShowDialog();
        }
    }
}
