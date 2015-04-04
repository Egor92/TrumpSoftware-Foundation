using System.Collections.Generic;

namespace TrumpSoftware.Common
{
    public class GluedNode<T>
    {
        private readonly int _objectCount;

        #region Children

        public IEnumerable<GluedNode<T>> Children { get; set; }

        #endregion

        #region Objects

        private T[] _objects;

        public T[] Objects
        {
            get { return _objects ?? (_objects = new T[_objectCount]); }
        }

        #endregion

        #region Ctor

        public GluedNode(int objectCount)
        {
            _objectCount = objectCount;
        }

        #endregion
    }
}
