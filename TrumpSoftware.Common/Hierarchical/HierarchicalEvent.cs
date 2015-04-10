using System;

namespace TrumpSoftware.Common.Hierarchical
{
    public abstract class HierarchicalEvent<T>
    {
        public EventDirection Direction { get; private set; }

        internal Type DataType { get; private set; }

        protected HierarchicalEvent(EventDirection direction)
        {
            Direction = direction;
            DataType = typeof (T);
        }
    }
}
