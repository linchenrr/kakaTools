using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reminder
{
    public class RemindConfig
    {

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
                    OffsetMinute=2,
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

        public int IntervalDays;
        //每次提醒后下次提醒的时间偏移分钟
        public int OffsetMinute;
    }

}
