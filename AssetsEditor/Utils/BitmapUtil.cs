using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;

namespace Assets.Editor
{
    public static class BitmapUtil
    {





        public static void SavePng(this BitmapSource source, String filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(fileStream);
            }
        }


        public static void FromBase64Transform(String fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = fileStream;
            img.EndInit();
        }





    }
}
