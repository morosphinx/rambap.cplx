using rambap.cplx.Modules.Connectivity.Templates;

using static rambap.cplxtests.LibTests.DSub.ContactType;
using static rambap.cplxtests.LibTests.DSub.ContactCounts;

namespace rambap.cplxtests.LibTests; 

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

        public DynamicDSub(string pn, ContactCounts ctn, ContactType contact, bool removable)
            : base(ToPinCount(ctn),() => GetPin(contact,removable))
        {
            PN = pn;
            ContactCounts = ctn;
            ContactType = contact;
            RemovableContacts = removable;
        }
    }

    // PN could be dynamic in a case of a library too large to be enumerated in code (eg : anything circular)
    public static DynamicDSub GetLibItem_Untyped(ContactCounts ctn, ContactType contact, bool removable)
    {
        var config = CplxFakeDsubSeriesConfigs.First(t => t.ContactCounts == ctn && t.ContactType == contact && t.RemovableContacts == removable);
        return new DynamicDSub(config.PN, config.ContactCounts, config.ContactType, config.RemovableContacts);
    }



    // TBD : different library styles : 2 - return pre-created, typed part
    // This one has the benefit of giving outside library a type to relies on
    // Not possible 

    public class DSub_1056 : DynamicDSub { public DSub_1056() : base("DSub_1056", _09, Male, false){} }

    static List<DynamicDSub> CplxFakeDsubSeriesPart { get; } =
        [
            new DSub_1056()
        ];

    public static DynamicDSub GetLibItem_Typed(ContactCounts ctn, ContactType contact, bool removable)
    {
        var connector = CplxFakeDsubSeriesPart.First(c => c.ContactCounts == ctn && c.ContactType == contact && c.RemovableContacts == removable);
        var type = connector.GetType();
        return (DynamicDSub)Activator.CreateInstance(type)!;
    }
}