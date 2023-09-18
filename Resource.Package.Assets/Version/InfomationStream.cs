using Resource.Package.Assets.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resource.Package.Assets.Version
{
    internal abstract class InfomationStream
    {


        public static InfomationStream GetReader(Byte[] version)
        {
            if (version[0] == 1 && version[1] == 0 && version[2] == 0) return new Stream_1_0_0();
            if (version[0] == 1 && version[1] == 0 && version[2] == 1) return new Stream_1_0_1();
            throw new ArgumentException("无效的资源包版本号");
        }




        public abstract FileInfomation Read(BinaryReader msReader);



        public abstract void Write(BinaryWriter msReader, FileInfomation info);


    }
}
