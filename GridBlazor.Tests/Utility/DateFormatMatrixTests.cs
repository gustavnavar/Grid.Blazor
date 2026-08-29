using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Filtering.Types;
using GridShared.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace GridBlazor.Tests.Utility
{
    /// <summary>
    ///     The five date formats the grid knows, across the locales that write them awkwardly.
    ///     <para>
    ///     <see cref="GridDateTimeFormatsTests"/> pins the rule; this pins the matrix. They are
    ///     separate because the rule is one sentence and the matrix is where it actually breaks:
    ///     a locale that quotes a word inside its pattern, one that puts the day last, one whose
    ///     calendar is not Gregorian at all.
    ///     </para>
    /// </summary>
    [TestClass]
    public class DateFormatMatrixTests
    {
        private const string Date = GridDateTimeFormats.DateType;
        private const string Time = GridDateTimeFormats.TimeType;
        private const string DateTimeLocal = GridDateTimeFormats.DateTimeLocalType;
        private const string Month = GridDateTimeFormats.MonthType;
        private const string Week = GridDateTimeFormats.WeekType;

        private static readonly DateTime Sample = new DateTime(2026, 9, 1, 14, 30, 0);

        private static CultureInfo C(string name)
        {
            return CultureInfo.GetCultureInfo(name);
        }

        [TestMethod]
        public void EachTypeIsWrittenItsOwnWayForTheReader()
        {
            var spanish = C("es-ES");
            Assert.AreEqual("01/09/2026", GridDateTimeFormats.ToDisplay(Sample, Date, null, spanish));
            Assert.AreEqual("14:30", GridDateTimeFormats.ToDisplay(Sample, Time, null, spanish));
            Assert.AreEqual("01/09/2026 14:30", GridDateTimeFormats.ToDisplay(Sample, DateTimeLocal, null, spanish));
            Assert.AreEqual("09/2026", GridDateTimeFormats.ToDisplay(Sample, Month, null, spanish));
            Assert.AreEqual("2026-W36", GridDateTimeFormats.ToDisplay(Sample, Week, null, spanish));

            // The same five, read by somebody whose locale puts the month first and the hour on
            // a twelve-hour clock.
            var american = C("en-US");
            Assert.AreEqual("09/01/2026", GridDateTimeFormats.ToDisplay(Sample, Date, null, american));
            Assert.AreEqual("2:30 PM", GridDateTimeFormats.ToDisplay(Sample, Time, null, american));
            Assert.AreEqual("09/01/2026 2:30 PM", GridDateTimeFormats.ToDisplay(Sample, DateTimeLocal, null, american));
            Assert.AreEqual("09/2026", GridDateTimeFormats.ToDisplay(Sample, Month, null, american));
        }

        [TestMethod]
        public void EachTypeTravelsAsIso()
        {
            Assert.AreEqual("2026-09-01", GridDateTimeFormats.ToTransport(Sample, Date));
            Assert.AreEqual("14:30", GridDateTimeFormats.ToTransport(Sample, Time));
            Assert.AreEqual("2026-09-01T14:30", GridDateTimeFormats.ToTransport(Sample, DateTimeLocal));
            Assert.AreEqual("2026-09", GridDateTimeFormats.ToTransport(Sample, Month));
            Assert.AreEqual("2026-W36", GridDateTimeFormats.ToTransport(Sample, Week));
        }

        [TestMethod]
        public void EveryTypeSurvivesTheRoundTripInEveryLocale()
        {
            // The one property the whole design rests on: whatever the reader is shown, what
            // reaches the server is the day they meant. A break here is a filter that silently
            // queries another date.
            var transports = new[]
            {
                new[] { Date, "2026-09-01" },
                new[] { Time, "14:30" },
                new[] { DateTimeLocal, "2026-09-01T14:30" },
                new[] { Month, "2026-09" },
                new[] { Week, "2026-W36" }
            };

            foreach (var name in new[] { "es-ES", "en-US", "en-GB", "sv-SE", "de-DE", "nl-NL",
                                         "hu-HU", "hr-HR", "sl-SI", "bg-BG", "ja-JP" })
            {
                var culture = C(name);
                foreach (var pair in transports)
                {
                    var type = pair[0];
                    var iso = pair[1];
                    var shown = GridDateTimeFormats.TransportToDisplay(iso, type, null, culture);
                    Assert.AreEqual(iso,
                        GridDateTimeFormats.DisplayToTransport(shown, type, null, culture),
                        name + " / " + type + " showed " + shown);
                }
            }
        }

        [TestMethod]
        public void AWeekIsIsoWhoeverReadsIt()
        {
            // An ISO week number is the one date format no locale writes differently, which is
            // why it is the one place a literal placeholder is still the right answer.
            foreach (var name in new[] { "es-ES", "en-US", "ja-JP", "bg-BG" })
            {
                Assert.AreEqual("2026-W36", GridDateTimeFormats.ToDisplay(Sample, Week, null, C(name)), name);
                Assert.AreEqual("yyyy-Www", GridDateTimeFormats.Placeholder(Week, null, C(name)), name);
            }

            // And it has no display pattern of its own: there is nothing for a locale to change.
            Assert.IsNull(GridDateTimeFormats.DisplayPattern(Week, C("es-ES")));
        }

        [TestMethod]
        public void PatternsSurviveTheAwkwardLocales()
        {
            // Day last, with a full stop after every field.
            Assert.AreEqual("yyyy. MM. dd.", GridDateTimeFormats.DatePattern(C("hu-HU")));
            Assert.AreEqual("yyyy. MM.", GridDateTimeFormats.MonthPattern(C("hu-HU")));

            // Day first, separated by a full stop AND a space: dropping the day has to take both
            // or the month pattern starts with an orphaned space.
            Assert.AreEqual("dd. MM. yyyy", GridDateTimeFormats.DatePattern(C("sl-SI")));
            Assert.AreEqual("MM. yyyy", GridDateTimeFormats.MonthPattern(C("sl-SI")));
            Assert.AreEqual("dd. MM. yyyy.", GridDateTimeFormats.DatePattern(C("hr-HR")));
            Assert.AreEqual("MM. yyyy.", GridDateTimeFormats.MonthPattern(C("hr-HR")));

            // A quoted word inside the pattern is text, not fields: it is copied through and it
            // is not where the day is looked for.
            // The space before the literal is a narrow no-break one (U+202F), which is exactly
            // the kind of character a hand-written pattern would have lost.
            Assert.AreEqual("dd.MM.yyyy\u202f'г'.", GridDateTimeFormats.DatePattern(C("bg-BG")));
            Assert.AreEqual("MM.yyyy\u202f'г'.", GridDateTimeFormats.MonthPattern(C("bg-BG")));

            // Already ISO-ordered: the padding leaves it alone.
            Assert.AreEqual("yyyy-MM-dd", GridDateTimeFormats.DatePattern(C("sv-SE")));
            Assert.AreEqual("yyyy-MM", GridDateTimeFormats.MonthPattern(C("sv-SE")));
        }

        [TestMethod]
        public void ANonGregorianCalendarStillRoundTrips()
        {
            // Persian dates are not Gregorian ones with a different separator - 2026 is 1405
            // there. The reader sees their own calendar and the server still gets the ISO day,
            // because display and transport never share a culture.
            var persian = C("fa-IR");

            var shown = GridDateTimeFormats.ToDisplay(Sample, Date, null, persian);
            Assert.AreNotEqual("2026-09-01", shown);
            StringAssert.StartsWith(shown, "14");

            Assert.AreEqual("2026-09-01", GridDateTimeFormats.ToTransport(Sample, Date));
            Assert.AreEqual("2026-09-01",
                GridDateTimeFormats.DisplayToTransport(shown, Date, null, persian));
        }

        [TestMethod]
        public void AnIsoValueIsNeverReadWithTheReadersCalendar()
        {
            // The bug this pins was found on screen, not here: a CRUD field under fa-IR showed
            // "1996/08/01 AP" for the ISO value 1996-08-01, because the transport pattern was
            // tried with the reader's culture first and "1996" parsed as a Persian year. The
            // value behind it was 2617-10-23, and saving the form would have written that.
            var persian = C("fa-IR");

            DateTime parsed;
            Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("1996-08-01", Date, null, persian, out parsed));
            Assert.AreEqual(new DateTime(1996, 8, 1), parsed);

            // What the reader is shown is their own calendar, and it comes back unchanged.
            var shown = GridDateTimeFormats.TransportToDisplay("1996-08-01", Date, null, persian);
            Assert.AreEqual("1375/05/11", shown);
            Assert.AreEqual("1996-08-01",
                GridDateTimeFormats.DisplayToTransport(shown, Date, null, persian));

            // Their own calendar is still read their way, which is what makes the order matter
            // rather than one rule winning outright.
            Assert.AreEqual("1996-08-01",
                GridDateTimeFormats.DisplayToTransport("1375/05/11", Date, null, persian));
        }

        [TestMethod]
        public void TheReaderIsUnderstoodWithoutTheLeadingZeros()
        {
            // The pattern asks for two digits; somebody typing one means the same day, and
            // refusing them buys nothing.
            Assert.AreEqual("2026-09-01",
                GridDateTimeFormats.DisplayToTransport("1/9/2026", Date, null, C("es-ES")));
            Assert.AreEqual("2026-09-01",
                GridDateTimeFormats.DisplayToTransport("9/1/2026", Date, null, C("en-US")));
        }

        [TestMethod]
        public void TheClockIsTwelveHourOnlyWhereTheLocaleSaysSo()
        {
            // Read off the locale's pattern, not guessed from the language: English is not
            // uniformly twelve-hour and Arabic is not uniformly twenty-four.
            Assert.IsTrue(GridDateTimeFormats.Uses12HourClock(C("en-US")));
            Assert.IsTrue(GridDateTimeFormats.Uses12HourClock(C("ar-SA")));

            Assert.IsFalse(GridDateTimeFormats.Uses12HourClock(C("en-GB")));
            Assert.IsFalse(GridDateTimeFormats.Uses12HourClock(C("es-ES")));
            Assert.IsFalse(GridDateTimeFormats.Uses12HourClock(C("de-DE")));
            Assert.IsFalse(GridDateTimeFormats.Uses12HourClock(C("fa-IR")));
        }

        [TestMethod]
        public void ParsingLandsOnTheColumnsOwnClrType()
        {
            // The overload every CRUD field goes through when the reader types instead of
            // picking. Getting the type wrong here does not throw - it writes nothing, and the
            // edit is quietly lost.
            var spanish = C("es-ES");
            object value;

            Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("01/09/2026", Date, null,
                typeof(DateTime), spanish, out value));
            Assert.AreEqual(new DateTime(2026, 9, 1), value);

            Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("01/09/2026", Date, null,
                typeof(DateTime?), spanish, out value));
            Assert.AreEqual(new DateTime(2026, 9, 1), value);

            Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("01/09/2026", Date, null,
                typeof(DateTimeOffset), spanish, out value));
            Assert.AreEqual(new DateTimeOffset(new DateTime(2026, 9, 1)), value);

            Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("01/09/2026", Date, null,
                typeof(DateOnly), spanish, out value));
            Assert.AreEqual(new DateOnly(2026, 9, 1), value);

            Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("14:30", Time, null,
                typeof(TimeOnly), spanish, out value));
            Assert.AreEqual(new TimeOnly(14, 30), value);

            Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("14:30", Time, null,
                typeof(TimeSpan), spanish, out value));
            Assert.AreEqual(new TimeSpan(14, 30, 0), value);
        }

        [TestMethod]
        public void ParsingRefusesWhatItCannotPlace()
        {
            var spanish = C("es-ES");
            object value;

            Assert.IsFalse(GridDateTimeFormats.TryParseDisplay("not a date", Date, null,
                typeof(DateTime), spanish, out value));
            Assert.IsNull(value);

            // A target that is not a date at all: the caller gets false rather than a surprise.
            Assert.IsFalse(GridDateTimeFormats.TryParseDisplay("01/09/2026", Date, null,
                typeof(int), spanish, out value));
            Assert.IsFalse(GridDateTimeFormats.TryParseDisplay("01/09/2026", Date, null,
                null, spanish, out value));
        }

        [TestMethod]
        public void AColumnFormatIsHonouredOnTheWayInAsWellAsOut()
        {
            // Whatever a column writes, it has to be able to read back - otherwise the reader
            // retypes exactly what the grid showed them and the filter rejects it.
            var pattern = "{0:yyyy|MM|dd}";
            var spanish = C("es-ES");

            var shown = GridDateTimeFormats.ToDisplay(Sample, Date, pattern, spanish);
            Assert.AreEqual("2026|09|01", shown);
            Assert.AreEqual("2026-09-01",
                GridDateTimeFormats.DisplayToTransport(shown, Date, pattern, spanish));
            Assert.AreEqual("yyyy|mm|dd", GridDateTimeFormats.Placeholder(Date, pattern, spanish));
        }

        [TestMethod]
        public void PatternOfReadsOnlyAFormatThatWritesTheValueAlone()
        {
            Assert.AreEqual("dd/MM/yyyy", GridDateTimeFormats.PatternOf("{0:dd/MM/yyyy}"));
            Assert.AreEqual("HH:mm", GridDateTimeFormats.PatternOf("{0:HH:mm}"));

            // A format that writes more than the value cannot be run backwards, so it is not a
            // pattern and saying so is the whole point.
            Assert.IsNull(GridDateTimeFormats.PatternOf("Created on {0:dd/MM/yyyy}"));
            Assert.IsNull(GridDateTimeFormats.PatternOf("{0:dd/MM/yyyy} ({1:d})"));
            Assert.IsNull(GridDateTimeFormats.PatternOf("dd/MM/yyyy"));
            Assert.IsNull(GridDateTimeFormats.PatternOf(""));
            Assert.IsNull(GridDateTimeFormats.PatternOf(null));
        }

        [TestMethod]
        public void ADurationGetsItsOwnFilterAndNotTheTextFallback()
        {
            // Before this the resolver fell through to TextFilterType - "not safe" by its own
            // comment - and a TimeSpan column was compared as a string. The clock in the filter
            // looked right and asked the wrong question.
            var resolver = new FilterTypeResolver();
            var filter = resolver.GetFilterType(typeof(TimeSpan));

            Assert.IsInstanceOfType(filter, typeof(TimeSpanFilterType));
            Assert.AreEqual(typeof(TimeSpan), filter.TargetType);

            // It reads what the wire carries, which is invariant HH:mm.
            Assert.AreEqual(new TimeSpan(14, 30, 0), filter.GetTypedValue("14:30"));
            Assert.IsNull(filter.GetTypedValue("not a time"));

            // Ordering makes sense for a duration; the text operators do not.
            Assert.AreEqual(GridFilterType.GreaterThan, filter.GetValidType(GridFilterType.GreaterThan));
            Assert.AreEqual(GridFilterType.Equals, filter.GetValidType(GridFilterType.Contains));
        }

        [TestMethod]
        public void TransportPatternsAreKnownForDatesAndNothingElse()
        {
            Assert.AreEqual("yyyy-MM-dd", GridDateTimeFormats.TransportPattern(Date));
            Assert.AreEqual("HH:mm", GridDateTimeFormats.TransportPattern(Time));
            Assert.AreEqual("yyyy-MM-ddTHH:mm", GridDateTimeFormats.TransportPattern(DateTimeLocal));
            Assert.AreEqual("yyyy-MM", GridDateTimeFormats.TransportPattern(Month));

            // Week is written by hand on both sides, so it has no pattern to name.
            Assert.IsNull(GridDateTimeFormats.TransportPattern(Week));
            Assert.IsNull(GridDateTimeFormats.TransportPattern("text"));

            foreach (var type in new[] { Date, Time, DateTimeLocal, Month, Week })
                Assert.IsTrue(GridDateTimeFormats.IsDateType(type), type);
            Assert.IsFalse(GridDateTimeFormats.IsDateType("text"));
            Assert.IsFalse(GridDateTimeFormats.IsDateType(null));
        }

        /// <summary>
        ///     Why every date input in a CRUD form has to hand <c>ChangeValue</c> its type
        ///     attribute, and why omitting it is not the harmless shorthand it looks like.
        ///     <para>
        ///     With the type, the text goes through <see cref="GridDateTimeFormats.TryParseDisplay"/>,
        ///     which reads the transport pattern with the invariant culture. Without it, the value
        ///     falls to <c>TypeDescriptor.GetConverter(...).ConvertFrom(text)</c>, which reads with
        ///     <c>CurrentCulture</c> - and under a calendar that is not Gregorian that turns a native
        ///     input's own ISO output into a date centuries away, or into an exception the caller
        ///     swallows as null. Three call sites in each of the create, update and inline-edit
        ///     components used to omit it.
        ///     </para>
        /// </summary>
        [TestMethod]
        public void AnIsoValueFromANativeInputSurvivesANonGregorianReader()
        {
            var esperado = new DateTime(2026, 9, 1);
            foreach (var nombre in new[] { "fa-IR", "th-TH", "ar-SA", "en-US" })
            {
                var culture = CultureInfo.GetCultureInfo(nombre);
                object value;

                Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("2026-09-01", Date, null,
                    typeof(DateTime), culture, out value), nombre);
                Assert.AreEqual(esperado, (DateTime)value, nombre);

#if !NETSTANDARD2_1 && !NET5_0
                Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("2026-09-01", Date, null,
                    typeof(DateOnly), culture, out value), nombre);
                Assert.AreEqual(DateOnly.FromDateTime(esperado), (DateOnly)value, nombre);

                Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("14:30", Time, null,
                    typeof(TimeOnly), culture, out value), nombre);
                Assert.AreEqual(new TimeOnly(14, 30), (TimeOnly)value, nombre);
