using rambap.cplx.Core;
using rambap.cplx.Export.TextFiles;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using static rambap.cplx.Export.Spreadsheet.Helpers;
using rambap.cplx.Export.Tables;

namespace rambap.cplx.Export.Spreadsheet;

public record SpreadsheetFillInstruction
{
    public required string SheetName { get; init; }
    public int ColStart { get; init; } = 1;
    public uint RowStart { get; init; } = 2;
}

public record InstanceContentInstruction : SpreadsheetFillInstruction
{
    public enum WriteDirection { Line, Column }
    public WriteDirection Direction { get; init; } = WriteDirection.Column;
    public required List<Func<Pinstance, string>> Lines { get; init; }
}

public record TableWriteInstruction : SpreadsheetFillInstruction
{
    public bool WriteHeader { get; init; } = false;
    public required ITableProducer Table { get; init; }
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

        // Force workbook to be recalculated on next load => required for good UX
        workbookPart.Workbook.CalculationProperties!.ForceFullCalculation = true;
        workbookPart.Workbook.CalculationProperties!.FullCalculationOnLoad = true;
        // Save changes
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

        switch (instruction.Direction)
        {
            case InstanceContentInstruction.WriteDirection.Line:
                var typeHints = Lines.Select(c => ColumnTypeHint.StringFormatable).ToList();
                FillInLineContent(sheetData, Lines, typeHints, instruction.RowStart, instruction.ColStart);
                break;
            case InstanceContentInstruction.WriteDirection.Column:
                FillInColumnContent(sheetData, Lines, instruction.RowStart, instruction.ColStart);
                break;
            default:throw new NotImplementedException();
        }
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
            var headerLine = instruction.Table.MakeHeaderLine();
            FillInLineContent(sheetData,
                headerLine,
                Enumerable.Range(0, headerLine.Count).Select(c => ColumnTypeHint.StringFormatable).ToList(),
                currentRow++, firstColl);
        }
        FillInTableContents(sheetData,
            instruction.Table.MakeContentLines(Content),
            instruction.Table.IColumns.Select(c => c.TypeHint).ToList(),
            currentRow, firstColl, columnStyles);

        worksheetPart.Worksheet.Save();
    }
}