using System;
using System.Collections.Generic;
using System.Globalization;

namespace GridShared.Utility
{
    /// <summary>
    ///     One day in a month grid: the date it stands for, and whether it belongs to the month
    ///     being shown or is spill from the one before or after.
    /// </summary>
    public struct CalendarDay
    {
        public CalendarDay(DateTime date, int dayNumber, bool inMonth)
        {
            Date = date;
            DayNumber = dayNumber;
            InMonth = inMonth;
        }

        /// <summary>The moment this cell selects, always Gregorian - it is what the model holds.</summary>
        public DateTime Date { get; private set; }

        /// <summary>The number to print, in the reader's calendar rather than the Gregorian one.</summary>
        public int DayNumber { get; private set; }

        /// <summary>False for the days either side that fill the first and last rows.</summary>
        public bool InMonth { get; private set; }
    }

    /// <summary>
    ///     The month a date picker draws, laid out for the reader's calendar and week.
    ///     <para>
    ///     Everything here goes through <see cref="CultureInfo.Calendar"/> rather than through
    ///     <c>DateTime</c>'s own arithmetic, which is Gregorian and only Gregorian. A Persian or
    ///     a Buddhist reader does not want September 2026 with translated labels: they want
    ///     Shahrivar 1405, whose month starts on a different day and has a different length.
    ///     Getting that wrong is not a cosmetic slip - it offsets every cell in the grid.
    ///     </para>
    ///     <para>
    ///     The dates the cells carry stay Gregorian <c>DateTime</c>s throughout, because that is
    ///     what the model holds and what travels to the server. Only what is printed changes.
    ///     </para>
    /// </summary>
    public class CalendarMonth
    {
        private const int Rows = 6;
        private const int Columns = 7;

        private readonly CultureInfo _culture;

        private CalendarMonth(CultureInfo culture, int year, int month)
        {
            _culture = culture;
            Year = year;
            Month = month;
        }

        /// <summary>The year in the reader's calendar, not the Gregorian one.</summary>
        public int Year { get; private set; }

        /// <summary>The month in the reader's calendar, 1-based.</summary>
        public int Month { get; private set; }

        /// <summary>The month whose grid contains this instant.</summary>
        public static CalendarMonth For(DateTime date, CultureInfo culture)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            return new CalendarMonth(culture, calendar.GetYear(date), calendar.GetMonth(date));
        }

        /// <summary>
        ///     The month this many months away. Counted in the reader's calendar, so twelve steps
        ///     is a year there even where a year is not twelve Gregorian months.
        /// </summary>
        public CalendarMonth Add(int months)
        {
            var calendar = _culture.Calendar;
            var moved = calendar.AddMonths(FirstDay(), months);
            return new CalendarMonth(_culture, calendar.GetYear(moved), calendar.GetMonth(moved));
        }

        /// <summary>The heading: the month named as the reader names it, and the year.</summary>
        public string Label
        {
            get
            {
                var name = _culture.DateTimeFormat.GetMonthName(Month);
                if (!string.IsNullOrEmpty(name))
                    name = char.ToUpper(name[0], _culture) + name.Substring(1);
                return name + " " + Year.ToString(_culture);
            }
        }

        /// <summary>
        ///     The year this many years away, keeping the month where it can.
        /// </summary>
        public CalendarMonth AddYears(int years)
        {
            var calendar = _culture.Calendar;
            var moved = calendar.AddYears(FirstDay(), years);
            var year = calendar.GetYear(moved);
            // A month that does not exist in the year landed on - the thirteenth of a leap year
            // in a lunisolar calendar - falls back to the last one that does.
            var month = Math.Min(Month, calendar.GetMonthsInYear(year));
            return new CalendarMonth(_culture, year, month);
        }

        /// <summary>The same year, a different month.</summary>
        public CalendarMonth WithMonth(int month)
        {
            return new CalendarMonth(_culture, Year, month);
        }

