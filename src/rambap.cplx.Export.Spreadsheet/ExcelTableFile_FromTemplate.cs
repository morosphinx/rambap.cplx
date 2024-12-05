using rambap.cplx.Core;
using rambap.cplx.Export.TextFiles;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using static rambap.cplx.Export.Spreadsheet.Helpers;

namespace rambap.cplx.Export.Spreadsheet;

public record TableWriteInstruction
{
    public required string SheetName { get; init; }
    public bool WriteHeader { get; init; } = false;
    public int ColStart { get; init; } = 1;
    public uint RowStart { get; init; } = 2;
    public required ITable Table { get; init; }
}

public class ExcelTableFile_FromTemplate : IInstruction
{
    public Pinstance Content { get; init; }
    public string TemplatePath { get; init; }

    public ExcelTableFile_FromTemplate(Pinstance content)
    {
        TemplatePath = "templates/cplx_base1.xlsx";
        Content = content;
    }


    public List<TableWriteInstruction> Tables { get; init; } = new();


    public void Do(string filepath)
    {
        File.Copy(TemplatePath, filepath, true);
        // Open The template
        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, true);

        WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart!;

        foreach(var tableInstruction in Tables)
        {
            var sheet = workbookPart.Workbook.Sheets!.Descendants<Sheet>().First(
                s =>
                {
                    EnumerateSheetProperties(s);
                    return s.GetAttributes().First(a => a.LocalName == "name").Value == tableInstruction.SheetName;
                }); 
            var sheetID = sheet.Id!.Value!;

            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheetID);
            EditSheet(worksheetPart, tableInstruction);
        }

        workbookPart.Workbook.Save();
        // Dispose the document.
        spreadsheetDocument.Dispose();
    }

    private static List<int> GetColumnStyles(Worksheet worksheet, IEnumerable<int> Indexes)
    {
        List<int> styleKeys = new ();
        foreach(var index in Indexes)
        {
            Column? col = Helpers.GetColumn(worksheet, index);
            if (col != null)
            {
                var colStyle = col.Style;
                if (colStyle != null)
                    styleKeys.Add((int)(uint)colStyle);
                else
                    styleKeys.Add(-1); // No style on this column
            }
            else
            {
                styleKeys.Add(-1); // No column for this cell
            }
        }
        return styleKeys;
    }

    private void EditSheet(WorksheetPart worksheetPart, TableWriteInstruction instruction)
    {
        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;
        var colRange = Enumerable.Range(instruction.ColStart, instruction.Table.ColumunCount);
        var columnStyles = GetColumnStyles(worksheetPart.Worksheet, colRange);

        // Excel Col and Rows are 1 indexed
        int firstColl = instruction.ColStart;
        uint currentRow = instruction.RowStart;
        if (instruction.WriteHeader)
        {
            FillInLineContent(sheetData,
                instruction.Table.MakeHeaderLine(),
                instruction.Table.MakeHeaderLine().Select(c => ColumnTypeHint.String).ToList(),
                currentRow++, firstColl);
        }
        FillInTableContents(sheetData,
            instruction.Table.MakeContentLines(Content),
            instruction.Table.ColumnTypeHints().ToList(),
            currentRow, firstColl, columnStyles);

        worksheetPart.Worksheet.Save();
    }
}