#endif

                Assert.IsTrue(GridDateTimeFormats.TryParseDisplay("14:30", Time, null,
                    typeof(TimeSpan), culture, out value), nombre);
                Assert.AreEqual(new TimeSpan(14, 30, 0), (TimeSpan)value, nombre);
            }
        }

        /// <summary>
        ///     The other half of the same fact, pinned so the reason above cannot be mistaken for
        ///     belt-and-braces: the fallback really does misread ISO, and differently in each
        ///     calendar. If a future framework fixes its converters this test fails, and the
        ///     comment on the call sites can be softened - but not before.
        /// </summary>
        [TestMethod]
        public void TheTypeConverterFallbackIsTheOneThatCannotBeTrusted()
        {
            var anterior = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa-IR");
                var persian = System.ComponentModel.TypeDescriptor.GetConverter(typeof(DateTime))
                    .ConvertFrom("2026-09-01");
                Assert.AreNotEqual(new DateTime(2026, 9, 1), (DateTime)persian,
                    "the Persian calendar reading 2026 as its own year is the whole reason for the type attribute");

                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("th-TH");
                var buddhist = System.ComponentModel.TypeDescriptor.GetConverter(typeof(DateTime))
                    .ConvertFrom("2026-09-01");
                Assert.AreNotEqual(new DateTime(2026, 9, 1), (DateTime)buddhist);

                // A time carries no calendar, so the fallback is safe there - which is why the
                // omission went unnoticed for as long as only times were checked by hand.
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa-IR");
                var span = System.ComponentModel.TypeDescriptor.GetConverter(typeof(TimeSpan))
                    .ConvertFrom("14:30");
                Assert.AreEqual(new TimeSpan(14, 30, 0), (TimeSpan)span);
            }
            finally
            {
                CultureInfo.CurrentCulture = anterior;
            }
        }

        /// <summary>
        ///     Which control a date or time column renders, now decided in one place instead of
        ///     four copies of an if-chain in each of the create, update and inline-edit
        ///     components.
        /// </summary>
        [TestMethod]
        public void EveryDateAndTimeTypeAnswersWithItsNaturalControl()
        {
            Assert.AreEqual(Date, GridDateTimeFormats.NaturalTypeOf(typeof(DateTime)));
            Assert.AreEqual(Date, GridDateTimeFormats.NaturalTypeOf(typeof(DateTimeOffset)));
            Assert.AreEqual(Time, GridDateTimeFormats.NaturalTypeOf(typeof(TimeSpan)));
#if !NETSTANDARD2_1 && !NET5_0
            Assert.AreEqual(Date, GridDateTimeFormats.NaturalTypeOf(typeof(DateOnly)));
            Assert.AreEqual(Time, GridDateTimeFormats.NaturalTypeOf(typeof(TimeOnly)));
#endif

            // Nullables answer as the type they wrap - a nullable date column is still a date.
            Assert.AreEqual(Date, GridDateTimeFormats.NaturalTypeOf(typeof(DateTime?)));
            Assert.AreEqual(Time, GridDateTimeFormats.NaturalTypeOf(typeof(TimeSpan?)));

            // Null is how a caller asks "is this mine at all?", so everything else must answer it.
            Assert.IsNull(GridDateTimeFormats.NaturalTypeOf(typeof(string)));
            Assert.IsNull(GridDateTimeFormats.NaturalTypeOf(typeof(int)));
            Assert.IsNull(GridDateTimeFormats.NaturalTypeOf(typeof(bool)));
            Assert.IsNull(GridDateTimeFormats.NaturalTypeOf(null));
        }

        /// <summary>
        ///     What the column asks for wins, and what it cannot mean does not.
        ///     <para>
        ///     The fallback is not tidiness: <c>ToTypeAttr</c> answers with an empty string for
        ///     <c>None</c> and <c>TextArea</c>, and an empty type attribute is not a date type, so
        ///     a column set to one used to render <c>type=""</c> and then read its value through
        ///     the culture-dependent converter. <c>DateOnly</c> and <c>TimeOnly</c> are here
        ///     because they used to ignore the column outright.
        ///     </para>
        /// </summary>
        [TestMethod]
        public void AColumnGetsTheInputTypeItAskedForOrItsOwn()
        {
            Assert.AreEqual(Month, GridDateTimeFormats.InputTypeFor(InputType.Month, typeof(DateTime)));
            Assert.AreEqual(Week, GridDateTimeFormats.InputTypeFor(InputType.Week, typeof(DateTime)));
            Assert.AreEqual(DateTimeLocal, GridDateTimeFormats.InputTypeFor(InputType.DateTimeLocal, typeof(DateTime)));
            Assert.AreEqual(Time, GridDateTimeFormats.InputTypeFor(InputType.Time, typeof(DateTime)));

#if !NETSTANDARD2_1 && !NET5_0
            // The change of behaviour: these two used to hardcode their own and ignore the column.
            Assert.AreEqual(Month, GridDateTimeFormats.InputTypeFor(InputType.Month, typeof(DateOnly)));
            Assert.AreEqual(DateTimeLocal, GridDateTimeFormats.InputTypeFor(InputType.DateTimeLocal, typeof(TimeOnly)));
#endif

            // Saying nothing gets the type's own.
            Assert.AreEqual(Date, GridDateTimeFormats.InputTypeFor(InputType.None, typeof(DateTime)));
            Assert.AreEqual(Time, GridDateTimeFormats.InputTypeFor(InputType.None, typeof(TimeSpan)));

            // Asking for something a date cannot be gets the type's own too, never an empty
            // attribute and never "number".
            foreach (var absurdo in new[] { InputType.Text, InputType.TextArea, InputType.File, InputType.Number })
            {
                Assert.AreEqual(Date, GridDateTimeFormats.InputTypeFor(absurdo, typeof(DateTime)), absurdo.ToString());
                Assert.AreEqual(Time, GridDateTimeFormats.InputTypeFor(absurdo, typeof(TimeSpan)), absurdo.ToString());
            }

            // A column that is not a date gets nothing whatever it asks for.
            Assert.IsNull(GridDateTimeFormats.InputTypeFor(InputType.Date, typeof(string)));
            Assert.IsNull(GridDateTimeFormats.InputTypeFor(InputType.None, typeof(int)));
        }
    }
}
