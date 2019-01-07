using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using OfferService;
using OfferService.Client;
using OfferService.Models;
using OfferService.Settings;
using OfferService.UnitTests.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace OfferFilterTests.UnitTests
{
    public class OfferFilterTests
    {
        private Mock<ApiClient> mockedApiClient;
        private Mock<IOptions<FundaApiSettings>> mockedOption;
        private Mock<ILogger<OfferFilter>> mockedLogger;

        private OfferFilter offerFilter;

        private void SetupApiClientGetData(OfferModel data) =>
            mockedApiClient.Setup(x => x.GetData<OfferModel>(It.IsAny<string>()))
                           .ReturnsAsync(data);

        private void SetupApiClientGetDataByPage(int pageIndex, OfferModel data) =>
            mockedApiClient.Setup(x => x.GetData<OfferModel>(It.Is<string>(y=> y.Contains($"page={pageIndex}"))))
                           .ReturnsAsync(data);

        [OneTimeSetUp]
        public void Initialize()
        {
            mockedApiClient = new Mock<ApiClient>();
            mockedOption = new Mock<IOptions<FundaApiSettings>>();
            mockedLogger = new Mock<ILogger<OfferFilter>>();

            FundaApiSettings fundaApiSettings = new FundaApiSettings
            {
                BaseUri = "http://partnerapi.funda.nl",
                Key = "123456989abcdef",
                OfferUriTemplate = "feeds/Aanbod.svc/json/{key}?type=koop&zo={searchQuery}&page={pageIndex}&pagesize={pageSize}"
            };
            mockedOption.SetupGet(x => x.Value).Returns(fundaApiSettings);

            offerFilter = new OfferFilter(mockedOption.Object, mockedApiClient.Object, mockedLogger.Object);
        }

        [Test]
        public async Task GetEstateAgentsByHighestSaleOrder_Returns_Three_Offers()
        {
            SetupApiClientGetData(OfferFilterResponse.ThreeOffersOnePage);

            var result = await offerFilter.GetEstateAgentsByHighestSaleOrder(It.IsAny<string>());

            result.Single(x => x.Name.Equals("Hoekstra en van Eck Amsterdam West"))
                  .OfferCount
                  .Should()
                  .Be(2);

            result.Single(x => x.Name.Equals("Fransen & Kroes Makelaars"))
                  .OfferCount
                  .Should()
                  .Be(1);

            result.Select(x => x.OfferCount)
                  .Aggregate((prev, next) => prev + next)
                  .Should()
                  .Be(3);
        }

        [Test]
        public async Task GetEstateAgentsByHighestSaleOrder_Returns_No_Offer()
        {
            SetupApiClientGetData(OfferFilterResponse.NoOffersNoPage);

            var result = await offerFilter.GetEstateAgentsByHighestSaleOrder(It.IsAny<string>());

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetEstateAgentsByHighestSaleOrder_Returns_Six_Offers_By_Three_Pages()
        {
            SetupApiClientGetDataByPage(1, OfferFilterResponse.SixOffersThreePages[0]);
            SetupApiClientGetDataByPage(2, OfferFilterResponse.SixOffersThreePages[1]);
            SetupApiClientGetDataByPage(3, OfferFilterResponse.SixOffersThreePages[2]);

            var result = await offerFilter.GetEstateAgentsByHighestSaleOrder(It.IsAny<string>());

            result.Single(x => x.Name.Equals("Hoekstra en van Eck Amsterdam West"))
                  .OfferCount
                  .Should()
                  .Be(4);

            result.Single(x => x.Name.Equals("Fransen & Kroes Makelaars"))
                  .OfferCount
                  .Should()
                  .Be(2);

            result.Select(x => x.OfferCount)
                  .Aggregate((prev, next) => prev + next)
                  .Should()
                  .Be(6);
        }
    }
} 