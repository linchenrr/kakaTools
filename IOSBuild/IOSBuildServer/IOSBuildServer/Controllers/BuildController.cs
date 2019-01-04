using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        [HttpPost]
        public ActionResult BuildIPA([FromBody] BuildParam buildParam)
        {

            var start = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = buildParam.targetShell + " " + buildParam.targetIpa,
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var buildResult = new BuildResult();

            try
            {
                var p = Process.Start(start);
                p.WaitForExit();

                var exitCode = p.ExitCode;
                var errorMsg = p.StandardError.ReadToEnd();

                if (exitCode != 0 || string.IsNullOrWhiteSpace(errorMsg) == false)
                {
                    buildResult.message = $@"build failed, exitCode:{exitCode},
shell:{buildParam.targetShell}
error:{errorMsg}
";
                }
                else
                {
                    buildResult.success = true;
                    buildResult.message = "build success";

                    var fs = new FileStream(buildParam.targetIpa, FileMode.Open);
                    return new FileStreamResult(fs, "application/octet-stream");
                }
            }
            catch (Exception e)
            {
                buildResult.message = $@"build failed, Exception:{e}";
            }

            return new JsonResult(buildResult);
        }

        public class BuildParam
        {
            public string targetShell;
            public string targetIpa;
        }

        public class BuildResult
        {
            public bool success;
            public string message;
        }

    }
}