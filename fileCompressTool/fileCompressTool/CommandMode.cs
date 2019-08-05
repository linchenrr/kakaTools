using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using KLib;

namespace fileCompressTool
{
    public class CommandMode
    {

        private delegate int CompressProcesser(string input, string output);

        static public void exec(Dictionary<String, String> args)
        {

            FileCompresser fc = new FileCompresser();
            fc.setCompressAlgorithm(CompressOption.lzma);

            if (args.ContainsKey("a"))
            {
                switch (args["a"].ToLower())
                {
                    case "lzma":
                        fc.setCompressAlgorithm(CompressOption.lzma);
                        break;

                    case "zlib":
                        fc.setCompressAlgorithm(CompressOption.zlib);
                        break;

                    case "gzip":
                        fc.setCompressAlgorithm(CompressOption.gzip);
                        break;

                    default:
                        MessageBox.Show("无效的参数 -a:" + args["a"]);
                        return;
                }
            }

            CompressProcesser processer = fc.compress;
            if (args.ContainsKey("op"))
            {
                switch (args["op"].ToLower())
                {
                    case "compress":
                    case "c":
                        processer = fc.compress;
                        break;

                    case "uncompress":
                    case "extract":
                    case "e":
                        processer = fc.uncompress;
                        break;

                    default:
                        MessageBox.Show("无效的参数: -op:" + args["op"]);
                        return;
                }
            }

            if (args.ContainsKey("input"))
            {
                var inputPath = args["input"].Trim();
                var outputPath = inputPath;
                if (args.ContainsKey("output"))
                    outputPath = args["output"].Trim();
                processer(inputPath, outputPath);
            }

        }
    }
}
