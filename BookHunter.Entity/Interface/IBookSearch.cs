using System.Threading.Tasks;
using BookHunter.Entity.Entity;

namespace BookHunter.Entity
{
    public interface IBookSearch
    {
        Task<BookCollection> Search(string keywords, uint pageNumber);
        bool HasBooks { get; }
    }
}
