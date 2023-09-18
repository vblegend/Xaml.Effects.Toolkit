using Resource.Package.Assets.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resource.Package.Assets.Version
{
    internal class Stream_1_0_1 : InfomationStream
    {
        public override FileInfomation Read(BinaryReader msReader)
        {
            var info = new FileInfomation();
            info.OffsetX = msReader.ReadInt16();
            info.OffsetY = msReader.ReadInt16();
            info.lpType = (ImageTypes)msReader.ReadByte();
            info.unknown1 = msReader.ReadByte();
            info.unknown2 = msReader.ReadByte();
            info.unknown3 = msReader.ReadByte();
            info.lpSize = msReader.ReadInt32();
            info.lpRawSize = msReader.ReadInt32();
            info.lpData = msReader.ReadInt32();
            return info;
        }

        public override void Write(BinaryWriter msWriter, FileInfomation info)
        {
            msWriter.Write(info.OffsetX);
            msWriter.Write(info.OffsetY);
            msWriter.Write((Byte)info.lpType);
            msWriter.Write(info.unknown1);
            msWriter.Write(info.unknown2);
            msWriter.Write(info.unknown3);
            msWriter.Write(info.lpSize);
            msWriter.Write(info.lpRawSize);
            msWriter.Write(info.lpData);
        }
    }
}
