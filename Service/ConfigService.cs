using MapShot_ver2.DAO;
using MapShot_ver2.Models;
using System;
using System.Configuration;
using System.Windows;

namespace MapShot_ver2.Service
{
    class ConfigService
    {

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
                    foreach (var j in OptionDAO.GetInstance().detailOptions)
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

        public void Save()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            KeyValueConfigurationCollection configCollection = config.AppSettings.Settings;
            configCollection.Clear();

            configCollection.Add("quality", OptionDAO.GetInstance().qualityIndex.ToString());
            configCollection.Add("zoomLevel", OptionDAO.GetInstance().zoomLevelIndex.ToString());
            configCollection.Add("mapType", OptionDAO.GetInstance().mapTypeIndex.ToString());

            foreach (var i in OptionDAO.GetInstance().detailOptions)
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
