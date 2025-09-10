using System;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.Standard.Tests.Tools
{
	public class TestTools
	{
		public static TestScope GetTestScope(Action<IServiceCollection> servicesAction = null)
		{
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddLogging();

			servicesAction?.Invoke(serviceCollection);

			var testServiceProvider = serviceCollection.BuildServiceProvider();

			return new TestScope(testServiceProvider);
		}
	}
}
