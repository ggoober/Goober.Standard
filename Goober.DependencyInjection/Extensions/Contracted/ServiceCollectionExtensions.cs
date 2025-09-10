using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Goober.Base.Extensions;
using Goober.DependencyInjection.Attributes;
using Goober.DependencyInjection.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.DependencyInjection.Extensions.Contracted
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddContractedServices(this IServiceCollection services)
		{
			if (services.All(s=>s.ServiceType != typeof(ContractedServiceMapper)))
				services.AddSingleton(new ContractedServiceMapper());
			return services;
		}

		/// <summary>
		/// Добавить сервис с привязкой к определенному контракту.
		/// К одному сервисному типу может быть привязано несколько инстанс-типов, по одному на каждый контракт.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="contractName">Наименование контракта</param>
		/// <param name="serviceLifetime">Время жизни инстанса.</param>
		public static IServiceCollection AddContractedService<TService, TImplementation>(this IServiceCollection services
			, string contractName
			, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
		{
			services.AddContractedService(
				contractName, 
				typeof(TService), 
				typeof(TImplementation), 
				serviceLifetime);
			return services;
		}

		/// <summary>
		/// Добавить сервис с привязкой к определенному контракту.
		/// К одному сервисному типу может быть привязано несколько инстанс-типов, по одному на каждый контракт.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="contractName">Наименование контракта</param>
		/// <param name="serviceType">Сервисный тип</param>
		/// <param name="instanceType">Инстанс-тип, реализующий непосредственно службу.</param>
		/// <param name="serviceLifetime">Время жизни инстанса.</param>
		public static IServiceCollection AddContractedService(this IServiceCollection services
			, string contractName
			, Type serviceType
			, Type instanceType
			, ServiceLifetime serviceLifetime)
		{
			var typeResolvingDict = services.GetContractedServicesMapper().TypeResolvingDictionary
				.GetOrAddDefault(serviceType,
					() => new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase));
			typeResolvingDict.AddOrSet(contractName, instanceType);
			services.Add(new ServiceDescriptor(instanceType, instanceType, serviceLifetime));
			return services;
		}

		/// <summary>
		/// Добавить делегат разрешения имени контракта для сервисного типа.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resolveFunc"></param>
		public static IServiceCollection AddContractNameResolver<T>(this IServiceCollection services, Func<Type, string> resolveFunc)
		{
			return services.AddContractNameResolver(typeof(T), resolveFunc);
		}

		/// <summary>
		/// Добавить делегат разрешения имени контракта для сервисного типа.
		/// </summary>
		/// <param name="serviceType">Сервисный тип (интерфейс).</param>
		/// <param name="resolveFunc">При наличии делегата, привязанного к типу, они не суммируются, а заменяется последним.</param>
		/// <exception cref="NotSupportedException"></exception>
		public static IServiceCollection AddContractNameResolver(this IServiceCollection services, Type serviceType, Func<Type, string> resolveFunc)
		{
			if (serviceType.IsGenericTypeDefinition)
				throw new NotSupportedException("Not support name contract resolving for open generic services");
			services.GetContractedServicesMapper()?
				.AddContractNameResolver(serviceType, resolveFunc);
			services.AddSingleton(serviceType, sp => sp.GetService(serviceType, null));
			return services;
		}

		internal static ContractedServiceMapper GetContractedServicesMapper(this IServiceCollection services)
		{
			var contractedServicesStore
				= services.FirstOrDefault(serviceDesc => serviceDesc.ServiceType == typeof(ContractedServiceMapper))?
					.ImplementationInstance as ContractedServiceMapper;

			if (contractedServicesStore == null)
				throw new InvalidOperationException(
					$"ContractedServices not initialized. For using this middleware run services.{nameof(AddContractedServices)}() first.");

			return contractedServicesStore;
		}
	}
}