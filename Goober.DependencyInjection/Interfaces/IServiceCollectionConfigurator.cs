using Goober.DependencyInjection.Attributes;
using Goober.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.DependencyInjection.Interfaces
{
	/// <summary>
	/// Класс, реализующий этот интерфейс и помеченный атрибутом <see cref="ServiceCollectionConfiguratorAttribute"/>
	/// инстанцируется при формировании коллекции сервисов <see cref="IServiceCollection"/>
	/// и вызывается его метод <see cref="Configure"/>
	/// <para><see cref="ServiceCollectionExtensions.RegisterModules"/></para>
	/// <para><see cref="ServiceCollectionExtensions.RegisterClasses"/></para>
	/// <para><see cref="ServiceCollectionExtensions.RegisterAssemblyClasses{TAssemblyMember}"/></para>
	/// </summary>
	public interface IServiceCollectionConfigurator
	{
		void Configure(IServiceCollection services, IConfiguration configuration = null);
	}
}