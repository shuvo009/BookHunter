using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BookHunter.Entity;
using BookHunter.Helper;
using BookHunter.Interface;
using BookHunter.Service.AmazonWebService;

namespace BookHunter.Service.Amazon
{
    public class AmazonSearch : ISearch
    {
        private readonly string _associateTag;
        private readonly string _accessKeyId;
        private readonly AWSECommerceServicePortTypeClient _amazonClient;

        public AmazonSearch()
        {
            _accessKeyId = "AKIAIYHNWRPBKLK6OUFQ";
            const string secretAccessKey = "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";
            _associateTag = "wwwcodaromane-20";

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport) { MaxReceivedMessageSize = int.MaxValue };

            _amazonClient = new AWSECommerceServicePortTypeClient(
                        binding,
                        new EndpointAddress("https://webservices.amazon.co.uk/onca/soap?Service=AWSECommerceService"));

            _amazonClient.ChannelFactory.Endpoint.EndpointBehaviors.Add(new AmazonSigningEndpointBehavior(_accessKeyId, secretAccessKey));
        }

        public async Task<SearchResult> Search(string keywords)
        {
            return await Task.Run(() =>
            {
                Int64 currentPageNumber = 1;
                Int64 totalPage = 20;
                //do
                //{
                var itemSearch = new ItemSearch();
                var itemSearchRequest = new ItemSearchRequest
                {
                    Keywords = keywords,
                    SearchIndex = "Books",
                    ResponseGroup = new[] { "Large" },
                    ItemPage = currentPageNumber.ToString(CultureInfo.InvariantCulture)
                };

                itemSearch.Request = new[] { itemSearchRequest };
                itemSearch.AWSAccessKeyId = _accessKeyId;
                itemSearch.AssociateTag = _associateTag;
                var response = _amazonClient.ItemSearchAsync(itemSearch).Result;
                //currentPageNumber++;
                return ReadResponse(response.ItemSearchResponse.Items);
                //if (!isSuccess)
                //    return false;
                //var pages = Convert.ToInt64(response.Items.First().TotalPages);
                //if (pages < 21)
                //    totalPage = pages;
                //} while (totalPage > currentPageNumber);
                //return true;
            });
        }

        #region Private Methods
        private SearchResult ReadResponse(IEnumerable<Items> searchItems)
        {
            var searchResult = new SearchResult();
            try
            {

                foreach (var amazonBookInfo in searchItems.First().Item)
                {
                    var bookInformation = new SearchResult
                    {
                        ProviderName = Provider.Amazon
                    };

                    if (amazonBookInfo.ItemAttributes != null)
                        bookInformation.Title = amazonBookInfo.ItemAttributes.Title ?? String.Empty;

                    if (amazonBookInfo.EditorialReviews != null && amazonBookInfo.EditorialReviews.Any())
                        bookInformation.ShortDescription = amazonBookInfo.EditorialReviews.First().Content;

                    if (amazonBookInfo.LargeImage != null)
                        bookInformation.CoverImageUrl = amazonBookInfo.LargeImage.URL;

                    try
                    {
                        bookInformation.Price = amazonBookInfo.Offers.Offer[0].OfferListing[0].Price.FormattedPrice;
                    }
                    catch (Exception)
                    {
                        bookInformation.Price = string.Empty;
                    }
                    searchResult.Add(bookInformation);
                }
                return searchResult;
            }
            catch (Exception)
            {
                return searchResult;
            }
        }
        #endregion
    }
}
