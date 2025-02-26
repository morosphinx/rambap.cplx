## The problem domain

TODO - describe the problem space and intended use cases


## The general process

4 phase of dev. Tool they use, their purpose, their input output
Quotation. Architecture. Development. Integration/Test.

Information sharing required. Different need => living document

## The issues

Why other existing optiosn don't match :

Mudying of Part / Component number when 1 in 1, goign quick => Forcign those to be explciit, different construct, hope it help  deseign to be clean

## Alternatives
CAD software : Produce BOM, but focus on a specific engineering domain problem. Require full defintion to generate BOM => not usable in Quotation / Architecture. Dubious source control
Excel : No soruce control. // Working difficult. No recursivity of representation. Documenting / commenting is generaly umpractical. 
Word : :
MBSE software : Focus on product lifecyle / requirement gathering / use case. We are only interested in the part tree. Same issues as CAD software.

### 1 - Use Excel and macros
Excel file not source controlable.
Cooperative work hard. Cannot add columns that inrest only some peoples.
Spreadsheet representation does not fit hierarchical design (part in part in another part)
Cannot track / diff changes, or drop comments

If you currently document systems using Excel, give cplx a try.

### 2 - Use markdown files on git
No standard, no validation.


### 3 - Use a PCB or electrical CAD software
Specialised use case - Do not match general case : special properties, libraries.
Cannot make placeholder parts or describe "rough" designs
Extensibility somewhat lacking.

### 4 - Use an MBSE software
Why not : Use an [MBSE](https://en.wikipedia.org/wiki/Model-based_systems_engineering) software suite 

MBSE focuses a lot on modeling behaviors and requirement traçability. CPLX is solely interested in representing the physical structure and properties of systems, somewhat analog to SysML [Block Definition Diagram](https://sysml.org/sysml-faq/what-is-block-definition-diagram.html) and [Internal Block Diagram](https://sysml.org/sysml-faq/what-is-internal-block-diagram.html).

MBSE involves transformation of the entire design process. Its scope is not pertinent for small scale projects. CPLX is intended to be a quick to use tool to improve existing workload, no matter the project size.


## Why a C# SDL to define hardware
- GIT, the history
- GIT, the colaboration
- Code structure, multiples files
- Code-like identifier for PN
- IDE support for renames
- The namespaces
- Dependency management, nuget
- Versioning
