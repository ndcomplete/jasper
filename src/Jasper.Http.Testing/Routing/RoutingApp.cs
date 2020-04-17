using System;
using TestingSupport;

namespace Jasper.Http.Testing.Routing
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
