using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrumpSoftware.Common.Helpers;

namespace TrumpSoftware.Common.Extensions
{
	public static class EnumerableExtentions
	{
		public static IEnumerable<T> GetRandom<T>(this IEnumerable<T> source, int count)
		{
            if (source == null)
                throw new ArgumentNullException("source");
            if (count < 0)
                throw new ArgumentException("Count must be not negative", "count");
            if (source.Count() < count)
                throw new ArgumentException("Requested count is greater than existing", "source");
            var list = source.ToList();
			var result = new List<T>();
			for (int i = 0; i < count; i++)
			{
				int number = RandomHelper.GetInt(list.Count);
				result.Add(list[number]);
				list.RemoveAt(number);
			}
			return result;
		}

        public static T GetRandom<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (!source.Any())
                throw new ArgumentException("Enumerable is empty", "source");
            return source.GetRandom(1).Single();
        }

		public static IEnumerable<T> Mix<T>(this IEnumerable<T> source)
		{
		    if (source == null)
		        throw new ArgumentNullException("source");

		    var randDictionary = new Dictionary<int, T>();
		    int count = source.Count();
		    for (int i = 0; i < count; i++)
		    {
		        T t = source.ElementAt(i);
		        randDictionary.Add(RandomHelper.GetInt(0, int.MaxValue), t);
		    }

		    return randDictionary.OrderBy(x => x.Key).Select(x => x.Value).ToArray();
		}

	    public static IEnumerable<T> GetSelfOrEmpty<T>(this IEnumerable<T> source)
	    {
	        return source ?? Enumerable.Empty<T>();
	    }

	    public static int GetCountOrZero(this IEnumerable source)
	    {
	        return source.OfType<object>().GetSelfOrEmpty().Count();
	    }
	}
}
