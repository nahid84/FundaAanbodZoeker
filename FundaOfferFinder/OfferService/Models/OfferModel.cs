using System.Collections.Generic;

namespace OfferService.Models
{
    /// <summary>
    /// Offer information model
    /// </summary>
    public class OfferModel
    {
        public MetadataModel Metadata { get; set; }
        public ICollection<OfferObjectModel> Objects { get; set; }
        public PagingModel Paging { get; set; }
        public int TotaalAantalObjecten { get; set; }
    }
}
