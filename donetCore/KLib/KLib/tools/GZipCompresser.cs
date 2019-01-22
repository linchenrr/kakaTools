using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using KLib;
using Ionic.Zlib;

namespace KLib
{
    public class GZipCompresser : ICompresser
    {

        public void compress(Stream inStream, Stream outStream)
        {

            GZipStream compressionStream = new GZipStream(outStream, CompressionMode.Compress, true);

            inStream.CopyTo(compressionStream);

            compressionStream.Close();

        }

        public void uncompress(Stream inStream, Stream outStream)
        {

            GZipStream compressionStream = new GZipStream(inStream, CompressionMode.Decompress, true);

            compressionStream.CopyTo(outStream);

        }

    }
}
