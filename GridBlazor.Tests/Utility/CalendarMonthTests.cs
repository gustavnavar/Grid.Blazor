using GridShared.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;

namespace GridBlazor.Tests.Utility
{
    /// <summary>
    ///     The grid a date picker draws. This is the half that cannot be checked by eye: an
    ///     off-by-one in the leading offset shifts every cell in the month, and the reader has no
    ///     way to tell a wrong calendar from a right one except by knowing what day it is.
    /// </summary>
    [TestClass]
    public class CalendarMonthTests
    {
        private static readonly DateTime FirstOfSeptember = new DateTime(2026, 9, 1);

        private static CultureInfo C(string name)
        {
            return CultureInfo.GetCultureInfo(name);
        }

        [TestMethod]
        public void TheWeekStartsWhereTheReadersWeekStarts()
        {
            // Monday-first and Sunday-first are both common and neither is a safe default: the
            // same month is drawn one column apart in Madrid and in Chicago.
            var spanish = CalendarMonth.For(FirstOfSeptember, C("es-ES"));
            Assert.AreEqual("lun", spanish.DayNames[0]);
            Assert.AreEqual(new DateTime(2026, 8, 31), spanish.Weeks[0][0].Date);
            Assert.AreEqual(DayOfWeek.Monday, spanish.Weeks[0][0].Date.DayOfWeek);

            var american = CalendarMonth.For(FirstOfSeptember, C("en-US"));
            Assert.AreEqual("Sun", american.DayNames[0]);
            Assert.AreEqual(new DateTime(2026, 8, 30), american.Weeks[0][0].Date);
            Assert.AreEqual(DayOfWeek.Sunday, american.Weeks[0][0].Date.DayOfWeek);
        }

        [TestMethod]
        public void TheHeadingsAreShortEnoughToFitAndStillTellTheDaysApart()
        {
            // Latin and Japanese abbreviations are short and distinct, so they stay.
            CollectionAssert.AreEqual(new[] { "lun", "mar", "mié", "jue", "vie", "sáb", "dom" },
                CalendarMonth.For(FirstOfSeptember, C("es-ES")).DayNames.ToArray());
            CollectionAssert.AreEqual(new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" },
                CalendarMonth.For(FirstOfSeptember, C("en-US")).DayNames.ToArray());

            // Persian abbreviates to whole words, which do not fit a column one day wide - and a
            // Persian calendar is written with single letters anyway.
            var persian = CalendarMonth.For(FirstOfSeptember, C("fa-IR")).DayNames;
            foreach (var name in persian)
                Assert.IsTrue(name.Length <= 4, "fa-IR heading too long: " + name);

            var arabic = CalendarMonth.For(FirstOfSeptember, C("ar-SA")).DayNames;
            foreach (var name in arabic)
                Assert.IsTrue(name.Length <= 4, "ar-SA heading too long: " + name);
        }

        [TestMethod]
        public void TheGridIsAlwaysSixRowsOfSeven()
        {
            // Always six, so the popup keeps its height as the reader pages through the year. A
            // control that resizes under the pointer loses the click already on its way.
            foreach (var name in new[] { "es-ES", "en-US", "fa-IR", "th-TH", "ar-SA", "sv-SE" })
            {
                var month = CalendarMonth.For(FirstOfSeptember, C(name));
                Assert.AreEqual(6, month.Weeks.Count, name);
                Assert.AreEqual(7, month.DayNames.Count, name);
                foreach (var week in month.Weeks)
                    Assert.AreEqual(7, week.Count, name);
            }
        }

        [TestMethod]
        public void TheDaysRunWithoutAGapOrARepeat()
        {
            foreach (var name in new[] { "es-ES", "en-US", "fa-IR", "th-TH", "ar-SA" })
            {
                var month = CalendarMonth.For(FirstOfSeptember, C(name));
                var dates = month.Weeks.SelectMany(w => w).Select(d => d.Date).ToList();

                Assert.AreEqual(42, dates.Count, name);
                for (var i = 1; i < dates.Count; i++)
                    Assert.AreEqual(dates[i - 1].AddDays(1), dates[i], name + " at " + i);
            }
        }

        [TestMethod]
        public void EachCalendarCountsItsOwnMonth()
        {
            // The same instant sits in a different month, of a different length, with a
            // different name and year, depending on the reader's calendar. Translating the
            // Gregorian month would be the wrong answer, not a rougher one.
            var persian = CalendarMonth.For(FirstOfSeptember, C("fa-IR"));
            Assert.AreEqual(1405, persian.Year);
            Assert.AreEqual(31, persian.Weeks.SelectMany(w => w).Count(d => d.InMonth));

            var buddhist = CalendarMonth.For(FirstOfSeptember, C("th-TH"));
            Assert.AreEqual(2569, buddhist.Year);
            Assert.AreEqual(30, buddhist.Weeks.SelectMany(w => w).Count(d => d.InMonth));

            var hijri = CalendarMonth.For(FirstOfSeptember, C("ar-SA"));
            Assert.AreEqual(1448, hijri.Year);
            Assert.AreEqual(29, hijri.Weeks.SelectMany(w => w).Count(d => d.InMonth));

            var gregorian = CalendarMonth.For(FirstOfSeptember, C("es-ES"));
            Assert.AreEqual(2026, gregorian.Year);
            Assert.AreEqual(9, gregorian.Month);
            Assert.AreEqual(30, gregorian.Weeks.SelectMany(w => w).Count(d => d.InMonth));
        }

