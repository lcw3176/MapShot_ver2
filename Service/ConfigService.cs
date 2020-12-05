using MapShot_ver2.DAO;
using MapShot_ver2.Models;
using System;
using System.Configuration;
using System.Windows;

namespace MapShot_ver2.Service
{
    class ConfigService
    {
        private OptionDAO optionDao = new OptionDAO();

        public Option Load()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection configCollection = config.AppSettings.Settings;
            
            if(configCollection["quality"] == null)
            {
                return null;
                 
            }

            int quality = int.Parse(configCollection["quality"].Value);
            int zoomLevel = int.Parse(configCollection["zoomLevel"].Value);
            int mapType = (int)Enum.Parse(typeof(MapTypeEnum), configCollection["mapType"].Value);

            if (configCollection["detail"] != null)
            {
                string[] details = configCollection["detail"].Value.Split(',');


                for (int i = 0; i < details.Length; i++)
                {
                    foreach (var j in optionDao.SelectAllDetailOptions())
                    {
                        if (j.Title == details[i])
                        {
                            j.Check = true;
                        }
                    }
                }

            }

            Option option = new Option();
            option.qualityIndex = quality;
            option.zoomLevelIndex = zoomLevel;
            option.mapTypeIndex = mapType;

            return option;
        }

        public void Save(Option option)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            KeyValueConfigurationCollection configCollection = config.AppSettings.Settings;
            configCollection.Clear();

            configCollection.Add("quality", option.qualityIndex.ToString());
            configCollection.Add("zoomLevel", option.zoomLevelIndex.ToString());
            configCollection.Add("mapType", option.mapTypeIndex.ToString());

            foreach (var i in option.detailOptions)
            {
                if (i.Check)
                {
                    configCollection.Add("detail", i.Title);
                }
                
            }

            config.Save(ConfigurationSaveMode.Modified);

        }

    }
}
