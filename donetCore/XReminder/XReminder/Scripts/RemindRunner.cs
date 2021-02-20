using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static XReminder.GlobalValues;

namespace XReminder
{
    public class RemindRunner
    {

        static public int CheckInterval = 1000;

        public RemindItem Data;
        public DateTime remindTime;
        public bool needAdvance;
        public DateTime advanceTime;
        private MediaPlayer player;
        private MediaPlayer player_advanceSound;
        public Action<RemindRunner> OnRemindTimeUpdate;
        public Action OnDataChanged;
        public DateTime NewStartTime;

        public bool Run(RemindItem data)
        {
            needReset = false;
            this.Data = data;
            if (data.IsActive == false)
                return false;

            Running();

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
                advanceTime = remindTime.Add(-Data.PreRemindTimeSpan);
        }

        public void SetStartTimeToNow()
        {
            NewStartTime = DateTime.Now;
            needReset = true;
        }

        private void reset()
        {
            Data.StartTime = NewStartTime;
            needReset = false;
            Run(Data);
            OnDataChanged();
        }

        static private TimeSpan ZeroTimeSpan = new TimeSpan();
        private bool needReset;
        private async void Running()
        {
            try
            {
                player = new MediaPlayer();
                player.Open(new Uri(Path.Combine(StartUpDir, "Sound", Data.RemindSound)));

                await Task.Delay(1);

                var targetTime = Data.StartTime;
                var now = DateTime.Now;
                //计算出下一次提醒的时间
                while (true)
                {
                    //if (targetTime >= now)
                    //程序开启后  过了30秒内都会提示
                    if ((now - targetTime).TotalSeconds < 30d)
                    {
                        remindTime = targetTime;
                        OnRemindTimeUpdate(this);
                        break;
                    }
                    if (Data.IntervalTimeSpan > TimeSpan.Zero)
                    {
                        targetTime = targetTime.Add(Data.IntervalTimeSpan);
                    }
                    else
                    {
                        remindTime = targetTime;
                        OnRemindTimeUpdate(this);
                        return;
                    }
                    //var diffDay = DateDiff(targetTime, now);
                    //if (diffDay == 0 || targetTime >= now)
                    //{
                    //    remindTime = targetTime;
                    //    OnRemindTimeUpdate(this);
                    //    break;
                    //}
                    //if (Data.IntervalDays > 0)
                    //{
                    //    targetTime = targetTime.AddDays(Data.IntervalDays).AddSeconds(Data.OffsetSeconds);
                    //}
                    //else
                    //    return;
                }

                CalculateAdvance();
                if (needAdvance)
                {
                    player_advanceSound = new MediaPlayer();
                    player_advanceSound.Open(new Uri(Path.Combine(StartUpDir, "Sound", Data.PreRemindSound)));
                }


                while (true)
                {
                    await Task.Delay(CheckInterval);
                    if (needReset)
                    {
                        reset();
                        return;
                    }

                    now = DateTime.Now;

                    if (needAdvance)
                    {
                        if (now >= advanceTime)
                        {
                            needAdvance = false;
                            //如果还没过正式提醒时间  则播放提示
                            if (now < remindTime)
                            {
                                player_advanceSound.Stop();
                                player_advanceSound.Play();
                                Data.ShowBalloon($@"即将开始: {Data.Text}
时间：{remindTime.ToShortTimeString()}");
                            }
                        }
                    }
                    else
                    {
                        if (now >= remindTime)
                        {
                            //程序开启后  过了2分钟内都会提示
                            //if ((now - remindTime).TotalMinutes < 2d)
                            {
                                player.Stop();
                                player.Play();
                                Data.ShowBalloon($@"{Data.Text}");
                            }

                            if (Data.IntervalTimeSpan > TimeSpan.Zero)
                            {
                                remindTime = remindTime.Add(Data.IntervalTimeSpan);
                                CalculateAdvance();
                                OnRemindTimeUpdate(this);
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
