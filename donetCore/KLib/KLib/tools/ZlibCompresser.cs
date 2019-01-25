using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zlib;

namespace KLib
{
    public class ZlibCompresser
    {

        static public void compress(Stream inStream, Stream outStream)
        {

            ZlibStream compressionStream = new ZlibStream(outStream, CompressionMode.Compress, true);

            inStream.CopyTo(compressionStream);

            compressionStream.Close();

        }

        static public void uncompress(Stream inStream, Stream outStream)
        {

            ZlibStream compressionStream = new ZlibStream(inStream, CompressionMode.Decompress, true);

            compressionStream.CopyTo(outStream);

        }

        static public byte[] compress(byte[] bytes)
        {
            var inStream = new MemoryStream(bytes);
            var outStream = new MemoryStream();
            compress(inStream, outStream);
            return outStream.ToArray();
        }

    }
}
