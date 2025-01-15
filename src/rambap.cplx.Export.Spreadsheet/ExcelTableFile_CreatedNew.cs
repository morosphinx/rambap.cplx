using rambap.cplx.Core;
using rambap.cplx.Export.TextFiles;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using static rambap.cplx.Export.Spreadsheet.Helpers;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Export.Spreadsheet;

public class ExcelTableFile_CreatedNew : IInstruction
{
    public required ITableProducer Table { protected get; init; }
    public Pinstance Content { get; init; }

    public ExcelTableFile_CreatedNew(Pinstance content)
    {
        Content = content;
    }

    public void Do(string filepath)
    {
        // Create a spreadsheet document by supplying the filepath.
        // By default, AutoSave = true, Editable = true, and Type = xlsx.
        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);

        // Add a WorkbookPart to the document.
        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        // Add a WorksheetPart to the WorkbookPart.
        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());

        // Add Sheets to the Workbook.
        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

        // Append a new worksheet and associate it with the workbook.
        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
        sheets.Append(sheet);


        Worksheet worksheet = worksheetPart.Worksheet;
        SheetData sheetData = worksheet.GetFirstChild<SheetData>()!;

        // Excel Col and Rows are 1 indexed
        int firstColl = 1;
        uint currentRow = 1;
        var headerLine = Table.MakeHeaderLine();
        FillInLineContent(sheetData,
            headerLine,
            Enumerable.Range(0, headerLine.Count).Select(i => ColumnTypeHint.StringFormatable).ToList(),
            currentRow++, firstColl);
        FillInTableContents(sheetData,
            Table.MakeContentLines(Content),
            Table.IColumns.Select(t=>t.TypeHint).ToList(),
            currentRow, firstColl);

        worksheetPart.Worksheet.Save();
        workbookPart.Workbook.Save();
        // Dispose the document.
        spreadsheetDocument.Dispose();
    }
}