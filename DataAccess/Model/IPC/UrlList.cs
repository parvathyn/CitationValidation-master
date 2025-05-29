namespace DataAccess.Model.IPC
{
    public class UrlList
    {
        public static string GetPlateStatusURL(IPCCustomerElement customer, IPCParameter parameter)
        {
            return string.Format("{0}", customer.BaseURL);
        }
    }
}
