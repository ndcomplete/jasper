using Shouldly;
using Xunit;

namespace Jasper.Http.Testing
{
    public class HttpSettingsTests
    {
        [Fact]
        public void default_compliance_mode_is_full()
        {
            new JasperHttpOptions().AspNetCoreCompliance.ShouldBe(ComplianceMode.FullyCompliant);
        }
    }
}
