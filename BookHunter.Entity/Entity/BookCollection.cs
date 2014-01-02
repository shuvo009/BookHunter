using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace BookHunter.Entity.Entity
{
    public class BookCollection : ObservableCollection<BookInformation>, ISupportIncrementalLoading
    {
        private readonly IEnumerable<IBookSearch> _searchEngines;
        private readonly string _keyword;
        private uint _currentPageNumber = 1;

        public BookCollection()
        {

        }

        public BookCollection(IEnumerable<IBookSearch> searchEngines, string keyword)
        {
            _searchEngines = searchEngines;
            _keyword = keyword;
            HasMoreItems = true;
        }

        public new void Add(BookInformation bookInformation)
        {
            if (bookInformation.Isbn == null)
                throw new NullReferenceException("ISBN cant be null.");

            if (Items.FirstOrDefault(x => x.Isbn.Equals(bookInformation.Isbn)) == null)
                base.Add(bookInformation);
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return Task.Run(async () =>
             {
                 foreach (var searchEngine in _searchEngines.Where(searchEngine => searchEngine.HasBooks))
                 {
                     var books = await searchEngine.Search(_keyword, _currentPageNumber);
                     foreach (var book in books)
                         Add(book);
                 }
                 HasMoreItems = _searchEngines.Any(x => x.HasBooks);
                 return new LoadMoreItemsResult() { Count = _currentPageNumber++ };
             }).AsAsyncOperation();
        }

        public bool HasMoreItems { get; private set; }
    }
}
