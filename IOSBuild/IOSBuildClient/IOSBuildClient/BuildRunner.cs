using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IOSBuildClient
{
    class BuildRunner
    {

        public bool Start(CommandDictionary commands)
        {
            var requestURL = commands.GetValue("requestURL", "http://192.168.61.129:8001/build/BuildIPA");
            var outputURL = commands.GetValue("outputURL", "http://192.168.61.129:8001/build/BuildOutput");
            var shellPath = commands.GetValue("shellPath", "/Users/kaka/Desktop/share/buildIPA.sh");
            var ipaPath = commands.GetValue("ipaPath", "/Users/kaka/Desktop/share/output.ipa");
            var writeLocalIpa = commands.GetValue("writeLocalIpa", "/output.ipa");
            var timeOut = int.Parse(commands.GetValue("timeOut", "3000"));

            Console.WriteLine();
            foreach (var kv in commands)
            {
                Console.WriteLine($@"{kv.Key}:{kv.Value}");
            }

            var buildParam = new BuildParam()
            {
                targetShell = shellPath,
                targetIpa = ipaPath,
            };

            try
            {
                var paramBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(buildParam));
                var request = (HttpWebRequest)WebRequest.Create(requestURL);
                //等待超时
                request.Timeout = timeOut * 1000;
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                request.ContentLength = paramBytes.Length;

                var thread = new Thread(() =>
                  {
                      Thread.Sleep(1000);
                      if (buildComplete)
                      {
                          outputGetComplete = true;
                          return;
                      }

                      while (buildComplete == false)
                      {
                          GetNewOutput(outputURL);
                          Thread.Sleep(1000);
                      }
                      GetNewOutput(outputURL);
                      outputGetComplete = true;
                  });

                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(paramBytes, 0, paramBytes.Length);
                }

                Console.WriteLine();
                Console.WriteLine();

                thread.Start();

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    SetBuildComplete();
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($@"request failed. StatusCode:{response.StatusCode}");
                        return false;
                    }

                    Console.WriteLine($@"responseInfo:{response.ContentType}, {response.ContentLength}, {response.ContentEncoding}");
                    Console.WriteLine();

                    //在这里对接收到的页面内容进行处理 
                    var responseStream = response.GetResponseStream();
                    if (response.ContentType == "application/octet-stream")
                    {
                        //response is ipa file
                        Console.WriteLine($@"build success, response is ipa file
downloading ipa...");
                        if (string.IsNullOrWhiteSpace(writeLocalIpa) == false)
                        {
                            var fs = File.Create(writeLocalIpa);
                            int i = 0;
                            var bufferSize = 1024 * 1024 * 10;
                            var buffer = new byte[bufferSize];
                            do
                            {
                                i = responseStream.Read(buffer, 0, bufferSize);

                                fs.Write(buffer, 0, i);
                            }
                            while (i > 0);

                            fs.Close();

                            Console.WriteLine($@"download ipa complete.
{writeLocalIpa}");
                        }
                    }
                    else
                    {
                        using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            var responseJson = reader.ReadToEnd();
                            Console.WriteLine($@"response json:
{responseJson}
");

                            var result = JsonConvert.DeserializeObject<BuildResult>(responseJson);
                            if (result.success)
                            {
                                Console.WriteLine($@"build success!");
                            }
                            else
                            {
                                Console.WriteLine($@"build failed!
message:{result.errorMsg}");
                                return false;
                            }
                        }
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                SetBuildComplete();
                Console.WriteLine($@"Error:
{e.ToString()}");

            }
            return false;
        }

        private bool buildComplete = false;
        private bool outputGetComplete = false;
        private void SetBuildComplete()
        {
            buildComplete = true;
            while (outputGetComplete == false)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private void GetNewOutput(string outputURL)
        {
            try
            {
                var outputRequest = (HttpWebRequest)WebRequest.Create(outputURL);
                outputRequest.Method = "GET";

                using (var response = (HttpWebResponse)outputRequest.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($@"request output log failed. StatusCode:{response.StatusCode}");
                        return;
                    }

                    var responseStream = response.GetResponseStream();
                    using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        var responseString = reader.ReadToEnd();
                        Console.Write(responseString);
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine($@"获取编译实时输出失败
{e}");
            }
        }

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
