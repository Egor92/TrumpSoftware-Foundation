using System;
using System.Collections.Generic;
using System.Linq;

namespace TrumpSoftware.Common
{
    public class CustomComparer<T> : IComparer<T>
    {
        #region Fields

        private readonly Func<T, IComparable>[] _getValueFuncs;

        #endregion

        #region Ctor

        public CustomComparer(params Func<T, IComparable>[] getValueFuncs)
        {
            _getValueFuncs = getValueFuncs;
        }

        #endregion

        #region Implementation of IComparer<T>

        public int Compare(T left, T right)
        {
            if (left == null || right == null)
                return 0;
            return _getValueFuncs.Select(getValueFunc => Compare(left, right, getValueFunc))
                                 .FirstOrDefault(x => x != 0);
        }

        #endregion

        private static int Compare(T left, T right, Func<T, IComparable> getValueFunc)
        {
            var xValue = getValueFunc(left);
            var yValue = getValueFunc(right);

            if (xValue == null && yValue == null)
                return 0;

            if (xValue == null)
            {
                return -1;
            }

            if (yValue == null)
            {
                return 1;
            }

            return xValue.CompareTo(yValue);
        }
    }
}
