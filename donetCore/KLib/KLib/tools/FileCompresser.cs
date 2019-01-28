using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using KLib;

namespace KLib
{
    public class FileCompresser
    {

        private delegate void CompressProcesser(Stream inStream, Stream outStream);

        private CompressProcesser compresser;
        private CompressProcesser uncompresser;

        public void setCompressAlgorithm(CompressOption algorithm)
        {

            switch (algorithm)
            {
                case CompressOption.lzma:
                    compresser = LZMACompresser.compress;
                    uncompresser = LZMACompresser.uncompress;
                    break;

                case CompressOption.gzip:
                    compresser = GZipCompresser.compress;
                    uncompresser = GZipCompresser.uncompress;
                    break;

                case CompressOption.zlib:
                    compresser = ZlibCompresser.compress;
                    uncompresser = ZlibCompresser.uncompress;
                    break;

            }

        }

        private int doProcess(CompressProcesser processer, string input, string output)
        {

            int success = 0;
            int i = 0;

            //处理文件
            if (File.Exists(input))
            {
                try
                {
                    Console.WriteLine($@"处理文件
input:{input}
output:{output}");
                    using (var inStream = File.Open(input, FileMode.Open, FileAccess.Read))
                    {
                        using (var ms = new MemoryStream())
                        {
                            processer(inStream, ms);

                            inStream.Close();

                            var outStream = File.Create(output);
                            ms.WriteTo(outStream);
                            outStream.Close();

                            success++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($@"处理{input}时出现异常:
{e}");
                }
            }
            //处理目录
            else if (Directory.Exists(input))
            {
                var inDir = new DirectoryInfo(input);
                var outDir = new DirectoryInfo(output);
                if (outDir.Exists == false)
                {
                    Console.WriteLine($@"创建目录{outDir.FullName}");
                    outDir.Create();
                }


                var files = inDir.GetFiles();
                foreach (var file in files)
                {
                    success += doProcess(processer, file.FullName, output + "/" + file.Name);
                }

                var dirs = inDir.GetDirectories();
                foreach (var dir in dirs)
                {
                    success += doProcess(processer, dir.FullName, output + "/" + dir.Name);
                }
            }

            return success;
        }

        public int compress(string input, string output)
        {
            return doProcess(compresser, input, output);
        }

        public int uncompress(string input, string output)
        {
            return doProcess(uncompresser, input, output);
        }

    }
}
