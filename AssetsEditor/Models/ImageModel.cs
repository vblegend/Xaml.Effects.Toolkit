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



        public ImageTypes Type
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


        public Int32 OffsetX
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

        private Int32 offsetX;




        public Int32 OffsetY
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

        private Int32 offsetY;

    }



}
