using Assets.Editor.Models;
using System.Windows;

namespace Assets.Editor.Views
{
    /// <summary>
    /// CreateAssetsPackage.xaml 的交互逻辑
    /// </summary>
    public partial class CreateAssetsInput : Window
    {

        public CreateAssetsInputModel Model { get; set; } = new CreateAssetsInputModel();

        public CreateAssetsInput()
        {
            this.DataContext = Model;
            InitializeComponent();
            this.Model.OnClose += Model_OnClose;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Model_OnClose(object sender, Xaml.Effects.Toolkit.Model.WindowDestroyArgs e)
        {
            this.DialogResult = e.DialogResult;
        }


    }
}
