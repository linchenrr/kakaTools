using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using static XReminder.GlobalValues;

namespace XReminder
{
    public class RemindRunner
    {

        static public int CheckInterval = 1000;

        public RemindItem Data;
        private Thread thread;
        public DateTime remindTime;
        public bool needAdvance;
        public DateTime advanceTime;
        private MediaPlayer player;
        private MediaPlayer player_advanceSound;

        public bool Run(RemindItem data)
        {
            this.Data = data;
            if (data.IsActive == false)
                return false;

            thread = new Thread(Running);
            thread.Start();

            return true;
        }

        private int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());
            TimeSpan sp = end.Subtract(start);
            return sp.Days;
        }

        private void CalculateAdvance()
        {
            needAdvance = Data.NeedAdvance;
            if (needAdvance)
                advanceTime = remindTime.AddSeconds(-Data.AdvanceSeconds);
        }

        private void Running()
        {
            try
            {
                //Thread.Sleep(500);
                //if (Data.BalloonText != null)
                //{
                //    Data.ShowBalloon(Data.BalloonText);
                //MessageBox.Show(Data.BalloonText);
                //}


                player = new MediaPlayer();
                player.Open(new Uri(Path.Combine(StartUpDir, Data.RemindSound)));

                var targetTime = Data.StartTime;
                var now = DateTime.Now;
                //计算出下一次提醒的时间
                while (true)
                {
                    var diffDay = DateDiff(targetTime, now);
                    if (diffDay == 0 || targetTime >= now)
                    {
                        remindTime = targetTime;
                        Data.OnRemindTimeUpdate(this);
                        break;
                    }
                    if (Data.IntervalDays > 0)
                    {
                        targetTime = targetTime.AddDays(Data.IntervalDays).AddSeconds(Data.OffsetSeconds);
                    }
                    else
                        return;
                }

                CalculateAdvance();
                if (needAdvance)
                {
                    player_advanceSound = new MediaPlayer();
                    player_advanceSound.Open(new Uri(Path.Combine(StartUpDir, Data.AdvanceSound)));
                }


                while (true)
                {
                    Thread.Sleep(CheckInterval);

                    now = DateTime.Now;

                    if (needAdvance)
                    {
                        if (now >= advanceTime)
                        {
                            needAdvance = false;
                            //如果还没过正式提醒时间  则播放提示
                            if (now < remindTime)
                            {
                                player_advanceSound.Play();
                                Data.ShowBalloon($@"预先提醒: {Data.Text}
时间：{remindTime.ToShortTimeString()}");
                            }
                        }
                    }
                    else
                    {
                        if (now >= remindTime)
                        {
                            //程序开启后  过了2分钟内都会提示
                            if ((now - remindTime).TotalMinutes < 2d)
                            {
                                player.Play();
                                Data.ShowBalloon($@"{Data.Text}");
                            }

                            if (Data.IntervalDays > 0)
                            {
                                remindTime = remindTime.AddDays(Data.IntervalDays).AddSeconds(Data.OffsetSeconds);
                                CalculateAdvance();
                                Data.OnRemindTimeUpdate(this);
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($@"错误：
{e.Message}");
                return;
            }

        }

    }
}
