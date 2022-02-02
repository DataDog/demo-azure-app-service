namespace OrchardCore.Sample
{
	public static class ConfigHelper
	{
		private static string _datadogApiKey;
		private static string _datadogEnv;
		private static string _datadogService;
		private static string _datadogVersion;
		private static string _datadogRumApplicationId;
		private static string _datadogRumClientToken;

		public static string DatadogApiKey => GetConfig("DD_API_KEY", ref _datadogApiKey);

		public static string DatadogEnv => GetConfig("DD_ENV", ref _datadogEnv);

		public static string DatadogService => GetConfig("DD_SERVICE", ref _datadogService);

		public static string DatadogVersion => GetConfig("DD_VERSION", ref _datadogVersion);

		public static string DatadogRumApplicationId => GetConfig("DD_APPLICATION_ID", ref _datadogRumApplicationId);

		public static string DatadogRumClientToken => GetConfig("DD_CLIENT_TOKEN", ref _datadogRumClientToken);

		public static bool AllDatadogVariablesSet =>
			DatadogApiKey != null &&
			DatadogEnv != null &&
			DatadogService != null &&
			DatadogVersion != null &&
			DatadogRumApplicationId != null &&
			DatadogRumClientToken != null;

		private static string GetConfig(string key, ref string value)
		{
			if (value == null)
			{
				value = System.Environment.GetEnvironmentVariable(key);
			}

			return value;
		}
	}
}