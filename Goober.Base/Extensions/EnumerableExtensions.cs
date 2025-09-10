using System;
using System.Collections.Generic;
using System.Linq;

namespace Goober.Base.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> SplitByParts<T>(this IEnumerable<T> list, int parts)
        {
            return list.Select((item, index) => new { index, item })
                       .GroupBy(x => x.index % parts)
                       .Select(x => x.Select(y => y.item));
        }

        public static IEnumerable<List<TSource>> SplitByCount<TSource>(this IEnumerable<TSource> list, int chunkCount)
        {
            if (list == null)
            {
                yield break;
            }

            if (chunkCount == 0)
            {
                throw new NotImplementedException();
            }

            var buffer = new List<TSource>(chunkCount);
            foreach (var source in list)
            {
                if (buffer.Count == chunkCount)
                {
                    yield return buffer;
                    buffer = new List<TSource>(chunkCount);
                }

                buffer.Add(source);
            }

            if (buffer.Count != 0)
            {
                yield return buffer;
            }
        }

        /// <summary>
        /// Разделить коллекцию на два массива по условию
        /// </summary>
        /// <param name="source">Исходная коллекция</param>
        /// <param name="predicate">Условие, по которому разделять значения</param>
        /// <param name="resultTrue">Значения, удовлетворяющие условию</param>
        /// <param name="resultFalse">Значения, не удовлетворяющие условию</param>
        public static void SplitByCondition<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate,
            out List<TSource> resultTrue,
            out List<TSource> resultFalse)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            resultTrue = new List<TSource>();
            resultFalse = new List<TSource>();
            foreach (var sourceElement in source)
            {
                if (predicate(sourceElement))
                {
                    resultTrue.Add(sourceElement);
                }
                else
                {
                    resultFalse.Add(sourceElement);
                }
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> items)
        {
            return items != null && items.Any();
        }

        public static bool IsOneOf<T>(this T value, params T[] items)
        {
            return items.Any(o => o.Equals(value));
        }

        public static bool In<TSource>(this TSource instance, params TSource[] set)
        {
            return set.Contains(instance);
        }

        public static bool NotIn<TSource>(this TSource instance, params TSource[] set)
        {
            return !instance.In(set);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
        {
	        var result = new HashSet<T>();
	        foreach (var item in collection) 
		        result.Add(item);
	        return result;
        }

        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> collection, Func<T, V> selectKeyFunc)
        {
	        if (selectKeyFunc == null) 
		        throw new ArgumentNullException(nameof(selectKeyFunc));

	        var set = new HashSet<V>();
	        foreach (var item in collection)
	        {
		        var key = selectKeyFunc(item);
		        if (set.Contains(key) == false)
		        {
			        set.Add(key);
			        yield return item;
		        }
	        }
        }

        /// <summary>
        /// Преобразовать набор пар ключ-значение в словарь
        /// </summary>
        /// <param name="source">Исходная коллекция ключ-значение</param>
        /// <param name="overwriteDuplicates">
        /// Способ выбора значения для словаря, если в исходной коллекции есть значения с дублирующимися ключами.
        /// <see langword="false"/> - выбирать первое значение (по умолчанию)
        /// <see langword="true"/> - выбирать последнее значение
        /// </param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, bool overwriteDuplicates = false)
        {
            var duplicatesFilteredSource = overwriteDuplicates
                ? source.GroupBy(pair => pair.Key).Select(group => group.Last())
                : source.Distinct(pair => pair.Key);

            return duplicatesFilteredSource
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
