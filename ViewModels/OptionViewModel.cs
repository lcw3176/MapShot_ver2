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

namespace MapShot_ver2.ViewModels
{
    class OptionViewModel : BaseViewModel
    {
        private string addressText = string.Empty;
        private string savePath = string.Empty;
        private string stateText = "대기중";
        private int pictureCount = 1;
        private int progressValue = 0;
        ConfigService configService = new ConfigService();
        OptionService optionService = new OptionService();

        private Option option = new Option();

        public string StateText
        {
            get { return stateText; }
            set
            {
                stateText = value;
                OnPropertyChanged("StateText");
            }
        }

        public int ZoomLevelIndex
        {
            get { return option.zoomLevelIndex; }
            set
            {
                option.zoomLevelIndex = value;
                OnPropertyChanged("ZoomLevelIndex");
            }
        }

        public int MapTypeIndex
        {
            get { return option.mapTypeIndex; }
            set
            {
                option.mapTypeIndex = value;
                OnPropertyChanged("MapTypeIndex");
            }
        }


        public int QualityIndex
        {
            get { return option.qualityIndex; }
            set
            {
                option.qualityIndex = value;
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

        public Dictionary<string, ObservableCollection<DetailOption>> optionDict { get; set; }

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
            optionDict = optionService.GetOptionDict();
        }

        private ObservableCollection<DetailOption> GetCheckedOption()
        {
            ObservableCollection<DetailOption> temp = new ObservableCollection<DetailOption>();

            foreach(var i in optionDict.Values)
            {
                foreach(var j in i)
                {
                    if(j.Check)
                    {
                        temp.Add(j);
                    }
                }
            }

            return temp;
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
            option.detailOptions = GetCheckedOption();

            if (!optionService.isValidate(option))
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

            CaptureService captureService = new CaptureService(option);
            captureService.count += CountEvent;
            captureService.complete += CompleteEvent;

            ProgressValue = 0;
            PictureCount = (int)Math.Pow((option.zoomLevelIndex + 1) * 2 + 1, 2);
            isBusy = true;
            StateText = "사진 수집중";

            Task.Run(() => 
            {
                captureService.StartCapture(locale, SavePath);
                isBusy = false;
            });

            
        }

        /// <summary>
        /// 사진 캡쳐, 병합 과정이 모두 끝났을 시 발생 이벤트
        /// </summary>
        private void CompleteEvent()
        {
            StateText = "대기중";
            ProgressValue = 0;
            PictureCount = 1;
            MessageBox.Show("작업이 완료되었습니다", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 사진저장 혹은 사진병합 작업 하나하나 끝날때마다 이벤트 발생, ProgressVar Value를 더해준다
        /// </summary>
        private void CountEvent()
        {
            ProgressValue += 1;
            
            if(ProgressValue >= pictureCount)
            {
                ProgressValue = 0;
                StateText = "사진 병합중";
            }
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
            option.detailOptions = GetCheckedOption();
            configService.Save(option);
        }
    }
}
