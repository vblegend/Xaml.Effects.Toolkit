﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Assets.Editor.Models;
using Xaml.Effects.Toolkit;

namespace Assets.Editor.Views
{
    /// <summary>
    /// VideoList.xaml 的交互逻辑
    /// </summary>
    public partial class VideoList : Window
    {
        public VideoListModel Model { get; set; }


        public VideoList(ObservableCollection<VideoInfo> VideoList)
        {

            InitializeComponent();
            this.DataContext = this.Model = new VideoListModel(VideoList);
            this.Model.OnClose += Model_OnClose;

        }

        private void Model_OnClose(object sender, Xaml.Effects.Toolkit.Model.WindowDestroyArgs e)
        {
            
            e.Apply(this);
        }
    }
}
