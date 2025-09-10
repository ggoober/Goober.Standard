using Goober.Base.Services;
using Goober.Base.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.Base.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddIndusoftDateTimeService(this IServiceCollection services)
		{
			services.AddSingleton<IDateTimeService, DateTimeService>();
		}

		public static IServiceCollection AddEasySettings(this IServiceCollection services)
		{
			services.AddTransient(typeof(ISettings<>), typeof(Settings<>));
			return services;
		}
	}
}