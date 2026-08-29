using GridShared.Columns;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;

namespace GridShared.Utility
{
    /// <summary>
    ///     Single definition of how a date or a time is written in the grid.
    ///     <para>
    ///     Two different jobs live here and they are deliberately kept apart:
    ///     </para>
    ///     <para>
    ///     <b>Transport</b> — the ISO 8601 strings exchanged between the client and the server:
    ///     query strings, filter values, the <c>value</c> attribute of the native HTML date inputs.
    ///     Always <c>yyyy-MM-dd</c>, never localized. A localized value there would be read as
    ///     another day the moment the reader's locale writes the month first.
    ///     </para>
    ///     <para>
    ///     <b>Display</b> — what the reader actually sees. The column's own pattern when it defines
    ///     one, otherwise the current culture's, which in Blazor WebAssembly is the browser's.
    ///     </para>
    ///     <para>
    ///     The order of day and month is exactly what changes between locales, and reading
    ///     <c>09/01</c> as the ninth of January when it is the first of September is a mistake
    ///     nothing in the text reveals. That is the whole reason presentation and transport cannot
    ///     share one format.
    ///     </para>
    /// </summary>
    public static class GridDateTimeFormats
    {
        /// <summary>HTML input type attributes this class knows how to write.</summary>
        public const string DateType = "date";
        public const string TimeType = "time";
        public const string DateTimeLocalType = "datetime-local";
        public const string WeekType = "week";
        public const string MonthType = "month";

        /// <summary>Client to server, in every locale. Never hand one of these to a reader.</summary>
        public const string DateTransportPattern = "yyyy-MM-dd";
        public const string TimeTransportPattern = "HH:mm";
        public const string DateTimeLocalTransportPattern = "yyyy-MM-ddTHH:mm";
        public const string MonthTransportPattern = "yyyy-MM";

        private static readonly ConcurrentDictionary<string, string> _datePatterns =
            new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> _monthPatterns =
            new ConcurrentDictionary<string, string>();

        /// <summary>
        ///     The ISO pattern an input of this type exchanges with the server, or null when the
        ///     type is not a date one.
        /// </summary>
        public static string TransportPattern(string type)
        {
            switch (type)
            {
                case DateType: return DateTransportPattern;
                case TimeType: return TimeTransportPattern;
                case DateTimeLocalType: return DateTimeLocalTransportPattern;
                case MonthType: return MonthTransportPattern;
                default: return null;
            }
        }

        /// <summary>Is this input type one whose value is a date, a time or an instant?</summary>
        public static bool IsDateType(string type)
        {
            return type == DateType || type == TimeType || type == DateTimeLocalType
                || type == WeekType || type == MonthType;
        }

        /// <summary>
        ///     The input type a CLR type asks for when the column says nothing: a date for the
        ///     three that carry one, a time for the two that carry only a time of day, and null
        ///     for everything else, which is how a caller asks "is this mine at all?".
        /// </summary>
        public static string NaturalTypeOf(Type clrType)
        {
            if (clrType == null)
                return null;
            var underlying = Nullable.GetUnderlyingType(clrType) ?? clrType;
            if (underlying == typeof(DateTime) || underlying == typeof(DateTimeOffset))
                return DateType;
            if (underlying == typeof(TimeSpan))
                return TimeType;
#if !NETSTANDARD2_1 && !NET5_0
            if (underlying == typeof(DateOnly))
                return DateType;
            if (underlying == typeof(TimeOnly))
                return TimeType;
#endif
            return null;
        }

        /// <summary>
        ///     Which input type a date or time column actually renders: what the column asked for
        ///     with <c>SetInputType</c>, or the CLR type's own if it asked for nothing.
        ///     <para>
        ///     Only the five date-shaped input types are honoured. The rest - <c>Text</c>,
        ///     <c>TextArea</c>, <c>File</c>, <c>Number</c> - fall back rather than being trusted,
        ///     because <see cref="InputTypeExtensions.ToTypeAttr"/> answers with an empty string
        ///     for some of them: a date column set to one used to render <c>type=""</c>, and an
        ///     empty type is not a date type, so the value then took the culture-dependent parse
        ///     this class exists to avoid. Nonsense in gives the natural control out, not a
        ///     broken one.
        ///     </para>
        ///     <para>
        ///     <c>DateOnly</c> and <c>TimeOnly</c> used to ignore <c>SetInputType</c> outright and
        ///     hardcode their own; going through here is what lets one branch serve all five CLR
        ///     types instead of four copies of the same markup.
        ///     </para>
        /// </summary>
        public static string InputTypeFor(InputType inputType, Type clrType)
        {
            var natural = NaturalTypeOf(clrType);
            if (natural == null)
                return null;
            var asked = inputType.ToTypeAttr();
            return IsDateType(asked) ? asked : natural;
        }

