using static rambap.cplx.Core.Support;

namespace rambap.cplx.Core;

// Internals and private behavior of the Part class
public partial class Part
{
    internal bool HasDoneCplxImplicitInitialisation { get; private set; } = false;

    internal class InitialisationContext
    {
        private Stack<Part> ClassStack = new Stack<Part>();
        private Stack<Type> TypeStack = new Stack<Type>();
        public void StartInitFor(Part newPart)
        {
            var newType = newPart.GetType();
            if (TypeStack.Contains(newType))
            {
                // We are already in the process of initialising this type => there is a definition recursion, raise error
                TypeStack.Push(newType);
                var typeNames = TypeStack.Reverse().Select(t => t.Name);
                var typeRoute = string.Join(" -> ", typeNames);
                throw new InvalidOperationException($"Type {newType} is recursively part of itself. Initialisation order : {typeRoute}");
            }
            ClassStack.Push(newPart);
            TypeStack.Push(newType);
        }

        public Part? CurrentLocationPart()
            => ClassStack.Count() > 0 ? ClassStack.Peek() : null;

        public void EndedInit()
        {
            ClassStack.Pop();
            TypeStack.Pop();
        }
    }


    internal void CplxImplicitInitialization()
        => CplxImplicitInitialization(new InitialisationContext());

    /// <summary>
    /// Initialise implicit constructs ommited in the cplx syntax
    /// </summary>
    private void CplxImplicitInitialization(InitialisationContext initContext)
    {
        if (!HasDoneCplxImplicitInitialisation)
        {
            // Set Part parent info
            Parent = initContext.CurrentLocationPart();
            // Add current part location info to the initContext
            initContext.StartInitFor(this);
            // Create Part properties/fields if null
            ScanObjectContentFor<Part>(this,
               (p, i) => {
                   p.CplxImplicitInitialization(initContext);
               },
               (t, i) => {
                   var p = CreatePartFromType(t, initContext);
                   p.CplxImplicitInitialization(initContext);
                   return p;
               });
            ScanObjectContentFor<IEnumerable<Part>>(this,
                (p, i) =>
                {
                    foreach (var part in p)
                        part.CplxImplicitInitialization(initContext);
                });
            // Create all IPartProperties, and
            // Assign properties Owners
            ScanObjectContentFor<IPartProperty>(this,
                (p, i) =>
                {
                    p.Owner = this;
                    if (p.Name is null || p.Name == "")
                        p.Name = i.Name; // If IPartProperty.name is not set, uses the field name as property name
                    p.IsPublic = i.IsPublicOrAssembly;
                },
                AutoContent.ConstructIfNulls);
            ScanObjectContentFor<IEnumerable<IPartProperty>>(this,
                (p, i) =>
                {
                    foreach (var prop in p)
                    {
                        prop.Owner = this;
                        if (prop.Name is null || prop.Name == "")
                            prop.Name = i.Name;
                        prop.IsPublic = i.IsPublicOrAssembly;
                    }
                },
                AutoContent.ConstructIfNulls);
            // Initialisation done, no need to do it again
            HasDoneCplxImplicitInitialisation = true;
            //
            initContext.EndedInit();
        }
    }

    // TODO / TBD : each part is rigth now created unique.
    // Is there a way to reuse parts (not including those created with new() non-default constructors) ?
    // When parts are referenced to etablish relation (eg : connection, slotting),
    // Object instance identity is used
    private static Part CreatePartFromType(Type type, InitialisationContext context)
    {
        if (!type.IsAssignableTo(typeof(Part)))
            throw new InvalidOperationException();
        var part = Activator.CreateInstance(type) as Part;
        if (part is null)
            throw new InvalidOperationException();
        return part;
    }
}

