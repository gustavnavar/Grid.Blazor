using System;
using System.Globalization;

namespace GridShared.Utility
{
    public class DateTimeUtils
    {
        public static int GetIso8601WeekOfYear(object dateTime)
        {
            if (dateTime != null && dateTime.GetType() == typeof(DateTime))
            {
                DateTime time = (DateTime)dateTime;
                Calendar cal = CultureInfo.InvariantCulture.Calendar;
                DayOfWeek day = cal.GetDayOfWeek(time);
                if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                {
                    time = time.AddDays(3);
                }
                return cal.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }
            else
                return 0;
        }

        public static Nullable<DateTime> FromString(string dateTime)
        {
            DateTime value;
            if (DateTime.TryParse(dateTime, out value))
                return value;
            return null;
        }

        public static string GetWeekDateTimeString(DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return string.Format("{0:yyyy}-W{1}", dateTime.Value, DateTimeUtils.GetIso8601WeekOfYear(dateTime.Value));
            return null;
        }

        public static string GetWeekDateTimeString(string dateTime)
        {
            var date = FromString(dateTime);
            return GetWeekDateTimeString(date);
        }

        public static Nullable<DateTime> FromIso8601WeekDate(string dateTime)
        {
            var date = dateTime.Split('-');
            if (date.Length == 2)
            {
                int year;
                if (int.TryParse(date[0], out year))
                {
                    if (date[1].StartsWith("W"))
                    {
                        int week;
                        if (int.TryParse(date[1].Remove(0, 1), out week))
                        {
                            // The 4th of January is always in ISO week 1, so stepping back to
                            // its Monday and forward by whole weeks lands on the week asked for.
                            // DayOfWeek counts Sunday as 0 and ISO counts it as 7: without the
                            // correction every year whose 4th of January is a Sunday - 2026,
                            // 2032, 2037 - comes back a week late, and the filter queries the
                            // wrong seven days without saying so.
                            DateTime result = new DateTime(year, 1, 4);
                            int isoDayOfWeek = result.DayOfWeek == DayOfWeek.Sunday
                                ? 7 : (int)result.DayOfWeek;
                            return result.AddDays((week - 1) * 7 - isoDayOfWeek + 1);
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;
        }

        public static string GetDateTimeStringFromIso8601WeekDate(string dateTime)
        {
            var date = FromIso8601WeekDate(dateTime);
            if (date.HasValue)
                return string.Format("{0:yyyy-MM-dd}", date.Value);
            return null;
        }

        public static Nullable<DateTime> FromMonthDate(string dateTime)
        {
            var date = GetDateTimeStringFromMonthDate(dateTime);
            return FromString(date);
        }

        public static string GetMonthDateTimeString(string dateTime)
        {
            var date = dateTime.Split('-');
            if (date.Length == 3)
            {
                return date[0] + "-" + date[1];
            }
            else
                return null;
        }

        public static string GetDateTimeStringFromMonthDate(string dateTime)
        {
            return dateTime + "-01";
        }

        public static string GetDateTimeStringFromDateTimeLocal(string dateTime)
        {
            return dateTime + "Z";
        }
    }
}
