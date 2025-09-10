using System;
using System.Collections.Generic;
using System.Linq;
using Goober.Base.Extensions;
using Goober.DependencyInjection.Extensions.Contracted;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.DependencyInjection
{
	public static class ServiceProviderStatic
	{
		internal static IServiceProvider ServiceProvider;

		public static IConfiguration Configuration { get; internal set; }

		/// <summary>
		/// Флаг наличия внутреннего сервис-провайдера.
		/// </summary>
		public static bool HasServiceProvider => ServiceProvider != null;

		/// <summary>
		/// Начать конфигурацию.
		/// Этот метод необходим, когда на старте приложения в среде выполнения не создается коллекция сервисов.
		/// Например, в .NET Framework-приложениях.
		/// </summary>
		/// <returns></returns>
		public static IServiceCollection StartConfigure(IServiceCollection services = null)
		{
			return services ?? new ServiceCollection();
		}

        ///// <summary>
        ///// Попытка установить внутренний сервис-провайдер, если такого еще не установлен.
        ///// </summary>
        ///// <param name="serviceProvider"></param>
        //public static bool SetServiceScopeFactory(IServiceProvider serviceProvider)
        //{
        //	if (ServiceProvider != null)
        //		return false;

        //	ServiceProvider = serviceProvider;
        //	return true;
        //}

        /// <summary>
        /// Возвращает инстанс для указанного сервисного типа
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="contractName">Наименование контракта, к котором привязано инстанцирование</param>
        /// <returns>Если инстанс-тип не найдет, вызывает исключение.</returns>
        /// <exception cref="InvalidOperationException">В случае отсутствия инстанс-типа</exception>
        public static object GetRequiredService(Type serviceType, string contractName = null)
        {
            return ServiceProvider.GetRequiredService(serviceType, contractName);
        }

        /// <summary>
        /// Возвращает инстанс для указанного сервисного типа, используя статичный <see cref="IServiceProvider"/> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contractName">Наименование контракта, к котором привязано инстанцирование</param>
        /// <returns>Если инстанс-тип не найдет, вызывает исключение.</returns>
        /// <exception cref="InvalidOperationException">В случае отсутствия инстанс-типа</exception>
        public static T GetRequiredService<T>(string contractName = null)
		{
			return ServiceProvider.GetRequiredService<T>(contractName);
		}

		public static IEnumerable<string> GetContractNames<T>()
		{
			return ServiceProvider.GetContractNames<T>();
		}

        /// <summary>
        /// Возвращает инстанс для указанного сервисного типа
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="contractName">Наименование контракта, к котором привязано инстанцирование</param>
        /// <returns>Если инстанс-тип не найдет, возращает null.</returns>
        public static object GetService(Type serviceType, string contractName = null)
		{
			return ServiceProvider.GetService(serviceType, contractName);
		}

        /// <summary>
        /// Возвращает инстанс для указанного сервисного типа, используя внутренний <see cref="IServiceProvider"/> 
        /// </summary>
		/// <typeparam name="T"></typeparam>
        /// <param name="contractName">Наименование контракта, к котором привязано инстанцирование</param>
        /// <returns>Если инстанс-тип не найдет, возращает null.</returns>
        public static T GetService<T>(string contractName = null)
		{
			return ServiceProvider.GetService<T>(contractName);
		}

		public static IEnumerable<T> GetServices<T>(string contractName = null)
		{
			return ServiceProvider.GetServices<T>(contractName);
		}

        public static IEnumerable<object> GetServices(Type serviceType)
        {
            return ServiceProvider.GetServices(serviceType);
        }
    }
}