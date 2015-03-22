using System;
using System.Collections.Generic;
using System.Linq;

namespace TrumpSoftware.Common
{
    public static class TreeHelper
    {
        public static int GetNodeCount<T>(T sourceRoot, Func<T, IEnumerable<T>> getChildren)
            where T : class
        {
            if (getChildren == null) throw new ArgumentNullException("getChildren");
            if (sourceRoot == null)
                return 0;
            return 1 + getChildren(sourceRoot).Sum(child => GetNodeCount(child, getChildren));
        }

        public static TTo TransformTree<TFrom, TTo>(TFrom sourceRoot, Func<TFrom, TTo> transformNode, Func<TFrom, IEnumerable<TFrom>> getSourceChildren, Func<TTo, ICollection<TTo>> getTargetChildrenCollection, Action<TTo, TFrom> internalNodeAction, Action<TTo, TFrom> leafNodeAction)
            where TFrom : class
            where TTo : class
        {
            if (transformNode == null) throw new ArgumentNullException("transformNode");
            if (getSourceChildren == null) throw new ArgumentNullException("getSourceChildren");
            if (getTargetChildrenCollection == null) throw new ArgumentNullException("getTargetChildrenCollection");
            if (sourceRoot == null)
                return null;
            var targetRoot = transformNode(sourceRoot);
            if (targetRoot != null)
            {
                var sourceChildren = getSourceChildren(sourceRoot);
                if (sourceChildren != null)
                {
                    var targetChildrenCollection = getTargetChildrenCollection(targetRoot);
                    if (targetChildrenCollection == null)
                        throw new Exception("targetChildrenCollection must be not null");
                    foreach (var sourceChild in sourceChildren)
                    {
                        var targetChild = TransformTree(sourceChild, transformNode, getSourceChildren, getTargetChildrenCollection, internalNodeAction, leafNodeAction);
                        targetChildrenCollection.Add(targetChild);
                    }

                    if (sourceChildren.Any())
                    {
                        if (internalNodeAction != null)
                            internalNodeAction(targetRoot, sourceRoot);
                    }
                    else
                    {
                        if (leafNodeAction != null)
                            leafNodeAction(targetRoot, sourceRoot);
                    }
                }
                else
                {
                    if (leafNodeAction != null)
                        leafNodeAction(targetRoot, sourceRoot);
                }
            }
            return targetRoot;
        }
    }
}
