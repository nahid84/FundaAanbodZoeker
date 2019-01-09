namespace OfferService.Models
{
    /// <summary>
    /// Paging section
    /// </summary>
    public class PagingModel
    {
        /// <summary>
        /// Number of pages
        /// </summary>
        public int AantalPaginas { get; set; }
        /// <summary>
        /// Current page accessed
        /// </summary>
        public int HuidigePagina { get; set; }
    }
}
