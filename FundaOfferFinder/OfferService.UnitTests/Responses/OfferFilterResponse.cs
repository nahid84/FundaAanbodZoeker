using OfferService.Models;

namespace OfferService.UnitTests.Responses
{
    internal class OfferFilterResponse
    {
        internal static OfferModel ThreeOffersOnePage =>
            new OfferModel
            {
                Metadata = new MetadataModel
                {
                    Titel = "Huizen te koop in heel Nederland"
                },
                Objects = new OfferObjectModel[]
                {
                    new OfferObjectModel
                    {
                        MakelaarNaam = "Hoekstra en van Eck Amsterdam West"
                    },
                    new OfferObjectModel
                    {
                        MakelaarNaam = "Fransen & Kroes Makelaars"
                    },
                    new OfferObjectModel
                    {
                        MakelaarNaam = "Hoekstra en van Eck Amsterdam West"
                    }
                },
                Paging = new PagingModel
                {
                    AantalPaginas = 1,
                    HuidigePagina = 1
                }
            };

        internal static OfferModel NoOffersNoPage =>
            new OfferModel
            {
                Metadata = new MetadataModel
                {
                    Titel = "Huizen te koop in heel Nederland"
                },
                Paging = new PagingModel
                {
                    AantalPaginas = 0,
                    HuidigePagina = 1
                }
            };

        internal static OfferModel[] SixOffersThreePages =>
            new OfferModel[]
            {
                new OfferModel
                {
                    Metadata = new MetadataModel
                    {
                        Titel = "Huizen te koop in heel Nederland"
                    },
                    Objects = new OfferObjectModel[]
                    {
                        new OfferObjectModel
                        {
                            MakelaarNaam = "Hoekstra en van Eck Amsterdam West"
                        },
                        new OfferObjectModel
                        {
                            MakelaarNaam = "Fransen & Kroes Makelaars"
                        }
                    },
                    Paging = new PagingModel
                    {
                        AantalPaginas = 3,
                        HuidigePagina = 1
                    }
                },
                new OfferModel
                {
                    Metadata = new MetadataModel
                    {
                        Titel = "Huizen te koop in heel Nederland"
                    },
                    Objects = new OfferObjectModel[]
                    {
                        new OfferObjectModel
                        {
                            MakelaarNaam = "Hoekstra en van Eck Amsterdam West"
                        },
                        new OfferObjectModel
                        {
                            MakelaarNaam = "Hoekstra en van Eck Amsterdam West"
                        }
                    },
                    Paging = new PagingModel
                    {
                        AantalPaginas = 3,
                        HuidigePagina = 2
                    }
                },
                new OfferModel
                {
                    Metadata = new MetadataModel
                    {
                        Titel = "Huizen te koop in heel Nederland"
                    },
                    Objects = new OfferObjectModel[]
                    {
                        new OfferObjectModel
                        {
                            MakelaarNaam = "Fransen & Kroes Makelaars"
                        },
                        new OfferObjectModel
                        {
                            MakelaarNaam = "Hoekstra en van Eck Amsterdam West"
                        }
                    },
                    Paging = new PagingModel
                    {
                        AantalPaginas = 3,
                        HuidigePagina = 3
                    }
                }
            };
    }
}
