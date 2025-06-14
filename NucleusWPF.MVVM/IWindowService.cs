using System.Windows;

namespace NucleusWPF.MVVM
{
    public interface IWindowService
    {
        string DefaultViewSuffix { get; set; }

        void Register<TViewModel, TWindow>() where TViewModel : class where TWindow : Window;

        void Show(object viewModel, string? suffix = null);

        bool? ShowDialog(object viewModel, string? suffix = null);
    }
}
