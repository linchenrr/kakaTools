using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace IOSBuildServer.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class BuildController : Controller
    {
        [HttpGet]
        public ActionResult Test()
        {
            return new JsonResult("It's running!");
        }

        [HttpGet]
        public ActionResult readFile(string path)
        {
            var fs = new FileStream("/Users/kaka/Desktop/share/1.pkg", FileMode.Open);
            return new FileStreamResult(fs, "application/octet-stream");
        }

        [HttpGet]
        public ActionResult<string> testSH()
        {
            var start = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                //Arguments = "build/buildAssetBundle.sh " + outputPath + " " + DLCReleasePath;
                Arguments = "/Users/kaka/Desktop/share/test.sh",
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var p = Process.Start(start);
            p.WaitForExit();

            var exitCode = p.ExitCode;
            return $@"exitCode:{exitCode}
{p.StandardOutput.ReadToEnd()}
{p.StandardError.ReadToEnd()}";
        }

        //        [HttpPost]
        //        public ActionResult BuildIPA([FromBody] BuildParam buildParam)
        //        {

        //            var start = new ProcessStartInfo
        //            {
        //                FileName = "/bin/sh",
        //                Arguments = buildParam.targetShell + " " + buildParam.targetIpa,
        //                CreateNoWindow = false,
        //                UseShellExecute = false,
        //                RedirectStandardOutput = true,
        //                RedirectStandardError = true,
        //            };

        //            var buildResult = new BuildResult();

        //            try
        //            {
        //                var p = Process.Start(start);
        //                p.WaitForExit();

        //                var exitCode = p.ExitCode;
        //                var buildMsg = p.StandardOutput.ReadToEnd();
        //                var errorMsg = p.StandardError.ReadToEnd();

        //                if (exitCode != 0 || string.IsNullOrWhiteSpace(errorMsg) == false)
        //                {
        //                    buildResult.buildMsg = buildMsg;
        //                    buildResult.errorMsg = $@"exitCode:{exitCode},
        //shell:{buildParam.targetShell}
        //error:{errorMsg}
        //";
        //                }
        //                else
        //                {
        //                    buildResult.success = true;
        //                    buildResult.buildMsg = "build success";

        //                    var fs = new FileStream(buildParam.targetIpa, FileMode.Open);
        //                    return new FileStreamResult(fs, "application/octet-stream");
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                buildResult.errorMsg = $@"build failed, Exception:{e}";
        //            }

        //            return new JsonResult(buildResult);
        //        }

        static private bool isBuilding;
        static private object lockObj = new object();
        static private StringBuilder newOutput;

        [HttpPost]
        public ActionResult BuildIPA([FromBody] BuildParam buildParam)
        {

            var buildResult = new BuildResult();

            if (isBuilding)
            {
                buildResult.success = false;
                buildResult.errorMsg = "目前正有另一个编译在进行，需等待其完成后再试";
                return new JsonResult(buildResult);
            }

            isBuilding = true;

            lock (lockObj)
            {
                newOutput = new StringBuilder();
            }

            var start = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = buildParam.targetShell + " " + buildParam.targetIpa,
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            try
            {
                var p = Process.Start(start);

                var outputSb = new StringBuilder();
                var buffer = new char[1024 * 20];
                while (p.HasExited == false)
                {
                    var readNum = p.StandardOutput.Read(buffer, 0, buffer.Length);
                    lock (lockObj)
                    {
                        newOutput.Append(buffer, 0, readNum);
                    }
                    outputSb.Append(buffer, 0, readNum);
                }

                var exitCode = p.ExitCode;
                var buildMsg = outputSb.ToString();
                var errorMsg = p.StandardError.ReadToEnd();

                if (exitCode != 0 || string.IsNullOrWhiteSpace(errorMsg) == false)
                {
                    buildResult.buildMsg = buildMsg;
                    buildResult.errorMsg = $@"exitCode:{exitCode},
shell:{buildParam.targetShell}
error:{errorMsg}
";
                }
                else
                {
                    buildResult.success = true;
                    buildResult.buildMsg = "build success";

                    var fs = new FileStream(buildParam.targetIpa, FileMode.Open);
                    isBuilding = false;
                    return new FileStreamResult(fs, "application/octet-stream");
                }
            }
            catch (Exception e)
            {
                buildResult.errorMsg = $@"build failed, Exception:{e}";
            }

            isBuilding = false;
            return new JsonResult(buildResult);
        }

        [HttpGet]
        public string BuildOutput()
        {
            string returnValue = null;
            lock (lockObj)
            {
                if (newOutput == null)
                    return "";
                returnValue = newOutput.ToString(); ;
                newOutput.Clear();
            }
            return returnValue;
        }

        public class BuildParam
        {
            public string targetShell;
            public string targetIpa;
        }

        public class BuildResult
        {
            public bool success;
            public string buildMsg;
            public string errorMsg;
        }

    }
}