namespace GridBlazor
{
    /// <summary>
    ///     Who spells a date the reader types into a filter or a CRUD form.
    ///     <para>
    ///     The distinction matters because it is not a style choice. An
    ///     <c>&lt;input type="date"&gt;</c> carries its value in ISO 8601 by specification, and
    ///     the browser paints it in <b>its own</b> language — not the page's, and not the culture
    ///     the grid was given. So a grid whose culture is chosen inside the application, by a
    ///     language selector rather than by the browser's settings, will show a cell and the
    ///     filter above it in two different formats, and nothing in code can change that while
    ///     the picker is the browser's.
    ///     </para>
    ///     <para>
    ///     Whichever mode is in force, what travels to the server is ISO either way.
    ///     </para>
    /// </summary>
    public enum DateInputMode
    {
        /// <summary>
        ///     The browser's own picker, with its calendar and its date keyboard on a phone.
        ///     Reads in the browser's language. This is the default, and it stays the default
        ///     because an application that follows the browser's locale has nothing to correct
        ///     and the native control brings a phone keyboard no markup can summon.
        /// </summary>
        Browser = 1,

        /// <summary>
        ///     A field the grid writes, reads and picks itself, in the culture the grid was
        ///     given — the column's own format where it defines one, that culture's otherwise.
        ///     Agrees with the cells whatever the browser is set to.
        ///     <para>
        ///     The field carries its own picker rather than the browser's, so this mode is no
        ///     longer a trade of the calendar for the format: a date gets a calendar, a time a
        ///     clock, a datetime-local both, and a month a grid of months — each drawn in the
        ///     grid's culture and its calendar, Gregorian or not. A week gets none, deliberately:
        ///     an ISO week number reads the same everywhere, so there is nothing to correct.
        ///     Filters and CRUD forms alike; read and delete forms stay read-only as they were.
        ///     </para>
        ///     <para>
        ///     What it does still cost is the phone's date keyboard, which belongs to the native
        ///     control and cannot be summoned from markup.
        ///     </para>
        /// </summary>
        Grid = 2
    }
}
