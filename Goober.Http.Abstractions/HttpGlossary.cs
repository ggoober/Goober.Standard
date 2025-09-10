namespace Goober.Http.Abstractions
{
	public static class HttpGlossary
	{
		public const long MaxResponseContentLength = 300 * 1024 * 1024;

		public const int TimeoutInMilliseconds = 120000;

		public const string PATCH = "PATCH";
		
		public const string X_FORWARDED_FOR = "X-Forwarded-For";

		public const string X_FORWARDED_PROTO = "X-Forwarded-Proto";

		public const string X_FORWARDED_HOST = "X-Forwarded-Host";

		public const string X_ORIGINAL_FOR = "X-Original-For";

		public const string X_ORIGINAL_PROTO = "X-Original-Proto";

		public const string X_ORIGINAL_HOST = "X-Original-Host";
	}
}
