using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Xml;
//using TestAPIConnect.Models;

namespace SocialWeb.Common
{
    public class ConvertHelper
    {
        private static ConvertHelper _current = new ConvertHelper();

        public static ConvertHelper Current
        {
            get
            {
                return _current != null ? _current : new ConvertHelper();
            }
        }

        public T ConvertObjToObj<T, K>(K objK) where T : new()
        {
            if (objK == null)
                return new T();
            Type t = typeof(T);
            object objConvert = new T();
            Type c = typeof(K);
            PropertyDescriptorCollection propObject = TypeDescriptor.GetProperties(c);
            PropertyDescriptorCollection propConvertor = TypeDescriptor.GetProperties(t);
            if (propConvertor != null)
            {
                foreach (PropertyDescriptor fieldConvert in propConvertor)
                {
                    foreach (PropertyDescriptor fieldObject in propObject)
                    {
                        if (fieldConvert.Name == fieldObject.Name)
                        {
                            object valueobj = fieldObject.GetValue(objK);
                            if (valueobj != null)
                            {
                                fieldConvert.SetValue(objConvert, valueobj);
                            }
                            else
                            {
                                if (fieldObject.PropertyType.Name.ToLower() == "string")
                                {
                                    fieldConvert.SetValue(objConvert, "");
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return (T)objConvert;
        }

        public object ConvertError(Exception ex)
        {
            object Err = null;
            if (((WebException)ex) != null)
            {
                if (((WebException)ex).Response != null)
                {
                    var responseWebException = new StreamReader(((WebException)ex).Response.GetResponseStream()).ReadToEnd();
                    if (!string.IsNullOrEmpty(responseWebException))
                    {
                        //if (responseWebException.IndexOf("error_description") >= 0)
                        //{
                        //    Err = new responseError<string>();
                        //    Err = JsonConvert.DeserializeObject<responseError<string>>(responseWebException);
                        //}
                        //else if (responseWebException.IndexOf("error") >= 0)
                        //{
                        //    Err = new responseError<error>();
                        //    Err = JsonConvert.DeserializeObject<responseError<error>>(responseWebException);
                        //}
                    }
                }
            }
            return Err;
        }

        public string ConvertFromXml(string xmlString)
        {
            StringBuilder objBuild = new StringBuilder();
            byte[] byteArray = Encoding.ASCII.GetBytes(xmlString);
            MemoryStream stream = new MemoryStream(byteArray);
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);
                var nodes = xmlDoc.DocumentElement.SelectNodes("//HoaDon");
                objBuild.Append("{");
                if (nodes != null)
                {
                    objBuild.Append("\"HoaDon\":[");
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if (i > 0)
                        {
                            objBuild.Append(",{");
                        }
                        else
                        {
                            objBuild.Append("{");
                        }
                        if (nodes[i].ChildNodes.Count > 0)
                        {
                            for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                            {

                                var itemName = nodes[i].ChildNodes.Item(j).Name;
                                var itemValue = nodes[i].ChildNodes.Item(j).InnerText;
                                if (j > 0)
                                {
                                    objBuild.AppendFormat(",\"{0}\": \"{1}\"", itemName, itemValue.Trim());

                                }
                                else
                                {
                                    objBuild.AppendFormat("\"{0}\": \"{1}\"", itemName, itemValue.Trim());
                                }
                            }
                        }
                        objBuild.Append("}");
                    }
                    objBuild.Append("]");
                }
                objBuild.Append("}");
            }
            return objBuild.ToString();
        }
    }
}