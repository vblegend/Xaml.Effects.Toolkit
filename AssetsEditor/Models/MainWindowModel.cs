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
using Assets.Editor.Views;
using Microsoft.Win32;
using Resource.Package.Assets.Common;
using Assets.Editor.Utils;
using System.Diagnostics;
using Xaml.Effects.Toolkit.Uitity;
using Assets.Editor.Common;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;

namespace Assets.Editor.Models
{

    public enum ViewGrids
    {
        [Description("人物阵列(8*4)")]
        GRID_8X4,
        [Description("怪物阵列(10*4)")]
        GRID_10X4,
        [Description("密集阵列(16*4)")]
        GRID_16X4


    }




    public class MainWindowModel : DialogModel
    {

        public readonly String ImageFilter = "Image File|*.png;*.bmp;*.gif;*.jpg;*.tif";

        public ObservableCollection<String> ListItems { get; set; } = new ObservableCollection<String>();

        public ObservableCollection<ImageModel> GridImages { get; set; } = new ObservableCollection<ImageModel>();


        public ICommand PreviewMouseWheelCommand { get; protected set; }

        public ICommand PageChangedCommand { get; protected set; }

        public ICommand SelectionChangedCommand { get; protected set; }

        public ICommand OffsetChangedCommand { get; protected set; }
        public IRelayCommand OffsetCommitCommand { get; protected set; }

        public IRelayCommand BatchOffsetCommitCommand { get; protected set; }

        public ICommand RenderTypeChangedCommand { get; protected set; }

        public ICommand ListGridChangedCommand { get; protected set; }

        public ICommand NewPackageCommand { get; protected set; }

        public ICommand OpenPackageCommand { get; protected set; }

        public IRelayCommand ClosePackageCommand { get; protected set; }

        public IRelayCommand ChangePasswordCommand { get; protected set; }

        public IRelayCommand ImportImageCommand { get; protected set; }

        public IRelayCommand ExportImageCommand { get; protected set; }


        public IRelayCommand RegFileTypeCommand { get; protected set; }


        public IRelayCommand Bmp2PngCommand { get; protected set; }

        public IRelayCommand PngFormatCommand { get; protected set; }


        public ICommand ThemesCommand { get; protected set; }

        public AssetFileStream assetFile { get; protected set; }


        public MainWindowModel()
        {
            ThemeManager.LoadThemeFromResource("/Assets.Editor;component/Assets/Themes/Black.xaml");
            this.ThemesCommand = new RelayCommand(Themes_Click);
            this.OpenPackageCommand = new RelayCommand(OpenPackage_Click);
            this.ClosePackageCommand = new RelayCommand(ClosePackage_Click, ClosePackage_CanClick);
            this.ImportImageCommand = new RelayCommand(ImportImage_Click, ImportImage_CanClick);
            this.ExportImageCommand = new RelayCommand(ExportImage_Click, ExportImage_CanClick);
            this.ChangePasswordCommand = new RelayCommand(ChangePassword_Click, ChangePassword_CanClick);
            this.PreviewMouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(ListView_PreviewMouseWheel);
            this.PageChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<double>>(ScrollBar_ValueChanged);
            this.SelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(ListView_SelectionChanged);
            this.OffsetChangedCommand = new RelayCommand<TextChangedEventArgs>(Offset_TextChanged);
            this.RenderTypeChangedCommand = new RelayCommand<SelectionChangedEventArgs>(RenderType_SelectionChanged);
            this.ListGridChangedCommand = new RelayCommand<SelectionChangedEventArgs>(ListGrid_SelectionChanged);
            this.OffsetCommitCommand = new RelayCommand(OffsetCommit_Click, OffsetCommit_CanClick);
            this.BatchOffsetCommitCommand = new RelayCommand(BatchOffsetCommit_Click, OffsetCommit_CanClick);
            this.NewPackageCommand = new RelayCommand(NewPackage_Click);
            this.RegFileTypeCommand = new RelayCommand(RegFileType_Click);
            this.Bmp2PngCommand = new RelayCommand(Bmp2Png_Click);
            this.PngFormatCommand = new RelayCommand(PngFormat_Click);
            this.currentPage = 0;
            this.Title = "Assets Editor - Power by Hanks";
            this.PageSize = 64;
            this.ZoomValue = 1;
            this.DrawingMode = DrawingMode.Raw;
            this.IsRegFileType = FileTypeRegister.FileTypeRegistered(".Asset");
            this.Selected = new ImageModel();
            this.ListGrid = ViewGrids.GRID_16X4;

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



        private void PngFormat_Click()
        {
            var dialog = new PngFormatDialog();
            dialog.Owner = MainWindow.Instance;
            dialog.ShowDialog();
        }



        private void Bmp2Png_Click()
        {
            var dialog = new BmpToPngDialog();
            dialog.Owner = MainWindow.Instance;
            dialog.ShowDialog();
        }

        private void RegFileType_Click()
        {
            if (this.IsRegFileType)
            {
                FileTypeRegister.UnRegisterFileType(".Asset");
            }
            else
            {
                FileTypeRegInfo fileTypeRegInfo = new FileTypeRegInfo(".osf");
                fileTypeRegInfo.Description = "Images Assets Resource File";
                fileTypeRegInfo.ExePath = Process.GetCurrentProcess().MainModule.FileName;
                fileTypeRegInfo.ExtendName = ".Asset";
                fileTypeRegInfo.IconPath = fileTypeRegInfo.ExePath;
                // 注册
                FileTypeRegister.RegisterFileType(fileTypeRegInfo);
            }
            this.IsRegFileType = FileTypeRegister.FileTypeRegistered(".Asset");
        }


        private Boolean ImportImage_CanClick()
        {
            return this.assetFile != null;
        }
        private void ImportImage_Click()
        {
            ImportDialog import = new ImportDialog();
            import.Model.stream = this.assetFile;
            import.Owner = MainWindow.Instance;
            import.ShowDialog();
            resizePage();
            refreshPage();
        }



        private Boolean ChangePassword_CanClick()
        {
            return this.assetFile != null;
        }
        private void ChangePassword_Click()
        {
            var dialog = new PasswordInput();
            dialog.Owner = MainWindow.Instance;
            dialog.Model.Title = "修改资源包密码";
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                this.assetFile.ChangePassword(dialog.Model.Password);
                MessageBox.Show("密码已修改","修改密码");
            }
        }

