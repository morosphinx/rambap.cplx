namespace rambap.cplx.Core; 

// Helper Function for Part Class
public partial class Part
{
    public void AssertIsOwnerOf(IPartProperty property)
    {
        if (this != property.Owner)
            throw new InvalidOperationException("Property must be owned by this");
    }
    public void AssertIsParentOf(IPartProperty property)
        => AssertIsParentOf(property.Owner!);

    public void AssertIsParentOf(Part seekPart)
    {
        if (seekPart == this)
            return; // We are in self component tree, all is good
        else if (seekPart.Parent == null)
            throw new InvalidOperationException("Part must be a component or subcomponent of this");
        else
            AssertIsParentOf(seekPart.Parent);
    }
}
