using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLib
{
    public class TimeUtils
    {

        static private DateTime serverTime = new DateTime();
        static private DateTime serverTimeUpdateTime = DateTime.Now;

        static private readonly DateTime time_1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        static public DateTime ServerTime
        {
            get
            {
                var time = new DateTime(serverTime.Ticks + (DateTime.Now.Ticks - serverTimeUpdateTime.Ticks), DateTimeKind.Utc);
                return time;
            }
            set
            {
                TimeUtils.serverTime = value;
                serverTimeUpdateTime = DateTime.Now;
            }
        }

        static public uint DateTimeToSeconds(DateTime dt)
        {
            return TicksToSeconds(dt.ToUniversalTime().Ticks - time_1970.Ticks);
        }

        /// <summary>
        /// 将时间戳转换为DateTime
        /// </summary>
        /// <param name="timestamp">秒</param>
        /// <returns></returns>
        static public DateTime SecondsToDateTime(uint timestamp)
        {
            return time_1970.AddSeconds(timestamp).ToLocalTime();
        }

        static public DateTime SecondsToDateTime(string timestamp)
        {
            return SecondsToDateTime(Convert.ToUInt32(timestamp));
        }

        static public long TicksToSeconds(string ticks)
        {
            return TicksToSeconds(Convert.ToInt64(ticks));
        }

        static public uint TicksToSeconds(long ticks)
        {
            return (uint)(ticks / 10000000);
        }

        static public long SecondsToTicks(uint seconds)
        {
            return seconds * 10000000;
        }

        //服务器当前时间与指定时间相差的秒数
        static public int SecondsFromServerTime(uint seconds)
        {
            var dt = SecondsToDateTime(seconds);
            var pastSeconds = TicksToSeconds(ServerTime.Ticks - dt.Ticks);
            return Convert.ToInt32(pastSeconds);
        }

        /// <summary>
        /// 将秒数转换成表示一段时间的对象
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        static public TimeSpan parseSecondToTimeVO(uint seconds)
        {
            /*
            var vo = new TimeVO();
            int day = 24 * 3600;
            vo.day = seconds / day;
            vo.hour = (seconds % day) / 3600;
            vo.minutes = (seconds % 3600) / 60;
            vo.seconds = seconds % 60;
             * return vo;
            */
            var timeSpan = new TimeSpan(((long)seconds) * 10000 * 1000);
            return timeSpan;
        }

        /// <summary>
        /// 将秒数转换成表示一段时间的字符串
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        static public string parseSecondToTimeString(uint seconds, string pattern = "$(day)天$(hour)小时$(minute)分$(second)秒")
        {
            var vo = parseSecondToTimeVO(seconds);
            var result = pattern.Replace("$(day)", vo.Days.ToString());
            result = result.Replace("$(hour)", formatTimePart(vo.Hours));
            result = result.Replace("$(minute)", formatTimePart(vo.Minutes));
            result = result.Replace("$(second)", formatTimePart(vo.Seconds));
            return result;
            //时间比较紧  先写死
            //return vo.Days + "天" + vo.Hours + "小时" + vo.Minutes + "分钟" + vo.Seconds + "秒";
        }

        static public string formatTimePart(int num)
        {
            if (num < 10 && num >= 0)
                return "0" + num;
            return num.ToString();
        }

    }

    /*
    public class TimeVO
    {
        public int day;
        public int hour;
        public int minutes;
        public int seconds;
    }
    */
}