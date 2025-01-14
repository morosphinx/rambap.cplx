using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Export.Spreadsheet;

/// <summary>
/// Helpers function to work with Open XML<br/>
/// See the <see href="https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/how-to-create-a-spreadsheet-document-by-providing-a-file-name">Open XML online documentation.</see>
/// </summary>
internal static partial class Helpers
{

    // There is a maximum to the accepted size of an xlsx CellValue when oppening with excel. Don't know how much. TODO : Find
    public static CellValue MakeValidCellValue(string raw)
    {
        int maxlength = 1024;
        string clampingWarning = raw.Length > maxlength ? "<!!! ...>" : "";
        return new CellValue(raw.Substring(0, Math.Min(raw.Length, maxlength)) + clampingWarning);
    }

    public static EnumValue<CellValues> TypeHintToDataType(ColumnTypeHint columnTypeHint)
    {
        return columnTypeHint switch
        {
            ColumnTypeHint.StringExact => new EnumValue<CellValues>(CellValues.String),
            ColumnTypeHint.StringFormatable => new EnumValue<CellValues>(CellValues.String),
            ColumnTypeHint.Numeric => new EnumValue<CellValues>(CellValues.Number),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Write 2D Strign data to and OpenXML SheetData
    /// </summary>
    /// <param name="sheetData">Sheet to write it to</param>
    /// <param name="contents">2D string data to write. content[rowindex][columnindex]</param>
    /// <param name="columnTypeHints">Column type hints</param>
    /// <param name="rowStart">Starting Row. 1-indexed</param>
    /// <param name="colStart">Stating Column.  1-index</param>
    public static void FillInTableContents(SheetData sheetData, IEnumerable<List<string>> contents, List<ColumnTypeHint> columnTypeHints, uint rowStart, int colStart = 1, List<int>? columnStyleIndexes = null)
    {
        uint currentRow = rowStart;
        foreach (var line in contents)
        {
            FillInLineContent(sheetData, line, columnTypeHints, currentRow, colStart, columnStyleIndexes);
            currentRow++;
        }
    }

    /// <summary>
    /// Write a Row of data to an OpenXML SheetData
    /// </summary>
    /// <param name="sheetData">Sheet to write it to</param>
    /// <param name="line">Data to write</param>
    /// <param name="columnTypeHints">Column type hints</param>
    /// <param name="row">Row to write to. 1-indexed</param>
    /// <param name="colStart">Starting Column.  1-index</param>
    public static void FillInLineContent(SheetData sheetData, List<string> line, List<ColumnTypeHint> columnTypeHints, uint row, int colStart = 1, List<int>? columnStyleIndexes = null)
    {
        int currentCol = colStart;
        foreach (var c in line)
        {
            if(c != "")
            {
                var currentColName = GetExcelColumnName(currentCol);
                var currentCell = sheetData.GetOrMakeCell(currentColName, row);
                currentCell.CellValue = MakeValidCellValue(c);
                currentCell.DataType = TypeHintToDataType(columnTypeHints[currentCol - 1]);

                // Use column style, if one exist
                if (columnStyleIndexes != null &&
                    currentCell.StyleIndex == null)
                {
                    var targetStyle = columnStyleIndexes[currentCol - colStart];
                    if (targetStyle > 0)
                        currentCell.StyleIndex = (uint) targetStyle;
                }
            }
            currentCol++;
        }
    }

    /// <summary>
    /// Write a Column of data to an OpenXML SheetData
    /// </summary>
    /// <param name="sheetData">Sheet to write it to</param>
    /// <param name="column">Data to write</param>
    /// <param name="rowStart">Starting Row. 1-indexed</param>
    /// <param name="col">Column to write to.  1-index</param>
    public static void FillInColumnContent(SheetData sheetData, List<string> column, uint rowStart, int col)
    {
        uint currentRow = rowStart;
        foreach (var c in column)
        {
            if (c != "")
            {
                var currentColName = GetExcelColumnName(col);
                var currentCell = sheetData.GetOrMakeCell(currentColName, currentRow);
                currentCell.CellValue = MakeValidCellValue(c);
                currentCell.DataType = TypeHintToDataType(ColumnTypeHint.StringFormatable);
            }
            currentRow++;
        }
    }

    /// <summary>
    /// Get a columndata from a worksheet
    /// </summary>
    /// <param name="worksheet">worksheet containing the column</param>
    /// <param name="col">Column index. 1-indexed</param>
    /// <returns>The column data, or null if there is no column defined at this index<br/>
    /// If multiples columns ranges match the index, return the first one</returns>
    public static Column? GetColumn(this Worksheet worksheet, int col)
    {
        return worksheet
            .GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Columns>()!
            .Elements<Column>().FirstOrDefault(
                c =>((c.Min ?? uint.MaxValue) <= col) && (col <= (c.Max ?? uint.MinValue))
                );
    }


    // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
    // If the cell already exists, returns it. 
    public static Cell GetOrMakeCell(this SheetData sheetData, string columnName, uint rowIndex) //WorksheetPart worksheetPart)
    {
        string cellReference = columnName + rowIndex;
        var row = sheetData!.GetOrMakeRow(rowIndex);
        var cell = row!.GetOrMakeCell(columnName + rowIndex);
        return cell;
    }

    internal static Row GetOrMakeRow(this SheetData sheetData, uint rowIndex)
    {
        Row? row = sheetData.Elements<Row>()
            .Where(r => r.RowIndex != null && r.RowIndex == rowIndex)
            .FirstOrDefault();
        // If the worksheet does not contain a row with the specified row index, insert one.
        if (row == null)
        {
            row = new Row() { RowIndex = rowIndex };
            sheetData.Append(row);
        }
        return row;
    }

    internal static Cell GetOrMakeCell(this Row row, string cellReference)
    {
        var cell = row.Elements<Cell>()
            .Where(c => c.CellReference is not null && c.CellReference.Value == cellReference)
            .FirstOrDefault();
        // If there is not a cell with the specified column name, insert one.  
        if (cell == null)
        {
            // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            Cell? refCell = row.Elements<Cell>()
                .Where(c => string.Compare(c.CellReference?.Value, cellReference, true) > 0)
                .FirstOrDefault();
            cell = new Cell() { CellReference = cellReference };
            row.InsertBefore(cell, refCell);
        }
        return cell;
    }

    // Given a worksheet, a column name, and a row index, gets the cell at the specified column and row.
    internal static Cell? GetSpreadsheetCell(Worksheet worksheet, string columnName, uint rowIndex)
    {
        IEnumerable<Row>? rows = worksheet.GetFirstChild<SheetData>()?.Elements<Row>().Where(r => r.RowIndex is not null && r.RowIndex == rowIndex);
        if (rows is null || rows.Count() == 0)
        {
            // A cell does not exist at the specified row.
            return null;
        }

        IEnumerable<Cell> cells = rows.First().Elements<Cell>().Where(c => string.Compare(c.CellReference?.Value, columnName + rowIndex, true) == 0);

        if (cells.Count() == 0)
        {
            // A cell does not exist at the specified column, in the specified row.
            return null;
        }

        return cells.FirstOrDefault();
    }

    public static void EnumerateSheetProperties(Sheet sheet)
    {
        foreach(var a in sheet.GetAttributes())
        {
            Console.WriteLine($"{a.LocalName} = {a.Value}");
            // Typical result :

            // name = Sheet1
            // sheetId = 1
            // id = rId1
        }
    }
}

