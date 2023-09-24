﻿using Microsoft.Toolkit.Mvvm.Input;
using Resource.Package.Assets;
using Resource.Package.Assets.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using Xaml.Effects.Toolkit.Model;


namespace Assets.Editor.Models
{

    public class ImportDialogModel : DialogModel
    {
        public readonly String ImageFilter = "Image File|*.png;*.bmp;*.gif;*.jpg;*.tif";
        public ICommand SelectSourceCommand { get; protected set; }
        public IRelayCommand ModeChangedCommand { get; protected set; }

        public AssetFileStream stream { get; set; }



        public ImportDialogModel()
        {
            this.Title = "导入资源";
            this.ImportSource = "";
            this.IsBatch = false;
            this.Progress = 0;
            this.RenderType = RenderTypes.Normal;
            this.FormatOptions = ImageFormat.ALLIMAGE;
            this.ImportUserData = ImageUserData.None;
            this.ImportOption = ImportOption.Append;
            this.ModeChangedCommand = new RelayCommand<RoutedEventArgs>(ImportMode_Changed);
            this.SelectSourceCommand = new RelayCommand(SelectSource_Click);
        }


        private void SelectSource_Click()
        {
            if (this.IsBatch)
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "选择最终目录";                         //目录对话框的描述字符串
                folderBrowserDialog.ShowNewFolderButton = true;                        //是否显示目录对话框左下角的“新建文件夹”按钮， true：显示， false：
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.ImportSource = folderBrowserDialog.SelectedPath;
                };
            }
            else
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Title = "选择图片文件";
                ofd.Filter = ImageFilter;
                if (ofd.ShowDialog() == true)
                {
                    this.ImportSource = ofd.FileName;
                }
            }
            this.Progress = 0;
            this.SubmitCommand.NotifyCanExecuteChanged();
        }




        private void ImportMode_Changed(RoutedEventArgs e)
        {
            this.ImportSource = "";
            this.SubmitCommand.NotifyCanExecuteChanged();
        }


        protected override bool Can_Submit()
        {
            return !string.IsNullOrEmpty(this.ImportSource);
        }

        /// <summary>
        /// 提交按钮事件
        /// </summary>
        protected override void Execute_Submit()
        {
            this.Progress = 0;
            List<String> files = new List<string>();
            var dir = "";
            if (this.IsBatch)
            {
                var fs = from file in Directory.EnumerateFiles(this.ImportSource, "*.*", SearchOption.TopDirectoryOnly) where FilterFile(file) select file;
                dir = this.ImportSource;
                files.AddRange(fs);
            }
            else
            {
                files.Add(this.ImportSource);
                dir = Path.GetDirectoryName(this.ImportSource);
            }
            this.ImportResources(dir, files);
            Console.WriteLine(files);
        }



        private async Task ImportResources(String dirName, List<String> files)
        {
            List<DataBlock> blocks = new List<DataBlock>();
            var fileCount = files.Count;
            Dictionary<String, DataInfo> schemas = new Dictionary<String, DataInfo>();
            if (this.ImportUserData == ImageUserData.SchemaJson)
            {
                var pname = Path.Combine(dirName, $"schema.json");
                if (System.IO.File.Exists(pname))
                {
                    var placements = System.IO.File.ReadAllText(pname);
                    schemas = JsonSerializer.Deserialize<Dictionary<String, DataInfo>>(placements);
                }
            }


            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var block = new DataBlock();
                if (file.EndsWith(".bmp"))
                {
                    block.lpRenderType = this.RenderType;
                }
                else
                {
                    block.lpRenderType = RenderTypes.Normal;
                }
                block.Data = System.IO.File.ReadAllBytes(file);
                var filename = Path.GetFileNameWithoutExtension(file);
                if (this.ImportUserData == ImageUserData.Placements)
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
                else if (this.ImportUserData == ImageUserData.SchemaJson)
                {
                    var _filename = Path.GetFileName(file);
                    if (schemas.TryGetValue(filename, out var schema))
                    {
                        block.OffsetX = schema.OffsetX;
                        block.OffsetY = schema.OffsetY;
                        block.lpRenderType = schema.lpRenderType;
                        block.Unknown1 = schema.Unknown1;
                        block.Unknown2 = schema.Unknown2;
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


        private Boolean FilterFile(string filename)
        {
            var ext = Path.GetExtension(filename).ToLower();
            if (this.FormatOptions == ImageFormat.ALLIMAGE) return ".bmp|.jpg|.gif|.png|.tiff|.tga".Contains(ext);
            if ((this.FormatOptions | ImageFormat.BMP) == ImageFormat.BMP) return ext == ".bmp";
            if ((this.FormatOptions | ImageFormat.JPG) == ImageFormat.JPG) return ext == ".jpg";
            if ((this.FormatOptions | ImageFormat.GIF) == ImageFormat.GIF) return ext == ".gif";
            if ((this.FormatOptions | ImageFormat.PNG) == ImageFormat.PNG) return ext == ".png";
            if ((this.FormatOptions | ImageFormat.TIFF) == ImageFormat.TIFF) return ext == ".tif";
            if ((this.FormatOptions | ImageFormat.TGA) == ImageFormat.TGA) return ext == ".tga";
            return false;
        }









        public RenderTypes RenderType
        {
            get
            {
                return this.renderType;
            }
            set
            {
                base.SetProperty(ref this.renderType, value);
            }
        }
        private RenderTypes renderType;




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



        public ImageFormat FormatOptions
        {
            get
            {
                return this.formatOptions;
            }
            set
            {
                base.SetProperty(ref this.formatOptions, value);
            }
        }
        private ImageFormat formatOptions;






        public ImportOption ImportOption
        {
            get
            {
                return this.importOption;
            }
            set
            {
                base.SetProperty(ref this.importOption, value);
            }
        }
        private ImportOption importOption;


        public ImageUserData ImportUserData
        {
            get
            {
                return this.importUserData;
            }
            set
            {
                base.SetProperty(ref this.importUserData, value);
            }
        }
        private ImageUserData importUserData;



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


        public String ImportSource
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