        [TestMethod]
        public void TheDayNumbersAreTheReadersAndTheDatesAreTheModelsl()
        {
            // What is printed is the day of the reader's calendar; what a cell selects is the
            // Gregorian instant the model holds. Confusing the two is how a picker comes to
            // write a date six centuries out.
            var persian = CalendarMonth.For(FirstOfSeptember, C("fa-IR"));
            var firstOfMonth = persian.Weeks.SelectMany(w => w).First(d => d.InMonth);

            Assert.AreEqual(1, firstOfMonth.DayNumber);
            Assert.AreEqual(new DateTime(2026, 8, 23), firstOfMonth.Date);
        }

        [TestMethod]
        public void TheDaysEitherSideAreMarkedAsSuch()
        {
            var month = CalendarMonth.For(FirstOfSeptember, C("es-ES"));
            var all = month.Weeks.SelectMany(w => w).ToList();

            Assert.IsFalse(all[0].InMonth);
            Assert.AreEqual(31, all[0].DayNumber);
            Assert.IsTrue(all[1].InMonth);
            Assert.AreEqual(1, all[1].DayNumber);
            Assert.IsFalse(all[all.Count - 1].InMonth);
        }

        [TestMethod]
        public void PagingCountsMonthsInTheReadersCalendar()
        {
            var month = CalendarMonth.For(FirstOfSeptember, C("es-ES"));
            Assert.AreEqual(10, month.Add(1).Month);
            Assert.AreEqual(8, month.Add(-1).Month);

            // Twelve steps is a year wherever the reader is, even where a year is not twelve
            // Gregorian months.
            foreach (var name in new[] { "es-ES", "fa-IR", "th-TH", "ar-SA" })
            {
                var start = CalendarMonth.For(FirstOfSeptember, C(name));
                var later = start.Add(12);
                Assert.AreEqual(start.Year + 1, later.Year, name);
                Assert.AreEqual(start.Month, later.Month, name);
            }
        }

        [TestMethod]
        public void AYearHasAsManyMonthsAsTheCalendarSays()
        {
            // Twelve is not universal. A lunisolar calendar has thirteen in a leap year, and a
            // grid that assumes twelve would simply hide one.
            foreach (var name in new[] { "es-ES", "en-US", "fa-IR", "th-TH", "ar-SA", "he-IL" })
            {
                var month = CalendarMonth.For(FirstOfSeptember, C(name));
                var expected = C(name).Calendar.GetMonthsInYear(month.Year);
                Assert.AreEqual(expected, month.MonthNames.Count, name);
                CollectionAssert.AllItemsAreNotNull(month.MonthNames.ToArray(), name);
            }

            var spanish = CalendarMonth.For(FirstOfSeptember, C("es-ES"));
            Assert.AreEqual(12, spanish.MonthNames.Count);
            Assert.AreEqual("Enero", spanish.MonthNames[0]);
            Assert.AreEqual("Diciembre", spanish.MonthNames[11]);
        }

        [TestMethod]
        public void APickedMonthLandsOnTheDayTheModelHolds()
        {
            // What the reader picks is a month of their calendar; what it selects is the
            // Gregorian instant the model stores.
            var persian = CalendarMonth.For(FirstOfSeptember, C("fa-IR"));
            var first = persian.FirstDayOf(persian.Month);

            Assert.AreEqual(new DateTime(2026, 8, 23), first);
            Assert.AreEqual(persian.Month, C("fa-IR").Calendar.GetMonth(first));
        }

        [TestMethod]
        public void PagingByYearKeepsTheMonthWhereItCan()
        {
            foreach (var name in new[] { "es-ES", "fa-IR", "th-TH", "ar-SA" })
            {
                var start = CalendarMonth.For(FirstOfSeptember, C(name));
                var later = start.AddYears(1);
                Assert.AreEqual(start.Year + 1, later.Year, name);
                Assert.IsTrue(later.Month <= C(name).Calendar.GetMonthsInYear(later.Year), name);
            }
        }

        [TestMethod]
        public void TheHeadingNamesTheMonthTheReaderKnows()
        {
            StringAssert.Contains(CalendarMonth.For(FirstOfSeptember, C("en-US")).Label, "September");
            StringAssert.Contains(CalendarMonth.For(FirstOfSeptember, C("en-US")).Label, "2026");

            // Not "September" in Persian letters: a different month altogether.
            StringAssert.Contains(CalendarMonth.For(FirstOfSeptember, C("fa-IR")).Label, "1405");
            StringAssert.Contains(CalendarMonth.For(FirstOfSeptember, C("th-TH")).Label, "2569");
        }
    }
}
