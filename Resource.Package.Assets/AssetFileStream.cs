using Resource.Package.Assets.Common;
using Resource.Package.Assets.Secure;
using System.Drawing;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace Resource.Package.Assets
{
    public class AssetFileStream : IDisposable
    {
        private static Byte[] defaultKey = Encoding.UTF8.GetBytes("!1a@2b#3c$4d%5e^6f&7g*8h(9i)0j.+");
        private static readonly UInt64 MAGIC = 8319385958189441315; // #!Assets
        private FileHeader header;
        private Byte[] password;
        private List<FileInfomation> Infomations = new List<FileInfomation>();
        private FileStream fileStream;

        public static AssetFileStream Create(String filename, String password, CompressionOption compressionOption = CompressionOption.NeverCompress)
        {
            var pwd = BuildPassword(password);
            var meta = new FileHeader();
            meta.CompressOption = compressionOption;
            meta.Magic = MAGIC;
            meta.Version = new byte[] { 1, 0, 0 };
            meta.TableDataSize = 0;
            meta.TableDataAddr = 24;
            meta.NumberOfFiles = 0;
            using (var file = File.Open(filename, FileMode.Create))
            {
                using (var writer = new BinaryWriter(file))
                {
                    writer.Write(meta.Magic);
                    writer.Write(meta.Version);
                    writer.Write((Byte)meta.CompressOption);
                    writer.Write(meta.NumberOfFiles);
                    writer.Write(meta.TableDataAddr);
                    writer.Write(meta.TableDataSize);
                    var tab = AES.Encrypt(new Byte[0], pwd);
                    writer.Write(tab);
                    writer.Seek(20, SeekOrigin.Begin);
                    writer.Write(tab.Length);
                }
            };
            return Open(filename, password);
        }


        public Int32 NumberOfFiles
        {
            get
            {
                return this.header.NumberOfFiles;
            }
        }

        public static AssetFileStream Open(String filename, String password)
        {
            var stream = new AssetFileStream();
            stream.OpenFile(filename, password);
            return stream;
        }


        public void ChangePassword(String password)
        {
            this.password = BuildPassword(password);
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8, true))
            {
                this.WriteIndex(writer);
            }
        }




        private static Byte[] BuildPassword(String password)
        {
            if (String.IsNullOrEmpty(password))
            {
                return defaultKey;
            }
            var originKey = Encoding.UTF8.GetBytes(password);
            var keys = originKey.Concat(defaultKey);
            return keys.Take(32).ToArray();
        }



        public void OpenFile(String filename, String password)
        {
            header.Version = new byte[3];
            this.password = BuildPassword(password);
            this.fileStream = File.Open(filename, FileMode.Open);
            using (var reader = new BinaryReader(fileStream, Encoding.UTF8, true))
            {
                header.Magic = reader.ReadUInt64();
                if (header.Magic != MAGIC)
                {
                    throw new Exception("无效的文件格式");
                }
                reader.Read(header.Version);
                header.CompressOption = (CompressionOption)reader.ReadByte();
                header.NumberOfFiles = reader.ReadInt32();
                header.TableDataAddr = reader.ReadInt32();
                header.TableDataSize = reader.ReadInt32();
                this.ReadIndex(reader);
            }
        }



        public DataBlock Read(Int32 index)
        {
            var info = this.Infomations[index];
            using (var reader = new BinaryReader(fileStream, Encoding.UTF8, true))
            {
                var node = new DataBlock();
                var data = new Byte[info.lpSize];
                reader.BaseStream.Position = info.lpData;
                reader.Read(data);
                if (info.lpRawSize != info.lpSize)
                {
                    // 解密 data
                    data = ZLib.Decompress(data, info.lpRawSize);
                }
                node.OffsetX = info.OffsetX;
                node.OffsetY = info.OffsetY;
                node.Data = data;
                return node;
            }
        }

        public void Replace(Int32 index, DataBlock data)
        {
            var info = this.Infomations[index];
            var task = Preconditioning(data).Result;
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8, true))
            {
                info.OffsetX = task.infomation.OffsetX;
                info.OffsetY = task.infomation.OffsetY;
                info.lpRawSize = task.infomation.lpRawSize;
                if (info.lpSize >= task.infomation.lpSize)
                {
                    info.lpSize = task.infomation.lpSize;
                    writer.Seek(info.lpData, SeekOrigin.Begin);
                    writer.Write(task.Data);
                }
                else
                {
                    var addr = Apply(task.Data.Length);
                    info.lpData = addr;
                    info.lpSize = task.infomation.lpSize;
                    writer.Seek(info.lpData, SeekOrigin.Begin);
                    writer.Write(task.Data);
                }
                this.WriteIndex(writer);
            }
        }



        public void UpdateOffset(Int32 index, Point data)
        {
            var info = this.Infomations[index];
            info.OffsetX += data.X;
            info.OffsetY += data.Y;
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8, true))
            {
                this.WriteIndex(writer);
            }
        }



        public void UpdateOffsets(Dictionary<Int32, Point> datas)
        {
            foreach (var item in datas)
            {
                var info = this.Infomations[item.Key];
                info.OffsetX += item.Value.X;
                info.OffsetY += item.Value.Y;
            }
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8, true))
            {
                this.WriteIndex(writer);
            }
        }


        private async Task<FileAsyncCache> Preconditioning(DataBlock item)
        {
            var task = new FileAsyncCache();
            task.infomation.OffsetX = item.OffsetX;
            task.infomation.OffsetY = item.OffsetY;
            task.infomation.lpRawSize = item.Data.Length;
            if (header.CompressOption == CompressionOption.MuchPossibleCompress || header.CompressOption == CompressionOption.MustCompressed)
            {
                task.Data = await ZLib.Compress(item.Data);
                if (task.Data.Length > task.infomation.lpRawSize && header.CompressOption == CompressionOption.MuchPossibleCompress)
                {
                    task.Data = item.Data;
                }
            }
            else
            {
                task.Data = item.Data;
            }
            task.infomation.lpSize = task.Data.Length;
            return task;
        }


        public Int32 BatchImport(IEnumerable<DataBlock> items)
        {
            var tasks = new List<Task<FileAsyncCache>>();
            foreach (var item in items)
            {
                tasks.Add(Preconditioning(item));
            }
            Task.WaitAll(tasks.ToArray());
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8, true))
            {
                foreach (var task in tasks)
                {
                    var cache = task.Result;
                    cache.infomation.lpData = header.TableDataAddr;
                    header.TableDataAddr = cache.infomation.lpData + cache.infomation.lpSize;
                    header.NumberOfFiles++;
                    writer.Seek(cache.infomation.lpData, SeekOrigin.Begin);
                    writer.Write(cache.Data);
                    this.Infomations.Add(cache.infomation);
                }
                this.WriteIndex(writer);
                return header.NumberOfFiles;
            }
        }




        public Int32 Import(DataBlock data)
        {
            var info = new FileInfomation();
            Byte[] outData = data.Data;
            if (header.CompressOption == CompressionOption.MuchPossibleCompress || header.CompressOption == CompressionOption.MustCompressed)
            {
                outData = ZLib.Compress(data.Data).Result;
                if (outData.Length > data.Data.Length && header.CompressOption == CompressionOption.MuchPossibleCompress)
                {
                    outData = data.Data;
                }
            }
            info.OffsetX = data.OffsetX;
            info.OffsetY = data.OffsetY;
            info.lpSize = outData.Length;
            info.lpRawSize = data.Data.Length;
            return this.Import(info, outData);
        }


        private Int32 Apply(Int32 size)
        {
            var addr = header.TableDataAddr;
            header.TableDataAddr = addr + size;
            return addr;
        }


        private Int32 Import(FileInfomation info, Byte[] data)
        {
            this.Infomations.Add(info);
            info.lpData = Apply(info.lpSize);
            header.NumberOfFiles++;
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8, true))
            {
                writer.Seek(info.lpData, SeekOrigin.Begin);
                writer.Write(data);
                this.WriteIndex(writer);
                return header.NumberOfFiles - 1;
            }
        }

        public Int32 Write(Byte[] data)
        {
            return this.Import(new DataBlock() { Data = data });
        }

        public void Close()
        {
            if (fileStream != null)
            {
                fileStream.Close();
                fileStream.Dispose();
                fileStream = null;
                this.Infomations = null;
                this.password = null;
            }
        }

        public void Dispose()
        {
            this.Close();
        }
        private void ReadIndex(BinaryReader reader)
        {
            Byte[] raw;
            var data = new Byte[header.TableDataSize];
            reader.BaseStream.Position = header.TableDataAddr;
            reader.Read(data);

            try
            {
                raw = AES.Decrypt(data, this.password);
            }
            catch (Exception)
            {
                throw new Exception("无效的密码");
            }
            using (var ms = new MemoryStream(raw))
            {
                this.Infomations = new List<FileInfomation>();
                using (var msReader = new BinaryReader(ms, Encoding.UTF8, true))
                {
                    for (int i = 0; i < header.NumberOfFiles; i++)
                    {
                        var info = new FileInfomation();
                        info.OffsetX = msReader.ReadInt32();
                        info.OffsetY = msReader.ReadInt32();
                        info.lpSize = msReader.ReadInt32();
                        info.lpRawSize = msReader.ReadInt32();
                        info.lpData = msReader.ReadInt32();
                        this.Infomations.Add(info);
                    }
                }
            }
        }






        private void WriteIndex(BinaryWriter writer)
        {
            using (var ms = new MemoryStream())
            {
                using (var msWriter = new BinaryWriter(ms, Encoding.UTF8, true))
                {
                    for (int i = 0; i < header.NumberOfFiles; i++)
                    {
                        msWriter.Write(Infomations[i].OffsetX);
                        msWriter.Write(Infomations[i].OffsetY);
                        msWriter.Write(Infomations[i].lpSize);
                        msWriter.Write(Infomations[i].lpRawSize);
                        msWriter.Write(Infomations[i].lpData);
                    }
                }
                var tab = AES.Encrypt(ms.ToArray(), this.password);
                header.TableDataSize = tab.Length;
                writer.Seek(12, SeekOrigin.Begin);
                writer.Write(header.NumberOfFiles);
                writer.Write(header.TableDataAddr);
                writer.Write(header.TableDataSize);
                writer.Seek(header.TableDataAddr, SeekOrigin.Begin);
                writer.Write(tab);
            }
        }


    }
}
