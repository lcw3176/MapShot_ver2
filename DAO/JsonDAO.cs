using MapShot_ver2.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapShot_ver2.DAO
{
    class JsonDAO
    {
        // key:구분 value:명칭들
        private Dictionary<string, ObservableCollection<DetailOption>> JsonDictionary = new Dictionary<string, ObservableCollection<DetailOption>>();
        public static JsonDAO Instance;

        public static JsonDAO GetInstance()
        {
            if (Instance == null)
            {
                Instance = new JsonDAO();
            }

            return Instance;
        }

        public Dictionary<string, ObservableCollection<DetailOption>> jsonDictionary
        {
            get { return JsonDictionary; }
            set { JsonDictionary = value; }
        }
    }
}
