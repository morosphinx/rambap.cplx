using rambap.cplx.Core;

namespace rambap.cplx.Export.Prodocs;

internal static class CommonSections
{
    public static string CommonHeader(Pinstance instance) =>
 $"""
 # Identification
 |#|Value|
 |-|-----|
 |PN|{instance.PN}|
 |Rev|{instance.Revision}|
 |Ver|{instance.Version}|
 |Date|{cplx.Globals.GenerationDate}|
 """;
}
