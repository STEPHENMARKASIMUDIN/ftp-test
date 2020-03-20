using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading;
using System.Web;
using System.Web.Configuration;

/// <summary>
/// Summary description for RequestHandler
/// </summary>
internal class Request
{


    public Request()
    {

    }
}
internal class RequestHandler : Request
{
    #region Private Variables
    private string _Method = string.Empty;
    private string _QueryString = string.Empty;
    //private byte[] _jsonData = null;
    private static readonly ILog kplog = LogManager.GetLogger(typeof(Biker));
    private string _EcommerceAPI = string.Empty;

    #endregion

    /*<summary>
        HTTPPostRequest Constructor
     <summary>*/
    public RequestHandler() { }
    //public RequestHandler(string Method, byte[] jsonData, string QueryString)
    //{

    //    _Method = Method;
    //    _QueryString = QueryString;
    //    _jsonData = jsonData;
    //}

    /*<summary>
        HTTPGetRequest Constructor
     <summary>*/
    public RequestHandler(string Method, string QueryString)
    {
        _EcommerceAPI = WebConfigurationManager.AppSettings["ecommerce"].ToString();
        _Method = Method;
        _QueryString = QueryString;
    }


    public virtual string HttpGetRequest()
    {
        int attempt = 0;
        do
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                if (attempt > 0)
                {

                    if (attempt == 1)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                    else
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    }
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_EcommerceAPI + _Method + _QueryString) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = Timeout.Infinite;
                WebResponse webresponse = request.GetResponse();
                String res = null;
                using (Stream response = webresponse.GetResponseStream())
                {
                    if (webresponse != null)
                    {
                        using (StreamReader reader = new StreamReader(response))
                        {
                            res = reader.ReadToEnd();
                            reader.Close();
                            webresponse.Close();
                        }
                    }
                }
                return res;
            }
            catch (WebException ex)
            {
                kplog.Fatal("Error Details: {0} : " + ex.ToString());
                if (ex.ToString().ToLower().Contains("underlying") || ex.ToString().ToLower().Contains("ssl"))
                {
                    if (attempt < 2)
                    {
                        attempt++;
                        Thread.Sleep(attempt * 2000);
                    }
                    else
                    {
                        try
                        {
                            using (WebResponse response = ex.Response)
                            {
                                HttpWebResponse httpResponse = (HttpWebResponse)response;
                                kplog.Fatal("Error code: {0} : " + httpResponse.StatusCode);
                                using (Stream data = response.GetResponseStream())
                                using (var reader = new StreamReader(data))
                                {
                                    string text = reader.ReadToEnd();
                                    String respcode = httpResponse.StatusCode.ToString();
                                    if (respcode == "422")
                                    {
                                        kplog.Info("_QueryString: " + _QueryString + " Response Message : Already fulfilled");
                                        kplog.Info("_QueryString: " + _QueryString + " Response Message : " + text);
                                        return "ERROR" + "|" + text;
                                    }
                                    kplog.Info("_QueryString: " + _QueryString + " Response Message : " + text);
                                    return "ERROR" + "|" + text;
                                }
                            }
                        }
                        catch (Exception tex)
                        {
                            return "ERROR" + "|" + tex.ToString();
                        }
                    }
                }
                else
                {
                    return "ERROR" + "|" + ex.ToString();
                }
            }
        } while (true);
    }
   
    //public virtual string PostPrefund(Uri uri, byte[] jsonDataBytes, string contentType, string method)
    //{
    //    int attempt = 0;
    //    do
    //    {
    //        try
    //        {
    //            kplog.Info(String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "Uri: ", uri, " | jsonDataBytes: ", jsonDataBytes, " | contentType: ",
    //                contentType, " | method: ", method));
    //            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
    //            if (attempt > 0)
    //            {

    //                if (attempt == 1)
    //                {
    //                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    //                }
    //                else
    //                {
    //                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
    //                }
    //            }
    //            WebRequest req = WebRequest.Create(uri);
    //            req.ContentType = contentType;
    //            req.Method = method;
    //            req.ContentLength = jsonDataBytes.Length;
    //            req.Timeout = Timeout.Infinite;

    //            Stream stream = req.GetRequestStream();
    //            stream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
    //            stream.Close();

    //            WebResponse webresponse = req.GetResponse();
    //            Stream response = webresponse.GetResponseStream();

    //            string res = null;
    //            if (response != null)
    //            {
    //                var reader = new StreamReader(response);
    //                res = reader.ReadToEnd();
    //                reader.Close();
    //                response.Close();
    //            }
    //            kplog.Info(String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", "uri: " + uri.ToString(), " | jsonDataBytes: ", jsonDataBytes,
    //                " | contentType: ", contentType, " | method: ", method, " - ", res));
    //            return res;
    //        }
    //        catch (WebException ex)
    //        {
    //            kplog.Fatal("Error #03: " + "uri: " + uri.ToString() + String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", " | jsonDataBytes: ", jsonDataBytes, " | contentType: ", contentType, " | method: ", method, " - ", ex));
    //            if (ex.ToString().ToLower().Contains("underlying") || ex.ToString().ToLower().Contains("ssl"))
    //            {
    //                if (attempt < 2)
    //                {
    //                    attempt++;
    //                    Thread.Sleep(attempt * 2000);
    //                }
    //                else
    //                {
    //                    try
    //                    {
    //                        using (WebResponse response = ex.Response)
    //                        {
    //                            HttpWebResponse httpResponse = (HttpWebResponse)response;
    //                            kplog.Fatal("Error code: {0} : " + httpResponse.StatusCode);
    //                            using (Stream data = response.GetResponseStream())
    //                            using (var reader = new StreamReader(data))
    //                            {
    //                                string text = reader.ReadToEnd();
    //                                String respcode = httpResponse.StatusCode.ToString();
    //                                if (respcode == "422")
    //                                {
    //                                    kplog.Info("_QueryString: " + _QueryString + " Response Message : Already fulfilled");
    //                                    kplog.Info("_QueryString: " + _QueryString + " Response Message : " + text);
    //                                    return "ERROR" + "|" + text;
    //                                }
    //                                kplog.Info("_QueryString: " + _QueryString + " Response Message : " + text);
    //                                return "ERROR" + "|" + text;
    //                            }
    //                        }
    //                    }
    //                    catch (Exception tex)
    //                    {
    //                        return "ERROR" + "|" + tex.ToString();
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                return "ERROR" + "|" + ex.ToString();
    //            }

    //        }
    //    } while (true);
    //}
}
//catch (TimeoutException ex)
//        {
//            kplog.Fatal("Error #02: " + "uri: " + uri.ToString() + String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", " | jsonDataBytes: ", jsonDataBytes, " | contentType: ", contentType, " | method: ", method, " - ", ex));
//            return ex.ToString();
//        }
//        catch (Exception ex)
//        {
//            kplog.Fatal("Error #03: " + "uri: " + uri.ToString() + String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", " | jsonDataBytes: ", jsonDataBytes, " | contentType: ", contentType, " | method: ", method, " - ", ex));
//            return ex.ToString();
//        }