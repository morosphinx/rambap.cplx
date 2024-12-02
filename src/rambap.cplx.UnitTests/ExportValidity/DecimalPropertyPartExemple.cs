﻿namespace rambap.cplx.UnitTests.ExportValidity;



internal static class DecimalPropertyPartExemple
{
    public static decimal ExpectedTotal_ExtensiveT => 2483;
    public static decimal ExpectedTotal_IntensiveT => 2223;
}

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
        // Expected total : 2483
        T Cost_A1 = Make(1000);
        T Cost_A2 = Make(1000);
        Part_B B1, B2;
        Part_C C1, C2;
        Part_D D1;
        Part_N N1;
    }

    class Part_B : Part
    {
        // Expected total : 220
        T Cost_B1 = Make(100);
        T Cost_B2 = Make(100);
        Part_C C1;
        Part_N N1;
    }

    class Part_C : Part
    {
        // Expected total : 20
        T Cost_C1 = Make(10);
        T Cost_C2 = Make(10);
    }

    class Part_D : Part
    {
        // Expected total : 3
        T Cost_D1 = Make(1);
        T Cost_D2 = Make(1);
        T Cost_D3 = Make(1);
    }

    class Part_N : Part
    {
        T Cost_C1 = Make(0);
        T Cost_C2 = Make(0);
    }
}

