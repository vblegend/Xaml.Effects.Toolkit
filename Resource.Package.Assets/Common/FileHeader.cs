using System.ComponentModel;

namespace Resource.Package.Assets.Common
{
    public enum CompressionOption : Byte
    {
        /// <summary>
        /// 永远不会压缩
        /// </summary>
        [Description("不要压缩")]
        NeverCompress = 1,

        /// <summary>
        /// 尽可能压缩
        /// </summary>
        [Description("尽可能压缩")]
        MuchPossibleCompress = 2,

        /// <summary>
        /// 必须压缩
        /// </summary>
        [Description("必须压缩")]
        MustCompressed = 3
    }







    internal struct FileHeader
    {
        public UInt64 Magic { get; set; }

        public CompressionOption CompressOption { get; set; }


        public Byte[] Version { get; set; }


        /// <summary>
        /// 目录数量
        /// </summary>
        public Int32 NumberOfFiles { get; set; }
        public Int32 TableDataAddr { get; set; }
        public Int32 TableDataSize { get; set; }

    }



    internal class FileAsyncCache
    {
        public FileAsyncCache()
        {
            infomation = new FileInfomation();
        }
        public FileInfomation infomation;
        public Byte[] Data;


    }



    public enum ImageTypes : Byte
    {
        [Description("Unknown")]
        Unknown = 0,
        [Description("Bmp")]
        BMP = 1,
        [Description("Png")]
        PNG = 2,
        [Description("Jpg")]
        JPG = 3,
        [Description("Gif")]
        GIF = 4,
        [Description("Tga")]
        TGA = 5,
        [Description("Tiff")]
        TIFF = 6
    }

    public enum RenderTypes : Byte
    {
        [Description("正常渲染")]
        Normal = 0,
        [Description("去除底色")]
        MaskColor = 1,
        [Description("混合渲染")]
        Blend = 2
    }







    public interface IReadOnlyDataBlock
    {

        public Int16 OffsetX { get;  }
        public Int16 OffsetY { get;  }

        public Byte[] Data { get;  }

        public ImageTypes lpType { get;  }
        public RenderTypes lpRenderType { get; }
        public Byte unknown2 { get; }
        public Byte unknown1 { get; }
    }


    public class DataInfo {

        public Int16 OffsetX { get; set; }
        public Int16 OffsetY { get; set; }

        public RenderTypes lpRenderType { get; set; }
        public Byte unknown2 { get; set; }
        public Byte unknown1 { get; set; }
    }










    public class DataBlock : DataInfo, IReadOnlyDataBlock
    {
        public ImageTypes lpType { get; set; }
        public Byte[] Data { get; set; }
    }


    internal class FileInfomation : DataInfo
    {



        public ImageTypes lpType { get; set; }
        /// <summary>
        /// 数据地址
        /// 4字节
        /// </summary>
        public Int32 lpData { get; set; }

        /// <summary>
        /// 原始数据大小
        /// </summary>
        public Int32 lpRawSize { get; set; }

        /// <summary>
        /// 数据大小
        /// 4字节
        /// </summary>
        public Int32 lpSize { get; set; }

    }




}
