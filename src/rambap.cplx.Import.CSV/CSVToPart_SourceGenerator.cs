﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;

namespace rambap.cplx.Import
{
    [Generator]
    public class CSVToPart_SourceGenerator : ISourceGenerator
    {
        private string Fetch(AnalyzerConfigOptions option, string key)
        {
            string result;
            option.TryGetValue(key, out result);
            return result;
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Find the main method
            //var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
            var files = context.AdditionalFiles.Where(f => f.Path.EndsWith(".cplx"));

            var file = files.FirstOrDefault();
            //context.AnalyzerConfigOptions.GetOptions(file).TryGetValue("Namespace");
            var opt = context.AnalyzerConfigOptions.GlobalOptions;
            var r = context.AnalyzerConfigOptions.GlobalOptions.Keys;
            // Build up the source code
            string source = $@"// <auto-generated/>
using System;
// {r.Select(k => k.ToString() + " : " + Fetch(opt,k)).Aggregate("",(i,j) => i + "\r\n // " + j)}
namespace rambap.cplx.Examples
{{
    public partial class ZEee54e
    {{
        void HelloFrom(string name) =>
            Console.WriteLine($""Generator says: Hi from '{{name}}'"");
    }}
}}
";
            //var typeName = mainMethod.ContainingType.Name;

            // Add the source code to the compilation
            context.AddSource($"zerer.g.cs", source);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}

// See for reference :
// https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
// https://devblogs.microsoft.com/dotnet/new-c-source-generator-samples/
// https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/CsvGenerator.cs
// https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/MustacheGenerator.cs