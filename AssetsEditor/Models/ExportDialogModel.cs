using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Resource.Package.Assets;
using Resource.Package.Assets.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Xaml.Effects.Toolkit.Model;


namespace Assets.Editor.Models
{

    public class ExportDialogModel : DialogModel
    {
        public readonly String ImageFilter = "Image File|*.png;*.bmp;*.gif;*.jpg;*.tif";
        public ICommand SelectSourceCommand { get; protected set; }
        public IRelayCommand ModeChangedCommand { get; protected set; }

        public AssetFileStream stream { get; set; }



        public ExportDialogModel()
        {
            this.Title = "导出资源";
            this.ExportDirectory = "";
            this.IsBatch = false;
            this.Progress = 0;
            this.StartIndex = 0;
            this.Length = 0;
            this.ImportOptions = ImageUserData.SchemaJson;
            this.ModeChangedCommand = new RelayCommand<RoutedEventArgs>(ImportMode_Changed);
            this.SelectSourceCommand = new RelayCommand(SelectSource_Click);
        }


        private void SelectSource_Click()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "选择最终目录";                         //目录对话框的描述字符串
            folderBrowserDialog.ShowNewFolderButton = true;                        //是否显示目录对话框左下角的“新建文件夹”按钮， true：显示， false：
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ExportDirectory = folderBrowserDialog.SelectedPath;
            };
            this.Progress = 0;
            this.SubmitCommand.NotifyCanExecuteChanged();
        }




        private void ImportMode_Changed(RoutedEventArgs e)
        {
            this.Length = 1;
            this.SubmitCommand.NotifyCanExecuteChanged();
        }


        protected override bool Can_Submit()
        {
            return !string.IsNullOrEmpty(this.ExportDirectory);
        }

        /// <summary>
        /// 提交按钮事件
        /// </summary>
        protected override void Execute_Submit()
        {
            this.Progress = 0;
            List<String> files = new List<string>();
            Dictionary<String, DataInfo> schemas = new Dictionary<String, DataInfo>();
            if (ImportOptions == ImageUserData.Placements)
            {
                Directory.CreateDirectory(Path.Combine(this.ExportDirectory, "Placements"));
            }
            for (int i = 10; i < 20; i++)
            {
                var block = this.stream.Read(i);
                var fileName = i.ToString().PadLeft(7, '0');
                var extName = this.GetFileName(block.lpType);
                var outFileName = Path.Join(this.ExportDirectory, fileName + extName);

                if (ImportOptions == ImageUserData.SchemaJson)
                {
                    schemas[fileName + extName] = new DataInfo()
                    {
                        lpRenderType = block.lpRenderType,
                        OffsetX = block.OffsetX,
                        OffsetY = block.OffsetY,
                        Unknown1 = block.Unknown1,
                        Unknown2 = block.Unknown2,
                    };
                }
                else if (ImportOptions == ImageUserData.Placements)
                {
                    var f = Path.Combine(this.ExportDirectory, $"Placements\\{fileName}.txt");
                    File.WriteAllLines(f, new String[] { block.OffsetX.ToString(), block.OffsetY.ToString() });
                }
                System.IO.File.WriteAllBytes(outFileName, block.Data);
            }

            if (ImportOptions == ImageUserData.SchemaJson)
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                //设置支持中文的unicode编码
                options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //启用驼峰格式
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //启用缩进设置
                options.WriteIndented = true;
                string json = JsonSerializer.Serialize(schemas, options);

                File.WriteAllText(Path.Combine(this.ExportDirectory, "schema.json"), json);
            }

            Console.WriteLine(files);
        }

        private string GetFileName(ImageTypes types)
        {
            if (types == ImageTypes.BMP) return ".bmp";
            if (types == ImageTypes.TGA) return ".tga";
            if (types == ImageTypes.TIFF) return ".tiff";
            if (types == ImageTypes.PNG) return ".png";
            if (types == ImageTypes.GIF) return ".gif";
            if (types == ImageTypes.JPG) return ".jpg";
            return ".png";
        }


        private void SaveSchema()
        {






        }






        private async Task ImportResources(List<String> files)
        {
            List<DataBlock> blocks = new List<DataBlock>();
            var fileCount = files.Count;
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var block = new DataBlock();
                block.Data = System.IO.File.ReadAllBytes(file);
                var filename = Path.GetFileNameWithoutExtension(file);
                if (this.ImportOptions == ImageUserData.Placements)
                {
                    var dirname = Path.GetDirectoryName(file);
                    var pname = Path.Combine(dirname, $"Placements\\{filename}.txt");
                    if (System.IO.File.Exists(pname))
                    {
                        var placements = System.IO.File.ReadAllLines(pname);
                        block.OffsetX = Int16.Parse(placements[0]);
                        block.OffsetY = Int16.Parse(placements[1]);
                    }
                }
                blocks.Add(block);
                this.Progress = (i + 1) / fileCount * 50;
                if (i % 10 == 0) Thread.Sleep(1);
            }
            this.stream.BatchImport(blocks, (value) =>
            {
                if (value % 10 == 0) Thread.Sleep(1);
                this.Progress = 50 + value / fileCount * 50;
            });


            this.DialogResult = true;
        }







        public Int32 StartIndex
        {
            get
            {
                return this.startIndex;
            }
            set
            {
                base.SetProperty(ref this.startIndex, value);
            }
        }
        private Int32 startIndex;


        public Int32 Length
        {
            get
            {
                return this.length;
            }
            set
            {
                base.SetProperty(ref this.length, value);
            }
        }
        private Int32 length;

        public Double Progress
        {
            get
            {
                return this.progress;
            }
            set
            {
                base.SetProperty(ref this.progress, value);
            }
        }
        private Double progress;



        public ImageUserData ImportOptions
        {
            get
            {
                return this.importOptions;
            }
            set
            {
                base.SetProperty(ref this.importOptions, value);
            }
        }
        private ImageUserData importOptions;



        public Boolean IsBatch
        {
            get
            {
                return this.isBatch;
            }
            set
            {
                base.SetProperty(ref this.isBatch, value);
            }
        }

        private Boolean isBatch;


        public String ExportDirectory
        {
            get
            {
                return this.importSource;
            }
            set
            {
                base.SetProperty(ref this.importSource, value);
            }
        }

        private String importSource;

    }
}
