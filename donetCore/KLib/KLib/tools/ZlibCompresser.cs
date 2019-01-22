using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zlib;

namespace KLib
{
    public class ZlibCompresser : ICompresser
    {

        public void compress(Stream inStream, Stream outStream)
        {

            ZlibStream compressionStream = new ZlibStream(outStream, CompressionMode.Compress, true);

            inStream.CopyTo(compressionStream);

            compressionStream.Close();

        }

        public void uncompress(Stream inStream, Stream outStream)
        {

            ZlibStream compressionStream = new ZlibStream(inStream, CompressionMode.Decompress, true);

            compressionStream.CopyTo(outStream);

        }

        static public byte[] compress(byte[] bytes)
        {
            var inStream = new MemoryStream(bytes);
            var outStream = new MemoryStream();
            (new ZlibCompresser()).compress(inStream, outStream);
            return outStream.ToArray();
        }

    }
}
