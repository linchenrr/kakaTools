using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Reminder
{
    public class RemindConfig
    {

        public int CheckInterval = 1;
        public bool HideOnStartUp;
        public List<RemindItem> Items;

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
                    IntervalDays=3,
                    OffsetSeconds=60,
                },
                },
            };
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            return json;
        }

    }

    public class RemindItem
    {
        public bool IsActive;
        public DateTime StartTime;
        public int AdvanceSeconds = -1;

        public int IntervalDays;
        //每次提醒后下次提醒的时间偏移分钟
        public int OffsetSeconds;

        public string AdvanceSound;
        public string RemindSound;

        public string Text;

        [JsonIgnore]
        public bool NeedAdvance => AdvanceSeconds > 0;
        [JsonIgnore]
        public Action<string> ShowBalloon;
        [JsonIgnore]
        public Label Txt;

        public Action<RemindRunner> OnRemindTimeUpdate;

        //public void OnRemindTimeUpdate(DateTime time)
        //{
        //    Txt.Text = $"{time.ToShortDateString()} {time.ToShortTimeString()}";
        //}
    }

}
