using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using KLib;

namespace UnityBuildTool
{
    public class BuildVersionProcesser
    {
        static public void Process(string buildInfoPath, string tamplateDir)
        {
            Console.WriteLine("buildInfoPath:" + buildInfoPath);
            Console.WriteLine("tamplateDir:" + tamplateDir);

            var xml = XElement.Load(buildInfoPath);

            Console.WriteLine("buildInfo:");
            Console.WriteLine(xml.ToString());

            var viewVersion = xml.Element("viewVersion").Value;
            var buildVersion = xml.Element("buildVersion").Value;
            var sourceFileName = xml.Element("sourceFileName").Value;
            var targetFileName = xml.Element("targetFileName").Value;
            var changeList = xml.Element("changeList").Value;

            var inputDir = Path.GetDirectoryName(buildInfoPath) + "/";

            var inputFile = inputDir + sourceFileName;

            if (File.Exists(inputFile))
            {
                var targetFile = inputDir + targetFileName;
                var APKBytes = File.ReadAllBytes(inputFile);
                if (File.Exists(targetFile))
                    File.Delete(targetFile);

                File.Move(inputFile, targetFile);

                foreach (var tamplatePath in Directory.GetFiles(tamplateDir))
                {
                    var tamplate_updateInfo = File.ReadAllText(tamplatePath);
                    var fileName = Path.GetFileName(tamplatePath);

                    tamplate_updateInfo = tamplate_updateInfo.Replace("$(viewVersion)", viewVersion);
                    tamplate_updateInfo = tamplate_updateInfo.Replace("$(buildVersion)", buildVersion);
                    tamplate_updateInfo = tamplate_updateInfo.Replace("$(updateURL)", targetFileName);
                    tamplate_updateInfo = tamplate_updateInfo.Replace("$(changeList)", changeList);

                    tamplate_updateInfo = tamplate_updateInfo.Replace("$(md5)", MD5Utils.BytesToMD5(APKBytes));
                    tamplate_updateInfo = tamplate_updateInfo.Replace("$(bytesTotal)", $@"{APKBytes.Length.ToString()} ({((float)APKBytes.Length / 1048576).ToString("F2")}MB)");

                    File.WriteAllText(inputDir + fileName, tamplate_updateInfo, Encoding.UTF8);
                }
            }
            else
            {
                throw new Exception($@"原始文件不存在:{inputFile}");
            }

            Console.WriteLine("完成");
        }
    }
}
