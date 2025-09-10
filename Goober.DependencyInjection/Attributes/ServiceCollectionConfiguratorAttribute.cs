using System;
using Goober.DependencyInjection.Extensions;
using Goober.DependencyInjection.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.DependencyInjection.Attributes
{
	/// <summary>
	/// Класс, помеченный этим атрибутом и реализующий <see cref="IServiceCollectionConfigurator"/>,
	/// инстанцируется при формировании коллекции сервисов <see cref="IServiceCollection"/>
	/// и вызывается его метод <see cref="IServiceCollectionConfigurator.Configure"/>
	/// <para>Вызывается в методах:</para>
	/// <para><see cref="ServiceCollectionExtensions.RegisterModules"/></para>
	/// <para><see cref="ServiceCollectionExtensions.RegisterClasses"/></para>
	/// <para><see cref="ServiceCollectionExtensions.RegisterAssemblyClasses{TAssemblyMember}"/></para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class ServiceCollectionConfiguratorAttribute : Attribute
	{
	}
}