using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IOSBuildClient
{
    class BuildRunner
    {

        public void Start(CommandDictionary commands)
        {
            var requestURL = commands.GetValue("requestURL", "http://192.168.61.129:8001/build/BuildIPA");
            var shellPath = commands.GetValue("shellPath", "/Users/kaka/Desktop/share/buildIPA.sh");
            var ipaPath = commands.GetValue("ipaPath", "/Users/kaka/Desktop/share/output.ipa");
            var writeLocalIpa = commands.GetValue("writeLocalIpa", "/output.ipa");

            Console.WriteLine($@"requestURL:{requestURL}");
            Console.WriteLine($@"shellPath:{shellPath}");

            var buildParam = new BuildParam()
            {
                targetShell = shellPath,
                targetIpa = ipaPath,
            };

            try
            {
                var paramBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(buildParam));
                var request = (HttpWebRequest)WebRequest.Create(requestURL);
                //等待超时40分钟
                request.Timeout = 40 * 60 * 1000;
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                request.ContentLength = paramBytes.Length;

                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(paramBytes, 0, paramBytes.Length);
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($@"request failed. StatusCode:{response.StatusCode}");
                        return;
                    }

                    Console.WriteLine(response.ContentType, response.ContentLength, response.ContentEncoding);

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
message:{result.message}");
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($@"Error:
{e.ToString()}");

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
        public string message;
    }

}
