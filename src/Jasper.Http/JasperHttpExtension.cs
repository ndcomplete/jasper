using Baseline;
using Baseline.Reflection;
using Jasper.Http.ContentHandling;
using Jasper.Http.MVCExtensions;
using Jasper.Http.Routing;
using Jasper.Serialization;
using Lamar;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Jasper.Http
{
    public class JasperHttpExtension : IJasperExtension
    {
        public void Configure(JasperOptions registry)
        {
            var options = new JasperHttpOptions();
            registry.Services.AddSingleton(options);

            registry.Services.ForConcreteType<ConnegRules>().Configure.Singleton();
            registry.Services.For<IHttpContextAccessor>().Use(x => new HttpContextAccessor());
            registry.Services.AddSingleton(options.Routes);

            registry.Services.ForSingletonOf<IUrlRegistry>().Use(options.Urls);


            registry.Services.Policies.Add(new RouteScopingPolicy(options.Routes));

            // SAMPLE: applying-route-policy
            // Applying a global policy
            options.GlobalPolicy<ControllerUsagePolicy>();

            options.IncludeTypes(x => x.CanBeCastTo<ControllerBase>());
            options.IncludeMethods(x => x.HasAttribute<HttpMethodAttribute>());

            registry.Services.Scan(x =>
            {
                x.AssemblyContainingType<JasperHttpExtension>();
                x.ConnectImplementationsToTypesClosing(typeof(ISerializerFactory<,>));
            });

            registry.Services.Scan(x =>
            {
                x.Assembly(registry.ApplicationAssembly);
                x.AddAllTypesOf<IRequestReader>();
                x.AddAllTypesOf<IResponseWriter>();
            });
            // ENDSAMPLE


            registry.Services.AddSingleton<IWriterRule, ActionResultWriterRule>();

            RouteBuilder.PatternRules.Insert(0, new HttpAttributePatternRule());
        }
    }
}
