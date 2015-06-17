using System;
using System.Collections.Generic;
using System.Linq;
using TrumpSoftware.Common.Extensions;

namespace TrumpSoftware.Common.Helpers
{
    public delegate IEnumerable<T> GetChildrenDelegate<T>(T source);
    public delegate ICollection<T> GetChildrenCollectionDelegate<T>(T source);

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

        public static TTo TransformTree<TFrom, TTo>(TFrom sourceRoot, Func<TFrom, TTo> transformNode, GetChildrenDelegate<TFrom> getSourceChildren, GetChildrenCollectionDelegate<TTo> getTargetChildrenCollection)
            where TFrom : class
            where TTo : class
        {
            return TransformTree(sourceRoot, transformNode, getSourceChildren, getTargetChildrenCollection, null, null);
        }

        public static TTo TransformTree<TFrom, TTo>(TFrom sourceRoot, Func<TFrom, TTo> transformNode, GetChildrenDelegate<TFrom> getSourceChildren, GetChildrenCollectionDelegate<TTo> getTargetChildrenCollection, Action<TTo, TFrom> internalNodeAction, Action<TTo, TFrom> leafNodeAction)
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

        public static IEnumerable<GluedNode<T>> GlueEnumerables<T>(IEnumerable<T>[] source, GetChildrenDelegate<T> getChildren)
            where T : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (getChildren == null)
                throw new ArgumentNullException("getChildren");

            int[] itemCounts = source.Select(x => x.GetCountOrZero()).ToArray();
            var maxItemCount = itemCounts.Max();

            var gluedNodes = new GluedNode<T>[maxItemCount];
            for (int i = 0; i < maxItemCount; i++)
            {
                gluedNodes[i] = new GluedNode<T>(source.Length);
                var childGlueSource = new IEnumerable<T>[source.Length];
                for (int j = 0; j < source.Length; j++)
                {
                    if (i < itemCounts[j])
                    {
                        var t = source[j].ElementAt(i);
                        gluedNodes[i].Objects[j] = t;
                        childGlueSource[j] = getChildren(t);
                    }
                }
                gluedNodes[i].Children = GlueEnumerables(childGlueSource, getChildren);
            }

            return gluedNodes;
        }

        public static IEnumerable<T> MakeTrees<T>(IEnumerable<T> source, Func<T,T> getParent, GetChildrenCollectionDelegate<T> getChildren)
            where T : class
        {
            var parentOfItems = source.ToDictionary(x => x, getParent);
            var rootItems = parentOfItems.Where(x => x.Value == null).Select(x => x.Key).ToArray();
            if (!rootItems.Any())
                throw new Exception("It's impossible to create trees. There is no items which parent is null");
            var notRootItems = source.Except(rootItems).ToArray();
            if (notRootItems.Any(x => !source.Contains(parentOfItems[x])))
                throw new Exception("It's impossible to create trees. At least one item has parent that doesn't contain in source enumerable");
            var childrenOfItems = source.ToDictionary(x => x, x => getChildren(x));
            foreach (var item in source)
                getChildren(item).Clear();
            foreach (var item in notRootItems)
            {
                var parent = parentOfItems[item];
                var children = childrenOfItems[parent];
                children.Add(item);
            }
            return rootItems;
        }

        public static IEnumerable<T> MakeTrees<T>(IEnumerable<T> source, Func<T,int> getId, Func<T,int> getParentId, Action<T,T> setParent, GetChildrenCollectionDelegate<T> getChildren, int defaultId)
            where T : class
        {
            var idOfItems = source.ToDictionary(x => x, getId);
            var parentIdOfItems = source.ToDictionary(x => x, getParentId);
            var rootItems = source.Where(x => parentIdOfItems[x] == defaultId).ToArray();
            if (!rootItems.Any())
                throw new Exception("It's impossible to create trees. There is no items which parent is null");
            var notRootItems = source.Except(rootItems).ToArray();
            if (notRootItems.Any(x => !idOfItems.Values.Contains(parentIdOfItems[x])))
                throw new Exception("It's impossible to create trees. At least one item has parent that doesn't contain in source enumerable");
            foreach (var item in source)
                getChildren(item).Clear();
            foreach (var item in rootItems)
                setParent(item, null);
            foreach (var item in notRootItems)
            {
                var parentId = parentIdOfItems[item];
                var parent = source.Single(x => idOfItems[x] == parentId);
                setParent(item, parent);
                getChildren(parent).Add(item);
            }
            return rootItems;
        }

        public static IEnumerable<T> GetNodes<T>(T rootNode, GetChildrenDelegate<T> getChildren, Action<T,T[]> collectedNodeAction, bool includeRoot, bool leafsOnly)
        {
            if (rootNode == null) throw new ArgumentNullException("rootNode");
            if (getChildren == null) throw new ArgumentNullException("getChildren");
            var nodes = new List<T>();
            var ancestorsByNode = new Dictionary<T, T[]>();
            CollectNodes(rootNode, getChildren, nodes, new Stack<T>(), ancestorsByNode, includeRoot, leafsOnly);
            foreach (var pair in ancestorsByNode)
            {
                var node = pair.Key;
                var ancestors = pair.Value;
                if (collectedNodeAction != null)
                    collectedNodeAction(node, ancestors);
            }
            return nodes;
        }

        private static void CollectNodes<T>(T node, GetChildrenDelegate<T> getChildren, ICollection<T> nodes, Stack<T> ancestors, IDictionary<T, T[]> ancestorsByNode, bool includeRoot, bool leafsOnly)
        {
            var children = getChildren(node);
            if (includeRoot)
            {
                if (leafsOnly && !children.Any())
                    nodes.Add(node);
                if (!leafsOnly)
                    nodes.Add(node);
                ancestorsByNode.Add(node, ancestors.ToArray());
                ancestors.Push(node);
            }
            foreach (var child in children)
                CollectNodes(child, getChildren, nodes, ancestors, ancestorsByNode, true, leafsOnly);
            if (ancestors.Any())
                ancestors.Pop();
        }
    }
}
