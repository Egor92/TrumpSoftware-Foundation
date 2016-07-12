using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrumpSoftware.Common.Extensions;
using TrumpSoftware.Common.Helpers;

namespace TrumpSoftware.Common.Test.Extensions
{
    [TestClass]
    public class CollectionExtensionsTest
    {
        [TestMethod]
        public void CanSortWithoutComparer()
        {
            IList<double> list = Enumerable.Range(0, 10)
                                           .Select(x => RandomHelper.GetDouble(0.0, 1.0))
                                           .ToList();
            list.Sort<double>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] > list[i + 1])
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void CanSortWithComparer()
        {
            IList<double> list = Enumerable.Range(0, 10)
                                           .Select(x => RandomHelper.GetDouble(0.0, 1.0))
                                           .ToList();
            var comparer = new CustomComparer<double>(x => -x);
            list.Sort<double>(comparer);
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (comparer.Compare(list[i], list[i + 1]) > 0)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void CanAddRange()
        {
            const int startCount = 10;
            List<int> list = Enumerable.Range(0, startCount).ToList();

            const int newItemsCount = 5;
            IEnumerable<int> newItems = Enumerable.Range(10, newItemsCount);

            list.AddRange(newItems);

            const int finalCount = startCount + newItemsCount;
            Assert.AreEqual(finalCount, list.Count);
            foreach (var newItem in newItems)
            {
                CollectionAssert.Contains(list, newItem);
            }
        }

        [TestMethod]
        public void CanRemoveRange()
        {
            const int startCount = 10;
            List<int> list = Enumerable.Range(0, startCount).ToList();

            const int oldItemsCount = 5;
            IList<int> oldItems = Enumerable.Range(0, oldItemsCount)
                                            .Select(x => 2*x)
                                            .ToList();

            list.RemoveRange(oldItems);

            const int finalCount = startCount - oldItemsCount;
            Assert.AreEqual(finalCount, list.Count);
            foreach (var newItem in oldItems)
            {
                CollectionAssert.DoesNotContain(list, newItem);
            }
        }

        [TestMethod]
        public void CanRemoveIf()
        {
            const int startCount = 10;
            IList<int> list = Enumerable.Range(0, startCount).ToList();

            var condition = new Func<int, bool>(x => x%3 == 0);

            var leftItemsCount = list.Count(condition);
            var keptItemsCount = list.Count - leftItemsCount;

            list.RemoveIf(condition);

            Assert.AreEqual(keptItemsCount, list.Count);
        }
    }
}