        private Boolean ExportImage_CanClick()
        {
            return this.assetFile != null;
        }
        private void ExportImage_Click()
        {
            ExportDialog export = new ExportDialog();
            export.Model.stream = this.assetFile;
            if (this.SelectedImage != null)
            {
                export.Model.StartIndex = this.SelectedImage.Index;
                export.Model.EndIndex = this.SelectedImage.Index;
                export.Model.IsBatch = false;
            }
            else
            {
                export.Model.StartIndex = 0;
                export.Model.EndIndex = this.assetFile.NumberOfFiles;
                export.Model.IsBatch = true;
            }
            export.Owner = MainWindow.Instance;
            export.ShowDialog();
        }


        private void OpenAssetPackage(String filename, String password)
        {
            ClosePackage_Click();
            try
            {
                this.assetFile = AssetFileStream.Open(filename, password);
                this.Title = $"Assets Editor - {filename}";
                this.CurrentPage = 0;
                resizePage();
                refreshPage();
                this.ClosePackageCommand.NotifyCanExecuteChanged();
                this.ImportImageCommand.NotifyCanExecuteChanged();
                this.ExportImageCommand.NotifyCanExecuteChanged();
                this.ChangePasswordCommand.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        private Boolean ClosePackage_CanClick()
        {
            return this.assetFile != null;
        }



        private void ClosePackage_Click()
        {
            this.SelectedImage = null;
            this.Selected.OffsetX = 0;
            this.Selected.OffsetY = 0;
            this.Selected.Index = 0;
            this.Selected.RenderType = RenderTypes.Normal;
            this.Selected.ImageType = ImageTypes.Unknown;
            this.Selected.Source = null;
            this.CurrentPage = 0;
            if (this.assetFile != null)
            {
                this.assetFile.Close();
                this.assetFile.Dispose();
                this.assetFile = null;
                this.ClosePackageCommand.NotifyCanExecuteChanged();

                this.offsetChanged = false;
                this.OffsetCommitCommand.NotifyCanExecuteChanged();
                this.BatchOffsetCommitCommand.NotifyCanExecuteChanged();
                this.ImportImageCommand.NotifyCanExecuteChanged();
                this.ExportImageCommand.NotifyCanExecuteChanged();
                this.ChangePasswordCommand.NotifyCanExecuteChanged();
            }
            resizePage();
            this.Title = "Assets Editor - Power by Hanks";
        }


        private void OpenPackage_Click()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                InitialDirectory = ConfigureUtil.GetValue("OpenAssetDirectory"),
                DereferenceLinks = false,
            };
            //ofd.InitialDirectory = @"D:\";
            ofd.Filter = "Assets Package|*.asset";
            if (ofd.ShowDialog() == true)
            {
                var dir = Path.GetDirectoryName(ofd.FileName);
                ConfigureUtil.SetValue("OpenAssetDirectory", dir);
                this.OpenFile(ofd.FileName);
            }
        }


