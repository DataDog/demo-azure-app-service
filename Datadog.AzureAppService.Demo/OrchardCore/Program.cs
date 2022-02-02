using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OrchardCore.Sample;
using Serilog;

namespace OrchardCore
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog((context, config) =>
				{
					// Figure out why env wasn't added in AAS, this should happen via the tracer
					var tags = new[] { $"env:{ConfigHelper.DatadogEnv ?? "empty"}" };
					config.WriteTo.DatadogLogs(ConfigHelper.DatadogApiKey, tags: tags);
					config.Enrich.FromLogContext();
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
