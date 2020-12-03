using MapShot_ver2.Commands;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using MapShot_ver2.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MapShot_ver2.Models;
using MapShot_ver2.DAO;

namespace MapShot_ver2.ViewModels
{
    class OptionViewModel : BaseViewModel
    {
        private string addressText = string.Empty;
        private string savePath = string.Empty;
        private int pictureCount = 1;
        private int progressValue = 0;
        ConfigService configService = new ConfigService();
        OptionService optionService = new OptionService();
        JsonService jsonService = new JsonService();

        public int ZoomLevelIndex
        {
            get { return OptionDAO.GetInstance().zoomLevelIndex; }
            set
            {
                OptionDAO.GetInstance().zoomLevelIndex = value;
                OnPropertyChanged("ZoomLevelIndex");
            }
        }

        public int MapTypeIndex
        {
            get { return OptionDAO.GetInstance().mapTypeIndex; }
            set
            {
                OptionDAO.GetInstance().mapTypeIndex = value;
                OnPropertyChanged("MapTypeIndex");
            }
        }


        public int QualityIndex
        {
            get { return OptionDAO.GetInstance().qualityIndex; }
            set
            {
                OptionDAO.GetInstance().qualityIndex = value;
                OnPropertyChanged("QualityIndex");
            }
        }

        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                progressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }

        public int PictureCount
        {
            get { return pictureCount; }
            set
            {
                pictureCount = value;
                OnPropertyChanged("PictureCount");
            }
        }
        public string SavePath
        {
            get { return savePath; }
            set
            {
                savePath = value;
                OnPropertyChanged("SavePath");
            }
        }

        public string AddressText
        {
            get { return addressText; }
            set
            {
                addressText = value;
                OnPropertyChanged("AddressText");
            }
        }

        public ObservableCollection<DetailOption> boundary 
        {
            get { return JsonDAO.GetInstance().jsonDictionary["boundary"]; }
            set
            {
                JsonDAO.GetInstance().jsonDictionary["boundary"] = value;
                OnPropertyChanged("boundary");
            }
        }

        public ObservableCollection<DetailOption> traffic
        {
            get { return JsonDAO.GetInstance().jsonDictionary["traffic"]; }
            set
            {
                JsonDAO.GetInstance().jsonDictionary["traffic"] = value;
                OnPropertyChanged("traffic");
            }
        }

        public ObservableCollection<DetailOption> airport
        {
            get { return JsonDAO.GetInstance().jsonDictionary["airport"]; }
            set
            {
                JsonDAO.GetInstance().jsonDictionary["airport"] = value;
                OnPropertyChanged("airport");
            }
        }

        public ObservableCollection<DetailOption> city
        {
            get { return JsonDAO.GetInstance().jsonDictionary["city"]; }
            set
            {
                JsonDAO.GetInstance().jsonDictionary["city"] = value;
                OnPropertyChanged("city");
            }
        }

        public ObservableCollection<DetailOption> earth
        {
            get { return JsonDAO.GetInstance().jsonDictionary["earth"]; }
            set
            {
                JsonDAO.GetInstance().jsonDictionary["earth"] = value;
                OnPropertyChanged("earth");
            }
        }

        public ObservableCollection<DetailOption> fish
        {
            get { return JsonDAO.GetInstance().jsonDictionary["fish"]; }
            set
            {
                JsonDAO.GetInstance().jsonDictionary["fish"] = value;
                OnPropertyChanged("fish");
            }
        }

        public ObservableCollection<DetailOption> industry
        {
            get { return JsonDAO.GetInstance().jsonDictionary["industry"]; }
            set
            {
                JsonDAO.GetInstance().jsonDictionary["industry"] = value;
                OnPropertyChanged("industry");
            }
        }

        public ObservableCollection<DetailOption> mountain
        {
            get { return JsonDAO.GetInstance().jsonDictionary["mountain"]; }
            set
            {
                JsonDAO.GetInstance().jsonDictionary["mountain"] = value;
                OnPropertyChanged("mountain");
            }
        }

        public ObservableCollection<DetailOption> usage
        {
            get { return JsonDAO.GetInstance().jsonDictionary["usage"]; }
            set
            {
                JsonDAO.GetInstance().jsonDictionary["usage"]= value;
                OnPropertyChanged("usage");
            }
        }

        public ObservableCollection<DetailOption> water
        {
            get { return JsonDAO.GetInstance().jsonDictionary["water"]; }
            set
            {
               JsonDAO.GetInstance().jsonDictionary["water"] = value;
                OnPropertyChanged("water");
            }
        }



        public ICommand saveSetCommand { get; private set; }
        public ICommand loadSetCommand { get; private set; }
        public ICommand setSavePathCommand { get; private set; }
        public ICommand checkCommand { get; private set; }
        public ICommand startCommand { get; private set; }

        private bool isBusy = false;

        public OptionViewModel()
        {
            saveSetCommand = new RelayCommand(saveSetMethod);
            loadSetCommand = new RelayCommand(loadSetMethod);
            setSavePathCommand = new RelayCommand(setSavePathMethod);
            startCommand = new RelayCommand(startMethod);
            InitCollections();
        }

        /// <summary>
        /// 체크박스 목록 갱신
        /// </summary>
        public void InitCollections()
        {
            jsonService.SetCollections();
            var dic = JsonDAO.GetInstance().jsonDictionary;

            boundary = dic["boundary"];
            traffic = dic["traffic"];
            airport = dic["airport"];
            city = dic["city"];
            earth = dic["earth"];
            fish = dic["fish"];
            industry = dic["industry"];
            mountain = dic["mountain"];
            usage = dic["usage"];
            water = dic["water"];
        }


        /// <summary>
        /// 캡쳐 시작 버튼
        /// </summary>
        /// <param name="obj"></param>
        private void startMethod(object obj)
        {
            if (isBusy)
            {
                MessageBox.Show("현재 작업중입니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!optionService.isValidate())
            {
                MessageBox.Show("세부설정은 최대 4개까지 설정 가능합니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(savePath) || string.IsNullOrEmpty(AddressText))
            {
                MessageBox.Show("빈칸을 모두 채워주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            KakaoService kakaoService = new KakaoService();
            List<string> locale = kakaoService.Search(AddressText);

            CaptureService captureService = new CaptureService();
            captureService.add += () => { ProgressValue += 1; };

            ProgressValue = 0;
            PictureCount = (int)Math.Pow((OptionDAO.GetInstance().zoomLevelIndex + 1) * 2 + 1, 2);
            isBusy = true;

            Task.Run(() => 
            {
                captureService.StartCapture(locale, SavePath);
                isBusy = false;
            });
        }

        /// <summary>
        /// 이미지 저장 경로 설정
        /// </summary>
        /// <param name="obj"></param>
        private void setSavePathMethod(object obj)
        {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SavePath = dialog.FileName;
            }
        }

        /// <summary>
        /// 설정 파일 불러오기
        /// </summary>
        /// <param name="obj"></param>
        private void loadSetMethod(object obj)
        {
            Option option = configService.Load();
            if (option == null)
            {
                MessageBox.Show("설정 파일이 존재하지 않습니다", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MapTypeIndex = option.mapTypeIndex;
            QualityIndex = option.qualityIndex;
            ZoomLevelIndex = option.zoomLevelIndex;
        }

        /// <summary>
        /// 설정 파일 저장하기
        /// </summary>
        /// <param name="obj"></param>
        private void saveSetMethod(object obj)
        {
            configService.Save();
        }
    }
}
