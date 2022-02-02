using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Functions.DemoDogChess
{
	public class AllTriggers
	{
		private const string SecondsInterval = "*/15 * * * * *";

		private readonly Uri MainSite;
		private readonly Uri CheckCart;
		private readonly HttpClient _httpClient;
		private readonly Random _random = new Random(DateTime.UtcNow.Millisecond);

		private static string AppName = "dogchess-web";

		public AllTriggers(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient();
			var uriText = $"https://{AppName}.azurewebsites.net/";
			MainSite = new Uri(uriText);
			CheckCart = new Uri(MainSite, "/user/cart");
		}

		[FunctionName("CheckAbandonedShoppingCarts")]
		public async Task CheckAbandonedShoppingCarts([TimerTrigger(SecondsInterval)] TimerInfo myTimer, ILogger log)
		{
			log.LogInformation($"CheckForAbandonedCarts function executed at: {DateTime.Now}");
			var cartsResponse = await GetFunctionHttp("abandonedcarts");
			var carts = cartsResponse.Split(",");
			foreach (var cart in carts)
			{
				await PostFunctionHttp("shoppingcartreminder", cart);
			}
		}

		[FunctionName("GetAbandonedShoppingCarts")]
		public async Task<string> GetAbandonedShoppingCarts(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "abandonedcarts")] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("GetAbandonedShoppingCarts request started.");

			if (PercentChance(5))
			{
				// long delay chance
				log.LogInformation("Refreshing cache.");
				await RandomSleep(blocking: true, minimumMilliseconds: 220, maximumMilliseconds: 8451);
			}
			else
			{
				await RandomSleep(blocking: false, minimumMilliseconds: 29, maximumMilliseconds: 121);
			}

			await _httpClient.GetStringAsync(CheckCart);

			var hasAbandonedCarts = PercentChance(35);
			if (!hasAbandonedCarts)
			{
				return "";
			}

			var carts = _random.Next(0, 5);
			var cartGuids = new List<string>();
			while (carts-- > 0)
			{
				cartGuids.Add(Guid.NewGuid().ToString());
			}

			log.LogInformation("GetAbandonedShoppingCarts request finished.");

			return string.Join(",", cartGuids.ToArray());
		}

		[FunctionName("SendEmailReminderAboutShoppingCart")]
		public async Task<IActionResult> SendEmailReminderAboutShoppingCart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "shoppingcartreminder")] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("SendEmailReminderAboutShoppingCart request started.");

			await RandomSleep(blocking: true, minimumMilliseconds: 15, maximumMilliseconds: 45);

			RandomError(percentChance: 2);

			log.LogInformation("SendEmailReminderAboutShoppingCart request finished.");

			return new OkObjectResult("Email reminder sent");
		}

		private async Task<string> GetFunctionHttp(string path)
		{
			string url = GetFunctionUrl(path);
			var simpleResponse = await _httpClient.GetStringAsync(url);
			return simpleResponse;
		}

		private async Task<HttpResponseMessage> PostFunctionHttp(string path, string body)
		{
			string url = GetFunctionUrl(path);
			var content = new StringContent(body, Encoding.UTF8, "text/plain");
			var simpleResponse = await _httpClient.PostAsync(url, content);
			return simpleResponse;
		}

		private static string GetFunctionUrl(string path)
		{
			var httpFunctionUrl = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME") ?? "localhost:7071";
			var url = $"http://{httpFunctionUrl}";
			return $"{url}/api/{path}";
		}

		private async Task RandomSleep(bool blocking, int minimumMilliseconds, int maximumMilliseconds)
		{
			var wait = _random.Next(minimumMilliseconds, maximumMilliseconds);
			if (blocking)
			{
				Thread.Sleep(wait);
			}
			else
			{
				await Task.Delay(wait);
			}
		}

		private void RandomError(int percentChance)
		{
			if (PercentChance(percentChance))
			{
				throw new Exception("An unknown error has occurred");
			}
		}

		private bool PercentChance(int percentChance)
		{
			return _random.Next(0, 100) < percentChance;
		}
	}
}
