using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using BookHunter.Entity;
using BookHunter.Entity.Entity;
using BookHunter.Service.Amazon;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace BookHunter.Test
{
    [TestClass]
    public class SearchTest
    {
        [TestMethod]
        public void Amazon_Search()
        {
            IBookSearch amazonBookSearch = new AmazonBookSearch();
            var searchResult = amazonBookSearch.Search("C#", 1).Result;
            Assert.AreEqual(10, searchResult.Count);
            Assert.IsTrue(amazonBookSearch.HasBooks);
        }

        [TestMethod]
        public void Amazon_Incremental_Search()
        {
            IBookSearch amazonBookSearch = new AmazonBookSearch();
            foreach (var pageNumber in Enumerable.Range(1, 10))
            {
                var searchResult = amazonBookSearch.Search("C#", Convert.ToUInt32(pageNumber)).Result;
                //Assert.AreNotEqual(0, searchResult.Count);
                if (pageNumber != 10)
                    Assert.IsTrue(amazonBookSearch.HasBooks);
            }
            Assert.IsFalse(amazonBookSearch.HasBooks);
        }

        [TestMethod]
        public void BookCollection_Test()
        {
            var searchEngines = new List<IBookSearch> { new AmazonBookSearch() };
            var bookCollection = new BookCollection(searchEngines, "C#");

            foreach (var pageNumber in Enumerable.Range(1, 10))
            {
                (bookCollection as ISupportIncrementalLoading).LoadMoreItemsAsync(Convert.ToUInt32(pageNumber)).GetAwaiter().GetResult();
            }

            Assert.IsFalse(bookCollection.HasMoreItems);
        }
    }
}
