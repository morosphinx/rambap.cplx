using rambap.cplx.Modules.Connectivity.Templates;

using static rambap.cplxtests.LibTests.DSub.ContactType;
using static rambap.cplxtests.LibTests.DSub.ContactCounts;

namespace rambap.cplxtests.LibTests; 

/// <summary>
/// A fake, abstract DSub series of connector <br/>
/// Part numbering of this series is arbitrary - there is no relatation between PN and behavior
/// </summary>
public static class DSub
{
    // Non-removable
    public class NonremContactF : Pin{ }
    public class NonremContactM : Pin { }

    // Removable, to be brough independently
    public class RemContactF : Pin { }
    public class RemContactM : Pin { }


    public enum ContactType
    {
        Male,
        Female,
    }
    public static Pin GetPin(ContactType type, bool removable)
        => type switch
        { 
            ContactType.Male => removable ? new RemContactM() : new NonremContactM(),
            ContactType.Female => removable ? new RemContactF() : new NonremContactF(),
            _ => throw new NotSupportedException()
        };

    public enum ContactCounts
    {
        _09,
        _15,
        _25,
        _37,
        _50,
    }
    public static int ToPinCount(ContactCounts p)
        => p switch
        {
            _09 => 9,
            _15 => 15,
            _25 => 25,
            _37 => 37,
            _50 => 50,
            _ => throw new NotImplementedException()
        };


    // TBD : different library styles : 1 - List of possible

    static List<(string PN, ContactCounts ContactCounts, ContactType ContactType, bool RemovableContacts)> CplxFakeDsubSeriesConfigs { get; } =
        [
            ("DSub_1056", _09, Male, false),
            ("DSub_1057", _15, Male, false),
            ("DSub_1058", _25, Male, false),
            ("DSub_1059", _37, Male, false),
            ("DSub_1060", _50, Male, false),
            ("DSub_1061", _09, Female, false),
            ("DSub_1062", _15, Female, false),
            ("DSub_1063", _25, Female, false),
            ("DSub_1064", _37, Female, false),
            ("DSub_1065", _50, Female, false),
        ];
    public class DynamicDSub : Connector
    {
        internal ContactCounts ContactCounts { get; init; }
        internal ContactType ContactType { get; init; }
        internal bool RemovableContacts { get; init; }

        public DynamicDSub(ContactCounts ctn, ContactType contact, bool removable)
            : base(ToPinCount(ctn),() => GetPin(contact,removable))
        {
            ContactCounts = ctn;
            ContactType = contact;
            RemovableContacts = removable;
        }
    }

    // PN could be dynamic in a case of a library too large to be enumerated in code (eg : anything circular)
    public static DynamicDSub GetLibItem_Untyped(ContactCounts ctn, ContactType contact, bool removable)
    {
        var config = CplxFakeDsubSeriesConfigs.First(t => t.ContactCounts == ctn && t.ContactType == contact && t.RemovableContacts == removable);
        return new DynamicDSub(config.ContactCounts, config.ContactType, config.RemovableContacts) { PN = config.PN };
    }



    // TBD : different library styles : 2 - return pre-created, typed part
    // This one has the benefit of giving outside library a type to relies on
    // Not possible 

    public class DSub_1056 : DynamicDSub { public DSub_1056() : base(_09, Male, false) { } }
    public class DSub_1057 : DynamicDSub { public DSub_1057() : base(_15, Male, false) { } }
    public class DSub_1058 : DynamicDSub { public DSub_1058() : base(_25, Male, false) { } }
    public class DSub_1059 : DynamicDSub { public DSub_1059() : base(_37, Male, false) { } }
    public class DSub_1060 : DynamicDSub { public DSub_1060() : base(_50, Male, false) { } }
    public class DSub_1061 : DynamicDSub { public DSub_1061() : base(_09, Female, false) { } }
    public class DSub_1062 : DynamicDSub { public DSub_1062() : base(_15, Female, false) { } }
    public class DSub_1063 : DynamicDSub { public DSub_1063() : base(_25, Female, false) { } }
    public class DSub_1064 : DynamicDSub { public DSub_1064() : base(_37, Female, false) { } }
    public class DSub_1065 : DynamicDSub { public DSub_1065() : base(_50, Female, false) { } }

    static List<DynamicDSub> CplxFakeDsubSeriesPart { get; } =
        [
            new DSub_1056() ,
            new DSub_1057() ,
            new DSub_1058() ,
            new DSub_1059() ,
            new DSub_1060() ,
            new DSub_1061() ,
            new DSub_1062() ,
            new DSub_1063() ,
            new DSub_1064() ,
            new DSub_1065() ,
        ];

    public static DynamicDSub GetLibItem_Typed(ContactCounts ctn, ContactType contact, bool removable)
    {
        var connector = CplxFakeDsubSeriesPart.First(c => c.ContactCounts == ctn && c.ContactType == contact && c.RemovableContacts == removable);
        var type = connector.GetType();
        return (DynamicDSub)Activator.CreateInstance(type)!;
    }

    public class Backshell_09 : Part { }
    public class Backshell_15 : Part { }
    public class Backshell_25 : Part { }
    public class Backshell_37 : Part { }
    public class Backshell_50 : Part { }
}