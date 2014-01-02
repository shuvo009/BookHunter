using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookHunter.Entity;

namespace BookHunter.Interface
{
    public interface ISearch
    {
        Task<SearchResult> Search(string keywords);
    }
}
