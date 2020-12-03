using MapShot_ver2.Models;
using System.Collections.ObjectModel;

namespace MapShot_ver2.DAO
{
    class OptionDAO
    {
        private Option option = new Option();
        private static OptionDAO Instance;

        public static OptionDAO GetInstance()
        {
            if(Instance == null)
            {
                Instance = new OptionDAO();
            }

            return Instance;
        }


        //public int quality 
        //{
        //    get { return option.quality; }
        //    set { option.quality = value; } 
        //}

        public int qualityIndex
        {
            get { return option.qualityIndex; }
            set { option.qualityIndex = value; }
        }

        //public int zoomLevel
        //{
        //    get { return option.zoomLevel; }
        //    set { option.zoomLevel = value; }
        //}

        public int zoomLevelIndex
        {
            get { return option.zoomLevelIndex; }
            set { option.zoomLevelIndex = value; }
        }

        //public MapTypeEnum mapType
        //{
        //    get { return option.mapType; }
        //    set { option.mapType = value; }
        //}
        public int mapTypeIndex
        {
            get { return option.mapTypeIndex; }
            set { option.mapTypeIndex = value; }
        }

        public ObservableCollection<DetailOption> detailOptions
        {
            get { return option.detailOptions; }
            set { option.detailOptions = value; }
        }
    }
}
