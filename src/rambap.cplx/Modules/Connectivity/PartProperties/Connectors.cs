using rambap.cplx.Core;

namespace rambap.cplx.PartProperties;

public class Connector : IPartProperty
{
    internal bool HasBeenExposed { get; set; }

    private Signal? signal;
    public Signal? Signal
    {
        get => signal;
        internal set
        {
            if (signal != null)
                throw new InvalidOperationException("A signal is already assigned to this connector");
            signal = value;
        }
    }
}

