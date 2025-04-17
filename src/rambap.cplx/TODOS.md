# TODOS


# Code

## Framework
- Clarify naming of Modules, Outputs, and Export utilies
    - What is the respondability split between Export vs Modules.[ModuleName].Output ?
		- Export are what define & create files. Output are how to data get out of the model
		- Concepts module define (only ? mostly ?) outputs. How the file look is and Export's job
- Clarify namespace of common types used by multiples modules (eg : Entities/Companies)
- Some kind of validation engine, that process the instance tree and return error / warnings in a file. Possible errors
    - A PN include a character that can be confused for a CSV separator (eg : '.' ';' ':', any whitespace other than '\s')
    - Two part share a PN but are not equivalent
        - Eg : a parametered constructor return same PN for different input values
    - Multiple properties / Component with the same (post instantiation, specified with attributes) name 
    - Costing : A cost is 0
    - Connectivity : A mate is made between two uncompatible connectors
    - Connectivity : Signal compatibility ?

## Core
- Better / cleaner generation call process
    - Generators are Instruction that take their Content delayed ? so generator can be defined (eg : Documentation Concept Properties) / prepared before instantiation ?
- Clarify the way CommonName is build and used : shouldn't have to check CommonName != PN when generating output
- Better default implementation of Outputs, with
    - Some better basic files
        - Differentiate component list (system view, include comments) from BOM (costing, include chosen suppliers & cost)
    - More configurable way to declare what part to include / ignore in the hierarchy
        - Currently, [CplxIgnore] do not allow each concept to configure chat to ignore. For exemple, Connectivity & Part tree need not be bothered by each Connector Pin, but Costing is.
    - A way to declare the relevant part to be documented as root when generating documentation for subcomponents (Only the part manufactured by you ? Only part not from a lib ? Maybe declaring each relevant root part is always required ?)
    - Correctly names files, with part name & revision
    - Generate to a dated folder
    - Way to include / cross reference document between each others (eg : plot integrated in a markdown)

Improvements to think about
- ✅ ~~Should a Component Type be exposed as the primary result of a Part Instantiation~~ ?
    - Would clarify root component naming
    - No instance would be without a Parent Component => Better null correctness
    - Make sense with bellow:
- Done, now : Rename the Component class to something that imply it's a Component Instance, and rename "Component" to "Subcomponent" in applicalbe instances
- Should the Subcomponent / PN / other core instance properies be stored (or just accessed) on the component instead of the Pinstance ?
- Should some properties be moved to the Component type, instead of the Instance Type ?
    - Would separates properties that are component locale (ex : usage in Connectivity) from those that are common for all Instance
    - Would sligthy improve external readbility
        - Writting Component.Instance each time get old
    - Instance could be reused between same PN
        - SubComponent[] array would need to also go on the component itself

Low Priority
- A common code to define extensive properties calculations
- Harmonise how unbacked (autoimplemented, "=>", or getter only) properties are handled by different concepts
    - How should and IEnumerable<> property, wrapping a list of cplx partProperties, work ?
- C# Analyser to raise warnings/errors on CPLX mistakes : 
    - A class contains a method with the same name as a PartInterface method, but do not implement said interface
    - Uninitialised classes members that have no parameterless constructor (can't be created by cplx auto init, need to reactivate CS8618 for them)  https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2017/extensibility/getting-started-with-roslyn-analyzers?view=vs-2017


## Concepts

### Costing Module
- Quotation System
- Use Quotation to update Costings
- Time spent

### Mass Module
- Columns
- Tests

### Connectivity Module
- A Tree like Signal definition hierarcy, like port exposition ?
- Signal auto rename when exposed from a subpart
- Signal propagation on wires for documentation
- A signal flow system table

### Mechanical occupation Module
Racking and the like
- Generation of mechanical occupation diagrams => In another lib ?

### Documentation Module
- Maybe declaring documents that can be generated in each part, using the documentation module, is the way ?

## Cplx Documentation
- Describe Part / Instance / Occurence concepts
- Exemples
- License
 
## Libraries and Env

### General
- A library with standard file generator for 
    - Part System Overview
    - Connectivity documents
    - Validation / traçability documents
    - Must use markdown with integrated diagrams / images
- A library with basic connectors
- A library using both previous libs to configure harnesses & generate doc

Low Priority
- Command line runner that can run generators on library dlls

### Export.Plot
- Use an SVG lib ?
- Generate visual component tree

Low Prio
- Generate extensive property diagrams

### Export.Spreadsheet
- Test template edges cases 
    - Cells type not matching the 
    - Fused cells
    - Localisation , . when attempty to writte double values ... due to IColumn.CellFor() passing the data through a string type, with formating that may not match what excel expect

## Stuff
- Some form of companion app to 
	- Help discoverability of part properties
	- Navigate a cplx dll lib, search parts in it based on some criteria, and copy / paste its type to the editor
- ✅🔳