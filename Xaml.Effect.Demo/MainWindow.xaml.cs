using System;
using System.Windows;
using Xaml.Effect.Demo.Models;

namespace Xaml.Effect.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowModel Model { get; set; } = new MainWindowModel();



        public MainWindow()
        {
            this.DataContext = Model;
            InitializeComponent();
            this.Model.OnClose += Model_OnClose;
        }

        private void Model_OnClose(object sender, Effects.Toolkit.Model.WindowDestroyArgs e)
        {
            Environment.Exit(0);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Model.InitWebView(WebCore);
        }
    }
}
