using Jasper;
using TestingSupport;

namespace HttpTests.Routing
{
    public class RoutingApp : JasperOptions
    {
        public RoutingApp()
        {
            Handlers.DisableConventionalDiscovery();

            Settings.Http(x =>
            {
                x.DisableConventionalDiscovery()
                    .IncludeType<SpreadHttpActions>()
                    .IncludeType<RouteEndpoints>();
            });
        }
    }
}
