﻿using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace NucleusWPF.MVVM
{
    /// <summary>
    /// Provides functionality for creating and displaying WPF windows associated with view models.
    /// Supports explicit view/viewmodel mapping and automatic view resolution by naming convention.
    /// </summary
    public class WindowService : IWindowService
    {
        private WindowService()
        {

        }

        private static IWindowService? _instance;

        /// <inheritdoc/>
        public static IWindowService Instance =>
            _instance ??= new WindowService();

        private readonly Dictionary<Type, Type> _mappedWindows = [];

        private string defaultViewSuffix = "Window";

        /// <inheritdoc/>
        public string DefaultViewSuffix
        {
            get => defaultViewSuffix;
            set => defaultViewSuffix = value;
        }

        private const string _viewModelSuffix = "ViewModel";

        /// <inheritdoc/>
        public void Register<TViewModel, TWindow>()
            where TViewModel : class
            where TWindow : Window =>
            _mappedWindows[typeof(TViewModel)] = typeof(TWindow);

        /// <inheritdoc/>
        public void Show(object viewModel, string? suffix = null)
        {
            var w = GetWindow(viewModel);
            w.Show();
        }

        /// <inheritdoc/>
        public void Show<TViewModel>(string? suffix = null) where TViewModel : class
        {
            var viewModel = DependencyInjection.Instance.Resolve<TViewModel>();
            var w = GetWindow(viewModel, suffix);
            w.Show();
        }

        /// <inheritdoc/>
        public bool? ShowDialog(object viewModel, string? suffix = null)
        {
            var w = GetWindow(viewModel);
            return w.ShowDialog();
        }

        /// <inheritdoc/>
        public bool? ShowDialog<TViewModel>(string? suffix = null) where TViewModel : class
        {
            var viewModel = DependencyInjection.Instance.Resolve<TViewModel>();
            var w = GetWindow(viewModel, suffix);
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
