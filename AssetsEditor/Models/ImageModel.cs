using Microsoft.Toolkit.Mvvm.ComponentModel;
using Resource.Package.Assets.Common;
using System;
using System.Windows.Media.Imaging;

namespace Assets.Editor.Models
{

    public class ImageModel : ObservableObject
    {
        public ImageModel()
        {


        }

        public void CopyFrom(ImageModel mode)
        {
            this.Source = mode.Source;
            this.OffsetX = mode.OffsetX;
            this.OffsetY = mode.OffsetY;
            this.ImageType = mode.ImageType;
            this.RenderType = mode.RenderType;
            this.Index = mode.Index;
        }






        public BitmapSource Source
        {
            get
            {
                return this.image;
            }
            set
            {
                base.SetProperty(ref this.image, value);
            }
        }

        private BitmapSource image;



        public ImageTypes ImageType
        {
            get
            {
                return this.type;
            }
            set
            {
                base.SetProperty(ref this.type, value);
            }
        }

        private ImageTypes type;



        public RenderTypes RenderType
        {
            get
            {
                return this.render;
            }
            set
            {
                base.SetProperty(ref this.render, value);
            }
        }

        private RenderTypes render;



        public Int32 Index
        {
            get
            {
                return this.index;
            }
            set
            {
                base.SetProperty(ref this.index, value);
            }
        }

        private Int32 index;


        public Int16 OffsetX
        {
            get
            {
                return this.offsetX;
            }
            set
            {
                base.SetProperty(ref this.offsetX, value);
            }
        }

        private Int16 offsetX;




        public Int16 OffsetY
        {
            get
            {
                return this.offsetY;
            }
            set
            {
                base.SetProperty(ref this.offsetY, value);
            }
        }

        private Int16 offsetY;

    }



}
