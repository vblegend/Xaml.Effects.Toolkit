﻿using System;
using System.Windows;
using Assets.Editor.Models;

namespace Assets.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }



        public MainWindowModel Model { get; set; } = new MainWindowModel();



        public MainWindow()
        {
            this.DataContext = Model;
            InitializeComponent();
            this.Model.OnClose += Model_OnClose;
            Instance = this;
        }

        private void Model_OnClose(object sender, Xaml.Effects.Toolkit.Model.WindowDestroyArgs e)
        {
            Environment.Exit(0);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NumbericTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
