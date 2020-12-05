using MapShot_ver2.DAO;
using MapShot_ver2.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapShot_ver2.Service
{
    class CaptureService
    {
        public delegate void AddProgress();
        public event AddProgress add;
        private Dictionary<int, string> fileName = new Dictionary<int, string>();
        private const int width = 1024;
        private const int height = 942;
        private decimal lngFlag = 0.011m;
        private decimal latFlag = 0.008m;
        private Option option;

        public CaptureService(Option option)
        {
            this.option = option;
        }

        public void StartCapture(List<string> locale, string path)
        {

            try
            {
                string place = locale[0];
                decimal firstLng = decimal.Parse(locale[1].Substring(0, 8));
                decimal firstLat = decimal.Parse(locale[2].Substring(0, 7));

                int blockNum = (int)Math.Pow((option.zoomLevelIndex  + 1) * 2 + 1, 2);
                int rotation = (int)Math.Sqrt(blockNum);
                decimal moveTopCoor = rotation / 2;

                string saveDirectory = Path.Combine(path, place);
                string zoom = (option.qualityIndex + 17).ToString();

                if (zoom == "18") { lngFlag /= 2; latFlag /= 2; saveDirectory += "[고화질]"; }

                firstLng -= lngFlag * moveTopCoor;
                firstLat += latFlag * moveTopCoor;
                DirectoryInfo di = new DirectoryInfo(saveDirectory);

                if (!di.Exists) { di.Create(); }

                List<Coor> coors = GetCoors(rotation, firstLng, firstLat);
                Parallel.ForEach(coors, i => GetImageFromUrl(i, saveDirectory, place));

                MakeOneShot(blockNum, saveDirectory, place);

                MessageBox.Show("작업이 완료되었습니다", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private List<Coor> GetCoors(int rotation, decimal lng, decimal lat)
        {
            List<Coor> coor = new List<Coor>();
            int index = 0;

            for (int i = 0; i < rotation; i++)
            {
                for (int j = 0; j < rotation; j++)
                {
                    coor.Add(new Coor()
                    {
                        index = index,
                        lng = lng,
                        lat = lat,
                    });
                    lng += lngFlag;
                    index++;
                }

                lng -= (lngFlag * rotation);
                lat -= latFlag;
            }

            return coor;
        }

        private void GetImageFromUrl(Coor coor, string saveDirectory, string place)
        {
            string url = "http://api.vworld.kr/req/image?service=image&request=getmap"; // &key=인증키&[요청파라미터]
            string key = "개발자 키";
            string baseMap = Enum.Parse(typeof(MapTypeEnum), option.mapTypeIndex.ToString()).ToString();
            string center = coor.lng + "," + coor.lat;
            string crs = "epsg:4326";
            string size = "1024,1024";
            string form = "jpeg";
            string zoom = (option.qualityIndex + 17).ToString();
            string query;

            query = string.Format("&key={0}&basemap={1}&center={2}&crs={3}&zoom={4}&size={5}&format={6}", key, baseMap, center, crs, zoom, size, form);
            StringBuilder sb = new StringBuilder();
            int count = 0;

            foreach (var i in option.detailOptions)
            {
                if (i.Check)
                {
                    count++;
                    break;
                }
            }

            if (count > 0)
            {
                foreach (var i in option.detailOptions)
                {
                    if (i.Check)
                    {
                        sb.Append(i.code);
                        sb.Append(",");
                    }

                }

                sb.Remove(sb.Length - 1, 1);

                query += string.Format("&layers={0}&styles={0}", sb.ToString());

            }

            string requestUrl = string.Format("{0}{1}", url, query);

            try
            {
                WebRequest request = WebRequest.Create(requestUrl);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();


                Bitmap bitmap = new Bitmap(data);
                string savePath = string.Format(Path.Combine(saveDirectory, string.Format("{0}[{1}].jpg", place, coor.index.ToString())));
                Bitmap clone = bitmap.Clone(new Rectangle(0, 0, width, height), PixelFormat.DontCare);
                clone.Save(savePath, ImageFormat.Jpeg);
                fileName.Add(coor.index, savePath);

                add();
                data.Dispose();
                clone.Dispose();
                bitmap.Dispose();
                response.Dispose();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MakeOneShot(int blockNum, string path, string addressFileName)
        {
            int root = (int)Math.Sqrt(blockNum);

            using (Bitmap bitmap = new Bitmap(width * root, height * root))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int i = 0; i < blockNum; i++)
                {
                    Image img = Image.FromFile(fileName[i]);
                    g.DrawImage(img, (i % root) * width, (i / root) * height);
                    img.Dispose();
                }

                bitmap.Save(Path.Combine(path, addressFileName + " 병합사진.jpg"), ImageFormat.Jpeg);

                fileName.Clear();
            }
        }
    }
}
