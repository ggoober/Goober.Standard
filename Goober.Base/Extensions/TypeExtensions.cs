using System;
using System.Reflection;

namespace Goober.Base.Extensions
{
	public static class TypeExtensions
	{
		public static Tattr GetCustomAttribute<Tattr>(this Type type)
			where Tattr : Attribute
		{
			return type.GetCustomAttribute(typeof(Tattr)) as Tattr;
		}
	}
}