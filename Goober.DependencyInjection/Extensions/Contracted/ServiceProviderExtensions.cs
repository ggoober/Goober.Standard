using System;
using System.Collections.Generic;
using System.Linq;
using Goober.Base.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.DependencyInjection.Extensions.Contracted
{
	public static class ServiceProviderExtensions
	{
		/// <summary>
		/// Возвращает инстанс для указанного сервисного типа
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="contractName">Наименование контракта, к котором привязано инстанцирование</param>
		/// <returns>Если инстанс-тип не найдет, возращает null.</returns>
		public static T GetService<T>(this IServiceProvider serviceProvider, string contractName)
		{
			return (T)serviceProvider.GetService(typeof(T), contractName);
		}

		/// <summary>
		/// Возвращает инстанс для указанного сервисного типа
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="contractName">Наименование контракта, к котором привязано инстанцирование</param>
		/// <returns>Если инстанс-тип не найдет, возращает null.</returns>
		public static object GetService(this IServiceProvider serviceProvider, Type serviceType, string contractName)
		{
			serviceProvider = serviceProvider.GetOrStatic();
			return serviceProvider.GetService(GetContractedType(serviceProvider, serviceType, contractName));
		}

		/// <summary>
		/// Возвращает инстанс для указанного сервисного типа
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="contractName">Наименование контракта, к котором привязано инстанцирование</param>
		/// <returns>Если инстанс-тип не найдет, вызывает исключение.</returns>
		/// <exception cref="InvalidOperationException">В случае отсутствия инстанс-типа</exception>
		public static T GetRequiredService<T>(this IServiceProvider serviceProvider, string contractName)
		{
			return (T)serviceProvider.GetRequiredService(typeof(T), contractName);
		}

		/// <summary>
		/// Возвращает инстанс для указанного сервисного типа
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="contractName">Наименование контракта, к котором привязано инстанцирование</param>
		/// <returns>Если инстанс-тип не найдет, вызывает исключение.</returns>
		/// <exception cref="InvalidOperationException">В случае отсутствия инстанс-типа</exception>
		public static object GetRequiredService(this IServiceProvider serviceProvider, Type serviceType, string contractName)
		{
			serviceProvider = serviceProvider.GetOrStatic();
			return serviceProvider.GetRequiredService(GetContractedType(serviceProvider, serviceType, contractName));
		}

		public static IEnumerable<string> GetContractNames<T>(this IServiceProvider serviceProvider)
		{
			serviceProvider = serviceProvider.GetOrStatic();
			return serviceProvider
				.GetContractedServicesMapper()
				.GetContractNames(typeof(T));
		}

		/// <summary>
		/// Возвращает инстансы для указанного сервисного типа
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="contractName">Наименование контракта, к котором привязано инстанцирование. Если пустой, то возвращает все инстансы.</param>
		public static IEnumerable<T> GetServices<T>(this IServiceProvider serviceProvider, string contractName)
		{
			serviceProvider = serviceProvider.GetOrStatic();

			if (string.IsNullOrWhiteSpace(contractName))
			{
				var contractedServices = serviceProvider
					.GetContractedServicesMapper()
					.GetContractNames(typeof(T))
					.Select(s => serviceProvider.GetService<T>(s));
				return contractedServices;
			}
			return serviceProvider
				.GetServices(GetContractedType(serviceProvider, typeof(T), contractName))
				.OfType<T>();
		}

		internal static ContractedServiceMapper GetContractedServicesMapper(this IServiceProvider serviceProvider, bool isRequired = true)
		{
			var contractedServicesStore
				= serviceProvider.GetService<ContractedServiceMapper>();

			if (contractedServicesStore == null && isRequired)
				throw new InvalidOperationException(
					$"ContractedServices not initialized. For using this middleware run services.{nameof(ServiceCollectionExtensions.AddContractedServices)}() first.");

			return contractedServicesStore;
		}
		

		internal static IServiceProvider GetOrStatic(this IServiceProvider serviceProvider)
		{
			return serviceProvider ?? ServiceProviderStatic.ServiceProvider;
		}

		private static Type GetContractedType(IServiceProvider serviceProvider, Type typeService,
			string contractName)
		{
			string contractNameResolved = contractName;
			if (string.IsNullOrEmpty(contractName))
				contractNameResolved = serviceProvider
					.GetContractedServicesMapper(false)
					?.GetDefaultContract(typeService) ?? contractName;

			Type resultType = typeService, contractedTypeService;

			if (string.IsNullOrEmpty(contractNameResolved) == false)
			{
				var contractedServicesMapper = serviceProvider.GetContractedServicesMapper();
				var loopTypeService = typeService;
				while (true)
				{
					if (contractedServicesMapper.TryGetTypeMap(loopTypeService, contractNameResolved, out contractedTypeService))
					{
						resultType = contractedTypeService;
						if (resultType.IsGenericTypeDefinition)
							resultType = resultType.MakeGenericType(typeService.GenericTypeArguments);
						break;
					}
					if (loopTypeService.IsGenericType)
						if (loopTypeService.IsGenericTypeDefinition == false)
						{
							loopTypeService = loopTypeService.GetGenericTypeDefinition();
							continue;
						}
					break;
				}
				
				if (resultType == null || string.IsNullOrEmpty(contractName) && contractedTypeService == null)
					throw new InvalidOperationException(
						$"Не найден инстанс сервисного типа {typeService.Name} для контракта '{contractNameResolved}'");
			}

			return resultType;
		}
	}
}