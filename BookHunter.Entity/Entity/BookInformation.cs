using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookHunter.Helper;

namespace BookHunter.Entity.Entity
{
    public class BookInformation : INotifyPropertyChanged
    {
        private string _title;
        private string _price;
        private string _shortDescription;
        private Provider _providerName;
        private string _coverImageUrl;
        private string _isbn;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RisePropertyChange();
            }
        }

        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                RisePropertyChange();
            }
        }

        public string ShortDescription
        {
            get { return _shortDescription; }
            set
            {
                _shortDescription = value;
                RisePropertyChange();
            }
        }

        public string CoverImageUrl
        {
            get { return _coverImageUrl; }
            set
            {
                _coverImageUrl = value;
                RisePropertyChange();
            }
        }

        public string Isbn
        {
            get { return _isbn; }
            set
            {
                _isbn = value;
                RisePropertyChange();
            }
        }

        public Provider ProviderName
        {
            get { return _providerName; }
            set
            {
                _providerName = value;
                RisePropertyChange();
            }
        }

        #region Private Method
        public event PropertyChangedEventHandler PropertyChanged;

        private void RisePropertyChange([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }

}