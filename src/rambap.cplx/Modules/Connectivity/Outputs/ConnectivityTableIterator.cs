using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Modules.Connectivity.Outputs;

internal class ConnectivityTableIterator : IIterator<ConnectivityTableContent>
{
    public IEnumerable<ConnectivityTableContent> MakeContent(Pinstance content)
    {
        var Connectivity = content.Connectivity()!;

        foreach(var group in Connectivity!.GetConnectionGroups())
        {
            var groupLeftConnector = group.LeftTopMost;
            var groupRightConnector = group.RigthTopMost;
            foreach (var connection in group.Connections)
            {
                bool shouldReverse = connection.ConnectorA.TopMostUser() != groupLeftConnector;
                if (shouldReverse)
                    yield return new ConnectivityTableContent()
                    {
                        LeftConnector = connection.ConnectorA,
                        RigthConnector = connection.ConnectorB
                    };
                else
                    yield return new ConnectivityTableContent()
                    {
                        LeftConnector = connection.ConnectorB,
                        RigthConnector = connection.ConnectorA
                    };
            }
        }
    }
}
