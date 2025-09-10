using System;
using System.Collections.Generic;
using System.Linq;

namespace Goober.Base.Extensions
{
	public static class DictionaryExtensions
	{
		public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TValue, bool> valuePred, out TValue value)
		{
			if (valuePred != null)
				foreach (var dictValue in dict.Values.Where(valuePred))
				{
					value = dictValue;
					return true;
				}
			value = default(TValue);
			return false;
		}

		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> coll)
		{
			if (dict != null && coll != null)
				foreach (var item in coll)
					dict.Add(item.Key, item.Value);
		}

		public static TValue AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value, Func<TValue, TValue> modifyCurrentValue = null)
		{
			if (!dict.TryGetValue(key, out var res))
				dict.Add(key, value);
			else
			{
				res = modifyCurrentValue != null 
					? modifyCurrentValue(res) 
					: value;
				dict[key] = res;
			}
			return res;
		}

		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
			where TValue : new()
		{
			TValue res;
			if (!dict.TryGetValue(key, out res))
				dict.Add(key, res = new TValue());
			return res;
		}

		public static TValue GetOrAddDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defal)
		{
			TValue res = defal;
			if (!dict.TryGetValue(key, out res))
				dict.Add(key, res = defal);
			return res;
		}
		public static TValue GetOrAddDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> getValueFunc)
		{
			TValue res = default(TValue); ;
			if (!dict.TryGetValue(key, out res) && getValueFunc != null)
				dict.Add(key, res = getValueFunc());
			return res;
		}

		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
		{
			TValue res;
			dict.TryGetValue(key, out res);
			return res;
		}

		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defal)
		{
			TValue res = default(TValue);
			if (!dict.TryGetValue(key, out res))
				res = defal;
			return res;
		}

		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> getValueFunc)
		{
			TValue res = default(TValue);
			if (!dict.TryGetValue(key, out res) && getValueFunc != null)
				res = getValueFunc();
			return res;
		}

		public static TValue GetOrAddDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
		{
			TValue res = default(TValue);
			if (!dict.TryGetValue(key, out res))
				dict.Add(key, res);
			return res;
		}
	}
}