using JW18001.ViewModels;
using System;
using System.Windows;

namespace JW18001.Views
{
    /// <summary>
    /// DbLoginView.xaml 的交互逻辑
    /// </summary>
    public partial class DbLoginView
    {
        public DbLoginView()
        {
            InitializeComponent();
            DataContext = new DbLoginViewModel();
            IsEnabledChanged += DbLoginView_IsEnabledChanged;
        }

        private void DbLoginView_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                Hide();
            }
        }

        private void DbLoginView_OnClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}