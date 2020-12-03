using MapShot_ver2.Commands;
using System.Windows;
using System.Windows.Input;

namespace MapShot_ver2.ViewModels
{
    class MapViewModel : BaseViewModel
    {

        public delegate void ChangeView();
        public event ChangeView change;

        public ICommand copyCommand { get; private set; }


        public MapViewModel()
        {
            copyCommand = new RelayCommand(copyCommandMethod);
        }

        private void copyCommandMethod(object obj)
        {
            if (!string.IsNullOrEmpty(Clipboard.GetText()))
            {
                change();
            }

        }
    }
}
