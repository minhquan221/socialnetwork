using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using System.Web.Http.Results;
using System.Xml.Serialization;
using SocialWeb.Models;

namespace SocialWeb.Common
{
    public class CacheMongoDBHelper
    {
        public static int ExpirateDefault = ConfigurationManager.AppSettings["Default:Cache:Time"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["Default:Cache:Time"]) : 30;
        // set permutations
        public const String strPermutation = "ouiveyxaqtd";
        public const Int32 bytePermutation1 = 0x19;
        public const Int32 bytePermutation2 = 0x59;
        public const Int32 bytePermutation3 = 0x17;
        public const Int32 bytePermutation4 = 0x41;

        //public CacheMongoDBHelper()
        //{
        //    var rsa = new RSACryptoServiceProvider();
        //    _privateKey = rsa.ToXmlString(true);
        //    _publicKey = rsa.ToXmlString(false);
        //}

        public static string BuildKeyCache(params object[] parameters)
        {
            StringBuilder ResponseString = new StringBuilder();

            foreach (var item in parameters)
            {
                if (item != null && !string.IsNullOrEmpty(item.ToString()))
                {
                    ResponseString.Append(item.ToString() + "_");
                }
                else
                {
                    ResponseString.Append("_");
                }
            }
            return Encrypt(ResponseString.ToString().Substring(0, ResponseString.Length - 1));
        }

