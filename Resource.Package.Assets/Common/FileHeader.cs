using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resource.Package.Assets.Common
{
    public enum CompressionOption : Byte
    {
        /// <summary>
        /// 永远不会压缩
        /// </summary>
        NeverCompress = 1,

        /// <summary>
        /// 尽可能压缩
        /// </summary>
        MuchPossibleCompress = 2,

        /// <summary>
        /// 必须压缩
        /// </summary>
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



    internal class FileAsyncCache {
        public FileAsyncCache()
        {
            infomation = new FileInfomation();
        }
        public FileInfomation infomation;
        public Byte[] Data;


    }


    public struct DataBlock
    {
        public Int32 OffsetX { get; set; }
        public Int32 OffsetY { get; set; }
        public Byte[] Data { get; set; }
    }

    internal class FileInfomation
    {

        /// <summary>
        /// 数据地址
        /// 4字节
        /// </summary>
        public Int32 lpData { get; set; }

        public Int32 OffsetX { get; set; }
        public Int32 OffsetY { get; set; }


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
