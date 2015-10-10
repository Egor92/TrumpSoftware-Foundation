using System;
using System.Collections.Generic;
using System.Linq;

namespace TrumpSoftware.Common
{
    public class CustomEqualityComparer<T> : IEqualityComparer<T>
    {
        #region Fields

        private readonly Func<T, object>[] _getValueFuncs;

        #endregion

        #region Ctor

        public CustomEqualityComparer(params Func<T, object>[] getValueFuncs)
        {
            _getValueFuncs = getValueFuncs;
        }

        #endregion

        #region Implementation of IEqualityComparer<T>

        public bool Equals(T x, T y)
        {
            if (x == null || y == null)
                return false;
            return _getValueFuncs.All(getValueFunc =>
            {
                var xValue = getValueFunc(x);
                var yValue = getValueFunc(y);
                return (xValue == null && yValue == null) || xValue == yValue;
            });
        }

        public int GetHashCode(T obj)
        {
            return _getValueFuncs.Aggregate(0, (hashCode, func) => AggregateHashCodes(obj, hashCode, func));
        }

        #endregion

        private static int AggregateHashCodes(T obj, int hashCode, Func<T, object> func)
        {
            var value = func(obj);
            if (value == null)
                return hashCode;
            return hashCode ^ value.GetHashCode();
        }
    }
}
