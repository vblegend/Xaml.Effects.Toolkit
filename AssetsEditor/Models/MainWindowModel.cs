using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Resource.Package.Assets;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xaml.Effects.Toolkit;
using Xaml.Effects.Toolkit.Model;
using Xaml.Effects.Toolkit.Uitity;
using static System.Net.WebRequestMethods;
using System.Windows;

namespace Assets.Editor.Models
{

    public class ImageModel : ObservableObject
    {
        public ImageModel()
        {

        }



        public BitmapSource Source
        {
            get
            {
                return this.image;
            }
            set
            {
                base.SetProperty(ref this.image, value);
            }
        }

        private BitmapSource image;




        public Int32 Index
        {
            get
            {
                return this.index;
            }
            set
            {
                base.SetProperty(ref this.index, value);
            }
        }

        private Int32 index;


    }





    public class MainWindowModel : DialogModel
    {

        public ObservableCollection<String> ListItems { get; set; } = new ObservableCollection<String>();

        public ObservableCollection<ImageModel> GridImages { get; set; } = new ObservableCollection<ImageModel>();

        /// <summary>
        /// 应用,需要时在派生类中重写
        /// </summary>
        public ICommand VideoListCommand { get; protected set; }

        /// <summary>
        /// 应用,需要时在派生类中重写
        /// </summary>
        public ICommand ThemesCommand { get; protected set; }


        public ICommand SettingCommand { get; protected set; }


        public ICommand PreviewMouseWheelCommand { get; protected set; }

        public ICommand PageChangedCommand { get; protected set; }


        public ICommand HomeCommand { get; protected set; }

        public ICommand RefreshCommand { get; protected set; }

        public ICommand GoBackCommand { get; protected set; }



        public AssetFileStream assetFile { get; protected set; }


        public MainWindowModel()
        {
            Trace.WriteLine("do");
            this.SettingCommand = new RelayCommand(Settings_Click);
            this.ThemesCommand = new RelayCommand(Themes_Click);
            this.VideoListCommand = new RelayCommand(VideoList_Click);
            this.HomeCommand = new RelayCommand(Home_Click);
            this.GoBackCommand = new RelayCommand(Goback_Click);
            this.RefreshCommand = new RelayCommand(Refresh_Click);
            this.PreviewMouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(MovieListView_PreviewMouseWheel);
            this.PageChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<double>>(ScrollBar_ValueChanged);
            this.currentPage = 50;
            this.Title = "Console";
            this.PageSize = 64;

            this.assetFile = AssetFileStream.Open("hum.asset", "123");
            resizePage();
            refreshPage();
        }

        private void resizePage()
        {
            var pagesize2 = this.assetFile.NumberOfFiles - (this.CurrentPage * this.PageSize);
            var pagesize = Math.Min(this.PageSize, pagesize2);
            this.PageElementCount = pagesize;
            this.GridImages.Clear();
            for (int i = 0; i < pagesize; i++)
            {
                this.GridImages.Add(new ImageModel());
            }
            this.TotalPage = this.assetFile.NumberOfFiles / this.PageSize;
            if (this.CurrentPage > this.totalPage)
            {
                this.CurrentPage = this.totalPage;
            }
        }

        private void refreshPage()
        {
            var pagesize2 = this.assetFile.NumberOfFiles - (this.CurrentPage * this.PageSize);
            var pagesize = Math.Min(this.PageSize, pagesize2);
            if (this.PageElementCount != pagesize)
            {
                this.resizePage();
            }
            var startAt = this.CurrentPage * this.PageSize;
            for (int i = 0; i < pagesize; i++)
            {
                if (startAt + i < this.assetFile.NumberOfFiles)
                {
                    var node = this.assetFile.Read(startAt + i);
                    this.GridImages[i].Index = startAt + i;
                    this.GridImages[i].Source = loadImageSource(node.Data);
                }
            }
        }



        private BitmapSource loadImageSource(Byte[] data){

            using (MemoryStream stream = new MemoryStream(data))
            {
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
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


        private void ScrollBar_ValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            refreshPage();
        }

        private void MovieListView_PreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && this.CurrentPage > 0)
            {
                this.CurrentPage--;
            }
            else if (e.Delta < 0 && this.CurrentPage < this.TotalPage)
            {
                this.CurrentPage++;
            }
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

        }


        private void Video_Closed(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 当前页实际显示的元素数量
        /// </summary>
        public Int32 PageElementCount
        {
            get
            {
                return this.pageEleCount;
            }
            set
            {
                base.SetProperty(ref this.pageEleCount, value);
            }
        }
        private Int32 pageEleCount;

        /// <summary>
        /// 当前页最多可显示元素的空位
        /// </summary>
        public Int32 PageSize
        {
            get
            {
                return this.pageSize;
            }
            set
            {
                base.SetProperty(ref this.pageSize, value);
            }
        }
        private Int32 pageSize;
        public Int32 TotalPage
        {
            get
            {
                return this.totalPage;
            }
            set
            {
                base.SetProperty(ref this.totalPage, value);
            }


        }
        private Int32 totalPage;

        public Int32 CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            set
            {
                base.SetProperty(ref this.currentPage, value);
            }
        }

        private Int32 currentPage;

    }
}
