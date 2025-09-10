using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Goober.Base.Extensions
{
	public static class RegexExtensions
	{
		public static IEnumerable<string> ToStrings(this Group groups)
		{
			return groups
				.Captures
				.Cast<Capture>()
				.Select(s => s.Value);
		}
		public static IEnumerable<string> ToStrings(this Match match, string groupName)
		{
			return match.Groups[groupName]
				.Captures
				.Cast<Capture>()
				.Select(s => s.Value);
		}
	}
}