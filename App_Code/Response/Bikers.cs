using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Summary description for Bikers
/// </summary>
/// 
[DataContract]
public class BikerLogin:Indicators
{
    
    private Property.BikersInfo _bikersInfo;
 
    [DataMember]

    public Property.BikersInfo bikersInfo { get { return _bikersInfo; } set { _bikersInfo = value; } }
    
}
[DataContract]
public class BikersBranches : Indicators
{
    
    private List<Property.BranchDetails> _bikersBranches;
    [DataMember]

    public List<Property.BranchDetails> bikersBranches { get { return _bikersBranches; } set { _bikersBranches = value; } }
}

[DataContract]
public class BikersDeliveries : Indicators
{
    
    private List<Property.BikersDeliveryDetails> _bikersDeliveries;
    [DataMember]

    public List<Property.BikersDeliveryDetails> bikersDeliveries { get { return _bikersDeliveries; } set { _bikersDeliveries = value; } }
    
    private List<Property.LotNumberList> _lotnumberlist;
       [DataMember]
    public List<Property.LotNumberList> lotnumberlist { get { return _lotnumberlist; } set { _lotnumberlist = value; } }
}

[DataContract]
public class BranchItems : Indicators
{
    
    private List<Property.BranchItemsDetails> _branchItems;
    [DataMember]

    public List<Property.BranchItemsDetails> branchItems { get { return _branchItems; } set { _branchItems = value; } }
}
[DataContract]
public class BookDeliveries : Indicators
{
    private string _qrcode;
    [DataMember]
    public string qrcode { get { return _qrcode; } set { _qrcode = value; } }
}
[DataContract]
public class RegisterBiker : Indicators
{
  
}
