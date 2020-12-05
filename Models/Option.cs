using System.Collections.ObjectModel;

namespace MapShot_ver2.Models
{
    class Option 
    {
        public int qualityIndex { get; set; } = 0;

        public int zoomLevelIndex { get; set; } = 0;

        public int mapTypeIndex { get; set; } = 4;
        public ObservableCollection<DetailOption> detailOptions { get; set; } = new ObservableCollection<DetailOption>();
    }
}
