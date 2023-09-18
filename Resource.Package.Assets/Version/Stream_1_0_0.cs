using Resource.Package.Assets.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resource.Package.Assets.Version
{
    internal class Stream_1_0_0 : InfomationStream
    {
        public override FileInfomation Read(BinaryReader msReader)
        {
            var info = new FileInfomation();
            info.OffsetX = (Int16)msReader.ReadInt32();
            info.OffsetY = (Int16)msReader.ReadInt32();
            info.lpSize = msReader.ReadInt32();
            info.lpRawSize = msReader.ReadInt32();
            info.lpData = msReader.ReadInt32();
            return info;
        }

        public override void Write(BinaryWriter msWriter, FileInfomation info)
        {
            msWriter.Write(info.OffsetX);
            msWriter.Write(info.OffsetY);
            msWriter.Write(info.lpSize);
            msWriter.Write(info.lpRawSize);
            msWriter.Write(info.lpData);
        }
    }
}
