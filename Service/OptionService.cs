using MapShot_ver2.DAO;
using MapShot_ver2.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapShot_ver2.Service
{
    class OptionService
    {
        private OptionDAO optionDao = new OptionDAO();

        public Dictionary<string, ObservableCollection<DetailOption>> GetOptionDict()
        {
            JObject json = optionDao.SelectAll();
            OptionKeyData key = new OptionKeyData();
            Dictionary<string, ObservableCollection<DetailOption>> dict = new Dictionary<string, ObservableCollection<DetailOption>>();

            foreach (string i in key.Keys)
            {
                string parentCollectionName = string.Empty;
                ObservableCollection<DetailOption> temp = new ObservableCollection<DetailOption>();

                foreach (var j in JObject.Parse(json[i].ToString()))
                {
                    if (j.Key == "key")
                    {
                        parentCollectionName = j.Value.ToString();
                        continue;
                    }

                    temp.Add(new DetailOption()
                    {
                        Check = false,
                        Title = j.Key,
                        code = j.Value.ToString(),
                        collectionName = parentCollectionName,
                    });
                }

                dict.Add(i, temp);

            }

            return dict;

        }

        public ObservableCollection<DetailOption> GetAllDetailOptions()
        {
            return OptionDAO.GetInstance().SelectAllDetailOptions();
        }


        public bool isValidate(Option option)
        {
            int count = 0;

            foreach(var i in option.detailOptions)
            {
                if (i.Check)
                {
                    count++;
                }
            }

            if(count > 4)
            {
                return false;
            }

            return true;
        }

    }
}