        /// <summary>
        ///     The months of this year, named as the reader names them. Not always twelve: a
        ///     lunisolar calendar such as the Hebrew one has thirteen in a leap year, and a month
        ///     grid that assumes twelve would hide one.
        /// </summary>
        public IList<string> MonthNames
        {
            get
            {
                var count = _culture.Calendar.GetMonthsInYear(Year);
                var names = new List<string>(count);
                for (var m = 1; m <= count; m++)
                {
                    var name = _culture.DateTimeFormat.GetMonthName(m);
                    if (!string.IsNullOrEmpty(name))
                        name = char.ToUpper(name[0], _culture) + name.Substring(1);
                    names.Add(name);
                }
                return names;
            }
        }

        /// <summary>The first day of a given month of this year, as the model holds it.</summary>
        public DateTime FirstDayOf(int month)
        {
            return _culture.Calendar.ToDateTime(Year, month, 1, 0, 0, 0, 0);
        }

        /// <summary>
        ///     The column headings, starting on the day the reader's week starts. Sunday-first
        ///     and Monday-first are both common and neither is a safe default.
        /// </summary>
        public IList<string> DayNames
        {
            get
            {
                var names = HeadingNames();
                var first = (int)_culture.DateTimeFormat.FirstDayOfWeek;
                var result = new List<string>(Columns);
                for (var i = 0; i < Columns; i++)
                    result.Add(names[(first + i) % Columns]);
                return result;
            }
        }

        /// <summary>
        ///     Six rows of seven days. Always six, so the popup does not change height as the
        ///     reader pages through the year - a control that resizes under the pointer loses
        ///     the click that was already on its way.
        /// </summary>
        public IList<IList<CalendarDay>> Weeks
        {
            get
            {
                var calendar = _culture.Calendar;
                var first = FirstDay();

                // Step back to the start of the week the 1st falls in.
                var offset = ((int)first.DayOfWeek - (int)_culture.DateTimeFormat.FirstDayOfWeek + Columns) % Columns;
                var cursor = first.AddDays(-offset);

                var weeks = new List<IList<CalendarDay>>(Rows);
                for (var row = 0; row < Rows; row++)
                {
                    var week = new List<CalendarDay>(Columns);
                    for (var column = 0; column < Columns; column++)
                    {
                        var inMonth = calendar.GetYear(cursor) == Year && calendar.GetMonth(cursor) == Month;
                        week.Add(new CalendarDay(cursor, calendar.GetDayOfMonth(cursor), inMonth));
                        cursor = cursor.AddDays(1);
                    }
                    weeks.Add(week);
                }
                return weeks;
            }
        }

        /// <summary>
        ///     Which of the two sets of short day names a heading can hold.
        ///     <para>
        ///     The abbreviated ones read best where they are short - <c>lun</c>, <c>Mon</c>,
        ///     <c>日</c>. Persian and Arabic abbreviate to whole words (<c>چهارشنبه</c>) which
        ///     collide into an unreadable smear in a column one day wide, and those languages
        ///     write their calendars with a single letter anyway. So the shortest set is not a
        ///     fallback there, it is what a reader expects.
        ///     </para>
        ///     <para>
        ///     It is not the right default everywhere, though: English shortens to S M T W T F S
        ///     and Spanish to D L M X J V S, both with letters repeating. Hence the measurement
        ///     rather than a preference.
        ///     </para>
        /// </summary>
        private string[] HeadingNames()
        {
            var abbreviated = _culture.DateTimeFormat.AbbreviatedDayNames;
            foreach (var name in abbreviated)
            {
                if (name != null && name.Length > 4)
                    return _culture.DateTimeFormat.ShortestDayNames;
            }
            return abbreviated;
        }

        private DateTime FirstDay()
        {
            return _culture.Calendar.ToDateTime(Year, Month, 1, 0, 0, 0, 0);
        }
    }
}
