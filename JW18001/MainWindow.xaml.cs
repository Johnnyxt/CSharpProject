using JW18001.ViewModels;
using System.Windows;

namespace JW18001
{
    /// <summary>
    /// MainLoginView.xaml 的交互逻辑
    /// </summary>
    public partial class MainLoginView : Window
    {
        public MainLoginView()
        {
            InitializeComponent();
            IsEnabledChanged += MainLoginView_IsEnabledChanged;
            DataContext = new MainLoginViewModel();
        }

        private void MainLoginView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                Hide();
            }
        }
    }
}