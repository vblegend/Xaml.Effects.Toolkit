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
using Resource.Package.Assets.Common;
using System.Windows.Media.Animation;
using Assets.Editor.Utils;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace Assets.Editor.Models
{



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

        public ICommand RenderTypeChangedCommand { get; protected set; }
        public ICommand NewPackageCommand { get; protected set; }

        public ICommand OpenPackageCommand { get; protected set; }

        public IRelayCommand SavePackageCommand { get; protected set; }

        public IRelayCommand ClosePackageCommand { get; protected set; }

        public IRelayCommand ChangePasswordCommand { get; protected set; }
        public IRelayCommand ReplaceImageCommand { get; protected set; }

        public IRelayCommand ImportImageCommand { get; protected set; }

        public IRelayCommand ExportImageCommand { get; protected set; }


        public IRelayCommand RegFileTypeCommand { get; protected set; }


        public ICommand ThemesCommand { get; protected set; }

        public AssetFileStream assetFile { get; protected set; }


        public MainWindowModel()
        {
            ThemeManager.LoadThemeFromResource("/Assets.Editor;component/Assets/Themes/Black.xaml");
            this.ThemesCommand = new RelayCommand(Themes_Click);
            this.OpenPackageCommand = new RelayCommand(OpenPackage_Click);
            this.ClosePackageCommand = new RelayCommand(ClosePackage_Click, ClosePackage_CanClick);
            this.SavePackageCommand = new RelayCommand(SavePackage_Click, SavePackage_CanClick);
            this.ReplaceImageCommand = new RelayCommand(ReplaceImage_Click, ReplaceImage_CanClick);
            this.ImportImageCommand = new RelayCommand(ImportImage_Click, ImportImage_CanClick);
            this.ExportImageCommand = new RelayCommand(ExportImage_Click, ExportImage_CanClick);
            this.ChangePasswordCommand = new RelayCommand(ChangePassword_Click, ChangePassword_CanClick);
            this.PreviewMouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(ListView_PreviewMouseWheel);
            this.PageChangedCommand = new RelayCommand<RoutedPropertyChangedEventArgs<double>>(ScrollBar_ValueChanged);
            this.SelectionChangedCommand = new RelayCommand<System.Windows.Controls.SelectionChangedEventArgs>(ListView_SelectionChanged);
            this.OffsetChangedCommand = new RelayCommand<System.Windows.Controls.TextChangedEventArgs>(Offset_TextChanged);
            this.RenderTypeChangedCommand = new RelayCommand<System.Windows.Controls.SelectionChangedEventArgs>(RenderType_SelectionChanged);
            this.OffsetCommitCommand = new RelayCommand(OffsetCommit_Click, OffsetCommit_CanClick);
            this.NewPackageCommand = new RelayCommand(NewPackage_Click);
            this.RegFileTypeCommand = new RelayCommand(RegFileType_Click);
            this.currentPage = 0;
            this.Title = "Assets Editor - Power by Hanks";
            this.PageSize = 64;
            this.ZoomValue = 1;
            this.DrawingMode = DrawingMode.Raw;
            this.IsRegFileType = FileTypeRegister.FileTypeRegistered(".asset");
            this.Selected = new ImageModel();


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




        private void RegFileType_Click()
        {
            if (this.IsRegFileType)
            {
                FileTypeRegister.UnRegisterFileType(".asset");
            }
            else
            {
                FileTypeRegInfo fileTypeRegInfo = new FileTypeRegInfo(".osf");
                fileTypeRegInfo.Description = "Images Assets Resource File";
                fileTypeRegInfo.ExePath = Process.GetCurrentProcess().MainModule.FileName;
                fileTypeRegInfo.ExtendName = ".asset";
                fileTypeRegInfo.IconPath = fileTypeRegInfo.ExePath;
                // 注册
                FileTypeRegister.RegisterFileType(fileTypeRegInfo);
            }
            this.IsRegFileType = FileTypeRegister.FileTypeRegistered(".asset");
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
                this.assetFile.Replace(this.SelectedImage.Index, new DataBlock { Data = data, OffsetX = 0, OffsetY = 0 });
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
                export.Model.Length = 1;
                export.Model.IsBatch = false;
            }
            else
            {
                export.Model.StartIndex = 0;
                export.Model.Length = this.assetFile.NumberOfFiles;
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
                resizePage();
                refreshPage();
                this.ClosePackageCommand.NotifyCanExecuteChanged();
                this.ReplaceImageCommand.NotifyCanExecuteChanged();
                this.ImportImageCommand.NotifyCanExecuteChanged();
                this.ExportImageCommand.NotifyCanExecuteChanged();
                this.ChangePasswordCommand.NotifyCanExecuteChanged();
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
            this.Selected.OffsetX = 0;
            this.Selected.OffsetY = 0;
            this.Selected.Index = 0;
            this.Selected.RenderType = RenderTypes.Normal;
            this.Selected.ImageType = ImageTypes.Unknown;
            this.Selected.Source = null;
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
                this.ChangePasswordCommand.NotifyCanExecuteChanged();
            }
            resizePage();
            this.Title = "Assets Editor - Power by Hanks";
        }


        private void OpenPackage_Click()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                DereferenceLinks = false,
            };
            //ofd.InitialDirectory = @"D:\";
            ofd.Filter = "Assets Package|*.asset";
            if (ofd.ShowDialog() == true)
            {
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
                    this.GridImages[i].ImageType = node.lpType;
                    this.GridImages[i].RenderType = node.lpRenderType;
                    this.GridImages[i].OffsetX = node.OffsetX;
                    this.GridImages[i].OffsetY = node.OffsetY;
                    this.GridImages[i].FileSize = node.Data.Length;
                }
            }
        }



        private BitmapSource loadImageSource(Byte[] data)
        {
            try
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
            this.ReplaceImageCommand.NotifyCanExecuteChanged();
            this.ExportImageCommand.NotifyCanExecuteChanged();
        }


        private void RenderType_SelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.SelectedImage != null && (this.Selected.RenderType != this.SelectedImage.RenderType))
            {
                this.offsetChanged = true;
                this.OffsetCommitCommand.NotifyCanExecuteChanged();
            }
        }

        private void Offset_TextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.SelectedImage != null && (this.Selected.OffsetX != this.SelectedImage.OffsetX || this.Selected.OffsetY != this.SelectedImage.OffsetY))
            {
                this.offsetChanged = true;
                this.OffsetCommitCommand.NotifyCanExecuteChanged();
            }
        }

        private Boolean offsetChanged = false;


        private Boolean OffsetCommit_CanClick()
        {
            return this.offsetChanged;
        }


        private void OffsetCommit_Click()
        {
            this.SelectedImage.OffsetX = this.Selected.OffsetX;
            this.SelectedImage.OffsetY = this.Selected.OffsetY;
            this.SelectedImage.RenderType = this.Selected.RenderType;
            this.assetFile.UpdateInfoNoWrite(this.SelectedImage.Index, new DataInfo()
            {
                lpRenderType = this.Selected.RenderType,
                OffsetX = this.Selected.OffsetX,
                OffsetY = this.Selected.OffsetY,
            });

            // this.assetFile.UpdateOffsetNoWrite(this.SelectedImage.Index, new System.Drawing.Point(this.Selected.OffsetX, this.Selected.OffsetY));
            this.offsetChanged = false;
            this.OffsetCommitCommand.NotifyCanExecuteChanged();
            this.needSave = true;
            this.SavePackageCommand.NotifyCanExecuteChanged();
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

    }
}
