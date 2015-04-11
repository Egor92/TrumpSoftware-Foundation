namespace TrumpSoftware.Common.Hierarchical
{
    internal class SubscriberHandlerPair<TSender, TData>
        where TSender : IHierarchical
    {
        internal IHierarchical Subscriber { get; set; }

        internal HierarchcalEventHandler<TSender, TData> Handler { get; set; }
    }
}
