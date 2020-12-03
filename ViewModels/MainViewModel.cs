using MapShot_ver2.Commands;
using System.Windows;
using System.Windows.Input;

namespace MapShot_ver2.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private BaseViewModel SelectedViewModel;

        public BaseViewModel selectedViewModel
        {
            get { return SelectedViewModel; }
            set
            {
                SelectedViewModel = value;
                OnPropertyChanged("selectedViewModel");
            }
        }
        
        public ICommand mapButtonClick { get; private set; }
        public ICommand optionButtonClick { get; private set; }
        public ICommand mergeButtonClick { get; private set; }

        private MapViewModel mapViewModel = new MapViewModel();
        private OptionViewModel optionViewModel = new OptionViewModel();
        //private MergeViewModel mergeViewModel = new MergeViewModel();


        public MainViewModel()
        {
            mapButtonClick = new RelayCommand(MapButtonClickMethod);
            optionButtonClick = new RelayCommand(OptionButtonClickMethod);
            //mergeButtonClick = new RelayCommand(MergeButtonClickMethod);
            mapViewModel.change += MapViewModel_CompleteCopy;

            MapButtonClickMethod(null);
        }

        /// <summary>
        /// 주소 복사 시 옵션 선택 창으로 넘김
        /// </summary>
        private void MapViewModel_CompleteCopy()
        {
            optionViewModel.AddressText = Clipboard.GetText();
            selectedViewModel = optionViewModel;
        }

        /// <summary>
        /// viewModel Change: mapViewModel
        /// </summary>
        /// <param name="obj"></param>
        private void MapButtonClickMethod(object obj)
        {
            selectedViewModel = mapViewModel;
        }

        /// <summary>
        /// viewModel Change: optionViewModel
        /// </summary>
        /// <param name="obj"></param>
        private void OptionButtonClickMethod(object obj)
        {
            selectedViewModel = optionViewModel;
        }

        //private void MergeButtonClickMethod(object obj)
        //{
        //    selectedViewModel = mergeViewModel;
        //}
    }
}
