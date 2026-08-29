using GridBlazor.Columns;
using GridBlazor.DataAnnotations;
using GridShared;
using GridShared.Totals;
using Microsoft.Extensions.Primitives;
using GridShared.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading;

namespace GridBlazor.Tests.Columns
{
    /// <summary>
    ///     What a date column actually puts in a cell, through the real column, because that is
    ///     the question a reader asks of the grid and the one place where a rule that only holds
    ///     in the formatter would still look broken on screen.
    /// </summary>
    [TestClass]
    public class DateCellRenderingTests
    {
        private GridColumnCollection<TestModel> _columns;
        private CultureInfo _previous;

        [TestInitialize]
        public void Init()
        {
            _previous = CultureInfo.CurrentCulture;
            Action<IGridColumnCollection<TestModel>> columns = c => c.Add(r => r.Id);
            var repo = new TestRepository();
            var grid = new TestGrid((q) => repo.GetAllService(columns, q, false, true), true, columns,
                Thread.CurrentThread.CurrentCulture);
            _columns = new GridColumnCollection<TestModel>(grid,
                new DefaultColumnBuilder<TestModel>(grid, new GridAnnotationsProvider()), grid.Settings.SortSettings);
        }

        [TestCleanup]
        public void Cleanup()
        {
            CultureInfo.CurrentCulture = _previous;
        }

        [TestMethod]
        public void ADateColumnWithNoFormatFollowsTheReadersLocale()
        {
            var column = _columns.Add(x => x.Created);
            var item = new TestModel { Created = new DateTime(2026, 9, 1, 14, 30, 0) };

            CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            Assert.AreEqual("01/09/2026", column.GetValue(item).ToString());

            CultureInfo.CurrentCulture = new CultureInfo("sv-SE");
            Assert.AreEqual("2026-09-01", column.GetValue(item).ToString());
        }

        [TestMethod]
        public void ANullableDateColumnBehavesTheSame()
        {
            // The demos' date columns are all DateTime?, and a nullable boxes as its underlying
            // type — so the kind is read off the value the same way. Worth pinning: if it did
            // not, every date column in every demo would fall through to ToString().
            var column = _columns.Add(x => x.NullableCreated, "CustomName");

            CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            Assert.AreEqual("01/09/2026",
                column.GetValue(new TestModel { NullableCreated = new DateTime(2026, 9, 1, 14, 30, 0) }).ToString());
            Assert.AreEqual("",
                column.GetValue(new TestModel { NullableCreated = null }).ToString());
        }

        [TestMethod]
        public void AColumnThatWantsTheHourAsksForIt()
        {
            // The date-only default is not a loss of the value, only of its spelling: the hour
            // is one Format away, and the column is where that belongs.
            var column = _columns.Add(x => x.Created).Format("{0:g}");
            var item = new TestModel { Created = new DateTime(2026, 9, 1, 14, 30, 0) };

            CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            Assert.AreEqual("1/9/2026 14:30", column.GetValue(item).ToString());
        }

        [TestMethod]
        public void TheGridLeavesDatesToTheBrowserUnlessToldOtherwise()
        {
            // The default has to be the browser's picker: it is the better control, and flipping
            // it would take the calendar away from every grid already using this library.
            Action<IGridColumnCollection<TestModel>> columns = c => c.Add(r => r.Id);
            var repo = new TestRepository();
            var grid = new TestGrid((q) => repo.GetAllService(columns, q, false, true), true, columns,
                Thread.CurrentThread.CurrentCulture);

            Assert.AreEqual(DateInputMode.Browser, grid.DateInputMode);

            // ...and an application that chooses its own culture can say so.
            var client = new GridClient<TestModel>((q) => repo.GetAllService(columns, q, false, true),
                new QueryDictionary<StringValues>(), false, "grid", columns)
                .SetDateInputMode(DateInputMode.Grid);

            Assert.AreEqual(DateInputMode.Grid, client.Grid.DateInputMode);
        }

