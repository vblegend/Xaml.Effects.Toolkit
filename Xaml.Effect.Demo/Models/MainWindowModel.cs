using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Xaml.Effects.Toolkit;

namespace Xaml.Effect.Demo.Models
{
    internal class MainWindowModel : ObservableObject
    {


        /// <summary>
        /// 应用,需要时在派生类中重写
        /// </summary>
        public ICommand HelpCommand { get; protected set; }


        /// <summary>
        /// 应用,需要时在派生类中重写
        /// </summary>
        public ICommand ThemesCommand { get; protected set; }


        public MainWindowModel()
        {
            Trace.WriteLine("do");

            this.ThemesCommand = new RelayCommand(Themes_Click);
            this.HelpCommand = new RelayCommand(Help_Click);
            this.Title = "Console";
        }


        private void Themes_Click()
        {
            if (ThemeManager.CurrentTheme == "Default")
            {
                ThemeManager.LoadTheme("Themes\\White.xaml");
            }
            else if (ThemeManager.CurrentTheme == "White")
            {
                ThemeManager.LoadTheme("Themes\\Black.xaml");
            }
            else if (ThemeManager.CurrentTheme == "Black")
            {
                ThemeManager.LoadTheme("Themes\\Image.xaml");
            }
            else
            {
                ThemeManager.LoadTheme("Themes\\White.xaml");
            }
            //Console.WriteLine(ThemeManager.CurrentTheme);
        }

        private void Help_Click()
        {
            if (this.BorderThickness.Bottom == 1)
            {
                this.BorderThickness = new Thickness(0);
            }
            else
            {
                this.BorderThickness = new Thickness(1);
            }

        }


        public Thickness BorderThickness
        {
            get
            {
                return this.borderThickness;
            }
            set
            {
                base.SetProperty(ref this.borderThickness, value);
            }
        }



        private String title;



        public String Title
        {
            get
            {
                return this.title;
            }
            set
            {
                base.SetProperty(ref this.title, value);
            }
        }



        private Thickness borderThickness;
    }
}
