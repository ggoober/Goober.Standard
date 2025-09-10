using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Goober.Base.Extensions;
using Goober.DependencyInjection.Attributes;
using Goober.DependencyInjection.Extensions.Contracted;
using Goober.DependencyInjection.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.DependencyInjection.Extensions
{
	public static class ServiceCollectionExtensions
	{
		internal static readonly List<string> ServiceAndRepositoryPostfix = new() { "Service", "Repository" };

		/// <summary>
		/// Закончить настройку коллекции сервисов и создать сервис-провайдер <see cref="IServiceProvider"/>.
		/// После этого метода изменения, внесенные в коллекцию сервисов, НЕ вносятся в сервис-провайдер.
		/// </summary>
		/// <param name="services"></param>
		public static IServiceCollection FinishConfigure(this IServiceCollection services)
		{
			ServiceProviderStatic.ServiceProvider = services.BuildServiceProvider();
			return services;
		}
		
		public static IServiceCollection SetConfiguration(this IServiceCollection services,
			IConfiguration configuration)
		{
			ServiceProviderStatic.Configuration = configuration;
			return services;
		}

		/// <summary>
		/// Добавить сервисы ленивой инициализации
		/// </summary>
		/// <param name="services"></param>
		/// <param name="isRequiredServicesByDefault">Гарантированость возвращаемого результата.
		/// Если его нет, вызывается исключение.</param>
		/// <returns></returns>
		public static IServiceCollection AddLazyServices(this IServiceCollection services, bool isRequiredServicesByDefault = true)
		{
			return isRequiredServicesByDefault
				? services.AddTransient(typeof(Lazy<>), typeof(LazyRequiredService<>))
				: services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));
		}

		/// <summary>
		/// Загрузить и зарегистрировать сборки по указанной маске файла
		/// </summary>
		/// <param name="services"></param>
		/// <param name="maskedPath">Поддерживается wildcard и подкаталоги. 
		/// Например: "Modules\*.dll" - загрузит и зарегистрирует типы из сборок c расширением .dll из подкаталога Modules</param>
		/// <returns></returns>
		public static IServiceCollection RegisterModules(this IServiceCollection services, string maskedPath = "*.dll")
		{
			if (!maskedPath.IsNullOrWhitespace())
			{
				var dir = Path.GetDirectoryName(maskedPath);
				dir = dir.IsNullOrWhitespace()
					? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
					: Path.GetFullPath(dir);

				AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
				{
					var filePath = Path.Combine(dir, args.Name.Split(',')[0] + ".dll");
					return File.Exists(filePath)
						? Assembly.LoadFrom(filePath)
						: null;
				};

				var fileMask = Path.GetFileName(maskedPath);
				var files = Directory.GetFiles(dir, fileMask);
				Debug.WriteLine($"Register for {files.Length} files in '{dir}'");

				var assemblies = AppDomain.CurrentDomain
					.GetAssemblies()
					.Select(a =>
					{
						try
						{
							return new Tuple<string, Assembly>(a.Location, a);
						}
						catch (Exception e)
						{
							return new Tuple<string, Assembly>(Guid.NewGuid().ToString(), a);
						}
					})
					.Distinct(a => a.Item1)
					.ToDictionary(a => a.Item1, a => a.Item2);

				Assembly assembly;
				foreach (var fileName in files)
				{
					if (assemblies.TryGetValue(Path.GetFileName(fileName), out assembly) == false)
						try
						{
							assembly = Assembly.LoadFrom(fileName);
						}
						catch
						{
							continue;
						}

					services.RegisterClasses(
						assembly: assembly
						, classesPostfix: ServiceAndRepositoryPostfix
						, serviceLifetime: ServiceLifetime.Scoped
						, optional: false);
				}
			}

			return services;
		}


		public static IServiceCollection RegisterClasses(this IServiceCollection services,
			Assembly assembly,
			List<string> classesPostfix,
			ServiceLifetime serviceLifetime,
			bool optional)
		{
			if ((classesPostfix?.Count ?? 0) == 0)
				throw new InvalidOperationException();

			var queryTypes = assembly
				.GetTypes()
				.Where(type => type.IsClass && !type.IsAbstract
				                            && !type.IsDefined(typeof(ServiceCollectionIgnoreRegistrationAttribute)))
				.ToList();

			if (queryTypes.Count > 0)
			{
				RegisterByConfigurator(services, queryTypes);
				RegisterWithExportAttribute(services, queryTypes);
				RegisterPostfixed(services, serviceLifetime, optional, queryTypes, classesPostfix);
			}

			return services;
		}

		public static IServiceCollection RegisterAssemblyClasses<TAssemblyMember>(this IServiceCollection services,
			List<string> classesPostfix = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
			bool optional = false)
		{
			return RegisterClasses(services: services,
				assembly: typeof(TAssemblyMember).Assembly,
				classesPostfix: classesPostfix ?? ServiceAndRepositoryPostfix,
				serviceLifetime: serviceLifetime,
				optional: optional);
		}

		private static void RegisterByConfigurator(IServiceCollection services, IEnumerable<Type> types)
		{
			var serviceConfigurators = types
				.Where(type => !type.IsAbstract && typeof(IServiceCollectionConfigurator).IsAssignableFrom(type)
				                                && type.IsDefined(typeof(ServiceCollectionConfiguratorAttribute)))
				.Select(type => (IServiceCollectionConfigurator)Activator.CreateInstance(type))
				.ToList();

			foreach (var serviceVisitor in serviceConfigurators)
				serviceVisitor.Configure(services, ServiceProviderStatic.Configuration);
		}

		/// <summary>
		/// Регистрация типов с атрибутом <see cref="ExportServiceAttribute"/>
		/// </summary>
		/// <param name="services"></param>
		/// <param name="types">Из списка удаляются успешно зарегистрированные типы</param>
		private static void RegisterWithExportAttribute(IServiceCollection services, IEnumerable<Type> types)
		{
			var explicitTypes = types
				.Select(type => new
				{
					Type = type,
					Attributes = type.GetCustomAttributes<ExportServiceAttribute>().ToArray()
				})
				.Where(typeTuple => typeTuple.Attributes.Any())
				.ToHashSet();

			foreach (var explicitTypeTuple in explicitTypes)
			foreach (var attribute in explicitTypeTuple.Attributes)
			{
				if (attribute.ContractName.IsNullOrEmpty())
					services.Add(new ServiceDescriptor(
						serviceType: attribute.InterfaceType
						, implementationType: explicitTypeTuple.Type
						, lifetime: attribute.Lifetime));
				else
					services.AddContractedService(
						attribute.ContractName
						, attribute.InterfaceType
						, explicitTypeTuple.Type
						, attribute.Lifetime);
			}
		}

		/// <summary>
		/// Регистрация типов с постфиксом из заданного списка
		/// </summary>
		/// <param name="services"></param>
		/// <param name="serviceLifetime"></param>
		/// <param name="optional"></param>
		/// <param name="types">Из списка удаляются успешно зарегистрированные типы</param>
		/// <param name="classesPostfix"></param>
		private static void RegisterPostfixed(IServiceCollection services
			, ServiceLifetime serviceLifetime, bool optional, IEnumerable<Type> types, List<string> classesPostfix)
		{
			var postfixTypes = types
				.Where(type => classesPostfix.Any(x => type.Name.EndsWith(x)))
				.ToHashSet();

			foreach (var implementType in postfixTypes)
			{
				var interfaceName = "I" + implementType.Name;
				var interfaceType = implementType.GetInterface(interfaceName);

				if (interfaceType == null)
					continue;

				services.Add(new ServiceDescriptor(interfaceType, implementType, serviceLifetime));
			}
		}
	}
}