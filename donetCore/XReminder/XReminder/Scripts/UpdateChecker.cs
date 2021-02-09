using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XReminder
{
    public class UpdateChecker
    {

        public const string UpdateInfoFileName = "XReminderUpdate.json";
        static public string UpdateInfoURL = $"http://www.nothingleft.cn:40000/site/{UpdateInfoFileName}";
        static public async Task RemoteCheckAsync(Action<VersionInfo> resultHandler)
        {
            var request = (HttpWebRequest)WebRequest.Create(UpdateInfoURL);
            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($@"request failed. StatusCode:{response.StatusCode}");
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
                Console.Write(e);
            }
        }
    }

    public class VersionInfo
    {
        public string Version;
        public string DownLoadURL;
        public string Code;
        public DateTime BuildDate;
        public string UpdateInfo;
    }
}
