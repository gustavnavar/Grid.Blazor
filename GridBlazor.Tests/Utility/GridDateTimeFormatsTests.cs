using GridShared.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace GridBlazor.Tests.Utility
{
    /// <summary>
    ///     Pins the one rule the whole date handling rests on: what the reader sees follows the
    ///     locale, what the client and the server exchange never does. The two are easy to
    ///     confuse — they were the same string until this work — and confusing them reads
    ///     <c>09/01</c> as the ninth of January when it is the first of September, which is a
    ///     mistake nothing in the text reveals.
    /// </summary>
    [TestClass]
    public class GridDateTimeFormatsTests
    {
        private static readonly CultureInfo Spanish = new CultureInfo("es-ES");
        private static readonly CultureInfo American = new CultureInfo("en-US");
        private static readonly CultureInfo British = new CultureInfo("en-GB");
        private static readonly CultureInfo Swedish = new CultureInfo("sv-SE");

        private static readonly DateTime FirstOfSeptember = new DateTime(2026, 9, 1, 14, 30, 0);

        [TestMethod]
        public void TransportIsIsoInEveryCulture()
        {
            foreach (var culture in new[] { Spanish, American, British, Swedish })
            {
                var previous = CultureInfo.CurrentCulture;
                CultureInfo.CurrentCulture = culture;
                try
                {
                    Assert.AreEqual("2026-09-01",
                        GridDateTimeFormats.ToTransport(FirstOfSeptember, GridDateTimeFormats.DateType),
                        culture.Name);
                    Assert.AreEqual("2026-09-01T14:30",
                        GridDateTimeFormats.ToTransport(FirstOfSeptember, GridDateTimeFormats.DateTimeLocalType),
                        culture.Name);
                    Assert.AreEqual("2026-09",
                        GridDateTimeFormats.ToTransport(FirstOfSeptember, GridDateTimeFormats.MonthType),
                        culture.Name);
                    Assert.AreEqual("14:30",
                        GridDateTimeFormats.ToTransport(FirstOfSeptember, GridDateTimeFormats.TimeType),
                        culture.Name);
                }
                finally
                {
                    CultureInfo.CurrentCulture = previous;
                }
            }
        }

        [TestMethod]
        public void DisplayFollowsTheReadersLocale()
        {
            Assert.AreEqual("01/09/2026",
                GridDateTimeFormats.ToDisplay(FirstOfSeptember, GridDateTimeFormats.DateType, null, Spanish));
            Assert.AreEqual("09/01/2026",
                GridDateTimeFormats.ToDisplay(FirstOfSeptember, GridDateTimeFormats.DateType, null, American));
            Assert.AreEqual("01/09/2026",
                GridDateTimeFormats.ToDisplay(FirstOfSeptember, GridDateTimeFormats.DateType, null, British));
            Assert.AreEqual("2026-09-01",
                GridDateTimeFormats.ToDisplay(FirstOfSeptember, GridDateTimeFormats.DateType, null, Swedish));
        }

        [TestMethod]
        public void DisplayPadsTheDayAndTheMonth()
        {
            // The locale decides the field order and the separator; the padding is ours, so a
            // date column lines up instead of shifting a character every ninth of the month.
            Assert.AreEqual("dd/MM/yyyy", GridDateTimeFormats.DatePattern(Spanish));
            Assert.AreEqual("MM/dd/yyyy", GridDateTimeFormats.DatePattern(American));
        }

        [TestMethod]
        public void ColumnFormatWinsOverTheLocale()
        {
            Assert.AreEqual("2026|09|01",
                GridDateTimeFormats.ToDisplay(FirstOfSeptember, GridDateTimeFormats.DateType,
                    "{0:yyyy|MM|dd}", Spanish));

            // ...and never reaches the wire.
            Assert.AreEqual("2026-09-01",
                GridDateTimeFormats.ToTransport(FirstOfSeptember, GridDateTimeFormats.DateType));
        }

        [TestMethod]
        public void MonthKeepsTheLocalesFieldOrderWithoutSpellingTheMonthOut()
        {
            Assert.AreEqual("MM/yyyy", GridDateTimeFormats.MonthPattern(Spanish));
            Assert.AreEqual("yyyy-MM", GridDateTimeFormats.MonthPattern(Swedish));
        }

        [TestMethod]
        public void WhatTheReaderTypesComesBackAsIso()
        {
            Assert.AreEqual("2026-09-01",
                GridDateTimeFormats.DisplayToTransport("01/09/2026", GridDateTimeFormats.DateType, null, Spanish));
            Assert.AreEqual("2026-09-01",
                GridDateTimeFormats.DisplayToTransport("09/01/2026", GridDateTimeFormats.DateType, null, American));

            // ISO typed into a Spanish grid still means that day.
            Assert.AreEqual("2026-09-01",
                GridDateTimeFormats.DisplayToTransport("2026-09-01", GridDateTimeFormats.DateType, null, Spanish));
        }

        [TestMethod]
        public void TransportAndDisplayAreInverses()
        {
            foreach (var culture in new[] { Spanish, American, British, Swedish })
            {
                var shown = GridDateTimeFormats.TransportToDisplay("2026-09-01",
                    GridDateTimeFormats.DateType, null, culture);
                Assert.AreEqual("2026-09-01",
                    GridDateTimeFormats.DisplayToTransport(shown, GridDateTimeFormats.DateType, null, culture),
                    culture.Name);
            }
        }

        [TestMethod]
        public void TextThatIsNotADateIsHandedBackUntouched()
        {
            // A half-typed filter is still the reader's; clearing the box under them is not an
            // improvement on leaving it alone.
            Assert.AreEqual("01/",
                GridDateTimeFormats.DisplayToTransport("01/", GridDateTimeFormats.DateType, null, Spanish));
            Assert.AreEqual("01/",
                GridDateTimeFormats.TransportToDisplay("01/", GridDateTimeFormats.DateType, null, Spanish));
        }

        [TestMethod]
        public void TimeSpanIsWrittenAndNotThrown()
        {
            // A TimeSpan is a duration, so it reads none of the time-of-day specifiers a locale's
            // pattern is written in — handing it one throws rather than falling back, which is
            // what every TimeSpan column used to do.
            var value = new TimeSpan(14, 30, 0);
            Assert.AreEqual("14:30", GridDateTimeFormats.ToTransport(value, GridDateTimeFormats.TimeType));
            Assert.AreEqual("14:30",
                GridDateTimeFormats.ToDisplay(value, GridDateTimeFormats.TimeType, null, Spanish));
            Assert.AreEqual("2:30 PM",
                GridDateTimeFormats.ToDisplay(value, GridDateTimeFormats.TimeType, null, American));
        }

        [TestMethod]
        public void ADurationLongerThanADayWritesItself()
        {
            var value = new TimeSpan(30, 0, 0);
            Assert.AreEqual(value.ToString(),
                GridDateTimeFormats.ToDisplay(value, GridDateTimeFormats.TimeType, null, Spanish));
        }

        [TestMethod]
        public void ToDisplayValueReadsTheKindOffTheValuesOwnType()
        {
            // A DateTime reads as a date: the hour is noise in every row of a column that has
            // none, and a column whose hour matters declares a format that spells it.
            Assert.AreEqual("01/09/2026",
                GridDateTimeFormats.ToDisplayValue(FirstOfSeptember, Spanish));
            Assert.AreEqual("01/09/2026",
                GridDateTimeFormats.ToDisplayValue(new DateOnly(2026, 9, 1), Spanish));
            Assert.AreEqual("14:30",
                GridDateTimeFormats.ToDisplayValue(new TimeOnly(14, 30), Spanish));

            // Anything that is not a date has nothing to gain here, and says so.
            Assert.IsNull(GridDateTimeFormats.ToDisplayValue(42, Spanish));
            Assert.IsNull(GridDateTimeFormats.ToDisplayValue("2026-09-01", Spanish));
            Assert.IsNull(GridDateTimeFormats.ToDisplayValue(null, Spanish));
        }

        [TestMethod]
        public void AFilterTakesAColumnFormatOnlyWhenItWritesADateAlone()
        {
            Assert.AreEqual("dd.MM.yyyy",
                GridDateTimeFormats.FilterDatePattern("{0:dd.MM.yyyy}", Spanish));

            // A filter asks for a day and the calendar it opens can only offer one, so a format
            // demanding an hour would make every value typed into it unreadable.
            Assert.AreEqual("dd/MM/yyyy",
                GridDateTimeFormats.FilterDatePattern("{0:dd.MM.yyyy HH:mm}", Spanish));
            Assert.AreEqual("dd/MM/yyyy",
                GridDateTimeFormats.FilterDatePattern(null, Spanish));

            // A format that writes more than the value cannot be run backwards at all.
            Assert.AreEqual("dd/MM/yyyy",
                GridDateTimeFormats.FilterDatePattern("Created on {0:dd.MM.yyyy}", Spanish));
        }

        [TestMethod]
        public void PlaceholderIsSpelledTheWayTheValueIs()
        {
            Assert.AreEqual("dd/mm/yyyy",
                GridDateTimeFormats.Placeholder(GridDateTimeFormats.DateType, null, Spanish));
            Assert.AreEqual("yyyy-mm-dd",
                GridDateTimeFormats.Placeholder(GridDateTimeFormats.DateType, null, Swedish));
            Assert.AreEqual("yyyy|mm|dd",
                GridDateTimeFormats.Placeholder(GridDateTimeFormats.DateType, "{0:yyyy|MM|dd}", Spanish));

            // An ISO week number is not something a locale writes differently, and its W is a
            // letter of the notation rather than a field, so it keeps its case.
            Assert.AreEqual("yyyy-Www",
                GridDateTimeFormats.Placeholder(GridDateTimeFormats.WeekType, null, Spanish));
        }

        [TestMethod]
        public void PatternsAreCachedByPatternAndNotByCultureName()
        {
            // A culture can be cloned with its pattern replaced. Keyed on the name, both clones
            // would answer with whichever arrived first.
            var clone = (CultureInfo)Spanish.Clone();
            clone.DateTimeFormat.ShortDatePattern = "yyyy.MM.dd";

            Assert.AreEqual("dd/MM/yyyy", GridDateTimeFormats.DatePattern(Spanish));
            Assert.AreEqual("yyyy.MM.dd", GridDateTimeFormats.DatePattern(clone));
            Assert.AreEqual("dd/MM/yyyy", GridDateTimeFormats.DatePattern(Spanish));
        }
    }
}
