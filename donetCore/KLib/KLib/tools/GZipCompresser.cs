using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KLib;
using Ionic.Zlib;

namespace KLib
{
    public class GZipCompresser
    {

        static public byte[] compress(byte[] bytes)
        {
            var output = new MemoryStream();
            var gzipStream = new GZipStream(output, CompressionMode.Compress, true);
            gzipStream.Write(bytes, 0, bytes.Length);
            gzipStream.Close();
            return output.ToArray();
        }

        static public void compress(Stream inStream, Stream outStream)
        {

            GZipStream compressionStream = new GZipStream(outStream, CompressionMode.Compress, true);

            inStream.CopyTo(compressionStream);

            compressionStream.Close();

        }

        static public void uncompress(Stream inStream, Stream outStream)
        {

            GZipStream compressionStream = new GZipStream(inStream, CompressionMode.Decompress, true);

            compressionStream.CopyTo(outStream);

        }

    }
}
