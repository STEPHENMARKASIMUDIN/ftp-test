using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Summary description for Property
/// </summary>
/// 
[DataContract]
public class Property
{
    [DataContract]
    public class BikerBranchCommonInfo 
    {
        private decimal _latitude { get; set; }

        private decimal _longitude { get; set; }
        private string _phone { get; set; }
        [DataMember]
        public string phone { get { return _phone; } set { _phone = value; } }
        [DataMember]
        public decimal latitude { get { return _latitude; } set { _latitude = value; } }
        [DataMember]
        public decimal longitude { get { return _longitude; } set { _longitude = value; } }
    }
 
    
    [DataContract]
    public class BikersInfo : BikerBranchCommonInfo
    {
      
        private string _walletno { get; set; }
       
        private string _custID { get; set; }
        
        private string _first_name { get; set; }
      
        private string _last_name { get; set; }
        private string _middle_name { get; set; }
        
        private string _address { get; set; }
        private string _vehicle_type { get; set; }
       
        private int _delivery_count_history { get; set; }
       
    

        private int _isActive { get; set; }

        private int _isAvailable { get; set; }

        private string _birthdate { get; set; }
        private string _license_no { get; set; }
        private string _expiry_date { get; set; }
        private string _full_contact { get; set; }
        private string _facebook { get; set; }
        private string _viber { get; set; }
        private string _gmail { get; set; }
        private string _nbi_clearance { get; set; }
        private string _brgy_clearance { get; set; }
        private string _vehicle { get; set; }
        private string _driver_licence { get; set; }
        private string _vehicle_OR { get; set; }
        private string _vehicle_CR { get; set; }
        [DataMember]
        public string nbi_clearance { get { return _nbi_clearance; } set { _nbi_clearance = value; } }
        [DataMember]
        public string brgy_clearance { get { return _brgy_clearance; } set { _brgy_clearance = value; } }
        [DataMember]
        public string vehicle { get { return _vehicle; } set { _vehicle = value; } }
        [DataMember]
        public string driver_licence { get { return _driver_licence; } set { _driver_licence = value; } }
        [DataMember]
        public string vehicle_OR { get { return _vehicle_OR; } set { _vehicle_OR = value; } }
        [DataMember]
        public string vehicle_CR { get { return _vehicle_CR; } set { _vehicle_CR = value; } }
        //
        [DataMember]
        public string birthdate { get { return _birthdate; } set { _birthdate = value; } }
        [DataMember]
        public string license_no { get { return _license_no; } set { _license_no = value; } }
        [DataMember]
        public string expiry_date { get { return _expiry_date; } set { _expiry_date = value; } }
        [DataMember]
        public string vehicle_type { get { return _vehicle_type; } set { _vehicle_type = value; } }
        [DataMember]
        public string full_contact { get { return _full_contact; } set { _full_contact = value; } }
        [DataMember]
        public string facebook { get { return _facebook; } set { _facebook = value; } }
        [DataMember]
        public string viber { get { return _viber; } set { _viber = value; } }
        [DataMember]
        public string gmail { get { return _gmail; } set { _gmail = value; } }

        [DataMember]
        public string walletno { get { return _walletno; } set { _walletno = value; } }
        [DataMember]
        public string custID { get { return _custID; } set { _custID = value; } }
        [DataMember]
        public string first_name { get { return _first_name; } set { _first_name = value; } }
        [DataMember]
        public string last_name { get { return _last_name; } set { _last_name = value; } }
        [DataMember]
        public string middle_name { get { return _last_name; } set { _last_name = value; } }
        [DataMember]
        public string address { get { return _address; } set { _address = value; } }
     
        [DataMember]
        public int delivery_count_history { get { return _delivery_count_history; } set { _delivery_count_history = value; } }
       
        [DataMember]
        public int isActive { get { return _isActive; } set { _isActive = value; } }
        [DataMember]
        public int isAvailable { get { return _isAvailable; } set { _isAvailable = value; } }

    }
    [DataContract]
    public class BranchDetails : BikerBranchCommonInfo
    {
        private string _isBooked;
        private int _branchcode;
      
        private string _branchname;
        
        private string _branchaddress;
       
        private int _zonecode;            

        //
        [DataMember]
        public string isBooked { get { return _isBooked; } set { _isBooked = value; } }
        [DataMember]
        public int branchcode { get { return _branchcode; } set { _branchcode = value; } }

        [DataMember]
        public string branchname { get { return _branchname; } set { _branchname = value; } }

        [DataMember]
        public string branchaddress { get { return _branchaddress; } set { _branchaddress = value; } }

        [DataMember]
        public int zonecode { get { return _zonecode; } set { _zonecode = value; } }

       
    }
    [DataContract]
    public class BikersDeliveryDetails : DeliveryModels
    {
       
        private string _lotnumber;
        [DataMember]
        public string lotnumber { get { return _lotnumber; } set { _lotnumber = value; } }
    
        private string _qrcode;
        [DataMember]
        public string qrcode { get { return _qrcode; } set { _qrcode = value; } }
     
        private string _delivery_status;
        [DataMember]
        public string delivery_status { get { return _delivery_status; } set { _delivery_status = value; } }

    }
    [DataContract]
    public class BranchItemsDetails : BranchDetails
    {
            
        private string _custName;
      
        private string _custContact;
      
        private string _custAddress;
      
        private string _orderdetail;
      
        private string _merchant;

        [DataMember]
        public string orderdetail { get { return _orderdetail; } set { _orderdetail = value; } }

        [DataMember]
        public string custName { get { return _custName; } set { _custName = value; } }
        [DataMember]
        public string custContact { get { return _custContact; } set { _custContact = value; } }
        [DataMember]
        public string custAddress { get { return _custAddress; } set { _custAddress = value; } }
        [DataMember]
        public string merchant { get { return _merchant; } set { _merchant = value; } }
    }
    [DataContract]
    public class DeliveryModels : BranchDetails
    {

