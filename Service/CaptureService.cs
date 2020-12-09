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
        public delegate void CompleteEvent();
        public event CompleteEvent count;
        public event CompleteEvent complete;

        private Dictionary<int, string> fileName = new Dictionary<int, string>();
        private const int width = 1024;   // 1024 1024
        private int height = 942;   // 1024 922 942
        private decimal lngMoveToRight = 0.011m;
        private decimal latMoveToBottom = 0.008m;
        private decimal prefixLocation = 37.7623m; // 이 지역을 기준으로 보정
        private decimal prefixCoor = 0.091185m; // 아래지역으로 내려갈수록 뭔가 이미지 오차가 생긴다. 보정값

        private Option option;

        public CaptureService(Option option)
        {
            this.option = option;
        }

        /// <summary>
        /// 캡쳐 시작
        /// </summary>
        /// <param name="locale">위경도, 장소</param>
        /// <param name="path">사용자 지정 저장 경로</param>
        public void StartCapture(List<string> locale, string path)
        {

            try
            {
                string place = locale[0];
                decimal firstLng = decimal.Parse(locale[1].Substring(0, 8));
                decimal firstLat = decimal.Parse(locale[2].Substring(0, 7));

                // height 보정하기
                height -= (int)Math.Round((Math.Abs(firstLat - prefixLocation) / prefixCoor));

                int blockNum = (int)Math.Pow((option.zoomLevelIndex  + 1) * 2 + 1, 2);
                int rotation = (int)Math.Sqrt(blockNum);

                string saveDirectory = Path.Combine(path, place);
                string zoom = (option.qualityIndex + 17).ToString();

                if (zoom == "18") { lngMoveToRight /= 2; latMoveToBottom /= 2; saveDirectory += "[고화질]"; }

                firstLng -= lngMoveToRight * (rotation / 2);
                firstLat += latMoveToBottom * (rotation / 2);
                DirectoryInfo di = new DirectoryInfo(saveDirectory);

                if (!di.Exists) { di.Create(); }

                List<Coor> coors = GetCoors(rotation, firstLng, firstLat);
                Parallel.ForEach(coors, i => GetImageFromUrl(i, saveDirectory, place));

                MakeOneShot(blockNum, saveDirectory, place);
                height = 942;
                complete();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 중심 좌표값을 이용해서 인근 구역 위경도값 가져오기
        /// </summary>
        /// <param name="rotation">반복 횟수(지도의 한 변의 사진갯수)</param>
        /// <param name="lng">경도</param>
        /// <param name="lat">위도</param>
        /// <returns></returns>
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
                    lng += lngMoveToRight;
                    index++;
                }

                lng -= (lngMoveToRight * rotation);
                lat -= latMoveToBottom;
            }

            return coor;
        }

        /// <summary>
        /// V-World에 해당 이미지 요청
        /// </summary>
        /// <param name="coor">좌표 클래스</param>
        /// <param name="saveDirectory">저장 디렉토리 위치</param>
        /// <param name="place">캡쳐한 지역명(파일 저장명에 사용)</param>
        private void GetImageFromUrl(Coor coor, string saveDirectory, string place)
        {   
            string url = "http://api.vworld.kr/req/image?service=image&request=getmap"; // &key=인증키&[요청파라미터]
            string key = "개발자 키";
            string baseMap = Enum.Parse(typeof(MapTypeEnum), option.mapTypeIndex.ToString()).ToString();
            string center = coor.lng + "," + coor.lat;
            string crs = "EPSG:4326";
            string size = "1024,1024";
            string form = "jpeg";
            string zoom = (option.qualityIndex + 17).ToString();
            string query;

            query = string.Format("" +
                "&key={0}" +
                "&basemap={1}" +
                "&center={2}" +
                "&crs={3}" +
                "&zoom={4}" +
                "&size={5}" +
                "&format={6}", key, baseMap, center, crs, zoom, size, form);

            StringBuilder sb = new StringBuilder();

            foreach (var i in option.detailOptions)
            {
                if (i.Check)
                {
                    sb.Append(i.code);
                    sb.Append(",");
                }

            }

            if(sb.Length >= 1)
            {
                sb.Remove(sb.Length - 1, 1);
                query += string.Format("" +
                    "&layers={0}" +
                    "&styles={0}", sb.ToString());
            }
            

            string requestUrl = string.Format("{0}{1}", url, query);
            string savePath = string.Format(Path.Combine(saveDirectory, string.Format("{0}[{1}].jpg", place, coor.index.ToString())));
            
            try
            {
                WebRequest request = WebRequest.Create(requestUrl);

                using (WebResponse response = request.GetResponse())
                using(Stream data = response.GetResponseStream())
                using (Bitmap bitmap = new Bitmap(data))
                using (Bitmap clone = bitmap.Clone(new Rectangle(0, 0, width, height), PixelFormat.DontCare))
                {
                    clone.Save(savePath, ImageFormat.Jpeg);
                    fileName.Add(coor.index, savePath);
                    count();
                }

  
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 여러개 사진 하나로 붙이기
        /// </summary>
        /// <param name="blockNum">총 사진 갯수</param>
        /// <param name="path">저장 디렉토리 경로</param>
        /// <param name="place">캡쳐한 지역명</param>
        private void MakeOneShot(int blockNum, string path, string place)
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
                    count();
                }

                bitmap.Save(Path.Combine(path, place + " 병합사진.jpg"), ImageFormat.Jpeg);

                fileName.Clear();
            }
        }
    }
}