        [TestMethod]
        public void EveryDateTypeInACellFollowsTheLocale()
        {
            // Not only DateTime: the kind is read off the value, so each of these has to land on
            // the right one. A DateOnly written with a time, or a TimeOnly with a date, would be
            // as wrong as ISO was.
            var item = new TestModel
            {
                Created = new DateTime(2026, 9, 1, 14, 30, 0),
                OffsetCreated = new DateTimeOffset(new DateTime(2026, 9, 1, 14, 30, 0), TimeSpan.Zero),
                DateOnlyField = new DateOnly(2026, 9, 1),
                TimeOnlyField = new TimeOnly(14, 30),
                TimeSpanField = new TimeSpan(14, 30, 0)
            };

            var offset = _columns.Add(x => x.OffsetCreated);
            var dateOnly = _columns.Add(x => x.DateOnlyField);
            var timeOnly = _columns.Add(x => x.TimeOnlyField);
            var timeSpan = _columns.Add(x => x.TimeSpanField);

            CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            Assert.AreEqual("01/09/2026", offset.GetValue(item).ToString(), "DateTimeOffset");
            Assert.AreEqual("01/09/2026", dateOnly.GetValue(item).ToString(), "DateOnly");
            Assert.AreEqual("14:30", timeOnly.GetValue(item).ToString(), "TimeOnly");
            Assert.AreEqual("14:30", timeSpan.GetValue(item).ToString(), "TimeSpan");

            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Assert.AreEqual("09/01/2026", dateOnly.GetValue(item).ToString(), "DateOnly en-US");
            Assert.AreEqual("2:30 PM", timeOnly.GetValue(item).ToString(), "TimeOnly en-US");
            Assert.AreEqual("2:30 PM", timeSpan.GetValue(item).ToString(), "TimeSpan en-US");
        }

        [TestMethod]
        public void TransportAndDisplayPartCompanyAtTheColumn()
        {
            // The three methods a CRUD field calls, and the reason they are three: the native
            // input needs ISO because that is the only thing it reads, the text one needs the
            // reader's spelling, and the hint has to match whichever was drawn.
            var column = (ICGridColumn)_columns.Add(x => x.Created);
            var value = new DateTime(2026, 9, 1, 14, 30, 0);
            CultureInfo.CurrentCulture = new CultureInfo("es-ES");

            Assert.AreEqual("2026-09-01", column.GetFormatedDateTime(value, "date"));
            Assert.AreEqual("01/09/2026", column.GetDisplayDateTime(value, "date"));
            Assert.AreEqual("dd/mm/yyyy", column.GetDateTimePlaceholder("date"));

            Assert.AreEqual("2026-09-01T14:30", column.GetFormatedDateTime(value, "datetime-local"));
            Assert.AreEqual("01/09/2026 14:30", column.GetDisplayDateTime(value, "datetime-local"));

            Assert.IsNull(column.GetFormatedDateTime(null, "date"));
            Assert.IsNull(column.GetDisplayDateTime(null, "date"));
        }

        [TestMethod]
        public void AColumnFormatReachesTheDisplayButNeverTheWire()
        {
            var column = (ICGridColumn)_columns.Add(x => x.Created).Format("{0:yyyy|MM|dd}");
            var value = new DateTime(2026, 9, 1, 14, 30, 0);
            CultureInfo.CurrentCulture = new CultureInfo("es-ES");

            Assert.AreEqual("2026|09|01", column.GetDisplayDateTime(value, "date"));
            Assert.AreEqual("yyyy|mm|dd", column.GetDateTimePlaceholder("date"));

            // The native input would not understand the column's format, so it never sees it.
            Assert.AreEqual("2026-09-01", column.GetFormatedDateTime(value, "date"));
        }

        [TestMethod]
        public void ATotalIsWrittenLikeTheColumnItSums()
        {
            // Max and Min of a date column are read in the same breath as the column, so they
            // follow the same rule - and a column format still governs.
            var total = new Total(new DateTime(2026, 9, 1, 14, 30, 0));

            CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            Assert.AreEqual("01/09/2026", total.GetString(null));
            Assert.AreEqual("2026|09|01", total.GetString("{0:yyyy|MM|dd}"));

            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            Assert.AreEqual("09/01/2026", total.GetString(null));
        }

        [TestMethod]
        public void AColumnFormatStillWins()
        {
            var column = _columns.Add(x => x.Created).Format("{0:yyyy-MM-dd}");
            var item = new TestModel { Created = new DateTime(2026, 9, 1, 14, 30, 0) };

            CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            Assert.AreEqual("2026-09-01", column.GetValue(item).ToString());
        }
    }
}
