using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GridBlazor.Resources;
using GridShared;
using GridShared.Columns;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GridBlazor
{
    public class ExcelCell
    {
        private string _content;

        public string Content {
            get { return _content; }
            set { 
                _content = RemoveInvalidXmlChars(value); 
            }
        }
        public int ColumnIndex { get; set; }
        public int RowIndex { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public CellValues Type { get; set; }

        public ExcelCell()
        {
        }

        public ExcelCell(string value, Type type = null)
        {
            Content = value;
            ColSpan = 1;
            RowSpan = 1;

            if(type == null)
                Type = CellValues.InlineString;
            else if (type == typeof(Int32) || type == typeof(double) || type == typeof(decimal) || type == typeof(byte)
                || type == typeof(Single) || type == typeof(float) || type == typeof(Int64) || type == typeof(Int16)
                || type == typeof(UInt64) || type == typeof(UInt32) || type == typeof(UInt16))
                Type = CellValues.Number;
            else if (type == typeof(DateTime) || type == typeof(DateTimeOffset)
#if !NETSTANDARD2_1 && !NET5_0
                || type == typeof(DateOnly) || type == typeof(TimeOnly)
#endif
                || type == typeof(decimal) || type == typeof(byte)
                || type == typeof(Single) || type == typeof(float) || type == typeof(Int64) || type == typeof(Int16)
                || type == typeof(UInt64) || type == typeof(UInt32) || type == typeof(UInt16))
                Type = CellValues.Date;
            else if(type == typeof(bool))
                Type = CellValues.Boolean;
            else
                Type = CellValues.InlineString;
        }

        private string RemoveInvalidXmlChars(string content)
        {
            return new string(content?.Where(ch => System.Xml.XmlConvert.IsXmlChar(ch)).ToArray());
        }
    }

    public class ExcelStatus
    {
        public string Message { get; set; }
        public bool Success
        {
            get { return string.IsNullOrWhiteSpace(Message); }
        }
    }

    public class ExcelData
    {
        public ExcelStatus Status { get; set; }
        public DocumentFormat.OpenXml.Spreadsheet.Columns ColumnConfigurations { get; set; }
        public List<List<ExcelCell>> Cells { get; set; }
        public string SheetName { get; set; }
        public bool ExtendedSheet { get; set; }

        public ExcelData() : this(false)
        { }

        // extended sheet allows merged cells and enforce column and row index use
        public ExcelData(bool extendedSheet)
        {
            Status = new ExcelStatus();
            Cells = new List<List<ExcelCell>>();
            ExtendedSheet = extendedSheet;
        }
    }

    public class ExcelWriter
    {
        private string ColumnLetter(int colIndex)
        {
            int div = colIndex + 1;
            string colLetter = string.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }

        private Cell CreateCell(string header, UInt32 index, string text, CellValues type)
        {
            Cell cell;

            if (type == CellValues.Number && double.TryParse(text, out double number))
            {
                cell = new Cell
                {
                    DataType = CellValues.Number,
                    CellReference = header + index,
                    CellValue = new CellValue(number.ToString(CultureInfo.InvariantCulture))
                };
            }
            else
            {
                cell = new Cell
                {
                    DataType = CellValues.InlineString,
                    CellReference = header + index
                };

                var istring = new InlineString();
                var t = new Text { Text = text };
                istring.AppendChild(t);
                cell.AppendChild(istring);
            }

            return cell;
        }

        private MergeCell CreateMergeCell(ExcelCell excelCell)
        {
            MergeCell mergeCell = new MergeCell
            {
                Reference = new StringValue(ColumnLetter(excelCell.ColumnIndex)
                        + (excelCell.RowIndex + 1) + ":"
                        + ColumnLetter(excelCell.ColumnIndex + excelCell.ColSpan - 1) +
                        +(excelCell.RowIndex + excelCell.RowSpan)),
            };
            return mergeCell;
        }

        public byte[] GenerateExcel<T>(IGridColumnCollection<T> columns, IEnumerable<T> items)
        {
            ExcelData excelData = new ExcelData();
            excelData.SheetName = Strings.Items;
            var header = new List<ExcelCell>();
            foreach (IGridColumn column in columns)
            {
                if (!(column.ExcelHidden ?? column.Hidden))
                {
                    header.Add(new ExcelCell(column.Title));
                }
            }
            excelData.Cells.Add(header);

            foreach (var item in items)
            {
                List<ExcelCell> row = new List<ExcelCell>();
                foreach (IGridColumn column in columns)
                {
                    if (!(column.ExcelHidden ?? column.Hidden))
                    {
                        var cell = column.GetExcelCell(item) as GridCell;
                        cell.Encode = false;
                        var type = ((IGridColumn<T>)column).GetTypeAndValue(item).Type;
                        row.Add(new ExcelCell(cell.ToString(), type));
                    }    
                }
                excelData.Cells.Add(row);
            }
            return GenerateExcel(excelData);
        }

        public byte[] GenerateExcel(ExcelData data)
        {
            var stream = new MemoryStream();

            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookpart = document.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();
                var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();

                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = document.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                var sheet = new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = data.SheetName ?? "Sheet 1"
                };
                sheets.AppendChild(sheet);

                if (data.ExtendedSheet)
                    AppendDataToExtendedSheet(worksheetPart.Worksheet, sheetData, data);
                else
                    AppendDataToSheet(sheetData, data);

                workbookpart.Workbook.Save();
            }

            return stream.ToArray();
        }

        private void AppendDataToSheet(SheetData sheetData, ExcelData data)
        {
            UInt32 rowIdex = 0;
            Row row;
            var cellIdex = 0;

            // Add sheet data
            foreach (var rowData in data.Cells)
            {
                cellIdex = 0;
                row = new Row { RowIndex = ++rowIdex };
                sheetData.AppendChild(row);
                foreach (var cellData in rowData)
                {
                    var cell = CreateCell(ColumnLetter(cellIdex++), rowIdex,
                        cellData.Content ?? string.Empty, cellData.Type);
                    row.AppendChild(cell);
                }
            }
        }

        private void AppendDataToExtendedSheet(Worksheet worksheet, SheetData sheetData,
            ExcelData data)
        {
            UInt32 rowIdex = 0;
            Row row;
            MergeCells mergeCells = new MergeCells();

            // Add sheet data
            foreach (var rowData in data.Cells)
            {
                row = new Row { RowIndex = ++rowIdex };
                sheetData.AppendChild(row);
                foreach (var excelCell in rowData)
                {
                    var cell = CreateCell(ColumnLetter(excelCell.ColumnIndex),
                        (uint)(excelCell.RowIndex + 1), excelCell.Content ?? string.Empty, excelCell.Type);
                    row.AppendChild(cell);
                    if (excelCell.ColSpan > 1 || excelCell.RowSpan > 1)
                    {
                        var mergeCell = CreateMergeCell(excelCell);
                        mergeCells.Append(mergeCell);
                    }
                }
            }
            worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
        }
    }
}
