using MapShot_ver2.DAO;

namespace MapShot_ver2.Service
{
    class OptionService
    {

        public void CheckOption(string param)
        {
            foreach(var i in OptionDAO.GetInstance().detailOptions)
            {
                if(i.Title == param)
                {
                    i.Check = !i.Check;
                }
            }
        }

        public bool isValidate()
        {
            int count = 0;

            foreach(var i in OptionDAO.GetInstance().detailOptions)
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
