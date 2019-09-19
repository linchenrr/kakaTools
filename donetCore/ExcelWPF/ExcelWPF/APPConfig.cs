using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExcelWPF
{
    public class APPConfig
    {
        public string ProjDir;
        public string ExcelLocalBaseDir;
        public string CodeOutputPath;

        public BranchConfig[] Branchs;
        public FTPConfig[] Ftps;

        static public APPConfig Config { get; private set; }

        static public void Init()
        {
            var json = File.ReadAllText("config.json", Encoding.UTF8);
            Config = JsonConvert.DeserializeObject<APPConfig>(json);
        }

        static public string ToJson()
        {
            var cfg = new APPConfig();
            cfg.Branchs = new BranchConfig[] { new BranchConfig(), new BranchConfig() };
            cfg.Ftps = new FTPConfig[] { new FTPConfig()
            {
                name="测试1服",
                host="ftp.nothingleft.cn",
                port=40021,
                userName="Share",
                pwd="3364476",
            } };
            var str = JsonConvert.SerializeObject(cfg, Formatting.Indented);
            return str;
        }

    }

    public class BranchConfig
    {
        public string Name { get; set; }
        public string LocalFolder { get; set; }
        public string SvnPath { get; set; }
    }

    public class FTPConfig
    {
        public string name { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public string url { get { return host + ":" + port; } }
        public string userName { get; set; }
        public string pwd { get; set; }
    }

}
