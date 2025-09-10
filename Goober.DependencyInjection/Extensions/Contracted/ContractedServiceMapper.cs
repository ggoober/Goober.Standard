using System;
using System.Collections.Generic;
using Goober.Base.Extensions;

namespace Goober.DependencyInjection.Extensions.Contracted
{
	internal class ContractedServiceMapper
	{
		public Dictionary<Type, Dictionary<string, Type>> TypeResolvingDictionary { get; }
			= new();

		public Dictionary<Type, Func<Type, string>> DefaultContractResolver = new();


		public string GetDefaultContract(Type serviceType)
		{
			return DefaultContractResolver.TryGetValue(serviceType, out var mapFunc) 
				? mapFunc?.Invoke(serviceType) 
				: string.Empty;
		}

		public IEnumerable<string> GetContractNames(Type serviceType)
		{
			if (TypeResolvingDictionary.TryGetValue(serviceType, out var dict))
				return dict.Keys;
			return Array.Empty<string>();
		}

		public bool TryGetTypeMap(Type type, string contractName, out Type implementType)
		{
			implementType = null;
			return TypeResolvingDictionary.TryGetValue(type, out var dict) &&
			       dict.TryGetValue(contractName, out implementType);
		}

		public void AddContractNameResolver(Type serviceType, Func<Type, string> resolveFunc)
		{
			DefaultContractResolver.AddOrSet(serviceType, resolveFunc);
		}
	}
}