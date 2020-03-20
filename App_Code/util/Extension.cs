using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Extension
/// </summary>
internal interface ISerializer
{

}
internal static class Extension
{

    public static string Data { get; set; }
    internal static String Serialize(this ISerializer ser)
    {      
        return JsonConvert.SerializeObject(ser, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        });
    }
    internal static T Deserialize<T>(this string str)
    {

        return JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        });
    }
    internal static byte[] ToByte(this string str) 
    {
        return UTF8Encoding.UTF8.GetBytes(str);
    }
    internal static Int32 ParseInt(this object value)
    {
        return Convert.ToInt32(value);
    }
    internal static Int64 ParseLongInt(this object value)
    {
        return Convert.ToInt64(value);
    }
    internal static Decimal ParseDecimal(this object value)
    {
        return Convert.ToDecimal(value);
    }
    internal static Double ParseDouble(this object value)
    {
        return Convert.ToDouble(value);
    }
    internal static Boolean IsNull(this object value)
    {
        if (value == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    internal static String ToDate(this object value, int format)
    {
        switch (format)
        {
            case 0:
                return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss").Trim();
            case 1:
                return Convert.ToDateTime(value).ToString("MM").Trim();
            case 2:
                return Convert.ToDateTime(value).ToString("yyyy").Trim();
            case 3:
                return Convert.ToDateTime(value).ToString("yyyy-MM-dd").Trim();
            case 4:
                return Convert.ToDateTime(value).ToString("MMMM dd, yyyy").Trim();
            default:
                return "0000-00-0000";
        }
    }
    internal static byte[] ToBase64(this string value)
    {
        return Convert.FromBase64String(value);
    }
    internal static Double Round(this double value)
    {
        return Math.Round(value, 2);
    }
}