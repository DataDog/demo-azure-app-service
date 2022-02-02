using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using StatsdClient;

namespace OrchardCore.Sample
{
	// Don't forget to add this filter to the filter collection in the Startup.cs file.
	public class DatadogInjectionFilter : IAsyncResultFilter
	{
		// To access the layout which contains the zones you need to use the ILayoutAccessor service.
		private readonly ILayoutAccessor _layoutAccessor;
		// To generate ad-hoc shapes the IShapeFactory can be used.
		private readonly IShapeFactory _shapeFactory;
		private readonly IDogStatsd _dogStatsd;

		public DatadogInjectionFilter(ILayoutAccessor layoutAccessor, IShapeFactory shapeFactory, IDogStatsd dogStatsd)
		{
			_layoutAccessor = layoutAccessor;
			_shapeFactory = shapeFactory;
			_dogStatsd = dogStatsd;
		}

		public async Task OnResultExecutionAsync(ResultExecutingContext filterContext, ResultExecutionDelegate next)
		{
			using (_dogStatsd.StartTimer("orchard.demo.datadoginjectionfilter.onresultexecution"))
			{
				// You can decide when the filter should be executed here. If this is a ViewResult or PageResult the shape
				// injection wouldn't make any sense since there wouldn't be any zones.
				if (!(filterContext.Result is ViewResult || filterContext.Result is PageResult))
				{
					_dogStatsd.Increment("dogchess.page.view");
					await next();
					return;
				}

				// The layout can be handled easily if this is dynamic.
				dynamic layout = await _layoutAccessor.GetLayoutAsync();

				// The dynamic Layout object will contain a Zones dictionary that you can use to access a zone.
				var contentZone = layout.Zones["Header"];

				// Here you can add an ad-hoc generated shape to the content zone.
				// This corresponds to ~/Views/DatadogRum.cshtml
				contentZone.Add(await _shapeFactory.New.DatadogRum());

				_dogStatsd.Increment("dogchess.rum.injection");

				await next();
			}
		}
	}
}