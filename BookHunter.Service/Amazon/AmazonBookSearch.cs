using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BookHunter.Entity;
using BookHunter.Entity.Entity;
using BookHunter.Helper;
using BookHunter.Service.AmazonWebService;

namespace BookHunter.Service.Amazon
{
    public class AmazonBookSearch : IBookSearch
    {
        private readonly string _associateTag;
        private readonly string _accessKeyId;
        private readonly AWSECommerceServicePortTypeClient _amazonClient;
        private const int TotalPage = 10;

        public bool HasBooks { get; private set; }
        public AmazonBookSearch()
        {
            HasBooks = true;
            _accessKeyId = "AKIAIYHNWRPBKLK6OUFQ";
            const string secretAccessKey = "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";
            _associateTag = "wwwcodaromane-20";

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport) { MaxReceivedMessageSize = int.MaxValue };

            _amazonClient = new AWSECommerceServicePortTypeClient(
                        binding,
                        new EndpointAddress("https://webservices.amazon.co.uk/onca/soap?Service=AWSECommerceService"));

            _amazonClient.ChannelFactory.Endpoint.EndpointBehaviors.Add(new AmazonSigningEndpointBehavior(_accessKeyId, secretAccessKey));
        }

        public async Task<BookCollection> Search(string keywords, uint pageNumber)
        {
            return await Task.Run(() =>
            {
                var itemSearch = new ItemSearch();
                var itemSearchRequest = new ItemSearchRequest
                {
                    Keywords = keywords,
                    SearchIndex = "Books",
                    ResponseGroup = new[] { "Large" },
                    ItemPage = pageNumber.ToString(CultureInfo.InvariantCulture)
                };

                itemSearch.Request = new[] { itemSearchRequest };
                itemSearch.AWSAccessKeyId = _accessKeyId;
                itemSearch.AssociateTag = _associateTag;
                var response = _amazonClient.ItemSearchAsync(itemSearch).Result;
                if(response.ItemSearchResponse.Items.FirstOrDefault()==null) return new BookCollection();
                var totalResultPages = Convert.ToInt32(response.ItemSearchResponse.Items.First().TotalPages);
                if (totalResultPages <= pageNumber || TotalPage <= pageNumber)
                    HasBooks = false;
                return ReadResponse(response.ItemSearchResponse.Items);
            });
        }

        #region Private Methods
        private BookCollection ReadResponse(IEnumerable<Items> items)
        {
            var bookCollection = new BookCollection();
            try
            {

                foreach (var product in items)
                {
                    foreach (var bookItemWithItem in product.Item)
                    {
                        var bookInformation = new BookInformation
                        {
                            ProviderName = Provider.Amazon
                        };

                        if (bookItemWithItem.ItemAttributes != null)
                        {
                            bookInformation.Title = bookItemWithItem.ItemAttributes.Title ?? String.Empty;
                            bookInformation.Isbn = bookItemWithItem.ItemAttributes.ISBN ?? string.Empty;
                            //bookInformation.ShortDescription = bookItemWithItem.ItemAttributes.
                        }

                        if (bookItemWithItem.LargeImage != null)
                        {
                            bookInformation.CoverImageUrl = bookItemWithItem.LargeImage.URL;
                        }

                        try
                        {
                            bookInformation.Price = bookItemWithItem.Offers.Offer[0].OfferListing[0].Price.FormattedPrice;
                        }
                        catch (Exception)
                        {
                            bookInformation.Price = string.Empty;
                        }
                        bookCollection.Add(bookInformation);
                    }
                }
                return bookCollection;
            }
            catch (Exception)
            {
                return bookCollection;
            }
        }
        #endregion

    }
}
