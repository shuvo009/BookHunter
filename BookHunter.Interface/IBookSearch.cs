using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookHunter.Entity;

namespace BookHunter.Interface
{
    public interface IBookSearch
    {
        Task<BookCollection> Search(string keywords, int pageNumber);
        bool HasBooks { get; }
    }
}
