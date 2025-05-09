﻿namespace rambap.cplxtests.CoreTests.ExportValidity;

internal static class DecimalPropertyPartExemple
{
    public static decimal ExpectedTotal_ExtensiveT => 3142483;
    public static decimal ExpectedTotal_IntensiveT => 2142223;
}

// TODO : add a part with a single property with subcomponents, and one without,
// in order to trigger & test ComponentPropertyIterator.StackPropertiesSingleChildBranches

internal abstract class DecimalPropertyPartExemple<T>
    // T Must be constructible, implicitly or with a constructor, from a decimal
    // Cannot be expressed in C# as far as i know
{
    // Create a T from a decimal value
    protected static T Make(decimal value)
    {
        // Find the implicit construction operator with a decimal parameter
        var dicimalImplicitConversion = typeof(T).GetMethods()
            .FirstOrDefault(m => 
            m.Name == "op_Implicit"
            && m.GetParameters().Length == 1
            && m.GetParameters()[0].ParameterType == typeof(decimal));
        if(dicimalImplicitConversion != null)
        {
            return (T)dicimalImplicitConversion.Invoke(null, [value])!; 
        } else
        {
            return (T)Activator.CreateInstance(typeof(T), value)!;
        }
    }


    public class Part_A : Part
    {
        // Expected total : 122_483
        T Prop_A1 = Make(1000);
        T Prop_A2 = Make(1000);
        Part_B aB1, aB2;
        Part_C aC1, aC2;
        Part_D aD1;
        Part_N aN1;
        Part_E aE1;
        Part_G aG1;
        Part_H aH1;
        Part_M aM1;
    }

    class Part_B : Part
    {
        // Expected total : 220
        T Prop_B1 = Make(100);
        T Prop_B2 = Make(100);
        Part_C bC1;
        Part_N bN1;
        
    }

    class Part_C : Part
    {
        // Expected total : 20
        T Prop_C1 = Make(10);
        T Prop_C2 = Make(10);
    }

    class Part_D : Part
    {
        // Expected total : 3
        T Prop_D1 = Make(1);
        T Prop_D2 = Make(1);
        T Prop_D3 = Make(1);
    }

    [CplxHideContents]
    class Part_E : Part
    {
        // Expected total : 140_000
        // We expect NOT to find the F part in the results due to the [CplxHidContent] Attribute
        T Prop_E1 = Make(100_000);

        Part_F eF1;
        Part_F eF2;
    }

    class Part_F : Part
    {
        // Expected total : 20_000
        T Prop_F1 = Make(10_000);
        T Prop_F2 = Make(10_000);
    }

    class Part_N : Part
    {
        T Prop_N1 = Make(0);
        T Prop_N2 = Make(0);
    }

    class Part_M : Part
    {

    }
    class Part_G : Part
    {
        T Prop_G1 = Make(1_000_000);
    }
    class Part_H : Part
    {
        Part_G hG1;
        T Prop_H1 = Make(1_000_000);
    }
}

