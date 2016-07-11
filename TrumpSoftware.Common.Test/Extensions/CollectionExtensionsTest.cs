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
            IList<double> list = Enumerable.Range(0, 10).Select(x => RandomHelper.GetDouble(0.0, 1.0))
                                                 .ToList();
            list.Sort<double>();
            for (int i = 0; i < list.Count-1; i++)
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
            IList<double> list = Enumerable.Range(0, 10).Select(x => RandomHelper.GetDouble(0.0, 1.0))
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
    }
}