        private int _quantity;
        private string _orderID;
    
        private string _custName;
    
        private string _custAddress;
    
        private string _merchant;
    
        private string _custContact;

        private string _custWalletno;
    
        private string _kptnno;
    
        private string _transdate;
    
        private string _orderdetail;
    
        private string _lineitemID;
    
        private string _operatorID;
        private string _receiverName;
        private string _receiverAddress;
        private string _receiverContact;
        private string _claim_code;


        [DataMember]
        public string receiverName { get { return _receiverName; } set { _receiverName = value; } }
        [DataMember]
        public string receiverAddress { get { return _receiverAddress; } set { _receiverAddress = value; } }
        [DataMember]
        public string receiverContact { get { return _receiverContact; } set { _receiverContact = value; } }
        [DataMember]
        public string claim_code { get { return _claim_code; } set { _claim_code = value; } }
        
        [DataMember]
        public int quantity { get { return _quantity; } set { _quantity = value; } }
              
        [DataMember]
        public string operatorID { get { return _operatorID; } set { _operatorID = value; } }
              
        [DataMember]
        public string lineitemID { get { return _lineitemID; } set { _lineitemID = value; } }
        [DataMember]
        public string orderdetail { get { return _orderdetail; } set { _orderdetail = value; } }
        [DataMember]
        public string orderID { get { return _orderID; } set { _orderID = value; } }      
        [DataMember]
        public string custName { get { return _custName; } set { _custName = value; } }
        [DataMember]
        public string custAddress { get { return _custAddress; } set { _custAddress = value; } }
        [DataMember]
        public string merchant { get { return _merchant; } set { _merchant = value; } }
        [DataMember]
        public string custContact { get { return _custContact; } set { _custContact = value; } }
        [DataMember]
        public string custWalletno { get { return _custWalletno; } set { _custWalletno = value; } }
        [DataMember]
        public string kptnno { get { return _kptnno; } set { _kptnno = value; } }
        [DataMember]
        public string transdate { get { return _transdate; } set { _transdate = value; } }
    }
    [DataContract]
    public class LotNumberList
    {
      
        private string _lotnumber;
        [DataMember]
        public string lotnumber { get { return _lotnumber; } set { _lotnumber = value; } }
        private string _branchaddress;
        [DataMember]
        public string branchaddress { get { return _branchaddress; } set { _branchaddress = value; } }
        private string _syscreated;
        [DataMember]
        public string syscreated { get { return _syscreated; } set { _syscreated = value; } }
    }
    [DataContract]
    public class BranchData : BranchDetails
    {
      
      
       
        private string _operatorID;
        [DataMember]
        public string operatorID { get { return _operatorID; } set { _operatorID = value; } }
    }
    [DataContract]
    public class Registration
    {
        private string _walletno;
        private string _custID;
        private string _first_name;
        private string _last_name;
        private string _birthdate;
        private string _address;
        private string _license_no;
        private string _expiry_date;
        private string _full_contact;
        private string _facebook;
        private string _viber;
        private string _gmail;
        private string _phone;
        private decimal _latitude;
        private decimal _longitude;
        private string _nbi_clearance;
        private string _brgy_clearance;
        private string _vehicle;
        private string _driver_licence;
        private string _vehicle_OR;
        private string _vehicle_CR;
        private string _vehicle_type;
        private string _pin;

        [DataMember]
        public string pin { get { return _pin; } set { _pin = value; } }

        [DataMember]
        public string walletno { get { return _walletno; } set { _walletno = value; } }
        [DataMember]
        public string vehicle_type { get { return _vehicle_type; } set { _vehicle_type = value; } }
        [DataMember]
        public string custID { get { return _custID; } set { _custID = value; } }
        [DataMember]
        public string first_name { get { return _first_name; } set { _first_name = value; } }
        [DataMember]
        public string last_name { get { return _last_name; } set { _last_name = value; } }
        [DataMember]
        public string birthdate { get { return _birthdate; } set { _birthdate = value; } }
        [DataMember]
        public string address { get { return _address; } set { _address = value; } }
        [DataMember]
        public string license_no { get { return _license_no; } set { _license_no = value; } }
        [DataMember]
        public string expiry_date { get { return _expiry_date; } set { _expiry_date = value; } }
        [DataMember]
        public string full_contact { get { return _full_contact; } set { _full_contact = value; } }
        [DataMember]
        public string facebook { get { return _facebook; } set { _facebook = value; } }
        [DataMember]
        public string gmail { get { return _gmail; } set { _gmail = value; } }
        [DataMember]
        public string viber { get { return _viber; } set { _viber = value; } }
        [DataMember]
        public string phone { get { return _phone; } set { _phone = value; } }
        [DataMember]
        public decimal latitude { get { return _latitude; } set { _latitude = value; } }
        [DataMember]
        public decimal longitude { get { return _longitude; } set { _longitude = value; } }
        [DataMember]
        public string nbi_clearance { get { return _nbi_clearance; } set { _nbi_clearance = value; } }
        [DataMember]
        public string brgy_clearance { get { return _brgy_clearance; } set { _brgy_clearance = value; } }
        [DataMember]
        public string vehicle { get { return _vehicle; } set { _vehicle = value; } }
        [DataMember]
        public string driver_licence { get { return _driver_licence; } set { _driver_licence = value; } }
        [DataMember]
        public string vehicle_OR { get { return _vehicle_OR; } set { _vehicle_OR = value; } }
        [DataMember]
        public string vehicle_CR { get { return _vehicle_CR; } set { _vehicle_CR = value; } }



    }
}