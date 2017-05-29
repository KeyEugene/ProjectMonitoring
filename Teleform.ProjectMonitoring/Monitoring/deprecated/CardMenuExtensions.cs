using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Phoenix.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Xml.Linq;
using System.Web.UI.WebControls;
using System.Text;
using System.Xml;
using Teleform.ProjectMonitoring.HttpApplication;

namespace MySpace
{
    [Obsolete("", true)]
    public static class CardMenuExtensions
    {
        public static void CreateMenu(this System.Web.UI.Page page, PlaceHolder menuHolder, string tableName)
        {
            PutNumberOnButtons(menuHolder, page.Request, tableName);
        }

        private static void PutNumberOnButtons(PlaceHolder ButtonHolder, HttpRequest request, string tableName)
        {
            using (var c = new SqlConnection(Global.ConnectionString))
            using (var adapter = new SqlDataAdapter("EXEC ConnectCountElements @tblName, @xml", c))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@xml", GenerateXml(request));
                adapter.SelectCommand.Parameters.AddWithValue("@tblName", tableName);

                var table = new DataTable();
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    var button = new Button();

                    button.Text = string.Format("{0} ({1})", row["text"].ToString(), row["amount"].ToString());
                    button.PostBackUrl = GeneratePostBackUrl(row["url"].ToString(), row["param"].ToString(), request, tableName);
                    button.DataBind();
                    
                    ButtonHolder.Controls.Add(button);
                }
            }
        }

        static private string GeneratePostBackUrl(string url, string parameters, HttpRequest request, string tableName)
        {
            var param = new XmlDocument();
            param.LoadXml(parameters);

            foreach (XmlNode p in param.FirstChild.ChildNodes)
            {
                foreach (var key in request.QueryString.AllKeys)
                {
                    if (key.ToString() != "target")
                        url += p.Attributes["name"].Value + "=" + request.QueryString[key] + "&";
                }
            }
            /*if (request.QueryString.AllKeys.Count() == 1)
                url += string.Format("{0}={1}", url == "~/details/Contracts.aspx?" && tableName == "division" ? "executorID" : tableName,request.QueryString["id"]); 
            else
                foreach (var key in request.QueryString.AllKeys)
                    url += key + "=" + request.QueryString[key] + "&";*/
            return url.Trim('&');
        }

        static private string GenerateXml(HttpRequest request)
        {
            var xml = new XElement("original");

            foreach (var key in request.QueryString.AllKeys)
            {
                //if(key.ToString() != "target")
                    xml.Add(new XAttribute(key, request.QueryString[key]));
            }

            return xml.ToString();
        }
    }
}