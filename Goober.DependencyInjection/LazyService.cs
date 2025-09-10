using System;
using Goober.DependencyInjection.Extensions.Contracted;

namespace Goober.DependencyInjection
{
	public class LazyRequiredService<T> : Lazy<T> where T : class
	{
		public LazyRequiredService(IServiceProvider provider)
			: base(() => provider.GetRequiredService<T>(string.Empty)) { }
	}

	public class LazyService<T> : Lazy<T> where T : class
	{
		public LazyService(IServiceProvider provider, string contractName)
			: base(() => provider.GetService<T>(contractName)) { }
	}
}