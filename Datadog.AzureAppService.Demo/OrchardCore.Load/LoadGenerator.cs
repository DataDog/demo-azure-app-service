using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using StatsdClient;

namespace OrchardCore.Load
{
	public static class LoadGenerator
	{
		private const int NumberOfRequests = 3;
		private const string EverySecond = "*/1 * * * * *";
		private const string EveryTenSeconds = "*/10 * * * * *";
		private const string EveryThirtySeconds = "*/30 * * * * *";

		private static string StatsPrefix = "perf.dotnet.dogchess";

		private static string AppName = "dogchess-web";

		private static readonly ConcurrentDictionary<string, HttpClient> Clients = new ConcurrentDictionary<string, HttpClient>();
		
		private static readonly Random _random = new Random(DateTime.UtcNow.Millisecond);

		[FunctionName("dogchess-web")]
		public static async Task DogchessWeb([TimerTrigger(EverySecond)] TimerInfo myTimer, ILogger log)
		{
			await BlastIt(AppName);
			if (PercentChance(3))
			{
				var httpClient = GetClient(AppName);
				await httpClient.GetStringAsync($"/cart/itemid/{Guid.NewGuid()}");
			}
		}

		[FunctionName("dogchess-web-cart")]
		public static async Task DogChessWebCart([TimerTrigger(EveryThirtySeconds)] TimerInfo myTimer, ILogger log)
		{
			var httpClient = GetClient(AppName);
			await httpClient.GetStringAsync("/cart");
		}

		[FunctionName("dogchess-web-shop")]
		public static async Task DogChessWebShop([TimerTrigger(EveryTenSeconds)] TimerInfo myTimer, ILogger log)
		{
			var httpClient = GetClient(AppName);
			await httpClient.GetStringAsync("/shop");
		}

		private static async Task BlastIt(string appName, string relativeUrl = "/", string stat = "home.index")
		{
			var httpClient = GetClient(appName);

			var howManyBlasts = NumberOfRequests;

			if (int.TryParse(Environment.GetEnvironmentVariable("DD_REQUESTS_PER_RUN"), out var blastOverride))
			{
				howManyBlasts = blastOverride;
			}

			//using (var statsService = new DogStatsdService())
			//{
			//    statsService.Configure(new StatsdConfig());
			while (howManyBlasts-- > 0)
			{
				//using (statsService.StartTimer($"{StatsPrefix}.{stat}"))
				//{
				await httpClient.GetStringAsync(relativeUrl);
				//}
			}
			//}
		}

		private static HttpClient GetClient(string app)
		{
			if (Clients.ContainsKey(app))
			{
				return Clients["app"];
			}

			var uriText = $"https://{app}.azurewebsites.net/";
			var uri = new Uri(uriText);

			var httpClient = new HttpClient()
			{
				BaseAddress = uri,
			};

			httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;

			Clients.AddOrUpdate(app, (k) => httpClient, (k, v) => httpClient);

			return httpClient;
		}

		private static bool PercentChance(int percentChance)
		{
			return _random.Next(0, 100) < percentChance;
		}
	}
}
