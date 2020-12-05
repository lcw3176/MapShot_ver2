using System.ComponentModel;

namespace MapShot_ver2.Models
{
    class DetailOption : INotifyPropertyChanged
    {
        private bool check = false;
        
        public string Title { get; set; }

        public bool Check
        {
            get { return check; }
            set 
            {
                check = value;
                OnPropertyChanged("Check");
            }
        }

        public string code { get; set; }
        public string collectionName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string param)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(param));
            }
        }
    }
}
