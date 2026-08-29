using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    /// <summary>
    ///     A calendar the grid draws itself, so the reader gets both a picker and the culture the
    ///     application chose.
    ///     <para>
    ///     The native <c>&lt;input type="date"&gt;</c> cannot give both: it is painted by the
    ///     browser in the browser's own language, and nothing in the page changes that - not the
    ///     <c>lang</c> attribute, not <c>CultureInfo</c>. Verified, not assumed. So an application
    ///     that picks its own culture had to choose between the picker and agreeing with its own
    ///     grid. This is what removes the choice.
    ///     </para>
    ///     <para>
    ///     <b>Value is always ISO</b>, in and out, whatever is drawn. See
    ///     <see cref="GridDateTimeFormats"/>: the text box and the calendar are display, the
    ///     string this component reads and raises is transport.
    ///     </para>
    /// </summary>
    public partial class GridDatePickerComponent<T> : ComponentBase, IDisposable
    {
        private readonly string _instanceId = Guid.NewGuid().ToString("N");
        private DotNetObjectReference<GridDatePickerComponent<T>> _self;
        private bool _watchingResize;

        private bool _open;
        private CalendarMonth _month;

        // Reposicionamiento: medido despues de pintar, no antes, porque hasta entonces no se
        // sabe cuanto ocupa el popup.
        private int _passes;
        private int _offsetX;
        private bool _rtl;
        private bool _keyboardUsed;
        private bool _above;

        // Foco movil dentro del popup. El calendario lo lleva sobre un dia; el reloj, sobre una
        // celda de columna y fila.
        private DateTime _focusedDate;
        private int _focusedColumn;
        private int _focusedRow;

        private ElementReference _field;
        private ElementReference _popup;

        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        /// <summary>The ISO value being edited, or empty. Never localized.</summary>
        [Parameter]
        public string Value { get; set; }

        /// <summary>Raised with the new ISO value, or with whatever the reader typed if it is not a date.</summary>
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        /// <summary>
        ///     The input type this stands in for. Every date type is accepted; the ones this
        ///     component cannot yet draw a picker for fall back to the text box on their own, so
        ///     no caller has to know which is which.
        /// </summary>
        [Parameter]
        public string Type { get; set; } = GridDateTimeFormats.DateType;

        /// <summary>
        ///     Which way the arrows point - and they are deliberately not chosen by direction.
        ///     <para>
        ///     U+2039 and U+203A carry <c>Bidi_Mirrored = Yes</c>, so a right-to-left run already
        ///     renders each as the other; the header is also a flex row, so the same run has
        ///     already put "previous" on the right. Both halves are handled before this property
        ///     is read. Swapping them here as well mirrored an already-mirrored glyph and left
        ///     "previous" pointing forwards - the very fault the swap was added to fix.
        ///     </para>
        ///     <para>
        ///     Which is why a replacement must not be an arbitrary arrow: U+25C0 and U+2190 are
        ///     not mirrored, and either would need the swap this one must not have.
        ///     </para>
        /// </summary>
        protected MarkupString PreviousGlyph
        {
            get { return new MarkupString("&#8249;"); }
        }

        /// <summary>See <see cref="PreviousGlyph"/>.</summary>
        protected MarkupString NextGlyph
        {
            get { return new MarkupString("&#8250;"); }
        }

        /// <summary>A calendar. An instant needs one as much as a date does.</summary>
        protected bool HasCalendar
        {
            get
            {
                return Type == GridDateTimeFormats.DateType
                    || Type == GridDateTimeFormats.DateTimeLocalType;
            }
        }

        /// <summary>A clock. Likewise, an instant needs one as much as a time does.</summary>
        protected bool HasClock
        {
            get
            {
                return Type == GridDateTimeFormats.TimeType
                    || Type == GridDateTimeFormats.DateTimeLocalType;
            }
        }

        /// <summary>A grid of months, for the type that asks for a month and not a day.</summary>
        protected bool HasMonths
        {
            get { return Type == GridDateTimeFormats.MonthType; }
        }

        /// <summary>
        ///     Both halves at once. An instant is picked in two moves, so neither of them may be
        ///     the one that closes the popup - the reader would lose the other half.
        /// </summary>
        protected bool IsInstant
        {
            get { return HasCalendar && HasClock; }
        }

        /// <summary>
        ///     Whether this type gets a picker at all. A week never will: an ISO week number
        ///     reads the same in every locale, which is the whole problem this component exists
        ///     to solve, so there is nothing there to solve.
        /// </summary>
        protected bool HasPicker
        {
            get { return HasCalendar || HasClock || HasMonths; }
        }

        /// <summary>Twelve-hour readers get an hour column of 1..12 and a meridiem column.</summary>
        protected bool Uses12Hour
        {
            get { return GridDateTimeFormats.Uses12HourClock(Culture); }
        }

        /// <summary>The hours offered, labelled the way this reader reads an hour.</summary>
        protected IEnumerable<int> Hours
        {
            get
            {
                if (Uses12Hour)
                    for (var h = 1; h <= 12; h++) yield return h;
                else
                    for (var h = 0; h < 24; h++) yield return h;
            }
        }

        /// <summary>
        ///     Every minute of the hour. Offering them in fives left the other fifty-five
        ///     reachable only by typing, and - worse - a value already holding one of them
        ///     highlighted nothing, so the column disagreed with the field it was editing.
        /// </summary>
        protected IEnumerable<int> Minutes
        {
            get
            {
                for (var m = 0; m < 60; m++) yield return m;
            }
        }

        private TimeSpan TimeOfDay
        {
            get
            {
                var selected = SelectedInstant;
                return selected.HasValue ? selected.Value.TimeOfDay : TimeSpan.Zero;
            }
        }

        protected int SelectedHourLabel
        {
            get
            {
                var hour = TimeOfDay.Hours;
                if (!Uses12Hour)
                    return hour;
                var twelve = hour % 12;
                return twelve == 0 ? 12 : twelve;
            }
        }

        protected int SelectedMinute
        {
            get { return TimeOfDay.Minutes; }
        }

        protected bool IsAfternoon
        {
            get { return TimeOfDay.Hours >= 12; }
        }

        protected string MeridiemLabel(bool afternoon)
        {
            var designator = afternoon
                ? Culture.DateTimeFormat.PMDesignator
                : Culture.DateTimeFormat.AMDesignator;
            return string.IsNullOrEmpty(designator) ? (afternoon ? "PM" : "AM") : designator;
        }

        protected string ClockCssClass(bool selected, int column, int row)
        {
            var css = "grid-timepicker-item";
            if (selected)
                css += " grid-timepicker-selected";
            if (IsFocusedClockItem(column, row))
                css += " grid-datepicker-focused";
            return css;
        }

        /// <summary>An hour from the column, put back on a twenty-four hour clock.</summary>
        protected Task SelectHour(int label)
        {
            var hour = label;
            if (Uses12Hour)
            {
                hour = label % 12;
                if (IsAfternoon) hour += 12;
            }
            return SetTime(hour, TimeOfDay.Minutes);
        }

        protected async Task SelectMinute(int minute)
        {
            if (!IsInstant)
                await Close();
            await SetTime(TimeOfDay.Hours, minute);
        }

        protected Task SelectMeridiem(bool afternoon)
        {
            var hour = TimeOfDay.Hours % 12;
            if (afternoon) hour += 12;
            return SetTime(hour, TimeOfDay.Minutes);
        }

        private Task SetTime(int hour, int minute)
        {
            // The day under the clock is kept for an instant, and irrelevant for a bare time.
            var day = IsInstant && SelectedInstant.HasValue ? SelectedInstant.Value.Date : DateTime.MinValue;
            var value = day.AddHours(hour).AddMinutes(minute);
            return Raise(GridDateTimeFormats.ToTransport(value, Type));
        }

        /// <summary>The column's composite format, when it defines one.</summary>
        [Parameter]
        public string ValuePattern { get; set; }

        /// <summary>Classes for the text box, so the host decides how it sits in its form.</summary>
        [Parameter]
        public string InputCssClass { get; set; }

        /// <summary>The field's id, so a label can still point at it.</summary>
        [Parameter]
        public string Id { get; set; }

        /// <summary>The field's name, as the form had it.</summary>
        [Parameter]
        public string Name { get; set; }

        /// <summary>What the column asked the browser to autocomplete with.</summary>
        [Parameter]
        public string AutoComplete { get; set; } = "off";

        /// <summary>Tooltip for the toggle, localized by the host.</summary>
        [Parameter]
        public string Title { get; set; }

        private CultureInfo Culture
        {
            get { return CultureInfo.CurrentCulture; }
        }

        /// <summary>What the reader reads: the column's format, or their locale's.</summary>
        protected string Display
        {
            get { return GridDateTimeFormats.TransportToDisplay(Value, Type, ValuePattern, Culture); }
        }

        protected string Placeholder
        {
            get { return GridDateTimeFormats.Placeholder(Type, ValuePattern, Culture); }
        }

        protected override void OnParametersSet()
        {
            // The month to open on follows the value while the popup is closed. Once it is open
            // the reader is paging, and pulling it back to the value under them would undo it.
            if (!_open)
                _month = CalendarMonth.For(SelectedInstant ?? DateTime.Today, Culture);
        }

        private DateTime? SelectedInstant
        {
            get
            {
                DateTime parsed;
                if (GridDateTimeFormats.TryParseDisplay(Value, Type, ValuePattern, Culture, out parsed))
                    return parsed;
                return null;
            }
        }

        protected string DayCssClass(CalendarDay day)
        {
            var css = day.InMonth ? "grid-datepicker-day" : "grid-datepicker-day grid-datepicker-outside";
            var selected = SelectedInstant;
            if (selected.HasValue && selected.Value.Date == day.Date.Date)
                css += " grid-datepicker-selected";
            else if (day.Date.Date == DateTime.Today)
                css += " grid-datepicker-today";
            if (IsFocusedDay(day))
                css += " grid-datepicker-focused";
            return css;
        }

        protected async Task Toggle()
        {
            _open = !_open;
            Remeasure();
            if (_open)
            {
                await WatchResize();
                _month = CalendarMonth.For(SelectedInstant ?? DateTime.Today, Culture);
                _focusedDate = (SelectedInstant ?? DateTime.Today).Date;
                _focusedColumn = 0;
                _focusedRow = 0;
                _keyboardUsed = false;
            }
            else
            {
                await StopWatchingResize();
            }
        }

        /// <summary>
        ///     A popup measures itself against the viewport it opened in. When that viewport
        ///     changes - a rotated phone, a dragged window - the measurement is stale and the
        ///     popup can end up half off the screen with no way back. So it starts again.
        /// </summary>
        [JSInvokable]
        public void OnWindowResized()
        {
            if (!_open)
                return;
            Remeasure();
            StateHasChanged();
        }

        private void Remeasure()
        {
            _passes = 0;
            _offsetX = 0;
            _above = false;
            // _rtl is not reset: the direction does not change with the popup, and keeping it
            // avoids a frame drawn on the wrong edge every time it reopens.
        }

        /// <summary>
        ///     Repositioning on resize is polish, so it must never be the reason a picker fails to
        ///     open. An application carrying an older copy of gridblazor.js has no
        ///     addResizeHandler, and without this that missing function threw straight out of the
        ///     click handler and took the whole component down.
        /// </summary>
        private async Task WatchResize()
        {
            try
            {
                _self = _self ?? DotNetObjectReference.Create(this);
                await JsRuntime.InvokeVoidAsync("gridJsFunctions.addResizeHandler", _instanceId, _self);
                _watchingResize = true;
            }
            catch (JSException)
            {
                _watchingResize = false;
            }
        }

        private async Task StopWatchingResize()
        {
            if (!_watchingResize)
                return;
            _watchingResize = false;
            try
            {
                await JsRuntime.InvokeVoidAsync("gridJsFunctions.removeResizeHandler", _instanceId);
            }
#if !NETSTANDARD2_1 && !NET5_0
            catch (JSDisconnectedException)
#else
            catch (JSException)
#endif
            {
                // The circuit went away before the popup did; there is nothing left to detach.
            }
        }

        public void Dispose()
        {
            // Fire and forget: a component being torn down cannot await, and a listener left
            // behind would hold a reference to a component that no longer renders.
            _ = StopWatchingResize();
            if (_self != null)
            {
                _self.Dispose();
                _self = null;
            }
        }

        /// <summary>
        ///     Going back to the text box puts the calendar away. Without it the popup sits over
        ///     whatever is below the field - in a filter, over its own Apply button.
        /// </summary>
        /// <summary>
        ///     Where the popup ends up once it has been measured: nudged left when it would run
        ///     off the right edge, and flipped above the field when it would run off the bottom.
        ///     A filter near the last column or the last row is the common case, not the corner
        ///     one.
        /// </summary>
        protected string PopupStyle
        {
            get
            {
                var style = string.Empty;
                if (_offsetX != 0)
                {
                    // The popup is anchored to the left edge of the field when the reader writes
                    // left to right and to the right edge when they do not, so the correction has
                    // to be applied to whichever edge is holding it.
                    var edge = _rtl ? "right:" : "left:";
                    style += edge + _offsetX.ToString(CultureInfo.InvariantCulture) + "px;";
                }
                if (_above)
                    style += "top:auto;bottom:100%;margin-top:0;margin-bottom:2px;";
                return style;
            }
        }

        /// <summary>
        ///     Measures where the popup actually landed and nudges it back into view.
        ///     <para>
        ///     It measures again after each nudge rather than trusting the first sum, because the
        ///     first measurement is taken before the offset exists and the popup does not always
        ///     move by exactly what was asked - a scrolled ancestor or a shifted parent popup
        ///     eats part of it. One pass left it still hanging off the edge; accumulating and
        ///     re-measuring converges. Three passes is the ceiling, so a layout that cannot be
        ///     satisfied stops rather than re-rendering for ever.
        ///     </para>
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_open || !HasPicker || _passes >= 3)
                return;

            ScreenPosition position = null;
            try
            {
                position = await JsRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", _popup);
            }
            catch (JSException)
            {
                // Same reasoning as WatchResize: a picker that cannot measure itself is still a
                // usable picker, just one sitting where the CSS put it.
                _passes = 3;
                return;
            }

            var moved = false;
            if (position != null && position.InnerWidth > 0)
            {
                var rtl = position.Direction == "rtl";
                if (rtl != _rtl)
                {
                    _rtl = rtl;
                    moved = true;
                }

                // Eight pixels of air, so it does not sit flush against the edge. Which edge it
                // can run off depends on the direction: a right-to-left popup grows leftwards
                // and falls off the left of the viewport, not the right.
                var overflow = _rtl
                    ? 8 - position.X
                    : position.X + position.Width - position.InnerWidth + 8;
                if (overflow > 0)
                {
                    _offsetX -= overflow;
                    moved = true;
                }

                // Flipping is only an improvement if it fits up there; otherwise it just moves
                // the problem to the other edge.
                var overflowBottom = position.Y + position.Height - position.InnerHeight;
                if (overflowBottom > 0 && !_above && position.Y - position.Height > 0)
                {
                    _above = true;
                    moved = true;
                }
            }

            if (_passes == 0)
            {
                await JsRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", _popup);
                if (HasClock)
                {
                    try
                    {
                        await JsRuntime.InvokeVoidAsync("gridJsFunctions.scrollSelectedIntoView", _popup);
                    }
                    catch (JSException)
                    {
                        // Older copy of gridblazor.js: the clock still works, it just opens at
                        // the top. Never a reason to fail to open.
                    }
                }
            }

            _passes++;
            if (moved)
                StateHasChanged();
        }

        protected async Task Close()
        {
            if (!_open)
                return;
            _open = false;
            await StopWatchingResize();
        }

        protected void Page(int months)
        {
            _month = _month.Add(months);
        }

        protected async Task Select(DateTime date)
        {
            // Picking a day keeps the hour that was already there; only a plain date closes on
            // the first move, because for it there is nothing else to choose.
            var value = IsInstant ? date.Date.Add(TimeOfDay) : date;
            if (!IsInstant)
                await Close();
            await Raise(GridDateTimeFormats.ToTransport(value, Type));
        }

        /// <summary>The month grid: the year being shown, and picking one of its months.</summary>
        protected async Task SelectMonth(int month)
        {
            await Close();
            await Raise(GridDateTimeFormats.ToTransport(_month.FirstDayOf(month), Type));
        }

        protected void PageYear(int years)
        {
            _month = _month.AddYears(years);
        }

        protected string MonthCssClass(int month)
        {
            var css = "grid-monthpicker-item";
            var selected = SelectedInstant;
            if (selected.HasValue && Culture.Calendar.GetYear(selected.Value) == _month.Year
                && Culture.Calendar.GetMonth(selected.Value) == month)
                css += " grid-timepicker-selected";
            return css;
        }

        /// <summary>
        ///     What the reader typed, read back through the same pattern it was written with.
        ///     Text that is not a date is passed on untouched rather than swallowed - a
        ///     half-typed value is still theirs.
        /// </summary>
        protected async Task TextChanged(ChangeEventArgs e)
        {
            var text = e.Value == null ? string.Empty : e.Value.ToString();
            await Raise(GridDateTimeFormats.DisplayToTransport(text, Type, ValuePattern, Culture));
        }

        protected async Task FieldKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                await Close();
        }

        /// <summary>
        ///     Where the keyboard is - and nothing at all until the keyboard has been used.
        ///     <para>
        ///     A focus ring painted the moment the popup opens marks whatever happens to be
        ///     first: hour zero in a clock, which reads as a value chosen rather than a cursor
        ///     resting. A reader who arrived with the mouse never asked for a cursor, and the
        ///     selected value already has its own highlight. So it appears on the first key and
        ///     not before.
        ///     </para>
        /// </summary>
        protected bool IsFocusedDay(CalendarDay day)
        {
            return _open && _keyboardUsed && HasCalendar && day.Date.Date == _focusedDate.Date;
        }

        protected bool IsFocusedClockItem(int column, int row)
        {
            // Never on an instant: there the arrows steer the calendar, so a cursor drawn on the
            // clock would sit wherever it happened to start and claim to be where the keys are not.
            return _open && _keyboardUsed && HasClock && !IsInstant
                && column == _focusedColumn && row == _focusedRow;
        }

        /// <summary>
        ///     The popup's keyboard. Default is always prevented, because a calendar that scrolls
        ///     the page under the reader every time they press Down is worse than no calendar. Tab
        ///     is therefore handled here too: it closes and hands focus back to the text box,
        ///     which is where a reader tabbing out of a popup expects to end up.
        /// </summary>
        protected async Task PopupKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape" || e.Key == "Tab")
            {
                await Close();
                await JsRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", _field);
                return;
            }

            // Any key that steers the popup means the reader is on the keyboard now, and the
            // cursor becomes worth showing.
            if (e.Key.StartsWith("Arrow") || e.Key == "PageUp" || e.Key == "PageDown"
                || e.Key == "Enter" || e.Key == " ")
                _keyboardUsed = true;

            // One handler, not both: for an instant the arrows steer the calendar, because both
            // moving at once would leave the reader unable to aim either. The clock stays on the
            // mouse there - a real gap, stated rather than hidden.
            if (HasCalendar)
                await CalendarKey(e);
            else if (HasClock)
                await ClockKey(e);
        }

        private async Task CalendarKey(KeyboardEventArgs e)
        {
            var moved = _focusedDate;
            switch (e.Key)
            {
                case "ArrowLeft": moved = _focusedDate.AddDays(-1); break;
                case "ArrowRight": moved = _focusedDate.AddDays(1); break;
                case "ArrowUp": moved = _focusedDate.AddDays(-7); break;
                case "ArrowDown": moved = _focusedDate.AddDays(7); break;
                case "PageUp": moved = _focusedDate.AddMonths(-1); break;
                case "PageDown": moved = _focusedDate.AddMonths(1); break;
                case "Enter":
                case " ":
                    await Select(_focusedDate);
                    await JsRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", _field);
                    return;
                default:
                    return;
            }

            _focusedDate = moved;
            // The month follows the focus over its own edges, so arrowing past the 1st or the
            // last day pages rather than stopping dead.
            _month = CalendarMonth.For(_focusedDate, Culture);
        }

        private async Task ClockKey(KeyboardEventArgs e)
        {
            var columns = Uses12Hour ? 3 : 2;
            switch (e.Key)
            {
                case "ArrowLeft":
                    _focusedColumn = (_focusedColumn - 1 + columns) % columns;
                    _focusedRow = 0;
                    break;
                case "ArrowRight":
                    _focusedColumn = (_focusedColumn + 1) % columns;
                    _focusedRow = 0;
                    break;
                case "ArrowUp":
                    _focusedRow = Math.Max(0, _focusedRow - 1);
                    break;
                case "ArrowDown":
                    _focusedRow = Math.Min(ColumnLength(_focusedColumn) - 1, _focusedRow + 1);
                    break;
                case "Enter":
                case " ":
                    await ActivateClockItem();
                    break;
            }
        }

        private int ColumnLength(int column)
        {
            if (column == 0) return Uses12Hour ? 12 : 24;
            if (column == 1) return 60;
            return 2;
        }

        private async Task ActivateClockItem()
        {
            if (_focusedColumn == 0)
            {
                var hours = new List<int>(Hours);
                await SelectHour(hours[_focusedRow]);
            }
            else if (_focusedColumn == 1)
            {
                var minutes = new List<int>(Minutes);
                await SelectMinute(minutes[_focusedRow]);
            }
            else
            {
                await SelectMeridiem(_focusedRow == 1);
            }
        }

        private async Task Raise(string transport)
        {
            Value = transport;
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(transport);
        }
    }
}
