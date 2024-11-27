using rambap.cplx.Core;
using rambap.cplx.Export.TextFiles;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using static rambap.cplx.Export.Spreadsheet.Helpers;

namespace rambap.cplx.Export.Spreadsheet;

public class ExcelTableFile_FromTemplate : AbstractTableFile
{
    public ExcelTableFile_FromTemplate(Pinstance content) : base(content)
    {
        TemplatePath = "templates/cplx_base1.xlsx";
    }

    public string TemplatePath { get; init; }
    public int ColStart { get; init; } = 1;
    public uint RowStart { get; init; } = 2;
    public bool WriteHeader { get; init; } = true;

    public override void Do(string filepath)
    {
        File.Copy(TemplatePath, filepath, true);
        // Open The template
        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, true);

        WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart!;
        var sheet = workbookPart.Workbook.Sheets!.First();

        var sheetPart = workbookPart.WorksheetParts.First();
        var sheetData = sheetPart.Worksheet.GetFirstChild<SheetData>()!;


        var TLCell = sheetData.GetOrMakeCell("A", 1);
        TLCell.CellValue = MakeValidCellValue("This is a title");
        TLCell.DataType = TypeHintToDataType(ColumnTypeHint.String);

        // Excel Col and Rows are 1 indexed
        int firstColl = ColStart;
        uint currentRow = RowStart;
        if (WriteHeader)
        {
            FillInLineContent(sheetData,
                Table.MakeHeaderLine(),
                Table.MakeHeaderLine().Select(c => ColumnTypeHint.String).ToList(),
                currentRow++, firstColl);
        }
        FillInTableContents(sheetData,
            Table.MakeContentLines(Content),
            Table.ColumnTypeHints().ToList(),
            currentRow, firstColl);

        sheetPart.Worksheet.Save();
        workbookPart.Workbook.Save();
        // Dispose the document.
        spreadsheetDocument.Dispose();
    }
}