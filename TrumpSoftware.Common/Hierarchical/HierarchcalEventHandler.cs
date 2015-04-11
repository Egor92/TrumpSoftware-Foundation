namespace TrumpSoftware.Common.Hierarchical
{
    public delegate void HierarchcalEventHandler<in TSender, in TData>(TSender sender, TData data)
        where TSender : IHierarchical;
}
