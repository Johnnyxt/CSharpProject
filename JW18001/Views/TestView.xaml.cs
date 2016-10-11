using JW18001.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JW18001.Views
{
    /// <summary>
    /// TestView.xaml 的交互逻辑
    /// </summary>
    public partial class TestView : Window
    {
        public TestView()
        {
            InitializeComponent();
            DataContext = new TestViewModel();
        }

        private void TestView_OnClosed(object sender, EventArgs e)
        {
            Person.IsThreadFlag = false;
            Application.Current.Shutdown();
        }
    }
}