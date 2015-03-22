using System;
using System.Collections.Generic;
using System.Linq;

namespace TrumpSoftware.Common
{
    public static class TreeHelper
    {
        public static int GetNodeCount<T>(T source, Func<T, IEnumerable<T>> getChildren)
            where T : class
        {
            if (getChildren == null) throw new ArgumentNullException("getChildren");
            if (source == null)
                return 0;
            return 1 + getChildren(source).Sum(child => GetNodeCount(child, getChildren));
        }

        public static TTo Transform<TFrom, TTo>(TFrom source, Func<TFrom, TTo> transformNode, Func<TFrom, IEnumerable<TFrom>> getSourceChildren, Func<TTo, ICollection<TTo>> getTargetChildrenCollection, Action<TTo, TFrom> internalNodeAction, Action<TTo, TFrom> leafNodeAction)
            where TFrom : class
            where TTo : class
        {
            if (transformNode == null) throw new ArgumentNullException("transformNode");
            if (getSourceChildren == null) throw new ArgumentNullException("getSourceChildren");
            if (getTargetChildrenCollection == null) throw new ArgumentNullException("getTargetChildrenCollection");
            if (source == null)
                return null;
            var target = transformNode(source);
            if (target != null)
            {
                var sourceChildren = getSourceChildren(source);
                if (sourceChildren != null)
                {
                    var targetChildrenCollection = getTargetChildrenCollection(target);
                    if (targetChildrenCollection == null)
                        throw new Exception("targetChildrenCollection must be not null");
                    foreach (var sourceChild in sourceChildren)
                    {
                        var targetChild = Transform(sourceChild, transformNode, getSourceChildren, getTargetChildrenCollection, internalNodeAction, leafNodeAction);
                        targetChildrenCollection.Add(targetChild);
                    }

                    if (sourceChildren.Any())
                    {
                        internalNodeAction(target, source);
                    }
                    else
                    {
                        leafNodeAction(target, source);
                    }
                }
                else
                {
                    leafNodeAction(target, source);
                }
            }
            return target;
        }
    }
}
