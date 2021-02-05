using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Threading;

namespace Reminder
{
    public class RemindRunner
    {

        private RemindItem Data;
        private Thread thread;
        public DateTime remindTime;
        private SoundPlayer player;

        public void Run(RemindItem data)
        {
            this.Data = data;
            if (data.IsActive == false)
                return;

            player = new SoundPlayer("ding.wav");
            player.Load();

            thread = new Thread(Running);
            thread.Start();
        }

        private int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());
            TimeSpan sp = end.Subtract(start);
            return sp.Days;
        }

        private void Running()
        {
            var targetTime = Data.StartTime;
            var now = DateTime.Now;
            //计算出下一次提醒的时间
            while (true)
            {
                var diffDay = DateDiff(targetTime, now);
                if (diffDay == 0 || targetTime >= now)
                {
                    remindTime = targetTime;
                    break;
                }
                if (Data.IntervalDays > 0)
                {
                    targetTime = targetTime.AddDays(Data.IntervalDays).AddMinutes(Data.OffsetMinute);
                }
                else
                    return;
            }

            while (true)
            {
                Thread.Sleep(1000);

                now = DateTime.Now;
                if (now >= remindTime)
                {
                    //程序开启后  过了30分钟内都会提示
                    if ((now - remindTime).TotalMinutes < 30d)
                    {
                        player.Play();
                    }

                    if (Data.IntervalDays > 0)
                    {
                        remindTime = remindTime.AddDays(Data.IntervalDays).AddMinutes(Data.OffsetMinute);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

    }
}
