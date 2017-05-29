using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Data;
using System.Windows.Forms;

namespace Teleform.Office.DBSchemeWordAddIn.XmlWebService
{
    public class XmlRequest
    {
        public static string CreateRequest(string envVar)
        {
            string urlRequest = Environment.GetEnvironmentVariable(envVar);

            return (string.IsNullOrEmpty(urlRequest) ? "" : urlRequest);
        }

        public static XmlDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return (xmlDoc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static DataTable GetBaseTables(XmlDocument doc)
        {
            DataTable table = new DataTable("AvailableStages");
            table.Columns.Add("tbl", typeof(string));
            table.Columns.Add("nameT", typeof(string));

            if (doc != null)
            {
                try
                {
                    XmlNodeList xmlNl = doc.SelectNodes("//AvailableStages/Stage");

                    foreach (XmlNode xn in xmlNl)
                    {
                        table.Rows.Add(xn.ChildNodes[0].InnerXml, xn.ChildNodes[1].InnerXml);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return table;
        }

        public static DataTable GetTableData(XmlDocument doc, string baseTableName)
        {
            DataTable table = new DataTable(string.Format("Entities.{0}", baseTableName));
            table.Columns.Add("table", typeof(string));
            table.Columns.Add("attribute", typeof(string));
            table.Columns.Add("attributeAlias", typeof(string));
            table.Columns.Add("pathAlias", typeof(string));

            var xmlNl = doc.SelectNodes(string.Format("//Entities.{0}/Entity", baseTableName));

            foreach (XmlNode xn in xmlNl)
            {
                var content = xn.ChildNodes;
                table.Rows.Add(content[0].InnerXml, content[1].InnerXml, content[2].InnerXml, content[3].InnerXml);
            }

            return table;
        }
    }
}
