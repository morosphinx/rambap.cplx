﻿using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityTableContent;
using static rambap.cplx.Modules.Connectivity.Outputs.ConnectivityColumns;
using rambap.cplx.Export.Tables;
using rambap.cplx.Modules.Base.Output;

namespace rambap.cplx.Modules.Connectivity.Outputs;

internal class ConnectivityTables
{
    public static TableProducer<ConnectivityTableContent> ConnectionTable(ConnectorIdentity identity)
        => new TableProducer<ConnectivityTableContent>()
        {
            Iterator = new ConnectivityTableIterator(),
            Columns = [
                    ConnectorPart(ConnectorSide.Left,identity,"CID", i => i.CID()),
                    ConnectorPart(ConnectorSide.Left,identity,"CN", i => i.CN),
                    ConnectorPart(ConnectorSide.Left,identity,"PN", i => i.PN),
                    ConnectorName(ConnectorSide.Left,identity),
                    Dashes(),
                    ConnectionKind(),
                    ConnectorName(ConnectorSide.Rigth,identity),
                    ConnectorPart(ConnectorSide.Rigth,identity,"PN", i => i.PN),
                    ConnectorPart(ConnectorSide.Rigth,identity,"CN", i => i.CN),
                    ConnectorPart(ConnectorSide.Rigth,identity,"CID", i => i.CID()),
                ]
        };

    public static TableProducer<IComponentContent> InterfaceControlDocumentTable()
        => new TableProducer<IComponentContent>()
        {
            Iterator = new ICDTableIterator(),
            Columns = [
                    IDColumns.ComponentNumberPrettyTree(),
                    ICDColumns.TopMostPortName(),
                    ICDColumns.MostRelevantPortName(),
                    ICDColumns.MostRelevantPortName_Regard(),
                    ICDColumns.SelfPortName(),
                ]
        };
}
