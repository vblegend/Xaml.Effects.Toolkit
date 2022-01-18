using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Xaml.Effects.Toolkit
{
    public class ThemeManager
    {

        static ThemeManager()
        {
            ThemeDictionary = (from s in Application.Current.Resources.MergedDictionaries where s.Source.OriginalString.EndsWith("Theme.xaml") select s).SingleOrDefault();
            getThemeName();
        }

        /// <summary>
        /// 系统主题字典
        /// </summary>
        public static ResourceDictionary ThemeDictionary { get; private set; }


        /// <summary>
        /// 当前主题名称
        /// </summary>
        public static String CurrentTheme { get; private set; }






        public static void LoadTheme(String fileName)
        {
            if (ThemeDictionary == null)
            {
                ThemeDictionary = (from s in Application.Current.Resources.MergedDictionaries where s.Source.OriginalString.EndsWith("Theme.xaml") select s).SingleOrDefault();
            }
            if (ThemeDictionary != null)
            {
                using (var stream = File.OpenRead(fileName))
                {
                    ParserContext pc = new ParserContext();
                    pc.XmlnsDictionary.Add("", "Magic.Tools");
                    pc.XmlnsDictionary.Add("", "Xaml.Effects.Toolkit");
                    var path = Environment.CurrentDirectory;
                    if (!path.EndsWith("\\"))
                    {
                        path = path + "\\";
                    }
                    pc.BaseUri = new Uri(path, UriKind.Absolute);
                    ResourceDictionary resourceDictionary = XamlReader.Load(stream, pc) as ResourceDictionary;
                    foreach (DictionaryEntry key in resourceDictionary)
                    {
                        if (ThemeDictionary.Contains(key.Key))
                        {
                            ThemeDictionary.Remove(key.Key);
                        }
                        ThemeDictionary.Add(key.Key, key.Value);
                    }
                    getThemeName();
                };

            }
        }



        private static void getThemeName()
        {
            if (ThemeDictionary != null)
            {
                foreach (DictionaryEntry item in ThemeDictionary)
                {
                    if (item.Key.Equals("ThemeName"))
                    {
                        CurrentTheme = item.Value.ToString();
                        return;
                    }
                }
            }
            CurrentTheme = String.Empty;
        }


    }
}
