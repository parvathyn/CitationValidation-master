using DataAccess;
using DataAccess.Model;
using DataAccess.Model.CitationValidation;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace CitationValidation
{
    [ServiceContract]
    public interface IValidateCitation
    {
        [OperationContract]
        [WebGet(UriTemplate = "/GetCitationData/{enfKey}",
            ResponseFormat = WebMessageFormat.Json)]
        TransactionTransform GetCitationData(string enfKey);


        [OperationContract]
        [WebGet(UriTemplate = "/GetCitationStatus/{enfKey}",
        ResponseFormat = WebMessageFormat.Json)]
        TransactionTransform GetCitationStatus(string enfKey);

        [OperationContract]
        [WebGet(UriTemplate = "/CitationTarget/{target}/{enfKey}",
        ResponseFormat = WebMessageFormat.Json)]
        TransactionTransform CitationTarget(string target, string enfKey);

        [OperationContract]
        [WebGet(UriTemplate = "/GetChicagoCitationData/{enfKey}",
            ResponseFormat = WebMessageFormat.Json)]
        ChicagoTransactionTransform GetChicagoCitationData(string enfKey);

        [OperationContract]
        [WebGet(UriTemplate = "/GetLogData/{customerId}/{folderName}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare)]
        string GetLogData(string customerId, string folderName);

        [OperationContract]
        [WebGet(UriTemplate = "/DownloadLogData/{customerId}/{folderName}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare)]
        Stream DownloadLogData(string customerId, string folderName);

        [OperationContract]
        [WebGet(UriTemplate = "/GetPlateNo/{customerId}/{plateNo}",
          ResponseFormat = WebMessageFormat.Json)]
        PlateNos GetPlateNo(string customerId, string plateNo);

        [OperationContract(Name = "GetPlateNoWithZone")]
        [WebGet(UriTemplate = "/GetPlateNo/{customerId}/{plateNo}/{zoneid}",ResponseFormat = WebMessageFormat.Json)]
        PlateNos GetPlateNo(string customerId, string plateNo, string zoneid);

        [OperationContract]
        [WebGet(UriTemplate = "/GetPlateLogData/{customerId}/{folderName}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare)]
        string GetPlateLogData(string customerId, string folderName);

        [OperationContract]
        [WebGet(UriTemplate = "/GetPlateLogData/{customerId}/{folderName}/{hour}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare)]
        string GetPlateLogDataSplited(string customerId, string folderName, string hour);


  
        [OperationContract]
        [WebGet(UriTemplate = "/Test/{enfKey}", BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
        Stream Test(string enfKey);




        [OperationContract]
        [WebGet(UriTemplate = "/getallactivepurchases/{firstParameter}/{secondParamter}", BodyStyle = WebMessageBodyStyle.Bare,
          ResponseFormat = WebMessageFormat.Xml)]
        Stream getallactivepurchases(string firstParameter, string secondParamter);
       

    }
}
