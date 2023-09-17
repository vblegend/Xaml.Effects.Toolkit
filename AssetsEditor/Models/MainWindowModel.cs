using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Resource.Package.Assets;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Xaml.Effects.Toolkit;
using Xaml.Effects.Toolkit.Model;
using System.Windows;
using Xaml.Effects.Toolkit.Converter;
using Assets.Editor.Views;
using Microsoft.Win32;

namespace Assets.Editor.Models
{



    public class MainWindowModel : DialogModel
    {

        public readonly String ImageFilter = "Image File|*.png;*.bmp;*.jpg";

        public ObservableCollection<String> ListItems { get; set; } = new ObservableCollection<String>();

        public ObservableCollection<ImageModel> GridImages { get; set; } = new ObservableCollection<ImageModel>();


        public ICommand PreviewMouseWheelCommand { get; protected set; }

        public ICommand PageChangedCommand { get; protected set; }

        public ICommand SelectionChangedCommand { get; protected set; }

        public ICommand OffsetChangedCommand { get; protected set; }
        public IRelayCommand OffsetCommitCommand { get; protected set; }


        public ICommand NewPackageCommand { get; protected set; }

        public ICommand OpenPackageCommand { get; protected set; }

        public IRelayCommand SavePackageCommand { get; protected set; }

        public IRelayCommand ClosePackageCommand { get; protected set; }


        public IRelayCommand ReplaceImageCommand { get; protected set; }

        public IRelayCommand ImportImageCommand { get; protected set; }

        public IRelayCommand ExportImageCommand { get; protected set; }



        public ICommand ThemesCommand { get; protected set; }

        public AssetFileStream assetFile { get; protected set; }


        public MainWindowModel()
        {

            this.ThemesCommand = new RelayCommand(Themes_Click);
            this.OpenPackageCommand = new RelayCommand(OpenPackage_Click);
            this.ClosePackageCommand = new RelayCommand(ClosePackage_Click, ClosePackage_CanClick);
            this.SavePackageCommand = new RelayCommand(SavePackage_Click, SavePackage_CanClick);

            this.ReplaceImageCommand = new RelayCommand(ReplaceImage_Click, ReplaceImage_CanClick);
            this.ImportImageCommand = new RelayCommand(ImportImage_Click, ImportImage_CanClick);
            this.ExportImageCommand = new RelayCommand(ExportImage_Click, ExportImage_CanClick);


            this.PreviewMouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(ListView_PreviewMouseWheel);
            this.PageChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<double>>(ScrollBar_ValueChanged);
            this.SelectionChangedCommand = new RelayCommand<System.Windows.Controls.SelectionChangedEventArgs>(ListView_SelectionChanged);
            this.OffsetChangedCommand = new RelayCommand<System.Windows.Controls.TextChangedEventArgs>(Offset_TextChanged);
            this.OffsetCommitCommand = new RelayCommand(OffsetCommit_Click, OffsetCommit_CanClick);
            this.NewPackageCommand = new RelayCommand(NewPackage_Click);
            this.currentPage = 0;
            this.Title = "Assets Editor - Power by Hanks";
            this.PageSize = 64;
            this.ZoomValue = 1;
            this.DrawingMode = DrawingMode.Raw;

            resizePage();
            refreshPage();
        }


        private void NewPackage_Click()
        {
            var dialog = new CreateAssetsInput();
            dialog.Owner = MainWindow.Instance;
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                this.OpenAssetPackage(dialog.Model.FileName, dialog.Model.Password);
            }
        }








        private Boolean ReplaceImage_CanClick()
        {
            return this.assetFile != null && this.SelectedImage != null;
        }

