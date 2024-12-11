using rambap.cplx.Core;
using rambap.cplx.Export.TextFiles;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using static rambap.cplx.Export.Spreadsheet.Helpers;

namespace rambap.cplx.Export.Spreadsheet;

public record SpreadsheetFillInstruction
{
    public required string SheetName { get; init; }
    public int ColStart { get; init; } = 1;
    public uint RowStart { get; init; } = 2;
}

public record InstanceContentInstruction : SpreadsheetFillInstruction
{
    public required List<Func<Pinstance, string>> Lines { get; init; }
}

public record TableWriteInstruction : SpreadsheetFillInstruction
{
    public bool WriteHeader { get; init; } = false;
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


    public List<InstanceContentInstruction> InstanceContents{ get; init; } = new();
    public List<TableWriteInstruction> Tables { get; init; } = new();


    public void Do(string filepath)
    {
        File.Copy(TemplatePath, filepath, true);
        // Open The template
        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, true);

        WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart!;

        WorksheetPart GetWorkSheet(string sheetName)
        {
            var sheet = workbookPart.Workbook.Sheets!.Descendants<Sheet>().First(
                s =>
                {
                    EnumerateSheetProperties(s);
                    return s.GetAttributes().First(a => a.LocalName == "name").Value == sheetName;
                });
            var sheetID = sheet.Id!.Value!;
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheetID);
            return worksheetPart;
        }

        foreach (var instanceContentInstruction in InstanceContents)
        {
            var worksheetPart = GetWorkSheet(instanceContentInstruction.SheetName);
            ApplyInstanceContentInstruction(worksheetPart, instanceContentInstruction);
        }

        foreach(var tableInstruction in Tables)
        {
            var worksheetPart = GetWorkSheet(tableInstruction.SheetName);
            ApplyTableWriteInstruction(worksheetPart, tableInstruction);
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

    private void ApplyInstanceContentInstruction(WorksheetPart worksheetPart, InstanceContentInstruction instruction)
    {
        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;

        List<string> Lines = instruction.Lines.Select(l => l.Invoke(Content)).ToList();

        FillInColumnContent(sheetData, Lines, instruction.RowStart, instruction.ColStart);
    }

    private void ApplyTableWriteInstruction(WorksheetPart worksheetPart, TableWriteInstruction instruction)
    {
        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;
        var colRange = Enumerable.Range(instruction.ColStart, instruction.Table.IColumns.Count());
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
            instruction.Table.IColumns.Select(c => c.TypeHint).ToList(),
            currentRow, firstColl, columnStyles);

        worksheetPart.Worksheet.Save();
    }
}