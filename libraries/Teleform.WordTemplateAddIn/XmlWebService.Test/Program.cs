using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Data;
using System.IO;

namespace XmlWebService.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string locationsRequest = CreateRequest();
                XmlDocument locationsResponse = MakeRequest(locationsRequest);
                ProcessResponse(locationsResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
        }

        private static string CreateRequest()
        {
            string urlRequest = "http://localhost:25000/monitoring/reporting/reportschemahandler.aspx";

            return urlRequest;
        }

        private static XmlDocument MakeRequest(string requestUrl)
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
                Console.WriteLine(ex.Message);
                Console.Read();
                return null;
            }
        }

        private static void ProcessResponse(XmlDocument xmlResponse)
        {
            Console.WriteLine("Parsing goes here...");

            XmlNodeList xmlNl = xmlResponse.SelectNodes("//AvailableStages/Stage");

            var availableStages = new List<string>();

            DataTable table = new DataTable("AvailableStages");
            table.Columns.Add("tbl", typeof(string));
            table.Columns.Add("nameT", typeof(string));

            foreach (XmlNode xn in xmlNl)
            {

                table.Rows.Add(xn.ChildNodes[0].InnerXml, xn.ChildNodes[1].InnerXml);

                availableStages.Add(xn.ChildNodes[0].InnerXml);
            }

            var intfoo = table.Rows.Count;

            foreach ( var stage in availableStages )
            {
                DataTable table1 = new DataTable(string.Format("Entities.{0}", stage));
                table1.Columns.Add("table", typeof(string));
                table1.Columns.Add("attribute", typeof(string));
                table1.Columns.Add("attributeAlias", typeof(string));
                table1.Columns.Add("pathAlias", typeof(string));

                xmlNl = xmlResponse.SelectNodes(string.Format("//Entities.{0}/Entity", stage));

                foreach ( XmlNode xn in xmlNl )
                {
                    var content = xn.ChildNodes;
                    table1.Rows.Add(content[0].InnerXml, content[1].InnerXml, content[2].InnerXml, content[3].InnerXml);
                }

                //StringBuilder sb = new StringBuilder();
                //foreach ( XmlNode node in xmlNl )
                //{
                //    sb.Append(node.OuterXml);
                //    XmlReader rd = XmlReader.Create(new StringReader(sb.ToString()));

                //    XmlDocument doc = new XmlDocument();
                //    doc.LoadXml(sb.ToString());


                //    table1.ReadXml(rd);
                //    var fp = table1.Rows.Count;
                //}

                //XmlReader rd = XmlReader.Create(new StringReader());
                //table.ReadXml(rd);
            }
        }
    }
}
