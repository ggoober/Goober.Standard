using System;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.DependencyInjection.Attributes
{

	/// <summary>
	/// Тип будет добавлен в коллекцию сервисов явным образом
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ExportServiceAttribute : Attribute
	{
		public Type InterfaceType { get; }
		public ServiceLifetime Lifetime { get; }
		public string ContractName { get; set; }

		public ExportServiceAttribute(Type interfaceType, ServiceLifetime lifetime = ServiceLifetime.Scoped, string contractName = null)
		{
			InterfaceType = interfaceType;
			ContractName = contractName;
			Lifetime = lifetime;
		}

		public ExportServiceAttribute(Type interfaceType, string contractName, ServiceLifetime lifetime = ServiceLifetime.Scoped)
		{
			InterfaceType = interfaceType;
			ContractName = contractName;
			Lifetime = lifetime;
		}
	}
}