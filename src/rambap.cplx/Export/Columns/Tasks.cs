using rambap.cplx.Core;
using rambap.cplx.Export.Iterators;

namespace rambap.cplx.Export.Columns;

public static class Tasks
{
    public static DelegateColumn<PartTtreeItem> TaskName()
        => new DelegateColumn<PartTtreeItem>("Task Name", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                    {
                        return n.Name;
                    }
                }
                return "";
            });

    public static DelegateColumn<PartTtreeItem> TaskType()
        => new DelegateColumn<PartTtreeItem>("Task Type", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                    {
                        return n.IsRecurent ? "Recurrent" : "Design" ;
                    }
                }
                return "";
            });

    public static DelegateColumn<PartTtreeItem> TaskCategory()
        => new DelegateColumn<PartTtreeItem>("Task Category", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                    {
                        return n.Category;
                    }
                }
                return "";
            });

    public static DelegateColumn<PartTtreeItem> TaskTotalDuration()
        => new DelegateColumn<PartTtreeItem>("Task Total Duration", ColumnTypeHint.String,
            i =>
            {
                if (i is LeafPartPropertyTableItem lpi)
                {
                    if (lpi.Property is Concepts.InstanceTasks.NamedTask n)
                    {
                        var total_duration = (n.IsRecurent ? lpi.Items.Count() : 1) * n.Duration_day;
                        return $"{total_duration}";
                    }
                }
                return "";
            });
}

