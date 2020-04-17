using System;
using System.Threading.Tasks;
using Alba;
using Baseline;
using Jasper.Runtime.Handlers;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using TestingSupport;
using Xunit;

namespace Jasper.Http.Testing
{
    public class BasicAppNoHandling : JasperOptions
    {
        public BasicAppNoHandling()
        {
            Handlers.DisableConventionalDiscovery();

            Services.Scan(_ =>
            {
                _.TheCallingAssembly();
                _.WithDefaultConventions();
            });
        }
    }


    public class RegistryFixture<T> : IDisposable where T : JasperOptions, new()
    {
        private readonly Lazy<SystemUnderTest> _sut = new Lazy<SystemUnderTest>(() =>
        {
            throw new NotImplementedException("redo");
//            var system = JasperAlba.For<T>();
//            system.Services.As<Container>().DisposalLock = DisposalLock.ThrowOnDispose;
//
//            return system;
        });

        public SystemUnderTest System => _sut.Value;

        public void Dispose()
        {
            System.Services.As<Container>().DisposalLock = DisposalLock.Unlocked;
            System?.Dispose();
        }

        public Task<IScenarioResult> Scenario(Action<Scenario> configuration)
        {
            return System.Scenario(configuration);
        }
    }

    [Collection("integration")]
    public class RegistryContext<T> : IClassFixture<RegistryFixture<T>> where T : JasperOptions, new()
    {
        private readonly RegistryFixture<T> _fixture;

        public RegistryContext(RegistryFixture<T> fixture)
        {
            _fixture = fixture;
        }

        protected SystemUnderTest Runtime => _fixture.System;


        protected HandlerGraph theHandlers()
        {
            return _fixture.System.Services.GetRequiredService<HandlerGraph>();
        }

        protected Task<IScenarioResult> scenario(Action<Scenario> configuration)
        {
            return _fixture.Scenario(configuration);
        }
    }
}
