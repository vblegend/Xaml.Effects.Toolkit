using Resource.Package.Assets.Common;


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
            info.lpRenderType = (RenderTypes)msReader.ReadByte();
            info.Unknown2 = msReader.ReadByte();
            info.Unknown1 = msReader.ReadByte();
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
            msWriter.Write((Byte)info.lpRenderType);
            msWriter.Write(info.Unknown2);
            msWriter.Write(info.Unknown1);
            msWriter.Write(info.lpSize);
            msWriter.Write(info.lpRawSize);
            msWriter.Write(info.lpData);
        }
    }
}
