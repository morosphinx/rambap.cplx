// This file tweak .NET & C# analyser for CPLX.
// Do not modify this file

// Global usings
#pragma warning disable IDE0005 // Remove unnecessary using directives 
global using rambap.cplx.Core;
global using rambap.cplx.Attributes;
global using rambap.cplx.PartInterfaces;
global using rambap.cplx.PartProperties;
global using static rambap.cplx.Helpers;
#pragma warning restore IDE0005 // Remove unnecessary using directives 

// Compiler warning that need to be disabled in the project file
// TODO : find a way to disable those from this file
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS0169 // The Field is never used
#pragma warning disable CS0649 // Field 'field' is never assigned to, and will always have its default value 'value'

// Global suppressions
// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project.
// Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type, member, etc.
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Not relevant to a CPLX model")]
[assembly: SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Not relevant to a CPLX model")]
[assembly: SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Not relevant to a CPLX model")]