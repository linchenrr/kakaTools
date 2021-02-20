using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using static XReminder.GlobalValues;

namespace XReminder
{
    public class RemindConfig
    {
        static public Encoding Encoding = new UTF8Encoding(false);

        public int CheckInterval = 1;
        public List<RemindItem> Items;

        public void Init()
        {
            if (CheckInterval < 1)
            {
                CheckInterval = 1;
            }
            foreach (var item in Items)
            {
                item.Init();
            }
        }

        static public string MakeDefaultConfig()
        {
            var config = new RemindConfig()
            {
                Items = new List<RemindItem>() {
                new RemindItem()
                {
                    IsActive=true,
                    //StartTime=DateTime.Now.AddMinutes(1),
                    StartTime=DateTime.Now.AddMinutes(-10),
                    Interval="3d 1m 30s",
                },
                },
            };
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            return json;
        }

    }

    public class RemindItem
    {
        public bool IsTestItem;
        public bool IsActive;
        public DateTime StartTime;

        //10d 20h5m  6s
        public string Interval;
        public TimeSpan IntervalTimeSpan;

        public string PreRemind;
        public TimeSpan PreRemindTimeSpan;

        public string PreRemindSound;
        public string RemindSound;

        public string Text;

        [JsonIgnore]
        public bool NeedAdvance => PreRemindTimeSpan > TimeSpan.Zero;
        [JsonIgnore]
        public Action<string> ShowBalloon;


        static private int GetTimeNum(Regex reg, string text)
        {
            //var str = @"10d 20h5m  6s";
            //text = str;
            var matchs = reg.Matches(text);
            if (matchs.Count > 0)
            {
                var d = matchs[0].Groups[1];
                return int.Parse(d.Value);
            }
            else
            {
                return 0;
            }
        }

        static private TimeSpan GetTimeSpan(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return TimeSpan.Zero;
            return new TimeSpan(GetTimeNum(reg_d, text), GetTimeNum(reg_h, text), GetTimeNum(reg_m, text), GetTimeNum(reg_s, text));
        }

        static private Regex reg_d = new Regex(@"(\d+)d", RegexOptions.IgnoreCase);
        static private Regex reg_h = new Regex(@"(\d+)h", RegexOptions.IgnoreCase);
        static private Regex reg_m = new Regex(@"(\d+)m", RegexOptions.IgnoreCase);
        static private Regex reg_s = new Regex(@"(\d+)s", RegexOptions.IgnoreCase);

        public void Init()
        {
            IntervalTimeSpan = GetTimeSpan(Interval);
            PreRemindTimeSpan = GetTimeSpan(PreRemind);
        }

    }

}
