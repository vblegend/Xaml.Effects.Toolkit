using System;
using System.Windows;
using Assets.Editor.Models;

namespace Assets.Editor
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

        private void Model_OnClose(object sender, Xaml.Effects.Toolkit.Model.WindowDestroyArgs e)
        {
            Environment.Exit(0);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
