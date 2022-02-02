using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StatsdClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ResourceManagement;
using OrchardCore.Sample;

namespace OrchardCore
{
	public class Startup
	{
		private static Task _dogHeartbeat;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOrchardCms();

			services.AddScoped<IResourceManifestProvider, ResourceManifestProvider>();
			services.Configure<MvcOptions>((options) =>
			{
				options.Filters.Add(typeof(DatadogInjectionFilter));
			});

			try
			{
				if (ConfigHelper.AllDatadogVariablesSet)
				{
					services.AddSingleton<IDogStatsd>((i) =>
					{
						var service = new DogStatsdService();
						service.Configure(new StatsdConfig());

						service.ServiceCheck("dogchess.aas", Status.OK);

						_dogHeartbeat = Task.Factory.StartNew(() =>
						{
							while (true)
							{
								Thread.Sleep(500);
								service.ServiceCheck("dogchess.aas", Status.OK);
							}
						// ReSharper disable once FunctionNeverReturns
					});

						return service;
					});
				}
				else
				{
					services.AddSingleton<IDogStatsd>(new NoOpDogStatsd());
				}
			}
			catch
			{
				// Environment variables needed for statsd setup are missing
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseOrchardCore();
		}
	}
}
