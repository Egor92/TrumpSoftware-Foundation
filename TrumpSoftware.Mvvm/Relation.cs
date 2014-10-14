using System;
using System.Collections.Generic;

namespace TrumpSoftware.Mvvm
{
    public class Relation
    {
        internal IList<IDataItem> Collection { get; private set; }

        private readonly Func<IDataItem, int> _getRelatedId;

        public Relation(IList<IDataItem> collection, Func<IDataItem, int> getRelatedId)
        {
            Collection = collection;
            _getRelatedId = getRelatedId;
        }

        internal int GetRelatedId(IDataItem dataItem)
        {
            return _getRelatedId(dataItem);
        }
    }
}
