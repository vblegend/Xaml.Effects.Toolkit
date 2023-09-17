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
using System.Windows.Documents;
using System.Collections.Generic;
using System.Security.Policy;
using Xaml.Effects.Toolkit.Converter;

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


        public Int32 OffsetX
        {
            get
            {
                return this.offsetX;
            }
            set
            {
                base.SetProperty(ref this.offsetX, value);
            }
        }

        private Int32 offsetX;




        public Int32 OffsetY
        {
            get
            {
                return this.offsetY;
            }
            set
            {
                base.SetProperty(ref this.offsetY, value);
            }
        }

        private Int32 offsetY;

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

        public ICommand SelectionChangedCommand { get; protected set; }

        public ICommand OffsetXChangedCommand { get; protected set; }
        public ICommand OffsetYChangedCommand { get; protected set; }



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
            this.PreviewMouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(ListView_PreviewMouseWheel);
            this.PageChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<double>>(ScrollBar_ValueChanged);
            this.SelectionChangedCommand = new RelayCommand<System.Windows.Controls.SelectionChangedEventArgs>(ListView_SelectionChanged);
            this.OffsetXChangedCommand = new RelayCommand<System.Windows.Controls.TextChangedEventArgs>(OffsetX_TextChanged);
            this.OffsetYChangedCommand = new RelayCommand<System.Windows.Controls.TextChangedEventArgs>(OffsetY_TextChanged);

   
            this.currentPage = 0;
            this.Title = "Assets Editor - Power by Hanks";
            this.PageSize = 64;
            this.ZoomValue = 1;
            this.DrawingMode = DrawingMode.AlphaBlend ;
            this.assetFile = AssetFileStream.Open("12345.asset", "123");
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
                    var source = loadImageSource(node.Data);
                    this.GridImages[i].Index = startAt + i;
                    this.GridImages[i].Source = source;
                    this.GridImages[i].OffsetX = node.OffsetX;
                    this.GridImages[i].OffsetY = node.OffsetY;

                }
            }
        }



        private BitmapSource loadImageSource(Byte[] data)
        {

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


        private BitmapSource BitmapFilter(BitmapSource bitmap, Int32 R, Int32 G, Int32 B, Int32 Alpha = 0xff)
        {
            FormatConvertedBitmap fb = new FormatConvertedBitmap();//图片像素格式转换类
            fb.BeginInit();
            fb.Source = bitmap;
            fb.DestinationFormat = PixelFormats.Bgra32;
            fb.EndInit();
            var stride = (fb.PixelWidth * fb.Format.BitsPerPixel + 7) / 8;
            byte[] buf = new byte[fb.PixelHeight * stride];
            fb.CopyPixels(Int32Rect.Empty, buf, stride, 0);
            for (long ic = 0; ic < buf.LongLength; ic += 4)
            {
                if (buf[ic] == R && buf[ic + 1] == G && buf[ic + 2] == B && buf[ic + 3] == Alpha)
                {
                    buf[ic] = 0x00;
                    buf[ic + 1] = 0x00;
                    buf[ic + 2] = 0x00;
                    buf[ic + 3] = 0x00;//透明处理
                }
            }
            var source = new WriteableBitmap(fb.PixelWidth, fb.PixelHeight, fb.DpiX, fb.DpiY, fb.Format, fb.Palette);
            source.WritePixels(new Int32Rect(0, 0, fb.PixelWidth, fb.PixelHeight), buf, stride, 0);
            return source;// BitmapSource.Create(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, bitmap.Format, null, buf, stride);
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

        private void ListView_SelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                this.SelectedImage = e.AddedItems[0] as ImageModel;
                this.OffsetX = this.SelectedImage.OffsetX;
                this.OffsetY = this.SelectedImage.OffsetY;
            }
            else
            {
                this.SelectedImage = null;
            }

        }



        private void OffsetX_TextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.SelectedImage.OffsetX != this.OffsetX)
            {
                this.SelectedImage.OffsetX = this.OffsetX;
                this.assetFile.UpdateOffsetNoWrite(this.SelectedImage.Index, new System.Drawing.Point(this.SelectedImage.OffsetX, this.SelectedImage.OffsetY));
            }
        }

        private void OffsetY_TextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.SelectedImage.OffsetY != this.OffsetY)
            {
                this.SelectedImage.OffsetY = this.OffsetY;
                this.assetFile.UpdateOffsetNoWrite(this.SelectedImage.Index, new System.Drawing.Point(this.SelectedImage.OffsetX, this.SelectedImage.OffsetY));
            }
        }




        private void ListView_PreviewMouseWheel(MouseWheelEventArgs e)
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
        public DrawingMode DrawingMode
        {
            get
            {
                return this.drawingMode;
            }
            set
            {
                base.SetProperty(ref this.drawingMode, value);
            }
        }
        private DrawingMode drawingMode;




        /// <summary>
        /// 当前页实际显示的元素数量
        /// </summary>
        public Double ZoomValue
        {
            get
            {
                return this.zoomValue;
            }
            set
            {
                base.SetProperty(ref this.zoomValue, value);
            }
        }
        private Double zoomValue;






        /// <summary>
        /// 当前页实际显示的元素数量
        /// </summary>
        public ImageModel SelectedImage
        {
            get
            {
                return this.selectedImage;
            }
            set
            {
                base.SetProperty(ref this.selectedImage, value);
            }
        }
        private ImageModel selectedImage;

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


        public Int32 OffsetX
        {
            get
            {
                return this.offsetX;
            }
            set
            {
                base.SetProperty(ref this.offsetX, value);
            }
        }

        private Int32 offsetX;




        public Int32 OffsetY
        {
            get
            {
                return this.offsetY;
            }
            set
            {
                base.SetProperty(ref this.offsetY, value);
            }
        }

        private Int32 offsetY;
    }
}
