using System;
using Jasper;
using TestingSupport;

namespace HttpTests.Routing
{
    public class RoutingApp : JasperOptions
    {
        public RoutingApp()
        {
            Handlers.DisableConventionalDiscovery();
            throw new NotImplementedException("redo");
//            Settings.Http(x =>
//            {
//                x.DisableConventionalDiscovery()
//                    .IncludeType<SpreadHttpActions>()
//                    .IncludeType<RouteEndpoints>();
//            });
        }
    }
}
