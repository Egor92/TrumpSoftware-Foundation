using System;
using System.Collections.Generic;
using System.Linq;

namespace TrumpSoftware.Common
{
	public static class EnumerableExtentions
	{
		public static IEnumerable<T> GetRandom<T>(this IEnumerable<T> enumerable, int count)
		{
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            if (count < 0)
                throw new ArgumentException("Count must be not negative", "count");
            if (enumerable.Count() > count)
                throw new ArgumentException("Requested count is greater than existing", "enumerable");
            var list = enumerable.ToList();
			var result = new List<T>();
			for (int i = 0; i < count; i++)
			{
				int number = RandomHelper.GetInt(list.Count);
				result.Add(list[number]);
				list.RemoveAt(number);
			}
			return result;
		}

        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            if (!enumerable.Any())
                throw new ArgumentException("Enumerable is empty", "enumerable");
            return enumerable.GetRandom(1).Single();
        }

		public static IEnumerable<T> Mix<T>(this IEnumerable<T> originalArray)
		{
		    if (originalArray == null)
		        throw new ArgumentNullException("originalArray");

		    var randDictionary = new Dictionary<int, T>();
		    int count = originalArray.Count();
		    for (int i = 0; i < count; i++)
		    {
		        T t = originalArray.ElementAt(i);
		        randDictionary.Add(RandomHelper.GetInt(0, int.MaxValue), t);
		    }

		    return randDictionary.OrderBy(x => x.Key).Select(x => x.Value).ToArray();
		}
	}
}