        private void ReplaceImage_Click()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = @"D:\";
            ofd.Filter = ImageFilter;
            if (ofd.ShowDialog() == true)
            {
                var data = File.ReadAllBytes(ofd.FileName);
                this.assetFile.Replace(this.SelectedImage.Index, new Resource.Package.Assets.Common.DataBlock { Data = data, OffsetX = 0, OffsetY = 0 });
                SavePackage_Click();
                refreshPage();
            }
        }

        private Boolean ImportImage_CanClick()
        {
            return this.assetFile != null;
        }
        private void ImportImage_Click()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = @"D:\";
            ofd.Filter = ImageFilter;
            if (ofd.ShowDialog() == true)
            {
                var data = File.ReadAllBytes(ofd.FileName);
                this.assetFile.Import(new Resource.Package.Assets.Common.DataBlock { Data = data, OffsetX = 0, OffsetY = 0 });
                SavePackage_Click();
                refreshPage();
            }
        }


        private Boolean ExportImage_CanClick()
        {
            return this.assetFile != null && this.SelectedImage != null;
        }
        private void ExportImage_Click()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //sfd.InitialDirectory = @"D:\";
            sfd.Filter = ImageFilter;
            if (sfd.ShowDialog() == true)
            {
                var node = this.assetFile.Read(this.SelectedImage.Index);
                File.WriteAllBytes(sfd.FileName, node.Data);
            }

        }


        private void OpenAssetPackage(String filename, String password)
        {
            ClosePackage_Click();
            try
            {
                this.assetFile = AssetFileStream.Open(filename, password);
                this.Title = $"Assets Editor - {filename}";
                resizePage();
                refreshPage();
                this.ClosePackageCommand.NotifyCanExecuteChanged();
                this.ReplaceImageCommand.NotifyCanExecuteChanged();
                this.ImportImageCommand.NotifyCanExecuteChanged();
                this.ExportImageCommand.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        private Boolean needSave { get; set; } = false;
        private Boolean SavePackage_CanClick()
        {
            return this.assetFile != null && this.needSave;
        }
        private void SavePackage_Click()
        {
            this.assetFile.Save();
            this.needSave = false;
            this.SavePackageCommand.NotifyCanExecuteChanged();
        }



        private Boolean ClosePackage_CanClick()
        {
            return this.assetFile != null;
        }



        private void ClosePackage_Click()
        {
            this.SelectedImage = null;
            this.OffsetX = 0;
            this.OffsetY = 0;
            if (this.assetFile != null)
            {
                this.assetFile.Close();
                this.assetFile.Dispose();
                this.assetFile = null;
                this.ClosePackageCommand.NotifyCanExecuteChanged();

                this.offsetChanged = false;
                this.OffsetCommitCommand.NotifyCanExecuteChanged();

                this.needSave = false;
                this.SavePackageCommand.NotifyCanExecuteChanged();
                this.ImportImageCommand.NotifyCanExecuteChanged();
                this.ExportImageCommand.NotifyCanExecuteChanged();
                this.ReplaceImageCommand.NotifyCanExecuteChanged();

            }
            resizePage();
            this.Title = "Assets Editor - Power by Hanks";
        }


        private void OpenPackage_Click()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = @"D:\";
            ofd.Filter = "Assets Package|*.asset";
            if (ofd.ShowDialog() == true)
            {
                var dialog = new PasswordInput();
                dialog.Owner = MainWindow.Instance;
                var result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    this.OpenAssetPackage(ofd.FileName, dialog.Model.Password);
                }
            }
        }





        private void resizePage()
        {
            var numberOfFiles = 0;
            if (this.assetFile != null) numberOfFiles = this.assetFile.NumberOfFiles;
            var pagesize2 = numberOfFiles - (this.CurrentPage * this.PageSize);
            var pagesize = Math.Min(this.PageSize, pagesize2);
            this.PageElementCount = pagesize;
            this.GridImages.Clear();
            for (int i = 0; i < pagesize; i++)
            {
                this.GridImages.Add(new ImageModel());
            }
            this.TotalPage = numberOfFiles / this.PageSize;
            if (this.CurrentPage > this.totalPage)
            {
                this.CurrentPage = this.totalPage;
            }
        }

        private void refreshPage()
        {
            if (this.assetFile == null) return;


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
            this.ReplaceImageCommand.NotifyCanExecuteChanged();
            this.ExportImageCommand.NotifyCanExecuteChanged();
        }



        private void Offset_TextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            this.offsetChanged = this.SelectedImage != null && (this.OffsetX != this.SelectedImage.OffsetX || this.OffsetY != this.SelectedImage.OffsetY);
            this.OffsetCommitCommand.NotifyCanExecuteChanged();
        }

        private Boolean offsetChanged = false;


        private Boolean OffsetCommit_CanClick()
        {
            return this.offsetChanged;
        }


        private void OffsetCommit_Click()
        {
            this.SelectedImage.OffsetX = this.OffsetX;
            this.SelectedImage.OffsetY = this.OffsetY;
            this.assetFile.UpdateOffsetNoWrite(this.SelectedImage.Index, new System.Drawing.Point(this.OffsetX, this.OffsetY));
            this.offsetChanged = false;
            this.OffsetCommitCommand.NotifyCanExecuteChanged();
            this.needSave = true;
            this.SavePackageCommand.NotifyCanExecuteChanged();
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
