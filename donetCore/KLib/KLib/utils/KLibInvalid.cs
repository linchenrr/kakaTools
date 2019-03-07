using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KLib
{
    public class KLibInvalid
    {
        //是否已经过期
        static public bool IsInvalid
        {
            get
            {
                var date = DateTime.Now;
                return date > ExpiresTime;
            }
        }

        static public DateTime ExpiresTime
        {
            get { return new DateTime(2019, 3, 22); }
        }

        public const string AssetBundleBuilderURL = "http://www.nothingleft.cn:40000/site/asb";
        public const string ExcelToolURL = "http://www.nothingleft.cn:40000/site/ext";
        static public async Task RemoteCheckAsync(string requestURL, Action<InvalidInfo> resultHandler)
        {
            InvalidInfo info = new InvalidInfo();

            var request = (HttpWebRequest)WebRequest.Create(requestURL);
            //等待超时3秒
            //request.Timeout = 3 * 1000;
            var needLog = !Console.IsInputRedirected;
            needLog = false;
            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        if (needLog)
                            Console.WriteLine($@"request failed. StatusCode:{response.StatusCode}");
                    }

                    //在这里对接收到的页面内容进行处理 
                    var responseStream = response.GetResponseStream();
                    if (response.ContentType == "application/octet-stream")
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


                        var bytes = LZMACompresser.uncompress(ms.ToArray());
                        var responseJson = Encoding.UTF8.GetString(bytes);
                        if (needLog)
                            Console.WriteLine($@"response:
{responseJson}
");
                        info = JsonConvert.DeserializeObject<InvalidInfo>(responseJson);

                        var mark = info.IsInvalid ? "..." : "....";

                        Console.WriteLine($@"$init{mark}");
                    }
                }
            }
            catch (Exception e)
            {
                if (needLog)
                    Console.Write(e);
            }

            resultHandler(info);
        }

        public class InvalidInfo
        {
            public string expiresTime;
            public string serverTime;
            public bool forceExpires = false;
            public bool disable_win = false;
            public bool disable_osx = false;

            public bool IsInvalid
            {
                get
                {
                    if (disable_win == false)
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            return false;
                    }
                    if (disable_osx == false)
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            return false;
                    }

                    if (forceExpires)
                        return true;

                    DateTime now;
                    if (DateTime.TryParse(serverTime, out now) == false)
                    {
                        now = DateTime.Now;
                    }
                    DateTime expiresDate;
                    if (DateTime.TryParse(expiresTime, out expiresDate))
                    {
                        return now > expiresDate;
                    }
                    return false;
                }
            }
        }
    }
}
