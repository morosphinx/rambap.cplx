using rambap.cplx.Core;
using System.Text;

namespace rambap.cplx.Export.Prodocs;

internal static class CommonSections
{
    public static string CommonHeader(Pinstance instance) =>
 $"""
 |#|Value|
 |-|-----|
 |PN|{instance.PN}|
 |Rev|{instance.Revision}|
 |Ver|{instance.Version}|
 |Date|{cplx.Globals.GenerationDate}|
 """;

    public static string GetText(this IEnumerable<string> lines, string separator = "\r\n")
        => string.Join(separator, lines);
}
