# cplx Workflow

cplx aim is to model and document hardware systems with software tools.

This allow hardware design activities to benefit from modern IDE code features, such as compilation checks, source control, dependency management, code navigation and editing automation.

This imply blending tools such as Git in organizational processes not used to those.  

To facilitate this, this document outlines the main process steps to get the most use out of cplx, and the companion tools to use along the way.

This document is intended for readers that may have either a solely  hardware or solely software background, and try to give each an overview of the other's concerns and processes.


## When to Use cplx
- When to use
    - You want to document high level (rack, benches, machinery, rooms) systems, possibly very early in the design process.
    - Your design has a lot of reuse or hierarchical elements, and accounting for costs[^1] in excel isn't cutting it
    - You have issues with a flurry of word / excel / other textual documents that represent your system, and none is an authoritative source or thruth, and there's a lot of duplicated[^2] information.
    - You have to produce BOM / Buy list / other text documents by hand and want a way to automate it, and reuse designs[^3]
    - You want to track changes to the above in source control.
- When not to use
    - What you are making is already covered by existing CAD software (ex : a PCB, an electrical cabinet with terminal blocks), and your design is refined enough to work with those tools.
    - You already have an MBSE process set up.

[^1]: Either financial costs, or engineering dimensions, such as power budget, I/O channel counts, etc.
[^2]: And likely incoherent.
[^3]: Either as is, or make a component library, or as a parametric / dynamic Part generator

For details, see the [Problem Domain](../appendix/ProblemDomain.md) documentation appendix.

## Workflow
TODO Here : Describe a cplx workflow, including
- Requirement gathering
- Setting up a C# dev env
- Setting up a git
- Working with cplx. See details in [Quickstart](./QuickStart.md)
    - Creating a project from template
    - Creating the basic parts
    - Defining and running basic document generation
- Reviews, pushes etc
    - Generating documents supporting the design process
    - Interacting with other CAD software
- Making multiple C# project for code reuse
    - Packages, branches
- Generating documents for production / stakeholders
    - **Archive and timetag the outputs you distribute** [^4]
- Updating the Parts models at a later date / making new revisions
    - Managing Part PN and Revision Number
    - Managing a component supplier / manufacturer change

[^4]: Best case would be using a continuous code integration platform. 