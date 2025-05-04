using rambap.cplx.Export.Text;
using rambap.cplx.Export.CoreTables;
using System.Runtime.CompilerServices;

namespace rambap.cplxtests.UsageTests;

 internal class MakeCplxRepoReadmeInstruction : TxtPInstanceFile
{
    /// <summary>
    /// File to be included used as exemple.
    /// Must contains the <see cref="Content"/>, and be marked as CopyToOutputDirectory=Always
    /// </summary>
    public required string ContentFilename { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Text of the cs file to put in the readme</returns>
    private string GetExemplePartSourceFileText()
    {
        // ReadExemple.cs is copied in the output build folder, see the *.csproj
        var fileLines = File.ReadAllLines(ContentFilename);
        // Skip(1) to skip the namespace declaration
        return string.Join("\r\n", fileLines.Skip(1));
    }

    public override string GetText()
    {
        return
$"""
# CPLX
## About
CPLX is a C# internal [domain specific language](https://en.wikipedia.org/wiki/Domain-specific_language) to define and document complex, hierarchical hardware systems.

CPLX definitions are source-controlable. CPLX calculates aggregate properties of systems, and generates documentation.

## An exemple
### Inputs
By writing the following definition :
``` Csharp
{GetExemplePartSourceFileText()}
```


### Outputs
CPLX generate files assisting multiples aspects of the design process :

- Component Tree

```
{string.Join("\r\n",
    new FixedWidthTableFormater().Format(
        SystemViewTables.ComponentTree_Detailled(),
        Content))
}
```

- Bill of materials

{string.Join("\r\n",
    new MarkdownTableFormater() 
        .Format(
        new BillOfMaterial() with { WriteTotalLine = true },
        Content))
}

- Cost Breakdown

{string.Join("\r\n",
    new MarkdownTableFormater()
        .Format(
        new CostBreakdown() with { WriteTotalLine = true },
        Content))
}

Ouputs are fully configurable. 

CPLX is build with extensibility in mind, and can document more concepts and physical properties as the design get refined.

## Getting started
Prerequisites : Use either [Visual Studio](https://visualstudio.microsoft.com/) or [VisualStudio Code](https://code.visualstudio.com/) IDE with C# support installed. 

1 - Install the [rambap.cplx.Templates](https://www.nuget.org/packages/rambap.cplx.Templates/) template package. To do so in a console :
```
dotnet new install rambap.cplx.Templates
```
2 - Create a new project using the <i>cplx.Templates.Exe</i> template you just installed, either through your IDE or with :
```
dotnet new cplxExecutable --name MyProjectName
```
3 - Edit the MyPart.cs file

4 - Run the project

## Documentation

[On github](https://github.com/morosphinx/rambap.cplx/tree/main/doc)

"""

;

    }
}

[TestClass]
public class MakeCplxRepoReadme
{
    [TestMethod]
    public void RemakeCplxRepoReadme()
    {
        var p = new ServerAssembly();
        var i = p.Instantiate();
        var MakeReadmeInstruction = new MakeCplxRepoReadmeInstruction() { Content = i, ContentFilename = "ReadmeExemple.cs" };

        var generatedFilePath = "C:\\TestFolder\\ReadmeExemple\\README.md";
        MakeReadmeInstruction.Do(generatedFilePath);

        var thisSource = GetThisSourceFilePath();
        var repoReadmePath = Path.Combine(thisSource, "..\\..\\..\\README.md");
        repoReadmePath = Path.GetFullPath(repoReadmePath);

        File.Copy(generatedFilePath, repoReadmePath, true);
    }

    private static string GetThisSourceFilePath([CallerFilePath] string? path = null)
        => path!;
}