        /// <summary>The reader's date pattern: their locale's field order and separator, padded.</summary>
        public static string DatePattern(CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;

            // Keyed on the pattern and not on the culture's name: a culture can be cloned with its
            // pattern replaced, and two instances answering to the same name would then share one
            // cached answer — whichever arrived first, for both.
            return _datePatterns.GetOrAdd(culture.DateTimeFormat.ShortDatePattern, Pad);
        }

        /// <summary>The reader's time of day, to the minute.</summary>
        public static string TimePattern(CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            return culture.DateTimeFormat.ShortTimePattern;
        }

        /// <summary>
        ///     Does this reader tell the time on a twelve-hour clock? Read off the locale's own
        ///     pattern rather than guessed from the language: English is not uniformly twelve-hour
        ///     (en-GB is not) and Arabic is not uniformly twenty-four.
        /// </summary>
        public static bool Uses12HourClock(CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            var pattern = culture.DateTimeFormat.ShortTimePattern;
            if (string.IsNullOrEmpty(pattern))
                return false;

            var quote = '\0';
            foreach (var c in pattern)
            {
                if (quote != '\0')
                {
                    if (c == quote) quote = '\0';
                    continue;
                }
                if (c == '\'' || c == '"')
                {
                    quote = c;
                    continue;
                }
                // h is the twelve-hour designator; H is the twenty-four hour one.
                if (c == 'h')
                    return true;
            }
            return false;
        }

        /// <summary>A date followed by a time, both as the reader writes them.</summary>
        public static string DateTimePattern(CultureInfo culture = null)
        {
            return DatePattern(culture) + " " + TimePattern(culture);
        }

        /// <summary>
        ///     A month and a year in the reader's field order. Derived from the short date pattern
        ///     with the day dropped rather than from <c>YearMonthPattern</c>, which spells the month
        ///     out — readable in a sentence, unparseable back out of an input.
        /// </summary>
        public static string MonthPattern(CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            return _monthPatterns.GetOrAdd(culture.DateTimeFormat.ShortDatePattern,
                p => DropDayField(Pad(p)));
        }

        /// <summary>
        ///     What a value of this input type looks like to the reader. Week has no entry: an ISO
        ///     week number is not something a locale writes differently.
        /// </summary>
        public static string DisplayPattern(string type, CultureInfo culture = null)
        {
            switch (type)
            {
                case DateType: return DatePattern(culture);
                case TimeType: return TimePattern(culture);
                case DateTimeLocalType: return DateTimePattern(culture);
                case MonthType: return MonthPattern(culture);
                default: return null;
            }
        }

        /// <summary>
        ///     The pattern a <b>date filter</b> is written in: the column's own when it defines
        ///     one that writes a date and nothing else, the reader's locale otherwise.
        ///     <para>
        ///     A column format carrying a time is deliberately not honoured here. A date filter
        ///     asks for a day, the calendar it opens can only offer a day, and a pattern
        ///     demanding an hour would make every value the reader typed into it unreadable.
        ///     </para>
        /// </summary>
        public static string FilterDatePattern(string valuePattern, CultureInfo culture = null)
        {
            var pattern = PatternOf(valuePattern);
            if (!string.IsNullOrEmpty(pattern) && WritesDateOnly(pattern))
                return pattern;
            return DatePattern(culture);
        }

        /// <summary>
        ///     Does this pattern write a date and no time at all? Letters the pattern quotes are
        ///     text and not fields, so they are skipped — the "de" of a Spanish long date must not
        ///     be read as a day.
        /// </summary>
        private static bool WritesDateOnly(string pattern)
        {
            var hasDate = false;
            var quote = '\0';

            for (var i = 0; i < pattern.Length; i++)
            {
                var c = pattern[i];

                if (quote != '\0')
                {
                    if (c == quote) quote = '\0';
                    continue;
                }
                if (c == '\'' || c == '"')
                {
                    quote = c;
                    continue;
                }
                if (c == '\\')
                {
                    i++;
                    continue;
                }

                if (c == 'd' || c == 'M' || c == 'y')
                    hasDate = true;
                else if (c == 'H' || c == 'h' || c == 'm' || c == 's' || c == 't'
                    || c == 'f' || c == 'F' || c == 'z' || c == 'K')
                    return false;
            }

            return hasDate;
        }

