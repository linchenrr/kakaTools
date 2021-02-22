using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public class UpdateChecker
{

    public const string UpdateInfoFileName = "UnityCracker.json";
    static public string UpdateInfoURL = $"http://tools.local/{UpdateInfoFileName}";
    static public async Task RemoteCheckAsync(Action<VersionInfo> resultHandler)
    {
        var request = (HttpWebRequest)WebRequest.Create(UpdateInfoURL);
        request.Timeout = 4000;
        try
        {
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($@"request failed. StatusCode:{response.StatusCode}");
                    RunFail(resultHandler);
                    return;
                }

                //在这里对接收到的页面内容进行处理 
                var responseStream = response.GetResponseStream();
                //if (response.ContentType == "application/octet-stream")
                {
                    var ms = new MemoryStream();
                    int i = 0;
                    var bufferSize = 1024;
                    var buffer = new byte[bufferSize];
                    do
                    {
                        i = responseStream.Read(buffer, 0, bufferSize);

                        ms.Write(buffer, 0, i);
                    }
                    while (i > 0);


                    var bytes = ms.ToArray();
                    var responseJson = new UTF8Encoding(false).GetString(bytes);
                    var info = JsonConvert.DeserializeObject<VersionInfo>(responseJson);
                    resultHandler(info);
                }
            }
        }
        catch (Exception e)
        {
            RunFail(resultHandler);
        }
    }

    static private void RunFail(Action<VersionInfo> resultHandler)
    {
        var info = new VersionInfo() { IsEnable = false };
        resultHandler(info);
    }
}


public class VersionInfo
{
    public bool IsEnable;
}

