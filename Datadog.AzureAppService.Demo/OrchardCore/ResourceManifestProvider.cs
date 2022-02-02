using OrchardCore.ResourceManagement;

namespace OrchardCore.Sample
{
	public class ResourceManifestProvider : IResourceManifestProvider
	{
		public void BuildManifests(IResourceManifestBuilder builder)
		{
			var manifest = builder.Add();
	
			manifest
				.DefineScript("datadog-rum-cdn")
				.SetUrl("~/scripts/vendor/datadog-rum.js")
				.SetCdn("https://www.datadoghq-browser-agent.com/datadog-rum.js")
				.SetCdnIntegrity("sha384-d1ztQ5a4z4kTUn5Dj6dDJBgLfHz05X2RrRM+OwPDme6aSBLeTl3SIrucxSeIJrdz")
			    .SetVersion("2.5.2");
		}
	}
}