        /// <summary>
        ///     The hint for a text input of this type, spelled in the reader's own pattern so the
        ///     placeholder and the value agree.
        /// </summary>
        public static string Placeholder(string type, string valuePattern = null, CultureInfo culture = null)
        {
            var pattern = PatternOf(valuePattern);
            if (!string.IsNullOrEmpty(pattern))
                return pattern.ToLowerInvariant();
            if (type == WeekType)
                return "yyyy-Www";
            var display = DisplayPattern(type, culture);
            return display == null ? null : display.ToLowerInvariant();
        }

        /// <summary>
        ///     Writes a value for a reader: the column's pattern when it defines one, the reader's
        ///     locale otherwise.
        /// </summary>
        /// <param name="value">the date, time or offset to write</param>
        /// <param name="type">the HTML input type the column is rendered as</param>
        /// <param name="valuePattern">the column's composite format, when it set one</param>
        /// <param name="culture">the reader's culture; the current one when null</param>
        public static string ToDisplay(object value, string type, string valuePattern = null,
            CultureInfo culture = null)
        {
            if (value == null)
                return null;
            culture = culture ?? CultureInfo.CurrentCulture;

            // The column wins over the locale wherever it has spoken.
            if (!string.IsNullOrEmpty(valuePattern))
                return string.Format(culture, valuePattern, value);

            if (type == WeekType)
                return string.Format(culture, "{0:yyyy}-W{1}", value, DateTimeUtils.GetIso8601WeekOfYear(value));

            var pattern = DisplayPattern(type, culture);
            if (pattern == null)
                return Convert.ToString(value, culture);

            var formattable = AsFormattableDate(value);
            return formattable == null
                ? Convert.ToString(value, culture)
                : formattable.ToString(pattern, culture);
        }

        /// <summary>
        ///     Writes a date, a time or an instant the way the reader writes them, working out
        ///     which of the three it is from the value's own type. Anything that is not one of
        ///     them answers null: this is what a column with no format of its own falls back to,
        ///     and a column of some other type has nothing to gain from a date helper.
        /// </summary>
        public static string ToDisplayValue(object value, CultureInfo culture = null)
        {
            var type = TypeOf(value);
            return type == null ? null : ToDisplay(value, type, null, culture);
        }

        /// <summary>
        ///     Which of the input types a value is, read off its CLR type.
        ///     <para>
        ///     <c>DateTime</c> answers <b>date</b>, without its time of day. Nearly every
        ///     <c>DateTime</c> column in a grid is a date — an order date, a birth date, a hire
        ///     date — and appending <c>0:00</c> to all of them is noise in every row to keep the
        ///     hour legible in the few that carry one. A column whose hour matters says so with
        ///     its own format: <c>Format("{0:g}")</c>, or any pattern spelling both halves.
        ///     </para>
        ///     <para>
        ///     This governs what is <b>read</b> — cells, group labels, totals. It does not reach
        ///     the CRUD inputs, which are told their type by the column's <c>InputType</c> and go
        ///     on editing the whole value.
        ///     </para>
        /// </summary>
        public static string TypeOf(object value)
        {
            if (value == null)
                return null;
            if (value is DateTime || value is DateTimeOffset)
                return DateType;
            if (value is TimeSpan)
                return TimeType;
#if !NETSTANDARD2_1 && !NET5_0
            if (value is DateOnly)
                return DateType;
            if (value is TimeOnly)
                return TimeType;
#endif
            return null;
        }

        /// <summary>
        ///     <c>TimeSpan</c> is the odd one out: it is a duration, so it reads none of the
        ///     time-of-day specifiers a locale's pattern is written in — <c>H</c> and <c>tt</c>
        ///     among them — and handing it one throws rather than falling back. A span inside a
        ///     day is a time of day here, so it is carried on a <c>DateTime</c> to be written;
        ///     anything longer is a duration in earnest and is left to write itself.
        /// </summary>
        private static IFormattable AsFormattableDate(object value)
        {
            if (value is TimeSpan)
            {
                var span = (TimeSpan)value;
                if (span < TimeSpan.Zero || span >= TimeSpan.FromDays(1))
                    return null;
                return DateTime.MinValue.Add(span);
            }
            return value as IFormattable;
        }

