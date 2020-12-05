using MapShot_ver2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace MapShot_ver2.DAO
{
    class OptionDAO
    {

        private const string path = "Resources/option.json";
        private static OptionDAO Instance;

        public static OptionDAO GetInstance()
        {
            if (Instance == null)
            {
                Instance = new OptionDAO();
            }

            return Instance;
        }

        public JObject SelectAll()
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (StreamReader value = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(value))
            {
                JObject json = (JObject)JToken.ReadFrom(reader);

                return json;
            }
        }

        public ObservableCollection<DetailOption> SelectAllDetailOptions()
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (StreamReader value = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(value))
            {
                JObject json = (JObject)JToken.ReadFrom(reader);
                ObservableCollection<DetailOption> temp = new ObservableCollection<DetailOption>();

                OptionKeyData key = new OptionKeyData();
                foreach (string i in key.Keys)
                {
                    string parentCollectionName = string.Empty;
                    

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
                }

                return temp;
            }
        }
    }
}
