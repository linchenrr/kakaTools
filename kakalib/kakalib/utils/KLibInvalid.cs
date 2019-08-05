using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public const string AssetBundleBuilderURL = "http://www.nothingleft.cn:40000/asb.json";
        public const string ExcelToolURL = "http://www.nothingleft.cn:40000/ext.json";
        static public async Task RemoteCheckAsync(string requestURL, Action<bool> resultHandler)
        {
            var res = false;
            var request = (HttpWebRequest)WebRequest.Create(requestURL);
            //等待超时3秒
            //request.Timeout = 3 * 1000;
            //var needLog = !Console.IsInputRedirected;
            var needLog = false;
            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        if (needLog)
                            Console.WriteLine($@"request failed. StatusCode:{response.StatusCode}");
                        res = false;
                    }

                    if (needLog)
                    {
                        Console.WriteLine($@"responseInfo:{response.ContentType}, {response.ContentLength}, {response.ContentEncoding}");
                        Console.WriteLine();
                    }

                    //在这里对接收到的页面内容进行处理 
                    var responseStream = response.GetResponseStream();

                    using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        var responseJson = reader.ReadToEnd();
                        if (needLog)
                            Console.WriteLine($@"response json:
{responseJson}
");

                        var result = JsonConvert.DeserializeObject<InvalidInfo>(responseJson);

                        Console.WriteLine($@"$({result.isInvalid})");
                        res = result.isInvalid;
                    }
                }
            }
            catch (Exception e)
            {
                if (needLog)
                    Console.Write(e);
                res = false;
            }

            resultHandler(res);
        }

        public class InvalidInfo
        {
            //是否过期
            public bool isInvalid;
        }

    }
}