        // encoding
        public static string Encrypt(string strData)
        {

            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(strData)));

        }


        // decoding
        public static string Decrypt(string strData)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(strData)));

        }

        // encrypt
        public static byte[] Encrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(strPermutation,
            new byte[] { bytePermutation1,
                         bytePermutation2,
                         bytePermutation3,
                         bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        // decrypt
        public static byte[] Decrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(strPermutation,
            new byte[] { bytePermutation1,
                         bytePermutation2,
                         bytePermutation3,
                         bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        public static List<T> GetCache<T>(out int errCode, out string errMessage, out long TotalRows, out DateTime ExpirateDate, string keycache)
        {
            errCode = 0;
            errMessage = string.Empty;
            TotalRows = 0;
            ExpirateDate = DateTime.Now;
            string ResponseData = string.Empty;
            try
            {
                byte[] postData;
                List<string> lit = new List<string>();
                StringBuilder paramstring = new StringBuilder();
                var objectJson = new
                {
                    keycache
                };
                paramstring.Append(JsonConvert.SerializeObject(objectJson));
                postData = Encoding.UTF8.GetBytes(paramstring.ToString());

                HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings["ServerNode:URL"] + ":" + ConfigurationManager.AppSettings["ServerNode:Port"] + "/getcachesearch");
                client.KeepAlive = false;
                client.Method = WebRequestMethods.Http.Post;
                string proxy = null;
                client.Proxy = new WebProxy(proxy, true);
                client.CookieContainer = new CookieContainer();
                client.ContentType = "application/json";
                client.ContentLength = postData.Length;

                using (var stream = client.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
                var response = (HttpWebResponse)client.GetResponse();
                ResponseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var resultobjectparse = ResponseData.Replace("\"ListObject\"", "~").Split('~');
                var resultObject = JsonConvert.DeserializeObject<ObjectCacheMongo<T>>(resultobjectparse[0].Substring(1, resultobjectparse[0].Length - 1) + "}");
                if (resultObject != null)
                {
                    TotalRows = resultObject.TotalRows;
                    ExpirateDate = resultObject.ExpirateDate != null ? DateTime.ParseExact(resultObject.ExpirateDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.CurrentCulture) : DateTime.Now;
                    var objList = JsonConvert.DeserializeObject<List<T>>(resultobjectparse[1].Substring(1, resultobjectparse[1].Length - 3));
                    return objList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                errCode = 0;
                errMessage = ex.ToString();
                return null;
            }
        }
        public static string ClearCache()
        {
            string ResponseData = string.Empty;
            try
            {
                HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings["ServerNode:URL"] + ":" + ConfigurationManager.AppSettings["ServerNode:Port"] + "/cleardatacache");
                client.KeepAlive = false;
                client.Method = WebRequestMethods.Http.Post;
                string proxy = null;
                client.Proxy = new WebProxy(proxy, true);
                client.CookieContainer = new CookieContainer();
                client.ContentType = "application/json";

                var response = (HttpWebResponse)client.GetResponse();
                ResponseData = new StreamReader(response.GetResponseStream()).ReadToEnd();

                return ResponseData;
            }
            catch (Exception ex)
            {
                return ResponseData;
            }
        }

        public static List<T> GetMongo<T>(ref long totalSize, params object[] parameters)
        {
            string ResponseData = string.Empty;
            try
            {
                byte[] postData;
                List<string> lit = new List<string>();
                StringBuilder paramstring = new StringBuilder();
                paramstring.Append(JsonConvert.SerializeObject(parameters));
                postData = Encoding.UTF8.GetBytes(paramstring.ToString());

                HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings["ServerNode:URL"] + ":" + ConfigurationManager.AppSettings["ServerNode:Port"] + "/querymongo");
                client.KeepAlive = false;
                client.Method = WebRequestMethods.Http.Post;
                string proxy = null;
                client.Proxy = new WebProxy(proxy, true);
                client.CookieContainer = new CookieContainer();
                client.ContentType = "application/json";
                client.ContentLength = postData.Length;

                using (var stream = client.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
                var response = (HttpWebResponse)client.GetResponse();
                ResponseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var resultObject = JsonConvert.DeserializeObject<ObjectQueryMongo<T>>(ResponseData);
                if (resultObject != null)
                {
                    totalSize = resultObject.TotalRows;
                    return resultObject.data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string CreateCache<T>(out int errCode, out string errMessage, string keycache, List<T> objectparams, long totalRows, int ExpirateTime = 0)
        {
            if (ExpirateTime == 0)
            {
                ExpirateTime = ExpirateDefault;
            }
            errCode = 0;
            errMessage = string.Empty;
            string ResponseData = string.Empty;
            try
            {
                byte[] postData;
                List<string> lit = new List<string>();
                StringBuilder paramstring = new StringBuilder();
                var CacheMongo = new ObjectCacheMongo<T>();
                CacheMongo.TotalRows = totalRows;
                CacheMongo.ExpirateDate = DateTime.Now.AddMinutes(ExpirateTime).ToString("dd-MM-yyyy HH:mm:ss");
                CacheMongo.ListObject = objectparams;
                CacheMongo._id = keycache;
                var objectdata = new
                {
                    keycache = keycache,
                    data = CacheMongo
                };
                paramstring.Append(JsonConvert.SerializeObject(objectdata));
                postData = Encoding.UTF8.GetBytes(paramstring.ToString());

                HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings["ServerNode:URL"] + ":" + ConfigurationManager.AppSettings["ServerNode:Port"] + "/createdatacache");
                client.KeepAlive = false;
                client.Method = WebRequestMethods.Http.Post;
                string proxy = null;
                client.Proxy = new WebProxy(proxy, true);
                client.CookieContainer = new CookieContainer();
                client.ContentType = "application/json";
                client.ContentLength = postData.Length;

                using (var stream = client.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
                var response = (HttpWebResponse)client.GetResponse();
                ResponseData = new StreamReader(response.GetResponseStream()).ReadToEnd();

                return ResponseData;
            }
            catch (Exception ex)
            {
                errCode = 0;
                errMessage = ex.ToString();
                return ResponseData;
            }
        }

        public static Task GetCacheAsync<T>(out int errCode, out string errMessage, out long TotalRows, out DateTime ExpirateDate, out List<T> data, string keycache)
        {
            errCode = 0;
            errMessage = string.Empty;
            TotalRows = 0;
            ExpirateDate = DateTime.Now;
            data = null;
            string ResponseData = string.Empty;
            try
            {
                byte[] postData;
                List<string> lit = new List<string>();
                StringBuilder paramstring = new StringBuilder();
                var objectJson = new
                {
                    keycache
                };
                paramstring.Append(JsonConvert.SerializeObject(objectJson));
                postData = Encoding.UTF8.GetBytes(paramstring.ToString());

                HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings["ServerNode:URL"] + ":" + ConfigurationManager.AppSettings["ServerNode:Port"] + "/getcachesearch");
                client.KeepAlive = false;
                client.Method = WebRequestMethods.Http.Post;
                string proxy = null;
                client.Proxy = new WebProxy(proxy, true);
                client.CookieContainer = new CookieContainer();
                client.ContentType = "application/json";
                client.ContentLength = postData.Length;

                using (var stream = client.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
                var response = (HttpWebResponse)client.GetResponse();
                ResponseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var resultobjectparse = ResponseData.Replace("\"ListObject\"", "~").Split('~');
                var resultObject = JsonConvert.DeserializeObject<ObjectCacheMongo<T>>(resultobjectparse[0].Substring(1, resultobjectparse[0].Length - 1) + "}");
                if (resultObject != null)
                {
                    TotalRows = resultObject.TotalRows;
                    ExpirateDate = DateTime.ParseExact(resultObject.ExpirateDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.CurrentCulture);
                    var objList = JsonConvert.DeserializeObject<List<T>>(resultobjectparse[1].Substring(1, resultobjectparse[1].Length - 3));
                    data = new List<T>();
                    data = objList;
                    return Task.FromResult(1);
                }
                else
                {
                    return Task.FromResult(0);
                }
            }
            catch (Exception ex)
            {
                errCode = 0;
                errMessage = ex.ToString();
                return Task.FromResult(0);
            }
        }

        public static Task CreateCacheAsync<T>(out int errCode, out string errMessage, string keycache, List<T> objectparams, long totalRows, int ExpirateTime = 0)
        {
            if (ExpirateTime == 0)
            {
                ExpirateTime = ExpirateDefault;
            }
            errCode = 0;
            errMessage = string.Empty;
            string ResponseData = string.Empty;
            try
            {
                byte[] postData;
                List<string> lit = new List<string>();
                StringBuilder paramstring = new StringBuilder();
                var CacheMongo = new ObjectCacheMongo<T>();
                CacheMongo.TotalRows = totalRows;
                CacheMongo.ExpirateDate = DateTime.Now.AddMinutes(ExpirateTime).ToString("dd-MM-yyyy HH:mm:ss");
                CacheMongo.ListObject = objectparams;
                CacheMongo._id = keycache;
                var objectdata = new
                {
                    keycache = keycache,
                    data = CacheMongo
                };
                paramstring.Append(JsonConvert.SerializeObject(objectdata));
                postData = Encoding.UTF8.GetBytes(paramstring.ToString());

                HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings["ServerNode:URL"] + ":" + ConfigurationManager.AppSettings["ServerNode:Port"] + "/createdatacache");
                client.KeepAlive = false;
                client.Method = WebRequestMethods.Http.Post;
                string proxy = null;
                client.Proxy = new WebProxy(proxy, true);
                client.CookieContainer = new CookieContainer();
                client.ContentType = "application/json";
                client.ContentLength = postData.Length;

                using (var stream = client.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
                var response = (HttpWebResponse)client.GetResponse();
                ResponseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (ResponseData == "1")
                {
                    return Task.FromResult(1);
                }
                else
                {
                    return Task.FromResult(0);
                }
            }
            catch (Exception ex)
            {
                errCode = 0;
                errMessage = ex.ToString();
                return Task.FromResult(-1);
            }
        }
    }
}