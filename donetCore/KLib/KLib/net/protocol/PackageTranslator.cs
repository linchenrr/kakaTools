using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KLib;

namespace protocol
{

    public class PackageTranslator
    {

        public byte[] Encode(BaseProtocolVO vo)
        {
            var binWriter = new ProtocolBinaryWriter(new MemoryStream());
            binWriter.Write(vo.MessageId);
            vo.encode(binWriter);
            binWriter.Seek(0, SeekOrigin.Begin);
            int len = (int)binWriter.BaseStream.Length;
            var bytes = new byte[len];
            binWriter.BaseStream.Read(bytes, 0, len);
            return bytes;
        }

        public BaseProtocolVO Decode(byte[] bytes)
        {
            var binReader = new ProtocolBinaryReader(new MemoryStream(bytes));
            var id = binReader.ReadInt32();

            var vo = ProtocolCenter.CreateProtocolVO(id);
            vo.decode(binReader);

            return vo;
        }

    }
}