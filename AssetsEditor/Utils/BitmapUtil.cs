using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Assets.Editor
{
    public static class BitmapUtil
    {
        public static readonly BitmapSource EmptyBitmapSource;

        public static readonly Byte[] EmptyBitmapData;


        static BitmapUtil()
        {
            EmptyBitmapSource = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null, new byte[4] { 255, 255, 255, 20 }, 4);
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(EmptyBitmapSource));
                encoder.Save(ms);
                EmptyBitmapData = ms.ToArray();
            }



        }




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
