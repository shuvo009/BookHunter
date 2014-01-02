using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace BookHunter.Base
{
    internal abstract class BookHunterViewModelBase : ViewModelBase
    {
        public bool IsBusy { get; set; }
    }
}
