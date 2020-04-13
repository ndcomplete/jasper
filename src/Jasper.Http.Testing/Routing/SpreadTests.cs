using Alba.Stubs;
using JasperHttp.Routing;
using Shouldly;
using Xunit;

namespace HttpTests.Routing
{
    public class SpreadTests
    {

        [Fact]
        public void the_canonical_path_is_blank()
        {
            new Spread(2).CanonicalPath().ShouldBe(string.Empty);
        }
    }
}
