using System.Windows;

namespace NucleusWPF.MVVM
{
    /// <summary>
    /// Provides functionality for creating and displaying WPF windows associated with view models.
    /// Supports explicit view/viewmodel mapping and automatic view resolution by naming convention.
    /// </summary>
    public class WindowService : IWindowService
    {
        private WindowService()
        {

        }

        private static IWindowService? _instance;

        /// <summary>
        /// Gets the singleton instance of the <see cref="WindowService"/>.
        /// </summary>
        public static IWindowService Instance =>
            _instance ??= new WindowService();

        private readonly Dictionary<Type, Type> _mappedWindows = [];

        private string defaultViewSuffix = "Window";

        /// <summary>
        /// Gets or sets the default suffix appended to view names when resolving views by convention.
        /// </summary>
        public string DefaultViewSuffix
        {
            get => defaultViewSuffix;
            set => defaultViewSuffix = value;
        }

        private const string _viewModelSuffix = "ViewModel";

        /// <summary>
        /// Registers an explicit mapping between a ViewModel type and a Window type.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type to register.</typeparam>
        /// <typeparam name="TWindow">The Window type to map to the ViewModel.</typeparam>
        public void Register<TViewModel, TWindow>()
            where TViewModel : class
            where TWindow : Window =>
            _mappedWindows[typeof(TViewModel)] = typeof(TWindow);

        /// <summary>
        /// Opens a window with its DataContext set to the specified ViewModel.
        /// The window type is resolved by explicit mapping or by naming convention.
        /// </summary>
        /// <param name="viewModel">The ViewModel to associate with the window's DataContext.</param>
        /// <param name="suffix">The suffix for the view name. If null, <see cref="DefaultViewSuffix"/> is used.</param>
        public void Show(object viewModel, string? suffix = null)
        {
            var w = GetWindow(viewModel);
            w.Show();
        }

        /// <summary>
        /// Opens a window as a dialog with its DataContext set to the specified ViewModel.
        /// The window type is resolved by explicit mapping or by naming convention.
        /// </summary>
        /// <param name="viewModel">The ViewModel to associate with the window's DataContext.</param>
        /// <param name="suffix">The suffix for the view name. If null, <see cref="DefaultViewSuffix"/> is used.</param>
        /// <returns>Returns the dialog result of the window.</returns>
        public bool? ShowDialog(object viewModel, string? suffix = null)
        {
            var w = GetWindow(viewModel);
            return w.ShowDialog();
        }

        //Initialiez a window instance with type and assign ViewModel as DataContext
        private static Window InitializeWindow(object viewModel, Type windowType)
        {
            var window = Activator.CreateInstance(windowType) as Window;
            _ = window ?? throw new InvalidOperationException($"Could not create instance of {windowType}");
            window.DataContext = viewModel;
            return window;
        }

        private Window GetWindow(object viewModel, string? suffix = null)
        {
            //get ViewModel type and check for explicit mapping
            var viewModelType = viewModel.GetType();
            if (_mappedWindows.TryGetValue(viewModelType, out var windowType))
                return InitializeWindow(viewModelType, windowType);

            //check that ViewModel meets naming convention
            var viewModelName = viewModelType.Name;
            if (!viewModelName.EndsWith(_viewModelSuffix, StringComparison.Ordinal))
                throw new ArgumentException($"'{viewModel}' must end with '{_viewModelSuffix}'");

            //resolve view type by convention
            var viewSuffix = suffix ?? defaultViewSuffix;
            var viewName = viewModelName.AsSpan(0, _viewModelSuffix.Length).ToString() + viewSuffix;
            viewName = viewName.Replace(".ViewModels.", ".Views.");
            var viewType = Type.GetType(viewName);
            _ = viewType ?? throw new InvalidOperationException($"Could not find a view for '{viewModel}'");
            return InitializeWindow(viewModel, viewType);
        }
    }
}
