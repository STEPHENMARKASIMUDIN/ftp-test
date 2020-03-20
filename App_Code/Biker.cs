using log4net;
using log4net.Config;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;


// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Biker : IBiker
{
    #region private variables
    private readonly Queries _Query = new Queries();

    private readonly string path;
    private static readonly ILog kplog = LogManager.GetLogger(typeof(Biker));
    private readonly DBConnection _ecomdbCon;
    private readonly DBConnection _walletCon;
    private readonly DBConnection _domesticCon;
    private readonly string _bikerftphost;
    private readonly string _ftpusername;
    private readonly string _ftppassword;



    private readonly int DB_DEADLOCK_RETRY_COUNT = 3;
    #endregion

    public Biker()
    {
        try
        {
            XmlConfigurator.Configure();

            path = "C:\\kpconfig\\ECommerceConf.ini";

            IniFile ini = new IniFile(path);
            string Serv = ini.IniReadValue("DBConfig ECommerce", "server");
            string DB = ini.IniReadValue("DBConfig ECommerce", "database");
            string UID = ini.IniReadValue("DBConfig ECommerce", "uid");
            string Password = ini.IniReadValue("DBConfig ECommerce", "password");
            string pool = ini.IniReadValue("DBConfig ECommerce", "pool");
            int maxcon = Convert.ToInt32(ini.IniReadValue("DBConfig ECommerce", "maxcon"));
            int mincon = Convert.ToInt32(ini.IniReadValue("DBConfig ECommerce", "mincon"));
            int tout = Convert.ToInt32(ini.IniReadValue("DBConfig ECommerce", "tout"));
            _ecomdbCon = new DBConnection(Serv, DB, UID, Password, pool, maxcon, mincon, tout);// DBDatabase.Create("server=" + Serv + ";database=" + DB + ";User Id=" + UID + ";Password=" + Password + ";Persist Security Info=True", "MySql.Data.MySqlClient");

            _bikerftphost = ini.IniReadValue("Ftp Biker", "ftp");
            _ftpusername = ini.IniReadValue("Ftp Biker", "Username");
            _ftppassword = ini.IniReadValue("Ftp Biker", "Password");


            string Serv1 = ini.IniReadValue("DBConfig mobile", "server");
            string DB1 = ini.IniReadValue("DBConfig mobile", "database");
            string UID1 = ini.IniReadValue("DBConfig mobile", "uid");
            string Password1 = ini.IniReadValue("DBConfig mobile", "password");
            string pool1 = ini.IniReadValue("DBConfig mobile", "pool");
            int maxcon1 = Convert.ToInt32(ini.IniReadValue("DBConfig mobile", "maxcon"));
            int mincon1 = Convert.ToInt32(ini.IniReadValue("DBConfig mobile", "mincon"));
            int tout1 = Convert.ToInt32(ini.IniReadValue("DBConfig mobile", "tout"));
            _walletCon = new DBConnection(Serv1, DB1, UID1, Password1, pool1, maxcon1, mincon1, tout1);// DBDatabase.Create("server=" + Serv + ";database=" + DB + ";User Id=" + UID + ";Password=" + Password + ";Persist Security Info=True", "MySql.Data.MySqlClient");

            string Serv2 = ini.IniReadValue("DBConfig Domestic", "server");
            string DB2 = ini.IniReadValue("DBConfig Domestic", "database");
            string UID2 = ini.IniReadValue("DBConfig Domestic", "uid");
            string Password2 = ini.IniReadValue("DBConfig Domestic", "password");
            string pool2 = ini.IniReadValue("DBConfig Domestic", "pool");
            int maxcon2 = Convert.ToInt32(ini.IniReadValue("DBConfig Domestic", "maxcon"));
            int mincon2 = Convert.ToInt32(ini.IniReadValue("DBConfig Domestic", "mincon"));
            int tout2 = Convert.ToInt32(ini.IniReadValue("DBConfig Domestic", "tout"));
            _domesticCon = new DBConnection(Serv2, DB2, UID2, Password2, pool2, maxcon2, mincon2, tout2);// DBDatabase.Create("server=" + Serv + ";database=" + DB + ";User Id=" + UID + ";Password=" + Password + ";Persist Security Info=True", "MySql.Data.MySqlClient");



        }
        catch (Exception ex)
        {
            kplog.Fatal(String.Format("{0}{1}", "Constructor Error: ", ex.ToString()));
            throw new Exception(ex.Message);
        }
    }

    ~Biker()
    {
        if (_ecomdbCon != null) _ecomdbCon.Dispose("DAPPER");
        if (_walletCon != null) _walletCon.Dispose("DAPPER");
        if (_domesticCon != null) _domesticCon.Dispose("DAPPER");
    }
    public Queries GetQueries()
    {
        return _Query;
    }
    /*<summary>
     Connection Test Ecommerce ecommercelogs DB(19.222) and Domestic kpforms DB(19.205) and Wallet DB (19.67) MySql database connection
      <summary>*/
    public string checkConnection()
    {
        string response = string.Empty;
        try
        {
            _ecomdbCon.OpenConnection();
            kplog.Info(" [Success in Connecting to ECommerceLogs DB] ");
            response += " [Success in Connecting to ECommerceLogs DB] ";
        }
        catch (Exception ecx)
        {
            kplog.Info(String.Format("{0}{1}", " [Unable to Connect to ECommerceLogs DB] ", ecx.ToString()));
            response += " [Unable to Connect to ECommerceLogs DB] : " + ecx.ToString();
        }
        try
        {
            _walletCon.OpenConnection();
            kplog.Info(" [Success in Connecting to Wallet DB] ");
            response += " [Success in Connecting to Wallet DB] ";
        }
        catch (Exception ecx)
        {
            kplog.Info(String.Format("{0}{1}", " [Unable to Connect to Wallet DB] ", ecx.ToString()));
            response += " [Unable to Connect to Wallet DB] : " + ecx.ToString();
        }
        try
        {
            _domesticCon.OpenConnection();
            kplog.Info(" [Success in Connecting to Domestic DB] ");
            response += " [Success in Connecting to Domestic DB] ";
        }
        catch (Exception ecx)
        {
            kplog.Info(String.Format("{0}{1}", " [Unable to Connect to Domestic DB] ", ecx.ToString()));
            response += " [Unable to Connect to Domestic DB] : " + ecx.ToString();
        }

        _walletCon.Dispose("TEST");
        _ecomdbCon.Dispose("TEST");
        _domesticCon.Dispose("TEST");

        return response;
    }

    private DateTime Getdate()
    {
        try
        {
            DateTime serverdate = Convert.ToDateTime("1986-05-29");
            _ecomdbCon.OpenConnection();
            var response = _ecomdbCon.Query<dynamic>("select now() as serverdate", null, 60).Single();
            serverdate = Convert.ToDateTime(response.serverdate);
            _ecomdbCon.CloseConnection();
            return serverdate;
        }
        catch (Exception ex)
        {
            kplog.Fatal(ex.ToString());
            _ecomdbCon.Dispose("DAPPER");
            throw new Exception(ex.Message);
        }

    }

    //January 29, 2019
    //ML Bikers
    public BikerLogin BikerLogin(string pin, string walletno)
    {

        try
        {
            //check pin in wallet db
            _walletCon.OpenConnection();
            List<string> checkPin = _walletCon.Query<string>(_Query.CheckPin, new { pin, walletno }, 60).ToList();
            if (checkPin.Count < 1)
            {
                kplog.Error(string.Format("Invalid Pin: {0}", pin));
                _walletCon.Dispose("DAPPER");
                return new BikerLogin { respcode = 0, respmsg = "Invalid pin number. Please Try Again." };
            }
            _walletCon.CloseConnection();

            _ecomdbCon.OpenConnection();

            List<Property.BikersInfo> bikersData = _ecomdbCon.Query<Property.BikersInfo>(_Query.BikersData, new { walletno }, 60).ToList();
            if (bikersData.Count < 1)
            {
                _ecomdbCon.Dispose("DAPPER");
                _walletCon.Dispose("DAPPER");
                kplog.Error(String.Format("Invalid Credentials! walletno: {0}", walletno));
                return new BikerLogin { respcode = 0, respmsg = "You need to register as ML Biker to continue. Thank you!" };
            }
            _ecomdbCon.Dispose("DAPPER");
            _walletCon.Dispose("DAPPER");
            return new BikerLogin { respcode = 1, respmsg = RespMessage(1), bikersInfo = bikersData[0] };
        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(5), tex.ToString()));
            _ecomdbCon.Dispose("DAPPER");
            _walletCon.Dispose("DAPPER");
            return new BikerLogin { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #01: " + String.Format("{0}{1}", RespMessage(3), ex.ToString()));
            _ecomdbCon.Dispose("DAPPER");
            _walletCon.Dispose("DAPPER");
            return new BikerLogin { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
        }
    }

    //Bikers assinged branches
    //after biker to branch registration
    //will display only if there are available home delivery orders
    //updated October 4, 2019
    public BikersBranches BikersBranches(string walletno)
    {
        try
        {

            var dt = Getdate();
            var year = dt.ToString("yyyy");
            var oldYear = (Convert.ToInt32(dt.ToString("yyyy")) - 1).ToString();
            List<Property.BranchDetails> BranchDetailsFinal = new List<Property.BranchDetails>();
            List<Property.BranchDetails> branchDetailsOld = new List<Property.BranchDetails>();
            _ecomdbCon.OpenConnection();
            List<Property.BranchDetails> branchDetails = _ecomdbCon.Query<Property.BranchDetails>(_Query.GetBranchDeliveries(year), new { walletno }, 60).ToList();
            if (branchDetails.Count < 1)
            {
                branchDetailsOld = _ecomdbCon.Query<Property.BranchDetails>(_Query.GetBranchDeliveries(oldYear), new { walletno }, 60).ToList();
                if (branchDetailsOld.Count < 1)
                {
                    _ecomdbCon.Dispose("DAPPER");
                    kplog.Error(String.Format("No Branches Available! walletno: {0}", walletno));
                    return new BikersBranches { respcode = 0, respmsg = "No Branches Available!" };
                }
            }
            BranchDetailsFinal = branchDetails.Count > 0 ? branchDetails : branchDetailsOld;
            for (int i = 0; i <= BranchDetailsFinal.Count - 1; i++)
            {
                List<dynamic> checkBooking = _ecomdbCon.Query<dynamic>(_Query.CheckPending, new { BranchDetailsFinal[i].branchcode, BranchDetailsFinal[i].zonecode, walletno }).ToList();
                if (checkBooking.Count < 1)
                {
                    BranchDetailsFinal[i].isBooked = "NULL";
                }
                else
                {
                    BranchDetailsFinal[i].isBooked = checkBooking[0].isBooked;
                }
            }

            _ecomdbCon.Dispose("DAPPER");
            return new BikersBranches { respcode = 1, respmsg = RespMessage(1), bikersBranches = BranchDetailsFinal };
        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(5), tex.ToString()));
            _ecomdbCon.Dispose("DAPPER");
            return new BikersBranches { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #01: " + String.Format("{0}{1}", RespMessage(3), ex.ToString()));
            _ecomdbCon.Dispose("DAPPER");
            return new BikersBranches { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
        }
    }

    //Request for booking
    //updated October 4, 2019
    public Indicators RequestForBooking(string walletno, string branchcode, string zonecode)
    {
        kplog.Info(string.Format("walletno :{0} branchcode: {1} zonecode: {2}", walletno, branchcode, zonecode));
        try
        {
            _ecomdbCon.OpenConnection();
            List<dynamic> checkBooking = _ecomdbCon.Query<dynamic>(_Query.CheckRequestBooking, new { walletno, branchcode, zonecode }, 60).ToList();
            if (checkBooking.Count > 0)
            {
                if (checkBooking[0].isBooked == 1)
                    return new Indicators { respcode = 0, respmsg = "Your are already booked on this branch." };
                else if (checkBooking[0].isBooked == 2)
                    return new Indicators { respcode = 0, respmsg = "Your request was disapproved by the branch." };
                else if (Convert.ToInt32(checkBooking[0].isActive) == 1 && checkBooking[0].isBooked == 0)
                    return new Indicators { respcode = 0, respmsg = "Your request was already sent. Please wait for branch approval. Thank you." };
                else if (Convert.ToInt32(checkBooking[0].isActive) == 0 && checkBooking[0].isBooked == 0 && checkBooking[0].total_delivery > 0)
                {
                    _ecomdbCon.StartDapperTran();
                    int requestBookingAgain = _ecomdbCon.Execute(_Query.RebookBranch, new { walletno, branchcode, zonecode }, 60);
                    if (requestBookingAgain < 1)
                    {
                        _ecomdbCon.RollbackTran();
                        _ecomdbCon.Dispose("DAPPER");
                        kplog.Error(string.Format("walletno: {0} branchcode: {1} Unable to rebook biker request!", walletno, branchcode));
                        return new Indicators { respcode = 0, respmsg = RespMessage(3) };
                    }
                    _ecomdbCon.CommitTran();
                    _ecomdbCon.Dispose("DAPPER");
                    return new Indicators { respcode = 1, respmsg = "Your Request is successful. Please wait for ML Shop Branch approval. Thank you!" };
                }
            }

            List<dynamic> BranchDetails = _ecomdbCon.Query<dynamic>(_Query.GetApprovedBranchDetails, new { walletno, branchcode, zonecode }, 60).ToList();
            if (BranchDetails.Count < 1)
            {
                kplog.Error(string.Format("walletno: {0} branchcode: {1} You are not booked on this branch", walletno, branchcode));
                _ecomdbCon.Dispose("DAPPER");
                return new Indicators { respcode = 0, respmsg = "You are not booked on this branch." };
            }

            _ecomdbCon.StartDapperTran();

            Dictionary<string, object> SaveNewBookingRequest = new Dictionary<string, object>() 
            {
                {"walletno",walletno},
                {"branchcode",branchcode},
                {"branchname",BranchDetails[0].branchname},
                {"branchaddress",BranchDetails[0].branchaddress},
                {"zonecode",zonecode},
                {"phone",BranchDetails[0].phone},
                {"latitude",BranchDetails[0].latitude},
                {"longitude",BranchDetails[0].longitude}                
            };
            int requestBook = _ecomdbCon.Execute(_Query.InsertBookingRequest, SaveNewBookingRequest, 60);
            if (requestBook < 1)
            {
                _ecomdbCon.RollbackTran();
                _ecomdbCon.Dispose("DAPPER");
                kplog.Error(string.Format("walletno: {0} branchcode: {1} Unable to save biker request!", walletno, branchcode));
                return new Indicators { respcode = 0, respmsg = RespMessage(3) };
            }
            _ecomdbCon.CommitTran();
            _ecomdbCon.Dispose("DAPPER");
            return new Indicators { respcode = 1, respmsg = "Your Request is successful. Please wait for ML Shop Branch approval. Thank you!" };
        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(5), tex.ToString()));
            _ecomdbCon.RollbackTran();
            _ecomdbCon.Dispose("DAPPER");
            return new Indicators { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #01: " + string.Format("{0}{1}", RespMessage(3), ex.ToString()));
            _ecomdbCon.RollbackTran();
            _ecomdbCon.Dispose("DAPPER");
            return new Indicators { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
        }
    }

    #region Old Code for PendingBookings
    ////Pending/Verified bookings
    ////October 7, 2019
    //public BikersBranches DisplayRequestBooking(string walletno)
    //{
    //    try
    //    {


    //        _ecomdbCon.OpenConnection();
    //        List<Property.BranchDetails> pendingRequest = _ecomdbCon.Query<Property.BranchDetails>(_Query.PendingRequest, new { walletno }, 60).ToList();
    //        if (pendingRequest.Count < 1)
    //        {
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Error(string.Format("No Deliveries Available! walletno: {0}", walletno));
    //            return new BikersBranches { respcode = 0, respmsg = "No Deliveries Available!" };
    //        }
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersBranches { respcode = 1, respmsg = RespMessage(1), bikersBranches = pendingRequest };
    //    }
    //    catch (TimeoutException tex)
    //    {
    //        kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(5), tex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersBranches { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //    }
    //    catch (Exception ex)
    //    {
    //        kplog.Fatal("Error #01: " + string.Format("{0}{1}", RespMessage(3), ex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersBranches { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //    }
    //}
    #endregion

    #region Old Branch Items
    // Available Items   
    //public BranchItems BranchItems(string branchcode, string walletno)
    //{
    //    try
    //    {

    //        var dt = Getdate();
    //        var year = dt.ToString("yyyy");
    //        var oldYear = (Convert.ToInt32(dt.ToString("yyyy")) - 1).ToString();
    //        List<Property.BranchItemsDetails> branchDeliveryOld = new List<Property.BranchItemsDetails>();
    //        _ecomdbCon.OpenConnection();
    //        List<Property.BranchItemsDetails> branchDelivery = _ecomdbCon.Query<Property.BranchItemsDetails>(_Query.GetBikerDeliveries(year), new { walletno, branchcode }, 60).ToList();
    //        if (branchDelivery.Count < 1)
    //        {
    //            branchDeliveryOld = _ecomdbCon.Query<Property.BranchItemsDetails>(_Query.GetBikerDeliveries(oldYear), new { walletno, branchcode }, 60).ToList();
    //            if (branchDeliveryOld.Count < 1)
    //            {
    //                _ecomdbCon.Dispose("DAPPER");
    //                kplog.Error(string.Format("No Deliveries Available! walletno: {0}", branchcode));
    //                return new BranchItems { respcode = 0, respmsg = "No Deliveries Available!" };
    //            }
    //        }
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BranchItems { respcode = 1, respmsg = RespMessage(1), branchItems = branchDelivery.Count < 1 ? branchDeliveryOld : branchDelivery };
    //    }
    //    catch (TimeoutException tex)
    //    {
    //        kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(5), tex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BranchItems { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //    }
    //    catch (Exception ex)
    //    {
    //        kplog.Fatal("Error #01: " + string.Format("{0}{1}", RespMessage(3), ex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BranchItems { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //    }
    //}
    #endregion

    #region Old Booking
    ////Book Order here
    ////Generate QR Code(CustID)
    //public BookDeliveries bookDeliveries(string branchcode, string zonecode, string walletno, string custID)
    //{
    //    try
    //    {



    //        var dt = Getdate();
    //        var year = dt.ToString("yyyy");
    //        var oldYear = (Convert.ToInt32(dt.ToString("yyyy")) - 1).ToString();
    //        List<Property.DeliveryModels> forBookingFinal = new List<Property.DeliveryModels>();
    //        List<Property.DeliveryModels> forBookingOld = new List<Property.DeliveryModels>();

    //        _ecomdbCon.OpenConnection();

    //        List<string> checkBiker = _ecomdbCon.Query<string>(_Query.CheckRider, new { walletno }, 60).ToList();
    //        if (checkBiker.Count < 1)
    //        {
    //            kplog.Error(string.Format("Rider {0} is not available", walletno));
    //            _ecomdbCon.Dispose("DAPPER");
    //            return new BookDeliveries { respcode = 0, respmsg = "Please check if you still have a pending delivery." };
    //        }
    //        List<Property.DeliveryModels> forBooking = _ecomdbCon.Query<Property.DeliveryModels>(_Query.ForBooking(year),
    //                                                    new { zonecode, walletno, branchcode }).ToList();

    //        if (forBooking.Count < 1)
    //        {
    //            forBookingOld = _ecomdbCon.Query<Property.DeliveryModels>(_Query.ForBooking(oldYear),
    //                                                  new { zonecode, walletno, branchcode }).ToList();
    //            if (forBooking.Count < 1)
    //            {
    //                kplog.Error("No data Found!");
    //                _ecomdbCon.Dispose("DAPPER");
    //                return new BookDeliveries { respcode = 0, respmsg = "No Data Found!" };
    //            }
    //        }

    //        forBookingFinal = forBooking.Count < 1 ? forBookingOld : forBooking;
    //        string insertBookings = string.Empty;
    //        Dictionary<string, object> BookingParameters = new Dictionary<string, object>();
    //        for (int no = 0; no <= forBookingFinal.Count - 1; no++)
    //        {
    //            insertBookings += "(@walletno" + no + ",@orderid" + no + ",@lineitemID" + no + ",@description" + no + ",@amount" + no
    //                      + ",@custName" + no + ",@custAddress" + no + ",@custPhone" + no + ",@custid" + no +
    //                      ",@branchcode" + no + ",@branchname" + no + ",@zonecode" + no + ",@branchAddress" + no + ",@operatorID" + no
    //                      + ",@transdate" + no + ",@KPTN" + no + ",@qrcode" + no + ",@status" + no
    //                      + ",@sysmodified" + no + ",@syscreated" + no + "),";

    //            BookingParameters.Add("walletno" + no, walletno);
    //            BookingParameters.Add("orderid" + no, forBookingFinal[no].orderID);
    //            BookingParameters.Add("lineitemID" + no, forBookingFinal[no].lineitemID);
    //            BookingParameters.Add("description" + no, forBookingFinal[no].orderdetails);
    //            BookingParameters.Add("amount" + no, 0);
    //            BookingParameters.Add("custName" + no, forBookingFinal[no].custName);
    //            BookingParameters.Add("custAddress" + no, forBookingFinal[no].custAddress);
    //            BookingParameters.Add("custPhone" + no, forBookingFinal[no].custContact);
    //            BookingParameters.Add("custid" + no, forBookingFinal[no].custWalletno);
    //            BookingParameters.Add("branchcode" + no, branchcode);
    //            BookingParameters.Add("branchname" + no, forBookingFinal[no].branchname);
    //            BookingParameters.Add("zonecode" + no, forBookingFinal[no].zonecode);
    //            BookingParameters.Add("branchAddress" + no, forBookingFinal[no].branchaddress);
    //            BookingParameters.Add("operatorID" + no, forBookingFinal[no].operatorID);
    //            BookingParameters.Add("transdate" + no, forBookingFinal[no].transdate);
    //            BookingParameters.Add("KPTN" + no, forBookingFinal[no].kptnno);
    //            BookingParameters.Add("qrcode" + no, custID);
    //            BookingParameters.Add("status" + no, "Pending");
    //            BookingParameters.Add("sysmodified" + no, dt);
    //            BookingParameters.Add("syscreated" + no, dt);
    //        }
    //        insertBookings = insertBookings.Substring(0, insertBookings.Length - 1);
    //        insertBookings += ";";

    //        int bookAttempt = 0;

    //        kplog.Info("Ready for Booking: " + JsonConvert.SerializeObject(forBookingFinal));

    //        do
    //        {
    //            try
    //            {
    //                bookAttempt++;
    //                _ecomdbCon.StartDapperTran();
    //                int booking = _ecomdbCon.Execute(_Query.InsertBookings(insertBookings), BookingParameters, 60);
    //                if (booking < 1)
    //                {
    //                    _ecomdbCon.RollbackTran();
    //                    _ecomdbCon.Dispose("DAPPER");
    //                    kplog.Error(string.Format("Branchcode: {0} Unable to Save Data!", branchcode));
    //                    return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
    //                }
    //                int updateStatus = _ecomdbCon.Execute(_Query.UpdateBikerStatus, new { walletno, status = 2 }, 60);
    //                if (updateStatus < 1)
    //                {
    //                    _ecomdbCon.RollbackTran();
    //                    _ecomdbCon.Dispose("DAPPER");
    //                    kplog.Error(string.Format("Walletno: {0} Unable Update Rider!", walletno));
    //                    return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
    //                }

    //                kplog.Info(string.Format("Success Booking for Branch: {0} and Biker: {1}", branchcode, walletno));
    //                _ecomdbCon.CommitTran();
    //                _ecomdbCon.Dispose("DAPPER");
    //                return new BookDeliveries { respcode = 1, respmsg = RespMessage(1), qrcode = custID };
    //            }
    //            catch (MySqlException mex)
    //            {
    //                _ecomdbCon.RollbackTran();
    //                kplog.Fatal(mex.ToString());
    //                if (bookAttempt <= DB_DEADLOCK_RETRY_COUNT)
    //                {
    //                    switch (mex.Number)
    //                    {
    //                        case 1213:
    //                            kplog.Fatal(walletno + " - " + "(ER_LOCK_DEADLOCK) Deadlock found when trying to get lock. ATTEMPTCOUNT = " + bookAttempt, mex);
    //                            Thread.Sleep(bookAttempt * 2000);
    //                            break;
    //                        case 1205:
    //                            kplog.Fatal(walletno + " - " + "(ER_LOCK_WAIT_TIMEOUT) Lock wait timeout exceeded; try restarting transaction. ATTEMPTCOUNT = " + bookAttempt, mex);
    //                            Thread.Sleep(bookAttempt * 2000);
    //                            break;
    //                        default:
    //                            Thread.Sleep(bookAttempt * 2000);
    //                            break;
    //                    }
    //                }
    //                else
    //                {

    //                    _ecomdbCon.Dispose("DAPPER");
    //                    switch (mex.Number)
    //                    {
    //                        case 1213:
    //                            return new BookDeliveries { respcode = 0, respmsg = "Deadlock found when trying to get lock. Please call MIS HelpDesk/Support for assistance.", errorDetails = mex.ToString() };
    //                        case 1205:
    //                            return new BookDeliveries { respcode = 0, respmsg = "Lock wait timeout exceeded; try restarting transaction. Please call MIS HelpDesk/Support for assistance.", errorDetails = mex.ToString() };
    //                        default:
    //                            return new BookDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = mex.ToString() };
    //                    }
    //                }
    //            }
    //            catch (TimeoutException tex)
    //            {

    //                kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(0), tex.ToString()));
    //                _ecomdbCon.RollbackTran();
    //                _ecomdbCon.Dispose("DAPPER");
    //                return new BookDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //            }
    //            catch (Exception ex)
    //            {
    //                kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(0), ex.ToString()));
    //                _ecomdbCon.RollbackTran();
    //                _ecomdbCon.Dispose("DAPPER");
    //                return new BookDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //            }
    //        } while (true);
    //    }
    //    catch (TimeoutException tex)
    //    {
    //        kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(0), tex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BookDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //    }
    //    catch (Exception ex)
    //    {
    //        kplog.Fatal("Error #03: " + string.Format("{0}{1}", RespMessage(0), ex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BookDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //    }
    //}
    #endregion

    // Assigned items to biker
    //October 7, 2019
    public BikersDeliveries BikerDeliveries(string walletno)
    {
        try
        {
            _ecomdbCon.OpenConnection();

            List<Property.BikersDeliveryDetails> bikersDeliveries = _ecomdbCon.Query<Property.BikersDeliveryDetails>(_Query.CurrentItems, new { walletno }, 60).ToList();

            if (bikersDeliveries.Count < 1)
            {
                _ecomdbCon.Dispose("DAPPER");
                return new BikersDeliveries { respcode = 0, respmsg = "You have no pending delivery." };
            }

            _ecomdbCon.Dispose("DAPPER");
            return new BikersDeliveries { respcode = 1, respmsg = RespMessage(1), bikersDeliveries = bikersDeliveries };
        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + string.Format("{0}{1} walletno: {2}", RespMessage(5), tex.ToString(), walletno));
            _ecomdbCon.Dispose("DAPPER");
            return new BikersDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #01: " + string.Format("{0}{1} walletno: {2}", RespMessage(3), ex.ToString(), walletno));
            _ecomdbCon.Dispose("DAPPER");
            return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
        }
    }

    //display by lot number
    //October 7, 2019
    public BikersDeliveries DisplayByLotNumber(string walletno, string lotnumber)
    {
        try
        {
            _ecomdbCon.OpenConnection();

            List<Property.BikersDeliveryDetails> bikersDeliveriesByLot = _ecomdbCon.Query<Property.BikersDeliveryDetails>(_Query.DisplayByLotNumber, new { walletno, lotnumber }, 60).ToList();

            if (bikersDeliveriesByLot.Count < 1)
            {
                _ecomdbCon.Dispose("DAPPER");
                return new BikersDeliveries { respcode = 0, respmsg = "No data found!" };
            }

            _ecomdbCon.Dispose("DAPPER");
            return new BikersDeliveries { respcode = 1, respmsg = RespMessage(1), bikersDeliveries = bikersDeliveriesByLot };
        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + String.Format("{0}{1} walletno: {2}", RespMessage(5), tex.ToString(), walletno));
            _ecomdbCon.Dispose("DAPPER");
            return new BikersDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #01: " + String.Format("{0}{1} walletno: {2}", RespMessage(3), ex.ToString(), walletno));
            _ecomdbCon.Dispose("DAPPER");
            return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
        }
    }

    //display all bikers deliveries
    //October 7, 2019
    public BikersDeliveries BikerHistory(string walletno)
    {
        try
        {
            _ecomdbCon.OpenConnection();

            List<Property.LotNumberList> lotlist = _ecomdbCon.Query<Property.LotNumberList>(_Query.DeliveryHistory, new { walletno }, 60).ToList();

            if (lotlist.Count < 1)
            {
                _ecomdbCon.Dispose("DAPPER");
                return new BikersDeliveries { respcode = 0, respmsg = "No data found!" };
            }

            _ecomdbCon.Dispose("DAPPER");
            return new BikersDeliveries { respcode = 1, respmsg = RespMessage(1), lotnumberlist = lotlist };
        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + String.Format("{0}{1} walletno: {2}", RespMessage(5), tex.ToString(), walletno));
            _ecomdbCon.Dispose("DAPPER");
            return new BikersDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #01: " + String.Format("{0}{1} walletno: {2}", RespMessage(3), ex.ToString(), walletno));
            _ecomdbCon.Dispose("DAPPER");
            return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
        }
    }

    //for customer - rider handshake
    //October 7, 2019
    public BookDeliveries ScanCode(string code, string walletno, string orderid, string lineitemID)
    {
        try
        {
            kplog.Info(string.Format("walletno: {0} Code: {1} orderid: {2}, lineitemID: {3}", walletno, code, orderid, lineitemID));

            DateTime dt = Getdate();
            _ecomdbCon.OpenConnection();

            List<string> claimCode = _ecomdbCon.Query<string>(_Query.CheckClaimCode(dt.ToString("yyyy")), new { qrcode = code, orderid }, 60).ToList();
            if (claimCode.Count < 1)
            {
                kplog.Error(String.Format("Invalid QR: walletno: {0} QR Code: {1}", walletno, code));
                _ecomdbCon.Dispose("DAPPER");
                return new BookDeliveries { respcode = 0, respmsg = "Invalid QR Code!" };
            }

            #region Old Condition
            //switch (type)
            //{
            //    case 0:
            //        List<string> checkQR = _ecomdbCon.Query<string>(_Query.CheckQR, new { qrcode = code, walletno = walletno }, 60).ToList();
            //        if (checkQR.Count < 1)
            //        {
            //            kplog.Error(String.Format("Invalid QR: walletno: {0} QR Code: {1}", walletno, code));
            //            _ecomdbCon.Dispose("DAPPER");
            //            return new BookDeliveries { respcode = 0, respmsg = "Invalid QR Code!" };
            //        }
            //        break;
            //    case 1:
            //        List<string> checkCustID = _ecomdbCon.Query<string>(_Query.CheckCustCode, new { custid = code, walletno = walletno }, 60).ToList();
            //        if (checkCustID.Count < 1)
            //        {
            //            kplog.Error(String.Format("Invalid CustID: walletno: {0} Customer Code: {1}", walletno, code));
            //            _ecomdbCon.Dispose("DAPPER");
            //            return new BookDeliveries { respcode = 0, respmsg = "Invalid Customer Code!" };
            //        }
            //        break;
            //    default:
            //        kplog.Error(String.Format("{0} Wallet: {1} OrderID: {2} LineItem: {3}", "Invalid Type: ", walletno, orderid, lineitemID));
            //        _ecomdbCon.Dispose("DAPPER");
            //        return new BookDeliveries { respcode = 0, respmsg = "Invalid QR Code!" };
            //}
            #endregion

            //get operatorID,branchcode,zonecode
            List<Property.BranchData> getOrder = _ecomdbCon.Query<Property.BranchData>(_Query.GetBranchData, new { orderid, lineitemID }, 60).ToList();
            if (getOrder.Count < 1)
            {
                kplog.Error(String.Format("Invalid QR: walletno: {0} QR Code: {1}", walletno, code));
                _ecomdbCon.Dispose("DAPPER");
                return new BookDeliveries { respcode = 0, respmsg = "Invalid QR Code!" };
            }

            //var dt = getdate();
            //var year = dt.ToString("yyyy");
            //var oldYear = (Convert.ToInt32(dt.ToString("yyyy")) - 1).ToString();
            int scanAttempt = 0;

            do
            {
                try
                {
                    scanAttempt++;
                    _ecomdbCon.StartDapperTran();

                    #region Update Biker Deliveries
                    int x = _ecomdbCon.Execute(_Query.UpdateBikerDelivery, new { walletno, orderid, lineitemID }, 60);
                    if (x < 1)
                    {
                        kplog.Fatal("Unable to Update Biker Deliveries");
                        _ecomdbCon.RollbackTran();
                        _ecomdbCon.Dispose("DAPPER");
                        return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
                    }
                    #endregion

                    #region Update Deliveries
                    int y = _ecomdbCon.Execute(_Query.UpdateDeliveries, new { orderid, lineitemID,walletno }, 60);
                    if (y < 1)
                    {
                        kplog.Fatal("Unable to update deliveries");
                        _ecomdbCon.RollbackTran();
                        _ecomdbCon.Dispose("DAPPER");
                        return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
                    }
                    #endregion

                    #region Update Booking Status
                    Dictionary<string, object> UpdateBookingParam = new Dictionary<string, object>() 
                    {
                        {"pending",1},
                        {"delivered",1},
                        {"walletno",walletno},
                        {"branchcode",getOrder[0].branchcode},
                        {"zonecode",getOrder[0].zonecode},
                    };
                    int UpdateBooking = _ecomdbCon.Execute(_Query.UpdateRiderRequest, UpdateBookingParam, 60);
                    if (UpdateBooking < 1)
                    {
                        _ecomdbCon.RollbackTran();
                        _ecomdbCon.Dispose("DAPPER");
                        kplog.Fatal("Unable to update request booking");
                        return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
                    }
                    #endregion

                    #region Check Delivery Total
                    List<dynamic> checkDeliveryTotal = _ecomdbCon.Query<dynamic>(_Query.CheckTotalDelivery, new { walletno, getOrder[0].branchcode, getOrder[0].zonecode }, 60).ToList();
                    if (checkDeliveryTotal.Count < 1)
                    {
                        _ecomdbCon.RollbackTran();
                        _ecomdbCon.Dispose("DAPPER");
                        kplog.Error(string.Format("orderID: {0} lineitemID: {1}", orderid, lineitemID));
                        return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
                    }
                    #endregion

                    #region Update Booking status
                    Dictionary<string, object> BookingAvailablity = new Dictionary<string, object>() 
                    {
                        {"isActive",checkDeliveryTotal[0].total_delivery == (checkDeliveryTotal[0].delivered + checkDeliveryTotal[0].returned) ? 0 : 1},
                        {"isAvailable",checkDeliveryTotal[0].total_delivery == (checkDeliveryTotal[0].delivered + checkDeliveryTotal[0].returned) ? 0 : 1},
                        {"isBooked",checkDeliveryTotal[0].total_delivery == (checkDeliveryTotal[0].delivered + checkDeliveryTotal[0].returned) ? 0 : 1},
                        {"walletno",walletno},
                        {"branchcode",getOrder[0].branchcode},
                        {"zonecode",getOrder[0].zonecode},
                    };
                    int UpdateBookingAvailablity = _ecomdbCon.Execute(_Query.UpdateRequestStatus, BookingAvailablity, 60);
                    if (UpdateBookingAvailablity < 1)
                    {
                        _ecomdbCon.RollbackTran();
                        _ecomdbCon.Dispose("DAPPER");
                        kplog.Error(string.Format("branchcode: {0} zonecode: {1} walletno: {2} Unable to update delivery request booking!",
                            getOrder[0].branchcode, getOrder[0].zonecode, walletno));
                        return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
                    }
                    #endregion


                    //call API here
                    string parameters = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}",
                        "/?orderid=", orderid, "&lineitemID=", lineitemID,
                        "&operatorid=", getOrder[0].operatorID, "&branchcode=",
                        getOrder[0].branchcode, "&zonecode=", getOrder[0].zonecode);
                    RequestHandler request = new RequestHandler("PayItem", parameters);
                    string response = request.HttpGetRequest();
                    if (!String.IsNullOrEmpty(response))
                    {
                        if (response.Contains("ERROR"))
                        {
                            string[] error = response.Split('|');
                            kplog.Fatal(error[1].ToString());
                            _ecomdbCon.RollbackTran();
                            _ecomdbCon.Dispose("DAPPER");
                            return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
                        }

                        Indicators ecomResponse = response.Deserialize<Indicators>();
                        if (ecomResponse.respmsg.ToLower().Equals("successfully paid"))
                        {
                            _ecomdbCon.CommitTran();
                            kplog.Info(String.Format("Successfully Pick-up: OrderID: {0} Code: {1} Walletno: {2}", orderid, code, walletno));

                            #region Old Process
                            ////check if no more pending deliveries then update
                            //List<string> checkDeliveries = _ecomdbCon.Query<string>(_Query.CheckDeliveries, new { walletno }, 60).ToList();
                            //if (checkDeliveries.Count < 1)
                            //{
                            //    _ecomdbCon.StartDapperTran();
                            //    int updateBikerStatus = _ecomdbCon.Execute(_Query.UpdateBikerStatus, new { walletno, status = 2 }, 60);
                            //    if (updateBikerStatus < 1)
                            //    {
                            //        _ecomdbCon.RollbackTran();
                            //        _ecomdbCon.Dispose("DAPPER");
                            //        kplog.Error(string.Format("Walletno: {0} Unable Update Rider!", walletno));
                            //        return new BookDeliveries { respcode = 1, respmsg = RespMessage(1) };
                            //    }
                            //    _ecomdbCon.CommitTran();
                            //    kplog.Info(string.Format("Successfully Updated Biker Status: OrderID: {0} Code: {1} Walletno: {2}", orderid, code, walletno));
                            //}
                            #endregion

                            _ecomdbCon.Dispose("DAPPER");
                            return new BookDeliveries { respcode = 1, respmsg = RespMessage(1) };
                        }
                        else
                        {
                            _ecomdbCon.RollbackTran();
                            _ecomdbCon.Dispose("DAPPER");
                            kplog.Fatal(ecomResponse.respmsg);
                            kplog.Fatal(ecomResponse.ErrorDetails);
                            return new BookDeliveries { respcode = 0, respmsg = ecomResponse.respmsg };
                        }
                    }
                    else
                    {
                        _ecomdbCon.RollbackTran();
                        _ecomdbCon.Dispose("DAPPER");
                        return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
                    }
                }
                catch (MySqlException mex)
                {
                    _ecomdbCon.RollbackTran();
                    kplog.Fatal(mex.ToString());
                    if (scanAttempt <= 1)
                    {
                        switch (mex.Number)
                        {
                            case 1213:
                                kplog.Fatal(code + " - " + "(ER_LOCK_DEADLOCK) Deadlock found when trying to get lock. ATTEMPTCOUNT = " + scanAttempt, mex);
                                Thread.Sleep(scanAttempt * 2000);
                                break;
                            case 1205:
                                kplog.Fatal(code + " - " + "(ER_LOCK_WAIT_TIMEOUT) Lock wait timeout exceeded; try restarting transaction. ATTEMPTCOUNT = " + scanAttempt, mex);
                                Thread.Sleep(scanAttempt * 2000);
                                break;
                            default:
                                Thread.Sleep(scanAttempt * 2000);
                                break;
                        }
                    }
                    else
                    {

                        _ecomdbCon.Dispose("DAPPER");
                        switch (mex.Number)
                        {
                            case 1213:
                                return new BookDeliveries { respcode = 0, respmsg = "Deadlock found when trying to get lock. Please call MIS HelpDesk/Support for assistance.", errorDetails = mex.ToString() };
                            case 1205:
                                return new BookDeliveries { respcode = 0, respmsg = "Lock wait timeout exceeded; try restarting transaction. Please call MIS HelpDesk/Support for assistance.", errorDetails = mex.ToString() };
                            default:
                                return new BookDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = mex.ToString() };
                        }
                    }
                }
                catch (TimeoutException tex)
                {

                    kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(0), tex.ToString()));
                    _ecomdbCon.RollbackTran();
                    _ecomdbCon.Dispose("DAPPER");
                    return new BookDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
                }
                catch (Exception ex)
                {
                    kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(0), ex.ToString()));
                    _ecomdbCon.RollbackTran();
                    _ecomdbCon.Dispose("DAPPER");
                    return new BookDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
                }
            } while (true);
        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(0), tex.ToString()));
            _ecomdbCon.Dispose("DAPPER");
            return new BookDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #03: " + string.Format("{0}{1}", RespMessage(0), ex.ToString()));
            _ecomdbCon.Dispose("DAPPER");
            return new BookDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
        }
    }

    #region Old KYC
    ////for branch
    ////verify rider then display booked items
    //public BikersDeliveries VerifyRider(string qrcode)
    //{
    //    try
    //    {
    //        kplog.Info(String.Format("CUSTID: {0}", qrcode));

    //        var dt = Getdate();
    //        var year = dt.ToString("yyyy");

    //        _ecomdbCon.OpenConnection();

    //        List<string> checkRider = _ecomdbCon.Query<string>(_Query.CheckRiderWalletno, new { qrcode }, 60).ToList();
    //        if (checkRider.Count < 1)
    //        {
    //            _ecomdbCon.Dispose("DAPPER");
    //            return new BikersDeliveries { respcode = 0, respmsg = "Unregistered Rider!" };
    //        }

    //        List<Property.BikersDeliveryDetails> bikersDeliveries = _ecomdbCon.Query<Property.BikersDeliveryDetails>(_Query.GetPendingItems(year), new { qrcode }, 60).ToList();
    //        if (bikersDeliveries.Count < 1)
    //        {
    //            _ecomdbCon.Dispose("DAPPER");
    //            return new BikersDeliveries { respcode = 0, respmsg = "No data found!" };
    //        }

    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersDeliveries { respcode = 1, respmsg = RespMessage(1), bikersDeliveries = bikersDeliveries };

    //    }
    //    catch (TimeoutException tex)
    //    {
    //        kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(0), tex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //    }
    //    catch (Exception ex)
    //    {
    //        kplog.Fatal("Error #03: " + String.Format("{0}{1}", RespMessage(0), ex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //    }
    //}
    ////Branch Registration 1st step
    //// sept. 27, 2019
    //public BikerLogin CheckRiderKYC(string walletno)
    //{
    //    try
    //    {
    //        kplog.Info(String.Format("walletno: {0}", walletno));

    //        _ecomdbCon.OpenConnection();

    //        #region Old Code
    //        //List<string> checkRider = _ecomdbCon.Query<string>(_Query.CheckRiderWalletno, new { walletno }, 60).ToList();
    //        //if (checkRider.Count < 1)
    //        //{
    //        //    _ecomdbCon.Dispose("DAPPER");
    //        //    return new BikerLogin { respcode = 0, respmsg = "Unregistered Rider!" };
    //        //}
    //        //_walletCon.OpenConnection();
    //        //List<Property.BikersImages> bikersPhoto = _walletCon.Query<Property.BikersImages>(_Query.GetPhotos, new { bikersData[0].walletno }, 60).ToList();
    //        //if (bikersPhoto.Count < 1)
    //        //{
    //        //    _walletCon.Dispose("DAPPER");
    //        //    kplog.Error(string.Format("No Photo! walletno: {0}", bikersData[0].walletno));
    //        //    return new BikerLogin { respcode = 0, respmsg = "Photo is Required!" };
    //        //}
    //        #endregion

    //        List<Property.BikersInfo> bikersData = _ecomdbCon.Query<Property.BikersInfo>(_Query.BikerKYC, new { walletno }, 60).ToList();
    //        if (bikersData.Count < 1)
    //        {
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Error(string.Format("Invalid Credentials! walletno: {0}", walletno));
    //            return new BikerLogin { respcode = 0, respmsg = "Unregistered Rider!" };
    //        }
    //        _ecomdbCon.Dispose("DAPPER");



    //        return new BikerLogin { respcode = 1, respmsg = RespMessage(1), bikersInfo = bikersData[0] };
    //    }
    //    catch (TimeoutException tex)
    //    {
    //        kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(0), tex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikerLogin { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //    }
    //    catch (Exception ex)
    //    {
    //        kplog.Fatal("Error #03: " + string.Format("{0}{1}", RespMessage(0), ex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikerLogin { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //    }
    //}
    #endregion

    #region Old Approved Booking
    //generate lotnumber
    //public BikersDeliveries ApproveBooking(string walletno, string qrcode)
    //{
    //    try
    //    {
    //        int bookAttempt = 0;
    //        string final_series = string.Empty;
    //        DateTime dt = Getdate();

    //        _ecomdbCon.OpenConnection();

    //        List<Property.BikersDeliveryDetails> bikersDeliveries = _ecomdbCon.Query<Property.BikersDeliveryDetails>(_Query.GetPendingItems(dt.ToString("yyyy")), new { qrcode }, 60).ToList();
    //        if (bikersDeliveries.Count < 1)
    //        {
    //            _ecomdbCon.Dispose("DAPPER");
    //            return new BikersDeliveries { respcode = 0, respmsg = "No data found!" };
    //        }
    //        string lotnumber = string.Empty;
    //        //update series here
    //        //generate lotnumber
    //        //=======================May 2, 2019
    //        List<dynamic> getSeries = _ecomdbCon.Query<dynamic>(_Query.CheckSeries, new { walletno }, 60).ToList();
    //        _ecomdbCon.StartDapperTran();
    //        if (getSeries.Count < 1)
    //        {
    //            int addSeries = _ecomdbCon.Execute(_Query.AddSeries, new { walletno }, 60);
    //            if (addSeries < 1)
    //            {
    //                _ecomdbCon.RollbackTran();
    //                _ecomdbCon.Dispose("DAPPER");
    //                return new BikersDeliveries { respcode = 0, respmsg = "Unable to Create Series Number!" };
    //            }
    //            final_series = "1";
    //        }
    //        else
    //        {
    //            final_series = getSeries[0].series.ToString() == "99999" ? "1" : (Convert.ToInt32(getSeries[0].series.ToString()) + 1).ToString();
    //            int updateSeries = _ecomdbCon.Execute(_Query.UpdateSeries, new { series = final_series, walletno }, 60);
    //            if (updateSeries < 1)
    //            {
    //                _ecomdbCon.RollbackTran();
    //                _ecomdbCon.Dispose("DAPPER");
    //                return new BikersDeliveries { respcode = 0, respmsg = "Unable to Update Series Number!" };
    //            }
    //        }
    //        _ecomdbCon.CommitTran();
    //        //yyyy-mm-dd-001-1-00001

    //        lotnumber = string.Format("{0}-{1}-{2}-001-1-{3}", dt.ToString("yyyy"), dt.ToString("MM"), dt.ToString("dd"), final_series.PadLeft(5, '0'));
    //        do
    //        {
    //            try
    //            {
    //                bookAttempt++;
    //                _ecomdbCon.StartDapperTran();
    //                int updatebooking = _ecomdbCon.Execute(_Query.ApproveDelivery, new { lotnumber, walletno, qrcode }, 60);
    //                if (updatebooking < 1)
    //                {
    //                    _ecomdbCon.RollbackTran();
    //                    _ecomdbCon.Dispose("DAPPER");
    //                    kplog.Error(string.Format("CUstid: {0} Unable to update Data!", qrcode));
    //                    return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3) };
    //                }

    //                kplog.Info(string.Format("Success Booking for walletno: {0}", walletno));
    //                _ecomdbCon.CommitTran();

    //                int z = 0;
    //                foreach (var item in bikersDeliveries)
    //                {
    //                    bikersDeliveries[z].status = "Picked-Up";
    //                    z++;
    //                }

    //                _ecomdbCon.Dispose("DAPPER");
    //                return new BikersDeliveries { respcode = 1, respmsg = RespMessage(1), bikersDeliveries = bikersDeliveries };
    //            }
    //            catch (MySqlException mex)
    //            {
    //                _ecomdbCon.RollbackTran();
    //                kplog.Fatal(mex.ToString());
    //                if (bookAttempt <= DB_DEADLOCK_RETRY_COUNT)
    //                {
    //                    switch (mex.Number)
    //                    {
    //                        case 1213:
    //                            kplog.Fatal(walletno + " - " + "(ER_LOCK_DEADLOCK) Deadlock found when trying to get lock. ATTEMPTCOUNT = " + bookAttempt, mex);
    //                            Thread.Sleep(bookAttempt * 2000);
    //                            break;
    //                        case 1205:
    //                            kplog.Fatal(walletno + " - " + "(ER_LOCK_WAIT_TIMEOUT) Lock wait timeout exceeded; try restarting transaction. ATTEMPTCOUNT = " + bookAttempt, mex);
    //                            Thread.Sleep(bookAttempt * 2000);
    //                            break;
    //                        default:
    //                            Thread.Sleep(bookAttempt * 2000);
    //                            break;
    //                    }
    //                }
    //                else
    //                {

    //                    _ecomdbCon.Dispose("DAPPER");
    //                    switch (mex.Number)
    //                    {
    //                        case 1213:
    //                            return new BikersDeliveries { respcode = 0, respmsg = "Deadlock found when trying to get lock. Please call MIS HelpDesk/Support for assistance.", errorDetails = mex.ToString() };
    //                        case 1205:
    //                            return new BikersDeliveries { respcode = 0, respmsg = "Lock wait timeout exceeded; try restarting transaction. Please call MIS HelpDesk/Support for assistance.", errorDetails = mex.ToString() };
    //                        default:
    //                            return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = mex.ToString() };
    //                    }
    //                }
    //            }
    //            catch (TimeoutException tex)
    //            {

    //                kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(0), tex.ToString()));
    //                _ecomdbCon.RollbackTran();
    //                _ecomdbCon.Dispose("DAPPER");
    //                return new BikersDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //            }
    //            catch (Exception ex)
    //            {
    //                kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(0), ex.ToString()));
    //                _ecomdbCon.RollbackTran();
    //                _ecomdbCon.Dispose("DAPPER");
    //                return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //            }
    //        } while (true);
    //    }
    //    catch (TimeoutException tex)
    //    {
    //        kplog.Fatal("Error #02: " + string.Format("{0}{1}", RespMessage(0), tex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //    }
    //    catch (Exception ex)
    //    {
    //        kplog.Fatal("Error #03: " + string.Format("{0}{1}", RespMessage(0), ex.ToString()));
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //    }
    //}
    #endregion

    //Cancel rider to branch booking
    //October 7, 2019
    public BookDeliveries CancelBooking(string walletno, string branchcode, string zonecode)
    {
        try
        {
            _ecomdbCon.OpenConnection();
            _ecomdbCon.StartDapperTran();

            int cancelBooking = _ecomdbCon.Execute(_Query.CancelBook, new { walletno, branchcode, zonecode }, 60);
            if (cancelBooking < 1)
            {
                _ecomdbCon.RollbackTran();
                _ecomdbCon.Dispose("DAPPER");
                kplog.Error(string.Format("orderid: {0} Unable to cancel Data!", walletno));
                return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
            }

            #region Old Process
            //int UpdateBikerActive = _ecomdbCon.Execute(_Query.UpdateBikerActive, new { walletno }, 60);
            //if (UpdateBikerActive < 1)
            //{
            //    _ecomdbCon.RollbackTran();
            //    _ecomdbCon.Dispose("DAPPER");
            //    kplog.Error(string.Format("orderid: {0} Unable to cancel Data!", walletno));
            //    return new BookDeliveries { respcode = 0, respmsg = RespMessage(3) };
            //}
            #endregion

            _ecomdbCon.CommitTran();
            return new BookDeliveries { respcode = 1, respmsg = RespMessage(1) };
        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(0), tex.ToString()));
            _ecomdbCon.RollbackTran();
            _ecomdbCon.Dispose("DAPPER");
            return new BookDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #03: " + String.Format("{0}{1}", RespMessage(0), ex.ToString()));
            _ecomdbCon.RollbackTran();
            _ecomdbCon.Dispose("DAPPER");
            return new BookDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
        }
    }

    #region Old Delivery Cancellation
    //public BikersDeliveries CancelDelivery(string orderid, string walletno)
    //{
    //    try
    //    {
    //        _ecomdbCon.OpenConnection();
    //        _ecomdbCon.StartDapperTran();

    //        int cancelBooking = _ecomdbCon.Execute(_Query.CancelDelivery, new { orderid, walletno }, 60);
    //        if (cancelBooking < 1)
    //        {
    //            _ecomdbCon.RollbackTran();
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Error(String.Format("orderid: {0} Unable to cancel Data!", orderid));
    //            return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3) };
    //        }
    //        int UpdateBikerActive = _ecomdbCon.Execute(_Query.UpdateBikerActive, new { walletno }, 60);
    //        if (UpdateBikerActive < 1)
    //        {
    //            _ecomdbCon.RollbackTran();
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Error(String.Format("orderid: {0} Unable to cancel Data!", orderid));
    //            return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3) };
    //        }
    //        _ecomdbCon.CommitTran();
    //        return new BikersDeliveries { respcode = 1, respmsg = RespMessage(1) };
    //    }
    //    catch (TimeoutException tex)
    //    {
    //        kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(0), tex.ToString()));
    //        _ecomdbCon.RollbackTran();
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersDeliveries { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //    }
    //    catch (Exception ex)
    //    {
    //        kplog.Fatal("Error #03: " + String.Format("{0}{1}", RespMessage(0), ex.ToString()));
    //        _ecomdbCon.RollbackTran();
    //        _ecomdbCon.Dispose("DAPPER");
    //        return new BikersDeliveries { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //    }
    //}
    #endregion

    //ML Wallet Biker Registration
    public RegisterBiker RegisterBiker(Property.Registration bikerInfo)
    {
        //kplog.Info(string.Format("Biker Info: {0}", JsonConvert.SerializeObject(bikerInfo)));
        try
        {
            if (bikerInfo.nbi_clearance.IsNull() || bikerInfo.driver_licence.IsNull() || bikerInfo.brgy_clearance.IsNull()
                || bikerInfo.vehicle.IsNull() || bikerInfo.vehicle_OR.IsNull() || bikerInfo.vehicle_CR.IsNull())
            {
                kplog.Error("lack information");
                return new RegisterBiker { respcode = 0, respmsg = "Complete information is required." };
            }
            _ecomdbCon.OpenConnection();

            List<dynamic> biker = _ecomdbCon.Query<dynamic>(_Query.CheckBiker, new { bikerInfo.walletno }, 60).ToList();
            if (biker.Count > 0)
            {
                _ecomdbCon.Dispose("DAPPER");
                kplog.Error(string.Format("Biker Already Exists {0}", bikerInfo.walletno));
                return new RegisterBiker { respcode = 0, respmsg = "Biker Already Exists!" };
            }
            _walletCon.OpenConnection();
            List<string> checkPin = _walletCon.Query<string>(_Query.CheckPin, new { bikerInfo.pin, bikerInfo.walletno }, 60).ToList();
            if (checkPin.Count < 1)
            {
                kplog.Error(string.Format("Invalid Pin: {0} walletno:{1} ", bikerInfo.pin, bikerInfo.walletno));
                _walletCon.Dispose("DAPPER");
                _ecomdbCon.Dispose("DAPPER");
                return new RegisterBiker { respcode = 0, respmsg = "Invalid pin number. Please Try Again." };
            }
            _walletCon.CloseConnection();
            Dictionary<string, object> BikerInfoParam = new Dictionary<string, object>()
                {
                    {"pin",bikerInfo.pin},
                    {"walletno",bikerInfo.walletno},
                    {"custID",bikerInfo.custID},
                    {"first_name",bikerInfo.first_name},
                    {"last_name",bikerInfo.last_name},
                    {"birthdate",bikerInfo.birthdate.ToDate(0)},
                    {"address",bikerInfo.address},
                    {"license_no",bikerInfo.license_no},
                    {"expiry_date",bikerInfo.expiry_date.ToDate(0) },
                    {"vehicle_type",bikerInfo.vehicle_type },
                    {"full_contact",bikerInfo.full_contact },
                    {"facebook",bikerInfo.facebook },
                    {"viber",bikerInfo.viber },
                    {"gmail",bikerInfo.gmail },
                    {"phone",bikerInfo.phone },
                    {"latitude",bikerInfo.latitude },
                    {"longitude",bikerInfo.longitude},
                    {"isActive",1},
                    {"isAvailable",1},
                };
            _ecomdbCon.StartDapperTran();
            int saveBiker = _ecomdbCon.Execute(_Query.RegisterBiker, BikerInfoParam, 60);
            if (saveBiker < 1)
            {
                _ecomdbCon.RollbackTran();
                _ecomdbCon.Dispose("DAPPER");
                _walletCon.Dispose("DAPPER");
                kplog.Error(string.Format("walletNo: {0} Unable to register biker!", JsonConvert.SerializeObject(bikerInfo)));
                return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), };
            }

            bool isUploaded = CreateBikerInfo(bikerInfo);
            if (isUploaded.Equals(false))
            {
                _ecomdbCon.RollbackTran();
                _ecomdbCon.Dispose("DAPPER");
                _walletCon.Dispose("DAPPER");
                kplog.Error(string.Format("walletNo: {0} Unable to save biker files!", JsonConvert.SerializeObject(bikerInfo)));
                return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), };
            }
            _ecomdbCon.CommitTran();
            _ecomdbCon.Dispose("DAPPER");
            _walletCon.Dispose("DAPPER");
            //Thread thread = new Thread(new ThreadStart(delegate ()
            //{
            //    CreateBikerInfo(bikerInfo);
            //}))
            //{
            //    IsBackground = true
            //};
            //thread.Start();

            kplog.Info(string.Format("Biker Created Successfully {0}", bikerInfo.walletno));
            return new RegisterBiker { respcode = 1, respmsg = "You're biker account is successfully created. Thank you!" };

        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(0), tex.ToString()));
            if (_ecomdbCon != null) _ecomdbCon.Dispose("DAPPER");
            if (_walletCon != null) _walletCon.Dispose("DAPPER");
            return new RegisterBiker { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #03: " + String.Format("{0}{1}", RespMessage(0), ex.ToString()));
            if (_ecomdbCon != null) _ecomdbCon.Dispose("DAPPER");
            if (_walletCon != null) _walletCon.Dispose("DAPPER");
            return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
        }
    }
    public RegisterBiker UpdateBiker(Property.Registration bikerInfo)
    {
        try
        {
            //kplog.Info(JsonConvert.SerializeObject(bikerInfo));

            //Thread thread = new Thread(new ThreadStart(delegate ()
            //{
            //    CreateBikerInfo(bikerInfo);
            //}))
            //{
            //    IsBackground = true
            //};
            //thread.Start();
            _walletCon.OpenConnection();
            List<string> checkPin = _walletCon.Query<string>(_Query.CheckPin, new { bikerInfo.pin, bikerInfo.walletno }, 60).ToList();
            if (checkPin.Count < 1)
            {
                kplog.Error(string.Format("Invalid Pin: {0} walletno:{1} ", bikerInfo.pin, bikerInfo.walletno));
                _walletCon.Dispose("DAPPER");
                return new RegisterBiker { respcode = 0, respmsg = "Invalid pin number. Please Try Again." };
            }
            _walletCon.CloseConnection();

            _ecomdbCon.OpenConnection();
            _ecomdbCon.StartDapperTran();
            bool isUploaded = CreateBikerInfo(bikerInfo);
            if (isUploaded.Equals(false))
            {
                _ecomdbCon.RollbackTran();
                _ecomdbCon.Dispose("DAPPER");
                _walletCon.Dispose("DAPPER");
                kplog.Error(string.Format("walletNo: {0} Unable to save biker files!", JsonConvert.SerializeObject(bikerInfo)));
                return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), };
            }
            Dictionary<string, object> BikerInfoParam = new Dictionary<string, object>()
                {
                    {"address",bikerInfo.address},
                    {"expiry_date",bikerInfo.expiry_date.ToDate(0) },
                    {"vehicle_type",bikerInfo.vehicle_type },
                    {"facebook",bikerInfo.facebook },
                    {"viber",bikerInfo.viber },
                    {"gmail",bikerInfo.gmail },
                    {"phone",bikerInfo.phone },
                    {"walletno",bikerInfo.walletno }
                };

            int updateBiker = _ecomdbCon.Execute(_Query.UpdateBikerInfo, BikerInfoParam, 60);
            if (updateBiker < 1)
            {
                _ecomdbCon.RollbackTran();
                _ecomdbCon.Dispose("DAPPER");
                _walletCon.Dispose("DAPPER");
                kplog.Error(string.Format("walletNo: {0} Unable to register biker!", JsonConvert.SerializeObject(bikerInfo)));
                return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), };
            }

            _ecomdbCon.CommitTran();
            _ecomdbCon.Dispose("DAPPER");
            _walletCon.Dispose("DAPPER");
            kplog.Info(string.Format("Biker Updated Successfully {0}", bikerInfo.walletno));
            return new RegisterBiker { respcode = 1, respmsg = "You're biker account is successfully updated. Thank you!" };
        }
        catch (Exception ex)
        {
            kplog.Fatal(ex.ToString());
            _ecomdbCon.RollbackTran();
            if (_ecomdbCon != null) _ecomdbCon.Dispose("DAPPER");
            if (_walletCon != null) _walletCon.Dispose("DAPPER");
            return new RegisterBiker { respcode = 0, respmsg = RespMessage(3) };
        }
    }
    public bool CreateBikerInfo(Property.Registration bikerInfo)
    {
        try
        {

            List<dynamic> bikerDocs = _ecomdbCon.Query<dynamic>(_Query.CheckBikerDR, new { bikerInfo.walletno }, 60).ToList();
            if (bikerDocs.Count < 1)
            {
                CreateProfileDirectoy(bikerInfo.walletno);
            }

            if (!bikerInfo.nbi_clearance.IsNull())
            {
                bool isUploaded = UploadFile(bikerInfo.nbi_clearance.ToBase64(), bikerInfo.walletno, "NBI");
                if (isUploaded.Equals(false))
                {
                    return false;
                }
                bool isInserted = InsertFiles(bikerInfo.walletno, string.Format("/{0}/{1}_NBI.jpg", bikerInfo.walletno, bikerInfo.walletno), null, null, null, null, null);
                if (isInserted.Equals(false))
                {
                    return false;
                }
            }
            if (!bikerInfo.brgy_clearance.IsNull())
            {
                bool isUploaded = UploadFile(bikerInfo.brgy_clearance.ToBase64(), bikerInfo.walletno, "BRGY");
                if (isUploaded.Equals(false))
                {
                    return false;
                }
                bool isInserted = InsertFiles(bikerInfo.walletno, null, string.Format("/{0}/{1}_BRGY.jpg", bikerInfo.walletno, bikerInfo.walletno), null, null, null, null);
                if (isInserted.Equals(false))
                {
                    return false;
                }
            }
            if (!bikerInfo.vehicle.IsNull())
            {
                bool isUploaded = UploadFile(bikerInfo.vehicle.ToBase64(), bikerInfo.walletno, "VEHICLE");
                if (isUploaded.Equals(false))
                {
                    return false;
                }
                bool isInserted = InsertFiles(bikerInfo.walletno, null, null, string.Format("/{0}/{1}_VEHICLE.jpg", bikerInfo.walletno, bikerInfo.walletno), null, null, null);
                if (isInserted.Equals(false))
                {
                    return false;
                }
            }
            if (!bikerInfo.driver_licence.IsNull())
            {
                bool isUploaded = UploadFile(bikerInfo.driver_licence.ToBase64(), bikerInfo.walletno, "LICENCE");
                if (isUploaded.Equals(false))
                {
                    return false;
                }
                bool isInserted = InsertFiles(bikerInfo.walletno, null, null, null, string.Format("/{0}/{1}_LICENCE.jpg", bikerInfo.walletno, bikerInfo.walletno), null, null);
                if (isInserted.Equals(false))
                {
                    return false;
                }
            }
            if (!bikerInfo.vehicle_OR.IsNull())
            {
                bool isUploaded = UploadFile(bikerInfo.vehicle_OR.ToBase64(), bikerInfo.walletno, "OR");
                if (isUploaded.Equals(false))
                {
                    return false;
                }
                bool isInserted = InsertFiles(bikerInfo.walletno, null, null, null, null, string.Format("/{0}/{1}_OR.jpg", bikerInfo.walletno, bikerInfo.walletno), null);
                if (isInserted.Equals(false))
                {
                    return false;
                }
            }
            if (!bikerInfo.vehicle_CR.IsNull())
            {
                bool isUploaded = UploadFile(bikerInfo.vehicle_CR.ToBase64(), bikerInfo.walletno, "CR");
                if (isUploaded.Equals(false))
                {
                    return false;
                }
                bool isInserted = InsertFiles(bikerInfo.walletno, null, null, null, null, null, string.Format("/{0}/{1}_CR.jpg", bikerInfo.walletno, bikerInfo.walletno));
                if (isInserted.Equals(false))
                {
                    return false;
                }
            }
            kplog.Info(string.Format("Success saving biker requirements: {0}", JsonConvert.SerializeObject(bikerInfo)));

            return true;
        }
        catch (Exception ex)
        {
            kplog.Fatal(ex.ToString());
            return false;
        }
    }
    private void CreateProfileDirectoy(string walletNo)
    {
        FtpWebRequest ftp;
        try
        {

            FtpWebResponse CreateForderResponse = null;
            ftp = (FtpWebRequest)FtpWebRequest.Create(_bikerftphost + "/" + walletNo);
            ftp.UseBinary = false;
            ftp.UsePassive = true;
            ftp.KeepAlive = false;
            ftp.Credentials = new NetworkCredential(_ftpusername, _ftppassword);
            ftp.Method = WebRequestMethods.Ftp.MakeDirectory;
            CreateForderResponse = (FtpWebResponse)ftp.GetResponse();

        }
        catch (WebException e)
        {
            string status = ((FtpWebResponse)e.Response).StatusDescription;
            kplog.Fatal("walletNo: " + walletNo + " - " + status + "\r\n" + e.ToString());
            throw new Exception(e.ToString());
        }
        catch (Exception ex)
        {
            kplog.Fatal("walletNo: " + walletNo + " - " + ex.ToString());
            throw new Exception(ex.ToString());
        }

    }
    private bool UploadFile(byte[] image, string walletNo, string filename)
    {
        FtpWebRequest ftpReq;
        try
        {
            kplog.Info("walletNo: " + walletNo + " filename: " + filename);

            string _filename = _bikerftphost + "/" + walletNo + "/" + walletNo + "_" + filename + ".jpg";
            ftpReq = (FtpWebRequest)WebRequest.Create(_filename);
            ftpReq.UseBinary = true;
            ftpReq.KeepAlive = false;
            ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
            ftpReq.Credentials = new NetworkCredential(_ftpusername, _ftppassword);

            ftpReq.ContentLength = image.Length;
            using (Stream s = ftpReq.GetRequestStream())
            {
                s.Write(image, 0, image.Length);
            }

            kplog.Info("walletNo: " + walletNo + " filename: " + filename + " - Success");
            return true;
        }
        catch (WebException e)
        {
            string status = ((FtpWebResponse)e.Response).StatusDescription;
            kplog.Fatal("walletNo: " + walletNo + " filename: " + filename + " - " + status + "\r\n" + e.ToString());
            return false;
        }
        catch (Exception ex)
        {
            kplog.Fatal("walletNo: " + walletNo + " filename: " + filename + " - " + ex.ToString());
            return false;
        }

    }
    public bool InsertFiles(string walletno, string nbi, string brgy, string vehicle, string drivers_licence, string vehicle_OR, string vehicle_CR)
    {
        try
        {
            kplog.Info(string.Format("walletno: {0} nbi: {1} brgy: {2} vehicle: {3} licence: {4} vehicle: {5} OR: {6} CR: {7}",
                walletno, nbi, brgy, vehicle, drivers_licence, vehicle, vehicle_OR, vehicle_CR));

            List<dynamic> bikerDocs = _ecomdbCon.Query<dynamic>(_Query.CheckBikerDR, new { walletno }, 60).ToList();
            if (bikerDocs.Count < 1)
            {
                Dictionary<string, object> BikerInfo = new Dictionary<string, object>()
                {
                    {"walletno",walletno},
                    {"nbi_clearance",nbi},
                    {"brgy_clearance",brgy},
                    {"vehicle",vehicle},
                    {"driver_licence",drivers_licence},
                    {"vehicle_OR",vehicle_OR},
                    {"vehicle_CR",vehicle_CR},
                };

                int saveRequirements = _ecomdbCon.Execute(_Query.SaveRequirements, BikerInfo, 60);
                if (saveRequirements < 1)
                {
                    kplog.Error(string.Format("walletNo: {0} Unable to save Data!", walletno));
                    return false;
                }
            }
            else
            {
                if (!nbi.IsNull() && brgy.IsNull() && vehicle.IsNull() && drivers_licence.IsNull() && vehicle_OR.IsNull() && vehicle_CR.IsNull())
                {
                    int updateNBI = _ecomdbCon.Execute("UPDATE mlbiker.BikerImages set nbi_clearance=@nbi WHERE walletno=@walletno", new { nbi, walletno }, 60);
                    if (updateNBI < 1)
                    {
                        kplog.Error(String.Format("walletno: {0} Unable to update nbi!", walletno));
                        return false;
                    }
                }
                else if (nbi.IsNull() && !brgy.IsNull() && vehicle.IsNull() && drivers_licence.IsNull() && vehicle_OR.IsNull() && vehicle_CR.IsNull())
                {
                    int updatebrgy = _ecomdbCon.Execute("UPDATE mlbiker.BikerImages set brgy_clearance=@brgy WHERE walletno=@walletno", new { brgy, walletno }, 60);
                    if (updatebrgy < 1)
                    {
                        kplog.Error(String.Format("walletno: {0} Unable to update brgy!", walletno));
                        return false;
                    }
                }
                else if (nbi.IsNull() && brgy.IsNull() && !vehicle.IsNull() && drivers_licence.IsNull() && vehicle_OR.IsNull() && vehicle_CR.IsNull())
                {
                    int updateVehicle = _ecomdbCon.Execute("UPDATE mlbiker.BikerImages set vehicle=@vehicle WHERE walletno=@walletno", new { vehicle, walletno }, 60);
                    if (updateVehicle < 1)
                    {
                        kplog.Error(String.Format("walletno: {0} Unable to update vehicle!", walletno));
                        return false;
                    }
                }
                else if (nbi.IsNull() && brgy.IsNull() && vehicle.IsNull() && !drivers_licence.IsNull() && vehicle_OR.IsNull() && vehicle_CR.IsNull())
                {
                    int update_licence = _ecomdbCon.Execute("UPDATE mlbiker.BikerImages set driver_licence=@drivers_licence WHERE walletno=@walletno", new { drivers_licence, walletno }, 60);
                    if (update_licence < 1)
                    {
                        kplog.Error(String.Format("walletno: {0} Unable to update drivers_licence!", walletno));
                        return false;
                    }
                }
                else if (nbi.IsNull() && brgy.IsNull() && vehicle.IsNull() && drivers_licence.IsNull() && !vehicle_OR.IsNull() && vehicle_CR.IsNull())
                {
                    int update_OR = _ecomdbCon.Execute("UPDATE mlbiker.BikerImages set vehicle_OR=@vehicle_OR WHERE walletno=@walletno", new { vehicle_OR, walletno }, 60);
                    if (update_OR < 1)
                    {
                        kplog.Error(String.Format("walletno: {0} Unable to update OR!", walletno));
                        return false;
                    }
                }
                else if (nbi.IsNull() && brgy.IsNull() && vehicle.IsNull() && drivers_licence.IsNull() && vehicle_OR.IsNull() && !vehicle_CR.IsNull())
                {
                    int update_OR = _ecomdbCon.Execute("UPDATE mlbiker.BikerImages set vehicle_CR=@vehicle_CR WHERE walletno=@walletno", new { vehicle_CR, walletno }, 60);
                    if (update_OR < 1)
                    {
                        kplog.Error(String.Format("walletno: {0} Unable to update OR!", walletno));
                        return false;
                    }
                }
            }
            return true;
        }
        catch (TimeoutException tex)
        {
            kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(0), tex.ToString()));
            return false;

        }
        catch (Exception ex)
        {
            kplog.Fatal("Error #03: " + String.Format("{0}{1}", RespMessage(0), ex.ToString()));
            return false;
        }
    }

    #region Old Approval Biker
    // Approve/Disapprove Biker
    // Biker to branch registration
    //september 27, 2019
    //Test user 
    //TEST1234 Mlinc1234567890
    //LAGR17094185 Mlinc123456789
    //public RegisterBiker ApprovedBiker(string walletno, string branchcode, string zonecode, bool isApproved, string operatorID)
    //{
    //    try
    //    {

    //        _ecomdbCon.OpenConnection();

    //        #region If Biker Exists
    //        List<dynamic> biker = _ecomdbCon.Query<dynamic>(_Query.ChecBranchkBiker, new { walletno, branchcode, zonecode }, 60).ToList();
    //        if (biker[0].isApproved == true && biker[0].isApproved == isApproved)
    //        {
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Error(string.Format("Biker Already Exists in Branch: {0} walletno: {1}", branchcode, walletno));
    //            return new RegisterBiker { respcode = 0, respmsg = "The Rider is already registered in your branch." };
    //        }
    //        if (biker[0].isApproved == false && biker[0].isApproved == isApproved)
    //        {
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Error(string.Format("Biker Already Exists in Branch: {0} walletno: {1}", branchcode, walletno));
    //            return new RegisterBiker { respcode = 0, respmsg = "The Rider is already disapproved." };
    //        }
    //        if (biker.Count > 0)
    //        {
    //            #region Update Biker Status
    //            _ecomdbCon.StartDapperTran();
    //            Dictionary<string, object> UpdateBikerParam = new Dictionary<string, object>()
    //                {
    //                     {"isApproved",Convert.ToInt16(isApproved) },
    //                     {"walletno",walletno},
    //                     {"branchcode",branchcode},
    //                     {"zonecode",zonecode}

    //                };
    //            int updateBiker = _ecomdbCon.Execute(_Query.UpdateBranchRider, UpdateBikerParam, 60);
    //            if (updateBiker < 1)
    //            {
    //                _ecomdbCon.RollbackTran();
    //                _ecomdbCon.Dispose("DAPPER");
    //                kplog.Error(string.Format("walletNo: {0} branchcode: {1} Unable to add biker to branch!", walletno, branchcode));
    //                return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), };
    //            }
    //            #endregion

    //            #region Check Biker Logs and Update/Insert new
    //            List<dynamic> CheckBikerLogs = _ecomdbCon.Query<dynamic>(_Query.CheckBikerLogs, new { walletno, operatorID }, 60).ToList();
    //            if (CheckBikerLogs.Count > 0)
    //            {
    //                Dictionary<string, object> updateBikerLogsParam = new Dictionary<string, object>()
    //                 {
    //                     {"isApproved",Convert.ToInt16(isApproved) },   
    //                     {"sysmodifier",operatorID},
    //                     {"walletno",walletno}                 

    //                 };
    //                int updateBikerLogs = _ecomdbCon.Execute(_Query.UpdateBranchRiderLogs, updateBikerLogsParam, 60);
    //                if (updateBikerLogs < 1)
    //                {
    //                    _ecomdbCon.RollbackTran();
    //                    _ecomdbCon.Dispose("DAPPER");
    //                    kplog.Error(string.Format("walletNo: {0} branchcode: {1} Unable to add biker to branch!", walletno, branchcode));
    //                    return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), };
    //                }
    //            }
    //            else
    //            {
    //                Dictionary<string, object> SaveBikerLogsParam = new Dictionary<string, object>()
    //                 {
    //                        {"walletno",walletno},
    //                         {"isApproved",Convert.ToInt16(isApproved) },                         
    //                         {"sysmodifier",operatorID}

    //                 };
    //                int SaveBikerLogs = _ecomdbCon.Execute(_Query.SaveBikerLogs, SaveBikerLogsParam, 60);
    //                if (SaveBikerLogs < 1)
    //                {
    //                    _ecomdbCon.RollbackTran();
    //                    _ecomdbCon.Dispose("DAPPER");
    //                    _ecomdbCon.Dispose("DAPPER");
    //                    kplog.Error(string.Format("walletNo: {0} branchcode: {1} Unable to add biker logs to branch!", walletno, branchcode));
    //                    return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), };
    //                }
    //            }
    //            #endregion

    //            _ecomdbCon.CommitTran();
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Info(string.Format("walletNo: {0} branchcode: {1} Success updating biker to branch isApproved: {2}!", walletno, branchcode, isApproved));
    //            return new RegisterBiker { respcode = 1, respmsg = isApproved.Equals(true) ? "Rider is now registered in your branch." : "Successfully disapproved rider." };
    //            #region Old Checking
    //            //_ecomdbCon.Dispose("DAPPER");
    //            //kplog.Error(string.Format("Biker Already Exists in Branch: {0} walletno: {1}", branchcode,walletno));
    //            //return new RegisterBiker { respcode = 0, respmsg = "The Rider is already registered in your branch." };
    //            #endregion

    //        }
    //        #endregion

    //        _domesticCon.OpenConnection();
    //        List<Property.BranchDetails> branchDetails = _domesticCon.Query<Property.BranchDetails>(_Query.GetBranchDetails, new { branchcode, zonecode }, 60).ToList();
    //        if (branchDetails.Count < 1)
    //        {
    //            _domesticCon.Dispose("DAPPER");
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Error(string.Format("Unable to fetch data from branch: {0} zonecode: {1}", branchcode, zonecode));
    //            return new RegisterBiker { respcode = 0, respmsg = "Unable to retreive data from branch." };
    //        }
    //        #region Save Biker to Branch as Approved/Disapproved
    //        Dictionary<string, object> BranchInfoParam = new Dictionary<string, object>()
    //            {
    //                {"walletno",walletno},
    //                {"branchcode",branchDetails[0].branchcode},
    //                {"branchname",branchDetails[0].branchname},
    //                {"branchaddress",branchDetails[0].branchaddress},
    //                {"zonecode",branchDetails[0].zonecode},
    //                {"phone",branchDetails[0].phone},
    //                {"latitude",branchDetails[0].latitude},
    //                {"longitude",branchDetails[0].longitude },
    //                {"isActive",1},
    //                {"isAvailable",1 },
    //                {"isApproved",Convert.ToInt16(isApproved) }
    //            };
    //        _ecomdbCon.StartDapperTran();
    //        int saveBiker = _ecomdbCon.Execute(_Query.SaveBikerToBranch, BranchInfoParam, 60);
    //        if (saveBiker < 1)
    //        {
    //            _ecomdbCon.RollbackTran();
    //            _domesticCon.Dispose("DAPPER");
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Error(string.Format("walletNo: {0} branchcode: {1} Unable to add biker to branch!", walletno, branchcode));
    //            return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), };
    //        }
    //        #endregion

    //        #region Save Biker Logs
    //        Dictionary<string, object> SaveBikerLogsNewParam = new Dictionary<string, object>()
    //             {
    //                    {"walletno",walletno},
    //                     {"isApproved",Convert.ToInt16(isApproved) },                         
    //                     {"sysmodifier",operatorID}

    //             };
    //        int SaveBikerNewLogs = _ecomdbCon.Execute(_Query.SaveBikerLogs, SaveBikerLogsNewParam, 60);
    //        if (SaveBikerNewLogs < 1)
    //        {
    //            _ecomdbCon.RollbackTran();
    //            _ecomdbCon.Dispose("DAPPER");
    //            _ecomdbCon.Dispose("DAPPER");
    //            kplog.Error(string.Format("walletNo: {0} branchcode: {1} Unable to add biker logs to branch!", walletno, branchcode));
    //            return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), };
    //        }
    //        #endregion

    //        _ecomdbCon.CommitTran();
    //        _domesticCon.Dispose("DAPPER");
    //        _ecomdbCon.Dispose("DAPPER");
    //        kplog.Info(string.Format("walletNo: {0} branchcode: {1} Success saving biker to branch:  isApproved: {2}", walletno, branchcode, isApproved));
    //        return new RegisterBiker { respcode = 1, respmsg = isApproved.Equals(true) ? "Rider is now registered in your branch." : "Successfully disapproved rider." };
    //    }
    //    catch (TimeoutException tex)
    //    {
    //        kplog.Fatal("Error #02: " + String.Format("{0}{1}", RespMessage(0), tex.ToString()));
    //        if (_ecomdbCon != null) _ecomdbCon.Dispose("DAPPER");
    //        return new RegisterBiker { respcode = 0, respmsg = RespMessage(5), errorDetails = tex.ToString() };
    //    }
    //    catch (Exception ex)
    //    {
    //        kplog.Fatal("Error #03: " + String.Format("{0}{1}", RespMessage(0), ex.ToString()));
    //        if (_ecomdbCon != null) _ecomdbCon.Dispose("DAPPER");
    //        return new RegisterBiker { respcode = 0, respmsg = RespMessage(3), errorDetails = ex.ToString() };
    //    }

    //}
    #endregion

    private string RespMessage(int msg)
    {

        switch (msg)
        {
            case 0:
                return "Service Error: ";
            case 1:
                return "Success!";
            case 2:
                return "No list of products";
            case 3:
                return "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience.";
            case 4:
                return "Please review your data and try again.";
            case 5:
                return "Unable to process request. Connection timeout occured. Please try again later.";
            case 6:
                return "Unable to process request. Failed in connecting to server. Please try again later.";
            case 7:
                return "Incorrect Username Or Password. Please Try Again.";
            case 8:
                return "Unable to process request. Some Record's already exist. Please try again later.";
            case 9:
                return "Already paid/processs.";
            case 10:
                return "Request Message sent!";
            case 11:
                return "Inactive User!";
            case 12:
                return "Successfully Log-in";
            case 13:
                return "Successfully Saved";
            case 14:
                return "Already fulfilled";
            default:
                return string.Empty;
        }
    }
}

