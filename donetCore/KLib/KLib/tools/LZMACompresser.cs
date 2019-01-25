using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SevenZip;
using lzma = SevenZip.Compression.LZMA;

namespace KLib
{
    public class LZMACompresser
    {

        static public byte[] compress(byte[] bytes)
        {
            byte[] outBytes = null;
            using (var inStream = new MemoryStream(bytes))
            {
                using (var outStream = new MemoryStream())
                {
                    compress(inStream, outStream);
                    outStream.Position = 0;
                    outBytes = new byte[outStream.Length];
                    outStream.Read(outBytes, 0, outBytes.Length);
                }
            }
            return outBytes;
        }

        static public byte[] uncompress(byte[] bytes)
        {
            byte[] outBytes = null;
            using (var inStream = new MemoryStream(bytes))
            {
                using (var outStream = new MemoryStream())
                {
                    uncompress(inStream, outStream);
                    outStream.Position = 0;
                    outBytes = new byte[outStream.Length];
                    outStream.Read(outBytes, 0, outBytes.Length);
                }
            }
            return outBytes;
        }

        static public void compress(Stream inStream, Stream outStream)
        {

            var coder = new lzma.Encoder();

            // Write the encoder properties
            coder.WriteCoderProperties(outStream);

            // Write the decompressed file size.
            outStream.Write(BitConverter.GetBytes(inStream.Length), 0, 8);

            // Encode the file.
            coder.Code(inStream, outStream, inStream.Length, -1, null);

        }

        static public void uncompress(Stream inStream, Stream outStream)
        {

            var coder = new lzma.Decoder();

            // Read the decoder properties
            byte[] properties = new byte[5];
            inStream.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            inStream.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            // Decompress the file.
            coder.SetDecoderProperties(properties);
            coder.Code(inStream, outStream, inStream.Length, fileLength, null);

        }

    }
}
