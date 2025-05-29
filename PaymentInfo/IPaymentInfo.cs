using DataAccess.Model;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PaymentInfo
{
   [ServiceContract]
   public interface IPaymentInfo
    {
       [OperationContract]
       [WebGet(UriTemplate = "/Payment/{customerId}/{zoneId}",
           ResponseFormat = WebMessageFormat.Json)]
       PaymentsData GetPaymentInfo(string customerId, string zoneId);

       [OperationContract]
       [WebGet(UriTemplate = "/PaymentBySpace/{customerId}/{zoneId}/{spaceId}",
           ResponseFormat = WebMessageFormat.Json)]
       PaymentsData GetPaymentBySpaceInfo(string customerId, string zoneId, string spaceId);
    }
}
