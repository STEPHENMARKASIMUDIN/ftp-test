using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
[ServiceContract]
public interface IBiker
{
    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "checkConnection")]
    string checkConnection();

    //Jan 28, 2019
    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "BikerLogin")]
    BikerLogin BikerLogin(string pin, string walletno);
    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "BikersBranches/?walletno={walletno}")]
    BikersBranches BikersBranches(string walletno);

    #region Old Process
    //[OperationContract]
    //[WebInvoke(Method = "GET",
    //    RequestFormat = WebMessageFormat.Json,
    //    ResponseFormat = WebMessageFormat.Json,
    //    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    //    UriTemplate = "BranchItems/?branchcode={branchcode}&walletno={walletno}")]
    //BranchItems BranchItems(string branchcode, string walletno);

    //[OperationContract]
    //[WebInvoke(Method = "POST",
    //    RequestFormat = WebMessageFormat.Json,
    //    ResponseFormat = WebMessageFormat.Json,
    //    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    //    UriTemplate = "bookDeliveries")]
    //BookDeliveries bookDeliveries(string branchcode, string zonecode, string walletno, string custID);
    #endregion

    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "BikerDeliveries/?walletno={walletno}")]
    BikersDeliveries BikerDeliveries(string walletno);
    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "BikerHistory/?walletno={walletno}")]
    BikersDeliveries BikerHistory(string walletno);
    [OperationContract]
    [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "DisplayByLotNumber/?walletno={walletno}&lotnumber={lotnumber}")]
    BikersDeliveries DisplayByLotNumber(string walletno, string lotnumber);
    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "ScanCode")]
    BookDeliveries ScanCode(string code, string walletno, string orderid, string lineitemID);

    #region Old KYC and Approved Booking
    //[OperationContract]
    //[WebInvoke(Method = "GET",
    //    RequestFormat = WebMessageFormat.Json,
    //    ResponseFormat = WebMessageFormat.Json,
    //    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    //    UriTemplate = "VerifyRider/?qrcode={qrcode}")]
    //BikersDeliveries VerifyRider(string qrcode);
    //[OperationContract]
    //[WebInvoke(Method = "GET",
    //    RequestFormat = WebMessageFormat.Json,
    //    ResponseFormat = WebMessageFormat.Json,
    //    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    //    UriTemplate = "CheckRiderKYC/?walletno={walletno}")]
    //BikerLogin CheckRiderKYC(string walletno);
    //[WebInvoke(Method = "GET",
    //   RequestFormat = WebMessageFormat.Json,
    //   ResponseFormat = WebMessageFormat.Json,
    //   BodyStyle = WebMessageBodyStyle.WrappedRequest,
    //   UriTemplate = "ApproveBooking/?walletno={walletno}&qrcode={qrcode}")]
    //BikersDeliveries ApproveBooking(string walletno, string qrcode);
    #endregion

    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "CancelBooking")]
    BookDeliveries CancelBooking(string walletno, string branchcode, string zonecode);
    
    #region Old Cancellation
    //[OperationContract]
    //[WebInvoke(Method = "GET",
    //    RequestFormat = WebMessageFormat.Json,
    //    ResponseFormat = WebMessageFormat.Json,
    //    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    //    UriTemplate = "CancelDelivery/?orderid={orderid}&walletno={walletno}")]
    //BikersDeliveries CancelDelivery(string orderid, string walletno);
    #endregion

    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "RegisterBiker")]
    RegisterBiker RegisterBiker(Property.Registration bikerInfo);
    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "UpdateBiker")]
    RegisterBiker UpdateBiker(Property.Registration bikerInfo);

    #region Old BIker Approval
    //[OperationContract]
    //  [WebInvoke(Method = "GET",
    //    RequestFormat = WebMessageFormat.Json,
    //    ResponseFormat = WebMessageFormat.Json,
    //    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    //    UriTemplate = "ApprovedBiker/?walletno={walletno}&branchcode={branchcode}&zonecode={zonecode}&isApproved={isApproved}&operatorID={operatorID}")]
    //RegisterBiker ApprovedBiker(string walletno, string branchcode, string zonecode, bool isApproved, string operatorID);
    #endregion

    [OperationContract]
    [WebInvoke(Method = "POST",
        RequestFormat=WebMessageFormat.Json,
        ResponseFormat=WebMessageFormat.Json,
        BodyStyle=WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "RequestForBooking")]
    Indicators RequestForBooking(string walletno, string branchcode, string zonecode);

    #region Old Contract for Pending Bookings
    //[OperationContract]
    //[WebInvoke(Method = "GET",
    //    RequestFormat = WebMessageFormat.Json,
    //    ResponseFormat = WebMessageFormat.Json,
    //    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    //    UriTemplate = "DisplayRequestBooking/?walletno={walletno}")]
    //BikersBranches DisplayRequestBooking(string walletno);
    #endregion
}
[DataContract]
public class Indicators
{
    [DataMember]
    public int respcode { get; set; }
    [DataMember]
    public string respmsg { get; set; }
    [DataMember]
    public string errorDetails { get; set; }
    [DataMember]
    public string ErrorDetails { get; set; }
}



