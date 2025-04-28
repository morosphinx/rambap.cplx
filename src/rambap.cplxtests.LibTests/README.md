# cplxtests.LibTests

This project define a library of fake cplx Parts to use in [cplxtests.UsageTests](../rambap.cplxtests.UsageTests). Thsi is because Parts defined in assemblies other than the executing assembly may be treated differently by cplx.




Parts in this library are not to be real world objects PNs, but may match existing standards.

For developpement purposes, this library lives in the cplx VisualStudio solution and reference the cplx.Core project directly. A production library should depend on the [cplx.Core](https://www.nuget.org/packages/rambap.cplx.Core/) package instead