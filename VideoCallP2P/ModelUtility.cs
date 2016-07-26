using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCallP2P
{
    public static class ModelUtility
    {

        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }

        public static long CurrentTimeMillis(this DateTime dt)
        {
            return (long)((dt - Jan1st1970).TotalMilliseconds);
        }

        public static DateTime GetLocalDateTime(long ms)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(ms).ToLocalTime();
        }

        public static long GetStartTimeMillisLocal(this DateTime dt)
        {
            return dt.ChangeTime(0, 0, 0, 0).CurrentTimeMillis() + (long)((DateTime.UtcNow - DateTime.Now).TotalMilliseconds);
        }

        public static long GetStartTimeMillis(long ms)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(ms).ChangeTime(0, 0, 0, 0).CurrentTimeMillis();
        }

        public static bool IsSameDateOf(this DateTime dt, DateTime cmpDt)
        {
            if (dt.Year == cmpDt.Year && dt.DayOfYear == cmpDt.DayOfYear)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key)
        {
            TValue ignored;
            return self.TryRemove(key, out ignored);
        }

        public static TValue TryGetValue<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key)
        {
            TValue value;
            return self.TryGetValue(key, out value) ? value : default(TValue);
        }

        public static string IsNull(string value)
        {
            if (value == null)
                return String.Empty;
            return Escape(value);
        }

        public static string IsExceded(string value, int length)
        {
            value = IsNull(value);
            value = value.Length > length ? value.Substring(0, length) : value;
            return value;
        }

        public static int IsBoolean(bool value)
        {
            if (value)
                return 1;
            return 0;
        }

        public static string Escape(string data)
        {
            data = data.Replace("'", "''");
            //data = data.Replace("\\", "\\\\");
            return data;
        }

        public static long ConvertToLong(object data)
        {
            long value = 0;
            try
            {
                value = long.Parse(data.ToString());
            }
            catch (Exception)
            {
                value = 0;

            }
            return value;
        }
        public static DateTime DateTimeFromMillisSince1970(long ms)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(ms).ToLocalTime();
            return date;
        }
        public static long MillisFromDateTimeSince1970(DateTime? d)
        {
            DateTime dt = d ?? default(DateTime);
            TimeSpan span = dt.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return ((long)span.TotalMilliseconds);
        }
    }
}