        public void OpenFile(String fileName)
        {
            var dialog = new PasswordInput();
            dialog.Owner = MainWindow.Instance;
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                this.OpenAssetPackage(fileName, dialog.Model.Password);
            }
        }






        private void resizePage()
        {
            UInt32 numberOfFiles = 0;
            if (this.assetFile == null) return;
            if (this.assetFile != null) numberOfFiles = this.assetFile.NumberOfFiles;
            var pagesize2 = numberOfFiles - (this.CurrentPage * this.PageSize);
            var pagesize = (UInt32)Math.Min(this.PageSize, pagesize2);
            this.PageElementCount = pagesize;
            this.GridImages.Clear();
            for (int i = 0; i < pagesize; i++)
            {
                this.GridImages.Add(new ImageModel());
            }
            this.TotalPage = (UInt32)(numberOfFiles / this.PageSize);
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
                    var node = this.assetFile.Read((UInt32)(startAt + i));
                    this.GridImages[i].Index = (UInt32)(startAt + i);
                    this.GridImages[i].ImageType = node.lpType;
                    this.GridImages[i].RenderType = node.lpRenderType;
                    this.GridImages[i].OffsetX = node.OffsetX;
                    this.GridImages[i].OffsetY = node.OffsetY;


                    if (node.Data.Length > 0)
                    {
                        this.GridImages[i].FileSize = node.Data.Length;
                        var source = loadImageSource(node.Data, node.lpType);
                        this.GridImages[i].Source = source;
                    }


                }
            }
        }



        private BitmapSource loadImageSource(Byte[] data, ImageTypes type)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(data))
                {

                    if (type == ImageTypes.TGA)
                    {
                        using (TargaImage tgaImage = new TargaImage(stream))
                        {
                            IntPtr ip = tgaImage.Image.GetHbitmap();
                            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            return bitmapSource;
                        };
                    }
                    BitmapImage result = new BitmapImage();
                    result.BeginInit();
                    result.CacheOption = BitmapCacheOption.OnLoad;
                    result.StreamSource = stream;
                    result.EndInit();
                    result.Freeze();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        private void Themes_Click()
        {
            // /Assets.Editor;component/Assets/Themes/background.png
            if (ThemeManager.CurrentTheme == "White")
            {
                ThemeManager.LoadThemeFromResource("/Assets.Editor;component/Assets/Themes/Black.xaml");
            }
            else if (ThemeManager.CurrentTheme == "Black")
            {
                ThemeManager.LoadThemeFromResource("/Assets.Editor;component/Assets/Themes/White.xaml");
            }
            //Console.WriteLine(ThemeManager.CurrentTheme);
        }





        private void ScrollBar_ValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            refreshPage();
            if (this.SelectedImage != null)
            {
                this.Selected.CopyFrom(this.SelectedImage);
            }
        }

        private void ListView_SelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                this.SelectedImage = e.AddedItems[0] as ImageModel;
                this.Selected.CopyFrom(this.SelectedImage);
            }
            else
            {
                this.SelectedImage = null;
            }
            this.ExportImageCommand.NotifyCanExecuteChanged();
        }


        private void ListGrid_SelectionChanged(SelectionChangedEventArgs e)
        {
            //if (this.ListGrid == ViewGrids.GRID_8X4)
            //{
            //    this.GridColumns = 8;
            //    this.GridRows = 4;
            //}
            //else if (this.ListGrid == ViewGrids.GRID_10X4)
            //{
            //    this.GridColumns = 10;
            //    this.GridRows = 4;
            //}
            //else if (this.ListGrid == ViewGrids.GRID_16X4)
            //{
            //    this.GridColumns = 16;
            //    this.GridRows = 4;
            //}
        }



        private void RenderType_SelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.SelectedImage != null && (this.Selected.RenderType != this.SelectedImage.RenderType))
            {
                this.offsetChanged = true;
                this.OffsetCommitCommand.NotifyCanExecuteChanged();
                this.BatchOffsetCommitCommand.NotifyCanExecuteChanged();
            }
        }

        private void Offset_TextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.SelectedImage != null && (this.Selected.OffsetX != this.SelectedImage.OffsetX || this.Selected.OffsetY != this.SelectedImage.OffsetY))
            {
                this.offsetChanged = true;
                this.OffsetCommitCommand.NotifyCanExecuteChanged();
                this.BatchOffsetCommitCommand.NotifyCanExecuteChanged();
            }
        }

        private Boolean offsetChanged = false;



        private void BatchOffsetCommit_Click()
        {
            var dialog = new BatchOffsetDialog();
            dialog.Owner = MainWindow.Instance;
            dialog.Model.stream = this.assetFile;
            dialog.Model.OffsetX = this.selected.OffsetX;
            dialog.Model.OffsetY = this.selected.OffsetY;
            dialog.Model.EndIndex = this.Selected.Index;
            dialog.Model.StartIndex = this.Selected.Index;

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                //for (UInt32 i = dialog.Model.StartIndex;  i < dialog.Model.EndIndex; i++)
                //{
                //}
                this.refreshPage();
                this.offsetChanged = false;
                this.OffsetCommitCommand.NotifyCanExecuteChanged();
                this.BatchOffsetCommitCommand.NotifyCanExecuteChanged();
            }
        }

        private Boolean OffsetCommit_CanClick()
        {
            return this.offsetChanged;
        }

        private void OffsetCommit_Click()
        {
            this.SelectedImage.OffsetX = this.Selected.OffsetX;
            this.SelectedImage.OffsetY = this.Selected.OffsetY;
            this.SelectedImage.RenderType = this.Selected.RenderType;
            this.assetFile.UpdateInfo(this.SelectedImage.Index, new DataInfo()
            {
                lpRenderType = this.Selected.RenderType,
                OffsetX = this.Selected.OffsetX,
                OffsetY = this.Selected.OffsetY,
            });

            // this.assetFile.UpdateOffsetNoWrite(this.SelectedImage.Index, new System.Drawing.Point(this.Selected.OffsetX, this.Selected.OffsetY));
            this.offsetChanged = false;
            this.OffsetCommitCommand.NotifyCanExecuteChanged();
            this.BatchOffsetCommitCommand.NotifyCanExecuteChanged();
        }




        /// <summary>
        /// 当前页实际显示的元素数量
        /// </summary>
        public Boolean IsRegFileType
        {
            get
            {
                return this.isRegFileType;
            }
            set
            {
                base.SetProperty(ref this.isRegFileType, value);
            }
        }
        private Boolean isRegFileType;



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


        public UInt32 PageElementCount
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
        private UInt32 pageEleCount;



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



        public UInt32 TotalPage
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
        private UInt32 totalPage;

        public UInt32 CurrentPage
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

        private UInt32 currentPage;







        public ImageModel Selected
        {
            get
            {
                return this.selected;
            }
            set
            {
                base.SetProperty(ref this.selected, value);
            }
        }

        private ImageModel selected;



        public ViewGrids ListGrid
        {
            get
            {
                return this.listGrid;
            }
            set
            {
                base.SetProperty(ref this.listGrid, value);

                if (this.ListGrid == ViewGrids.GRID_8X4)
                {
                    this.GridColumns = 8;
                    this.GridRows = 4;
                }
                else if (this.ListGrid == ViewGrids.GRID_10X4)
                {
                    this.GridColumns = 10;
                    this.GridRows = 4;
                }
                else if (this.ListGrid == ViewGrids.GRID_16X4)
                {
                    this.GridColumns = 16;
                    this.GridRows = 4;
                }

                var firstIndex = this.PageSize * this.CurrentPage;
                this.PageSize = this.GridColumns  * this.GridRows;

                this.CurrentPage = (UInt32)(firstIndex / this.PageSize);


                refreshPage();
            }
        }

        private ViewGrids listGrid;






        public Int32 GridColumns
        {
            get
            {
                return this.gridColumns;
            }
            set
            {
                base.SetProperty(ref this.gridColumns, value);
            }
        }

        private Int32 gridColumns;


        public Int32 GridRows
        {
            get
            {
                return this.gridRows;
            }
            set
            {
                base.SetProperty(ref this.gridRows, value);
            }
        }

        private Int32 gridRows;
    }
}
