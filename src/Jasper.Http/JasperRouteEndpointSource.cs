using System.Collections.Generic;
using System.Linq;
using Lamar;
using LamarCodeGeneration;
using LamarCompiler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Jasper.Http
{


    public class JasperRouteEndpointSource : EndpointDataSource
    {
        private readonly IContainer _container;
        private Endpoint[] _endpoints;
        public JasperRouteEndpointSource(IContainer container)
        {
            _container = container;
        }

        public override IChangeToken GetChangeToken()
        {
            return null;
        }

        public override IReadOnlyList<Endpoint> Endpoints => _endpoints ?? (_endpoints = BuildEndpoints().ToArray());

        private IEnumerable<Endpoint> BuildEndpoints()
        {
            var builder = _container.QuickBuild<RouteBuilder>();
            // TODO -- there may be some extra configuration on JasperHttpOptions
            return builder.BuildEndpoints();
        }

        internal class RouteBuilder
        {
            private readonly JasperHttpOptions _httpOptions;
            private readonly IContainer _container;
            private readonly JasperOptions _options;

            public RouteBuilder(JasperHttpOptions httpOptions, IContainer container, JasperOptions options)
            {
                _httpOptions = httpOptions;
                _container = container;
                _options = options;
            }

            public IEnumerable<Endpoint> BuildEndpoints()
            {
                var graph = _httpOptions.Routes;
                graph.Container = _container;

                var actions = _httpOptions.FindActions(_options.ApplicationAssembly).GetAwaiter().GetResult();

                // TODO -- Need to apply policies!

                foreach (var action in actions)
                {
                    graph.AddRoute(action);
                }

                var rules = _options.Advanced.CodeGeneration;
                var generatedAssembly = new GeneratedAssembly(rules);

                var services = graph.AssemblyTypes(rules, generatedAssembly);
                new AssemblyGenerator().Compile(generatedAssembly, services);


                foreach (var chain in graph)
                {
                    yield return chain.BuildEndpoint(_container);
                }

            }
        }
    }
}
