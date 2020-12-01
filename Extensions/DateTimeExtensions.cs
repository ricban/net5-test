using System;

namespace Covid19.Client.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime Tomorrow(this DateTime datetime)
        {
            return datetime.AddDays(1);
        }

        public static DateTime Yesterday(this DateTime datetime)
        {
            return datetime.AddDays(-1);
        }

        public static string UpdatedAgo(this DateTime dateTime, string prefix = "")
        {
            const int DaysInYear = 365;
            const decimal DaysInMonth = 30.4166667M;

            var timespan = DateTime.Now - dateTime;
            var value = 1M;
            var unit = "minute";

            if (timespan.Days > 0)
            {
                if (timespan.Days >= DaysInYear)
                {
                    value = Math.Floor(decimal.Divide(timespan.Days, DaysInYear));
                    unit = "year";
                }
                else if (timespan.Days >= DaysInMonth)
                {
                    value = Math.Floor(decimal.Divide(timespan.Days, DaysInMonth));
                    unit = "month";
                }
                else if (timespan.Days > 0)
                {
                    value = timespan.Days;
                    unit = "day";
                }
            }
            else if (timespan.Hours > 0)
            {
                value = timespan.Hours;
                unit = "hour";
            }
            else if (timespan.Minutes > 0)
            {
                value = timespan.Minutes;
            }

            return $"{prefix} {value} {unit}{(value > 1 ? "s" : "")} ago".Trim();
        }
    }
}