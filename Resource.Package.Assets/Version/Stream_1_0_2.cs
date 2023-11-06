using Resource.Package.Assets.Common;


namespace Resource.Package.Assets.Version
{
    internal class Stream_1_0_2 : InfomationStream
    {
        public override FileInfomation Read(BinaryReader msReader)
        {
            var info = new FileInfomation();
            info.Width = msReader.ReadUInt16();
            info.Height = msReader.ReadUInt16();
            info.OffsetX = msReader.ReadInt16();
            info.OffsetY = msReader.ReadInt16();
            info.lpType = (ImageTypes)msReader.ReadByte();
            info.lpRenderType = (RenderTypes)msReader.ReadByte();
            info.Unknown2 = msReader.ReadByte();
            info.Unknown1 = msReader.ReadByte();
            info.lpSize = msReader.ReadUInt32();
            info.lpRawSize = msReader.ReadUInt32();
            info.lpData = msReader.ReadUInt32();
            return info;
        }

        public override void Write(BinaryWriter msWriter, FileInfomation info)
        {
            msWriter.Write((UInt16)info.Width);
            msWriter.Write((UInt16)info.Height);
            msWriter.Write(info.OffsetX);
            msWriter.Write(info.OffsetY);
            msWriter.Write((Byte)info.lpType);
            msWriter.Write((Byte)info.lpRenderType);
            msWriter.Write(info.Unknown2);
            msWriter.Write(info.Unknown1);
            msWriter.Write(info.lpSize);
            msWriter.Write(info.lpRawSize);
            msWriter.Write(info.lpData);
        }
    }
}
