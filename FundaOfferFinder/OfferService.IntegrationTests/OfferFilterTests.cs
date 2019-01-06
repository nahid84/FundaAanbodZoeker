using FluentAssertions;
using OfferService.Client;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OfferService.IntegrationTests
{
    public class OfferFilterTests : IClassFixture<FundaApiSettingsFixture>
    {
        private OfferFilter offerFilter;

        public OfferFilterTests(FundaApiSettingsFixture settingsFixture)
        {
            offerFilter = new OfferFilter(settingsFixture, new JsonApiClient(settingsFixture));
        }

        [Fact]
        public async Task GetEstateAgentsByHighestSaleOrder_Returns_574_Offers()
        {
            var result = await offerFilter.GetEstateAgentsByHighestSaleOrder("amsterdam","tuin");

            result.Select(x => x.OfferCount)
                  .Aggregate((prev, next) => prev + next)
                  .Should()
                  .Be(574);
        }

        [Fact]
        public async Task GetEstateAgentsByHighestSaleOrder_Returns_Zero_Offer()
        {
            var result = await offerFilter.GetEstateAgentsByHighestSaleOrder("amsterda", "tuin");

            result.Should().BeEmpty();
        }
    }
}
