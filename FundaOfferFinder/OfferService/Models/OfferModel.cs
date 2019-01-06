﻿using System.Collections.Generic;

namespace OfferService.Models
{
    public class OfferModel
    {
        public MetadataModel Metadata { get; set; }
        public ICollection<OfferObjectModel> Objects { get; set; }
        public PagingModel Paging { get; set; }
    }
}
