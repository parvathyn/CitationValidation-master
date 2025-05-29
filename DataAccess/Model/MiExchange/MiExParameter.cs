
namespace DataAccess.Model.MiExchange
{
    public class MiExParameter
    {
        private string _plateNo;
        private string _zonelist;

        public MiExParameter(string plateNo, string zonelist)
        {
            this._plateNo = plateNo;
            this._zonelist = zonelist;
        }
        public string PlateNo
        {
            get
            {
                return this._plateNo;
            }
        }

        public string Zonelist 
        {
            get
            {
                return this._zonelist;
            }
        }
    }
}
