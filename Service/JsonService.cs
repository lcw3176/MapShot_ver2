using MapShot_ver2.DAO;
using MapShot_ver2.Models;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace MapShot_ver2.Service
{
    class JsonService
    {
        public void SetCollections()
        {
            JObject json = new JObject();
            JObject boundary = new JObject();
            boundary.Add("key", "boundary");
            boundary.Add("광역시도", "lt_c_adsido");
            boundary.Add("시군구", "lt_c_adsigg");
            boundary.Add("읍면동", "lt_c_ademd");

            JObject traffic = new JObject();
            traffic.Add("key", "traffic");
            traffic.Add("교통노드", "lt_p_moctnode");
            traffic.Add("교통링크", "lt_l_moctlink");

            JObject city = new JObject();
            city.Add("key", "city");
            city.Add("도시계획(공간시설)", "lt_c_upisuq153");
            city.Add("도시계획(공공문화체육시설)", "lt_c_upisuq155");
            city.Add("도시계획(교통시설)", "lt_c_upisuq152");
            city.Add("도시계획(기타기반시설)", "lt_c_upisuq159");
            city.Add("도시계획(도로)", "lt_c_upisuq151");
            city.Add("도시계획(방재시설)", "lt_c_upisuq156");
            city.Add("도시계획(유통공급시설)", "lt_c_upisuq154");
            city.Add("지구단위계획", "lt_c_upisuq161");
            city.Add("토지이용계획도", "lt_c_lhblpn");

            JObject industry = new JObject();
            industry.Add("key", "industry");
            industry.Add("단지경계", "lt_c_damdan");
            industry.Add("단지시설용지", "lt_c_damyoj");

            JObject water = new JObject();
            water.Add("key", "water");
            water.Add("대권역", "lt_c_wkmbbsn");
            water.Add("중권역", "lt_c_wkmmbsn");
            water.Add("표준권역", "lt_c_wkmsbsn");
            water.Add("하천망", "lt_c_wkmstrm");

            JObject usage = new JObject();
            usage.Add("key", "usage");
            usage.Add("취락지구", "lt_c_uq128");

            JObject mountain = new JObject();
            mountain.Add("key", "mountain");
            mountain.Add("산림입지도", "lt_c_fsdifrsts");
            mountain.Add("산지(보안림)", "lt_c_flisfk300");
            mountain.Add("산지(자연휴양림)", "lt_c_flisfk100");
            mountain.Add("산지(채종림)", " lt_c_flisfk200");

            JObject earth = new JObject();
            earth.Add("key", "earth");
            earth.Add("사업지구경계도", "lt_c_lhzone");
            earth.Add("지적도", "lp_pa_cbnd_bubun");

            JObject airport = new JObject();
            airport.Add("key", "airport");
            airport.Add("비행금지구역", "lt_c_aisprhc");
            airport.Add("비행장교통구역", "lt_c_aisatzc");
            airport.Add("비행정보구역", "lt_c_aisfirc");

            JObject fish = new JObject();
            fish.Add("key", "fish");
            fish.Add("국가산업단지", "lt_c_wgisiegug");
            fish.Add("농공단지", "lt_c_wgisienong");

            json.Add("boundary", boundary);
            json.Add("airport", airport);
            json.Add("city", city);
            json.Add("earth", earth);
            json.Add("fish", fish);
            json.Add("industry", industry);
            json.Add("mountain", mountain);
            json.Add("traffic", traffic);
            json.Add("usage", usage);
            json.Add("water", water);
            JsonKeyData key = new JsonKeyData();

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

                JsonDAO.GetInstance().jsonDictionary.Add(i, temp);

                foreach (var k in temp)
                {
                    OptionDAO.GetInstance().detailOptions.Add(k);
                }
            }

        }
    }
}
