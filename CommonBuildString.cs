using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.ComponentModel;

namespace SocialWeb.Common
{
    public class CommonBuildString<T> where T: class
    {
        private static CommonBuildString<T> _current = new CommonBuildString<T>();

        public static CommonBuildString<T> Current
        {
            get
            {
                return _current != null ? _current : new CommonBuildString<T>();
            }
        }

        public string BuildCombobox(List<T> obj, string AttributeId, string AttributeName, string FieldValue, string FieldDisplay, List<string[]> attribute=null)
        {
            StringBuilder objBuild = new StringBuilder();
            objBuild.AppendFormat("<select id=\"{0}\" name=\"{1}\"", AttributeId, AttributeName);
            if (attribute != null)
            {
                foreach (var item in attribute)
                {
                    objBuild.AppendFormat(" {0}=\"{1}\"", item[0], item[1]);
                }
            }
            objBuild.Append(">");
            foreach (var item in obj)
            {
                PropertyDescriptorCollection fieldObject = TypeDescriptor.GetProperties(item);
                PropertyDescriptor objValue = fieldObject.Find(FieldValue, false);
                PropertyDescriptor objDisplay = fieldObject.Find(FieldDisplay, false);
                objBuild.AppendFormat("<option value=\"{0}\">{1}</option>", objValue.GetValue(item), objDisplay.GetValue(item));
            }
            objBuild.AppendFormat("</select>");
            return objBuild.ToString();
        }
        public string BuildTextbox(string AttributeId, string AttributeName, string valuetext ="", List<string[]> attribute = null)
        {
            StringBuilder objBuild = new StringBuilder();
            objBuild.AppendFormat("<input type=\"text\" id=\"{0}\" name=\"{1}\" value=\"{2}\"", AttributeId, AttributeName, valuetext);
            if (attribute != null)
            {
                foreach (var item in attribute)
                {
                    objBuild.AppendFormat(" {0}=\"{1}\"", item[0], item[1]);
                }
            }
            objBuild.Append(" />");
            return objBuild.ToString();
        }
    }
}