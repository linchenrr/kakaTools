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

            Console.WriteLine($@"requestURL:{requestURL}");
            Console.WriteLine($@"shellPath:{shellPath}");

            var buildParam = new BuildParam()
            {
                targetShell = shellPath,
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

                    //在这里对接收到的页面内容进行处理 
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
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
    }

    public class BuildResult
    {
        public bool success;
        public string message;
    }

}