        /// <summary>Writes a value for the server, or for a native HTML date input.</summary>
        public static string ToTransport(object value, string type)
        {
            if (value == null)
                return null;

            if (type == WeekType)
                return string.Format(CultureInfo.InvariantCulture, "{0:yyyy}-W{1}", value,
                    DateTimeUtils.GetIso8601WeekOfYear(value));

            var pattern = TransportPattern(type);
            if (pattern == null)
                return Convert.ToString(value, CultureInfo.InvariantCulture);

            var formattable = AsFormattableDate(value);
            return formattable == null
                ? Convert.ToString(value, CultureInfo.InvariantCulture)
                : formattable.ToString(pattern, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Reads back what a reader typed into a text input and answers the ISO string the
        ///     server expects.
        ///     <para>
        ///     Liberal on input on purpose: the column's pattern first, then the reader's locale,
        ///     then ISO — somebody who types <c>2026-08-27</c> into a Spanish grid means that day,
        ///     and refusing it buys nothing.
        ///     </para>
        ///     <para>
        ///     Text that is not a date at all is handed back untouched, together with
        ///     <see cref="TransportToDisplay"/>, which is what keeps a half-typed filter on screen
        ///     instead of clearing the box under the reader mid-keystroke.
        ///     </para>
        /// </summary>
        public static string DisplayToTransport(string text, string type, string valuePattern = null,
            CultureInfo culture = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            DateTime value;
            if (!TryParseDisplay(text, type, valuePattern, culture, out value))
                return text;
            return ToTransport(value, type);
        }

        /// <summary>
        ///     The inverse: rewrites the ISO string held for the server the way the reader reads
        ///     it. See <see cref="DisplayToTransport"/> for what happens to text that is not a
        ///     date.
        /// </summary>
        public static string TransportToDisplay(string text, string type, string valuePattern = null,
            CultureInfo culture = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            DateTime value;
            if (!TryParseDisplay(text, type, valuePattern, culture, out value))
                return text;
            return ToDisplay(value, type, valuePattern, culture);
        }

        /// <summary>Reads a date a reader wrote. See <see cref="DisplayToTransport"/>.</summary>
        public static bool TryParseDisplay(string text, string type, string valuePattern,
            CultureInfo culture, out DateTime value)
        {
            value = default(DateTime);
            if (string.IsNullOrWhiteSpace(text))
                return false;
            culture = culture ?? CultureInfo.CurrentCulture;
            text = text.Trim();

            if (type == WeekType)
            {
                var week = DateTimeUtils.FromIso8601WeekDate(text);
                if (week.HasValue)
                {
                    value = week.Value;
                    return true;
                }
                return false;
            }

            // What the reader writes is read the reader's way: their column's format first, then
            // their locale's.
            var display = new[] { PatternOf(valuePattern), DisplayPattern(type, culture) };
            foreach (var pattern in display)
            {
                if (string.IsNullOrEmpty(pattern))
                    continue;
                if (DateTime.TryParseExact(text, pattern, culture, DateTimeStyles.None, out value))
                    return true;
                if (DateTime.TryParseExact(text, pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                    return true;
            }

            // What the wire carries is read the only way it is ever written, and never with the
            // reader's culture. That is not tidiness: under a calendar that is not Gregorian,
            // TryParseExact("1996-08-01", "yyyy-MM-dd", fa-IR) succeeds and reads 1996 as a
            // Persian year - the value comes back as 2617-10-23, and a form that saves it writes
            // six centuries into the record without failing.
            var transport = new[]
            {
                TransportPattern(type),
                DateTransportPattern,
                "yyyy-MM-ddTHH:mm",
                "yyyy-MM-ddTHH:mm:ss"
            };
            foreach (var pattern in transport)
            {
                if (string.IsNullOrEmpty(pattern))
                    continue;
                if (DateTime.TryParseExact(text, pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                    return true;
            }

            // Last resort for anything else the reader may have typed. ISO has already been ruled
            // out above, so reading this one their way cannot misread the wire's format.
            return DateTime.TryParse(text, culture, DateTimeStyles.None, out value)
                || DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
        }

        /// <summary>
        ///     Reads what a reader typed and hands back a value of the column's own CLR type, so a
        ///     caller does not have to know that a time input feeds a <c>TimeSpan</c> and a date one
        ///     a <c>DateOnly</c>.
        /// </summary>
        /// <param name="targetType">the column's property type; nullable or not, either works</param>
        public static bool TryParseDisplay(string text, string type, string valuePattern, Type targetType,
            CultureInfo culture, out object value)
        {
            value = null;
            if (targetType == null)
                return false;
            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

            DateTime parsed;
            if (!TryParseDisplay(text, type, valuePattern, culture, out parsed))
                return false;

            if (underlying == typeof(DateTime))
            {
                value = parsed;
                return true;
            }
            if (underlying == typeof(DateTimeOffset))
            {
                value = new DateTimeOffset(parsed);
                return true;
            }
            if (underlying == typeof(TimeSpan))
            {
                value = parsed.TimeOfDay;
                return true;
            }
#if !NETSTANDARD2_1 && !NET5_0
            if (underlying == typeof(DateOnly))
            {
                value = DateOnly.FromDateTime(parsed);
                return true;
            }
            if (underlying == typeof(TimeOnly))
            {
                value = TimeOnly.FromDateTime(parsed);
                return true;
            }
#endif
            return false;
        }

        /// <summary>
        ///     The bare pattern inside a column's composite format: <c>{0:dd/MM/yyyy}</c> answers
        ///     <c>dd/MM/yyyy</c>. Anything else answers null — a format that writes more than the
        ///     value itself cannot be run backwards.
        /// </summary>
        public static string PatternOf(string valuePattern)
        {
            if (string.IsNullOrEmpty(valuePattern))
                return null;
            if (!valuePattern.StartsWith("{0:", StringComparison.Ordinal))
                return null;
            if (!valuePattern.EndsWith("}", StringComparison.Ordinal))
                return null;
            var pattern = valuePattern.Substring(3, valuePattern.Length - 4);
            return pattern.IndexOf('{') >= 0 ? null : pattern;
        }

        /// <summary>
        ///     Widens the single-letter fields of a date pattern: <c>d</c> and <c>M</c> to two
        ///     digits, a two-digit year to four. What the locale decides is the field order and the
        ///     separator; the padding is a presentation choice, and dropping it would misalign every
        ///     date column in the grid for nothing. Text the locale quotes inside the pattern is
        ///     copied through untouched — a letter inside a quoted word is not a field.
        /// </summary>
        private static string Pad(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return pattern;

            var result = new StringBuilder(pattern.Length + 4);
            var quote = '\0';

            for (var i = 0; i < pattern.Length; i++)
            {
                var c = pattern[i];

                if (quote != '\0')
                {
                    result.Append(c);
                    if (c == quote) quote = '\0';
                    continue;
                }

                if (c == '\'' || c == '"')
                {
                    quote = c;
                    result.Append(c);
                    continue;
                }

                if (c != 'd' && c != 'M' && c != 'y')
                {
                    result.Append(c);
                    continue;
                }

                var run = 1;
                while (i + run < pattern.Length && pattern[i + run] == c) run++;

                // Four letters of day or month is a name, not a number — leave those alone.
                if (c == 'y') result.Append(c, run <= 2 ? 4 : run);
                else result.Append(c, run == 1 ? 2 : run);

                i += run - 1;
            }

            return result.ToString();
        }

        /// <summary>
        ///     Removes the day field and the separator it leaves behind, turning a padded short date
        ///     into a month pattern that keeps the locale's field order.
        /// </summary>
        private static string DropDayField(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return pattern;

            // The day field, not merely the first d: a locale may quote a word containing one,
            // and Bulgarian's trailing 'g' is a reminder that quoted text is in here.
            var start = -1;
            var quote = '\0';
            for (var i = 0; i < pattern.Length; i++)
            {
                var c = pattern[i];
                if (quote != '\0')
                {
                    if (c == quote) quote = '\0';
                }
                else if (c == '\'' || c == '"')
                {
                    quote = c;
                }
                else if (c == 'd')
                {
                    start = i;
                    break;
                }
            }
            if (start < 0)
                return pattern;

            var end = start;
            while (end < pattern.Length && pattern[end] == 'd') end++;

            // The separator on whichever side the day sat goes with it, so neither end is left
            // dangling - the whole run of it, because a locale may write '. ' and taking one
            // character would leave the space orphaned at the front of the pattern. A quote is
            // where the run stops: what follows is text, not a separator.
            if (end < pattern.Length && IsSeparator(pattern[end]))
            {
                while (end < pattern.Length && IsSeparator(pattern[end])) end++;
            }
            else
            {
                while (start > 0 && IsSeparator(pattern[start - 1])) start--;
            }

            return pattern.Remove(start, end - start).Trim();
        }

        private static bool IsSeparator(char c)
        {
            return !char.IsLetter(c) && c != '\'' && c != '"';
        }
    }
}
