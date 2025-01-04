namespace rambap.cplx.Core; 

// Helper methods for Part Class
public partial class Part
{
    /// <summary>
    /// Check that the property is owned by this part
    /// </summary>
    /// <param name="property">Property whose owner we want to assert</param>
    /// <exception cref="InvalidOperationException">Throw if the assertion is false</exception>
    public void AssertIsOwner(IPartProperty property)
    {
        if (!HasDoneCplxImplicitInitialisation)
            throw new InvalidOperationException("Part is not initialised. Create an Instance with this part first");
        if (this != property.Owner)
            throw new InvalidOperationException("Property must be owned by this part");
    }

    /// <summary>
    /// Check that the property is owned by this part or one of its subcomponents
    /// </summary>
    /// <param name="property">Property whose owner we want to assert</param>
    /// <exception cref="InvalidOperationException">Throw if the assertion is false</exception>
    public void AssertIsOwnerOrParent(IPartProperty property)
    {
        if (!HasDoneCplxImplicitInitialisation)
            throw new InvalidOperationException("Part is not initialised. Create an Instance with this part first");
        AssertIsSelfOrSubComponent(property.Owner!,"Property must be owned by this part or one of its subcomponents");
    }


    /// <summary>
    /// Check that the property is owned by one of this part subcomponents
    /// </summary>
    /// <param name="property">Property whose owner we want to assert</param>
    /// <exception cref="InvalidOperationException">Throw if the assertion is false</exception>
    public void AssertIsOwnedBySubComponent(IPartProperty property)
    {
        if (!HasDoneCplxImplicitInitialisation)
            throw new InvalidOperationException("Part is not initialised. Create an Instance with this part first");
        if(property.Owner == this)
            throw new InvalidOperationException("Property must be owned by a subcomponent");
        AssertIsSelfOrSubComponent(property.Owner!);
    }

    public void AssertIsASubComponent(Part part)
    {
        if (!HasDoneCplxImplicitInitialisation)
            throw new InvalidOperationException("Part is not initialised. Create an Instance with this part first");
        if (part == this)
            throw new InvalidOperationException("Part must be a subcomponent");
        AssertIsSelfOrSubComponent(part);
    }

    private void AssertIsSelfOrSubComponent(Part seekPart, string? customMessage = null)
    {
        if (seekPart == this)
            return; // We are in self component tree, all is good
        else if (seekPart.Parent == null)
        {
            var message = customMessage ?? "Part must be a component or subcomponent of this";
            throw new InvalidOperationException(message);
        }
        else
            AssertIsSelfOrSubComponent(seekPart.Parent);
    }
}
