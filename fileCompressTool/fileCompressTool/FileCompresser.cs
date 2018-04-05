using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using KLib;

namespace fileCompressTool
{
    public class FileCompresser
    {

        private delegate void CompressProcesser(Stream inStream, Stream outStream);

        private ICompresser compresser;

        public void setCompressAlgorithm(CompressOption algorithm)
        {

            switch (algorithm)
            {
                case CompressOption.lzma:
                    compresser = new LZMACompresser();
                    break;

                case CompressOption.gzip:
                    compresser = new GZipCompresser();
                    break;

                case CompressOption.zlib:
                    compresser = new ZlibCompresser();
                    break;

            }

        }

        private int doProcess(CompressProcesser processer, String[] pathList, String newFileSuffix = "")
        {

            int success = 0;
            int i = 0;
            while (i < pathList.Length)
            {
                String path = pathList[i];

                //处理文件
                if (File.Exists(path))
                {
                    Stream inStream = null;
                    try
                    {

                        String outputPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + newFileSuffix + Path.GetExtension(path);

                        inStream = File.Open(path, FileMode.Open, FileAccess.Read);
                        MemoryStream ms = new MemoryStream();

                        processer(inStream, ms);

                        inStream.Close();

                        Stream outStream = File.Create(outputPath);
                        ms.WriteTo(outStream);

                        outStream.Close();

                        success++;

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("处理" + path + "时出现异常:\r\n" + e.Message);
                    }
                    finally
                    {
                        try
                        {
                            inStream.Dispose();
                        }
                        catch { }
                    }
                }
                //处理目录
                else if (Directory.Exists(path))
                {

                    DirectoryInfo di = new DirectoryInfo(path);

                    FileSystemInfo[] fileInfos = di.GetFileSystemInfos();

                    ArrayList newPathList = new ArrayList();

                    for (int k = 0; k < fileInfos.Length; k++)
                    {
                        //如果不是隐藏文件  则添加文件或子目录
                        if ((fileInfos[k].Attributes & FileAttributes.Hidden) == 0)
                        {
                            newPathList.Add(fileInfos[k].FullName);
                        }

                    }

                    success += doProcess(processer, (String[])newPathList.ToArray(Type.GetType("System.String")), newFileSuffix);

                }

                i++;
            }

            return success;

        }

        public int compress(String[] pathList, String newFileSuffix = "")
        {

            return doProcess(compresser.compress, pathList, newFileSuffix);

        }

        public int uncompress(String[] pathList, String newFileSuffix = "")
        {

            return doProcess(compresser.uncompress, pathList, newFileSuffix);

        }

    }
}
