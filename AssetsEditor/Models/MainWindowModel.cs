using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Assets.Editor.Views;
using Xaml.Effects.Toolkit;
using Xaml.Effects.Toolkit.Model;
using Xaml.Effects.Toolkit.Uitity;
using static System.Net.WebRequestMethods;

namespace Assets.Editor.Models
{
    public class MainWindowModel : DialogModel
    {

        string script = @"
                function findParent (parent){
                    var count = 0;
                    while(parent.parentElement != null && count < 8){
                        if(parent.parentElement.className.startsWith('itemCol')) return parent.parentElement;
                        parent = parent.parentElement;
                        count++;
                    }
                    return null;
                }
                document.addEventListener('click', function (event)
                {
                      var root = findParent(event.srcElement)
                        console.log(root);
                      if(root){
                          var titleElement = root.querySelector('div[class^=""title__""]');
                          console.log(titleElement);
                          if(titleElement){
                               window.chrome.webview.postMessage('@'+titleElement.innerText);
                          }
                      }
                });

                

                document.body.style.opacity = '0.1';
                console.log('监视中');
";





        /// <summary>
        /// 应用,需要时在派生类中重写
        /// </summary>
        public ICommand VideoListCommand { get; protected set; }


        /// <summary>
        /// 应用,需要时在派生类中重写
        /// </summary>
        public ICommand ThemesCommand { get; protected set; }


        public ICommand SettingCommand { get; protected set; }


        


        public ICommand HomeCommand { get; protected set; }

        public ICommand RefreshCommand { get; protected set; }

        public ICommand GoBackCommand { get; protected set; }




        public ObservableCollection<VideoInfo> VideoList { get; protected set; }




        public MainWindowModel()
        {
            Trace.WriteLine("do");
            this.SettingCommand = new RelayCommand(Settings_Click);
            this.ThemesCommand = new RelayCommand(Themes_Click);
            this.VideoListCommand = new RelayCommand(VideoList_Click);
            this.HomeCommand = new RelayCommand(Home_Click);
            this.GoBackCommand = new RelayCommand(Goback_Click);
            this.RefreshCommand = new RelayCommand(Refresh_Click);
            this.VideoList = new ObservableCollection<VideoInfo>();
            this.webOpacity = 100;
            this.Title = "Console";
        }








        private void Themes_Click()
        {
            // /Assets.Editor;component/Assets/Themes/background.png
            if (ThemeManager.CurrentTheme == "Default")
            {
                ThemeManager.LoadThemeFromResource("/Assets.Editor;component/Assets/Themes/White.xaml");
            }
            else if (ThemeManager.CurrentTheme == "White")
            {
                ThemeManager.LoadThemeFromResource("/Assets.Editor;component/Assets/Themes/Black.xaml");
            }
            else if (ThemeManager.CurrentTheme == "Black")
            {
                ThemeManager.LoadThemeFromResource("/Assets.Editor;component/Assets/Themes/Image.xaml");
            }
            else
            {
                ThemeManager.LoadThemeFromResource("/Assets.Editor;component/Assets/Themes/White.xaml");
            }
            //Console.WriteLine(ThemeManager.CurrentTheme);
        }


        

        private void Settings_Click()
        {

        }

        private void Goback_Click()
        {

        }

        private void Refresh_Click()
        {
   
        }


        private void Home_Click()
        {

        }


        private void VideoList_Click()
        {
            if (this.VideoListWindow != null)
            {
                this.VideoListWindow.Close();
                return;
            }
            this.VideoListWindow = new VideoList(this.VideoList);
            this.VideoListWindow.Closed += Video_Closed;

            this.VideoListWindow.Show();

        }


        private VideoList  VideoListWindow { get; set; }


        private void Video_Closed(object sender, EventArgs e)
        {
            this.VideoListWindow = null;
        }

        public Int32 WebOpacity
        {
            get
            {
                return this.webOpacity;
            }
            set
            {
                base.SetProperty(ref this.webOpacity, value);
            }
        }

        private Int32 webOpacity;

    }
}
