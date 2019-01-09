namespace OfferService.Settings
{
    /// <summary>
    /// Settings for the Funda api service
    /// </summary>
    public class FundaApiSettings
    {
        /// <summary>
        /// Base Uri of the REST service
        /// </summary>
        public string BaseUri { get; set; }
        /// <summary>
        /// Key used to access the service
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Relative part of the offer Uri
        /// </summary>
        public string OfferUriTemplate { get; set; }
    }
}
