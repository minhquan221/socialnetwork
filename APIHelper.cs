using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using SocialWeb.Models;

namespace SocialWeb.Common
{
    public class APIHelper
    {

        //public static HttpWebRequest builHeader(HttpWebRequest client, List<string[]> header = null, string authen = "", string Id = "", string Secret = "", string strprivatekey = "", string strcertificate = "", string strdata = "")
        //{
        //    var ClientId = string.IsNullOrEmpty(Id) ? LoginUser.Current.ClientId : Id;
        //    var clientSecret = string.IsNullOrEmpty(Secret) ? LoginUser.Current.ClientSecret : Secret;
        //    var PrivateKey = string.IsNullOrEmpty(strprivatekey) ? LoginUser.Current.PrivateKey : strprivatekey;
        //    var Certificate = string.IsNullOrEmpty(strcertificate) ? LoginUser.Current.Certificate : strcertificate;
        //    var Authentication = LoginUser.Current.Authentication;
        //    client.Headers.Add("X-IBM-Client-Id", ClientId);
        //    client.Headers.Add("X-IBM-Secret-Id", clientSecret);
        //    client.Headers.Add("X-IBM-Client-Secret", clientSecret);
        //    var Signature = CommonJoseHelper.Current.Sign2(strdata, PrivateKey);
        //    client.Headers.Add("X-Signature", Signature);
        //    client.Headers.Add("X-Client-Certificate", Certificate);
        //    if (string.IsNullOrEmpty(authen))
        //        client.Headers.Add("Authorization", "Bearer " + Authentication);
        //    else
        //        client.Headers.Add("Authorization", "Bearer " + authen);
        //    if (header != null)
        //    {
        //        foreach (var item in header)
        //        {
        //            client.Headers.Add(item[0], item[1]);
        //        }
        //    }
        //    return client;
        //}

        public static string RandomTransId()
        {
            string result = string.Empty;
            var gui = Guid.NewGuid();
            result = gui.ToString();
            return result.Substring(0, 32);

        }

        public static T CallAPI<T>(ref int status, ref string Msg, ref object Err, string function, string method = WebRequestMethods.Http.Post, int offset = 0, string ContentType = "application/json", List<string[]> header = null, object parameters = null, string authen = "", string clientId = "", string clientSecret = "", string strprivatekey = "", string strcertificate = "") where T : new()
        {
            status = -1;
            Msg = string.Empty;
            Err = null;
            string ResponseData = string.Empty;
            try
            {
                var url = string.IsNullOrEmpty(LoginUser.Current.apiUrl) ? ConfigurationManager.AppSettings["Client:Url:API"] : LoginUser.Current.apiUrl;
                byte[] postData;
                List<string> lit = new List<string>();
                StringBuilder paramstring = new StringBuilder();
                paramstring.Append(JsonConvert.SerializeObject(parameters));
                postData = Encoding.ASCII.GetBytes(paramstring.ToString());

                HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(url + function);

                #region certificate

                //client.Credentials = CredentialCache.DefaultCredentials;
                client.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                #endregion
                client.KeepAlive = false;
                client.Method = method;
                string proxy = null;
                client.Proxy = new WebProxy(proxy, true);
                //client = builHeader(client, header, authen, clientId, clientSecret, strprivatekey, strcertificate, paramstring.ToString());
                client.CookieContainer = new CookieContainer();
                client.ContentType = ContentType;

                client.ContentLength = postData.Length;

                using (var stream = client.GetRequestStream())
                {
                    stream.Write(postData, offset, postData.Length);
                }
                var response = (HttpWebResponse)client.GetResponse();
                ResponseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (ResponseData.IndexOf("error") >= 0)
                {
                    status = 0;
                    //if (ResponseData.IndexOf("error_description") >= 0)
                    //{
                    //    Err = new responseError<string>();
                    //    Err = JsonConvert.DeserializeObject<responseError<string>>(ResponseData);
                    //}
                    //else
                    //{
                    //    Err = new responseError<error>();
                    //    Err = JsonConvert.DeserializeObject<responseError<error>>(ResponseData);
                    //}
                    return new T();
                }
                else
                {
                    var resultObject = JsonConvert.DeserializeObject<T>(ResponseData);
                    return resultObject;
                }
            }
            catch (Exception ex)
            {
                status = 0;
                Msg = ex.ToString();
                Err = ConvertHelper.Current.ConvertError(ex);
                return new T();
            }
        }


        public static JsonResultData AccessToken<T>(ref int status, ref string Msg, ref object Err, LoginModel user) where T : new()
        {
            status = -1;
            Msg = string.Empty;
            Err = null;
            string ResponseData = string.Empty;
            try
            {
                var url = string.IsNullOrEmpty(user.apiTokenUrl) ? ConfigurationManager.AppSettings["Client:Url:AccessToken"] : user.apiTokenUrl;
                //var ClientId = ConfigurationManager.AppSettings["Client:Id"];
                //var clientSecret = ConfigurationManager.AppSettings["Client:Secret"];

                var ClientId = user.ClientId;
                var clientSecret = user.ClientSecret;
                var Username = user.Username;
                var Password = user.Password;

                var postData = "scope=" + HttpUtility.UrlEncode("OCBSIT");
                postData += "&username=" + HttpUtility.UrlEncode(Username);
                postData += "&password=" + HttpUtility.UrlEncode(Password);
                postData += "&client_id=" + HttpUtility.UrlEncode(ClientId);
                postData += "&client_secret=" + HttpUtility.UrlEncode(clientSecret);
                postData += "&grant_type=" + HttpUtility.UrlEncode("password");

                var body = Encoding.ASCII.GetBytes(postData);

                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(url);
                httpWReq.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/x-www-form-urlencoded";

                httpWReq.ContentLength = body.Length;

                using (var stream = httpWReq.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                }

                HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                //var response = (HttpWebResponse)client.GetResponse();
                ResponseData = new StreamReader(httpWResp.GetResponseStream()).ReadToEnd();
                var resultObject = JsonConvert.DeserializeObject<T>(ResponseData);
                return new JsonResultData
                {
                    IsOk = true,
                    dataErr = null,
                    Msg = string.Empty,
                    dataObj = resultObject
                };
            }
            catch (Exception ex)
            {
                status = 0;
                Msg = ex.ToString();
                Err = ConvertHelper.Current.ConvertError(ex);
                return new JsonResultData
                {
                    IsOk = false,
                    dataErr = Err,
                    Msg = Msg,
                    dataObj = null
                };
            }
        }


        public static object GetFromNode(object parameters = null, string strprivatekey = "")
        {
            string ResponseData = string.Empty;
            try
            {
                var data = new
                {
                    data = parameters,
                    privatekey = strprivatekey
                };
                byte[] postData;
                List<string> lit = new List<string>();
                StringBuilder paramstring = new StringBuilder();
                paramstring.Append(JsonConvert.SerializeObject(data));
                postData = Encoding.ASCII.GetBytes(paramstring.ToString());

                HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings["ServerNode:URL"] + ":" + ConfigurationManager.AppSettings["ServerNode:Port"] + "/signaturedata");

                #region certificate

                //client.Credentials = CredentialCache.DefaultCredentials;
                client.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                #endregion
                client.KeepAlive = false;
                client.Method = "POST";
                string proxy = null;
                client.Proxy = new WebProxy(proxy, true);
                client.CookieContainer = new CookieContainer();
                client.ContentType = "application/json";
                client.Headers.Add("X-Private-Key", strprivatekey);
                client.ContentLength = postData.Length;

                using (var stream = client.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
                var response = (HttpWebResponse)client.GetResponse();
                ResponseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}