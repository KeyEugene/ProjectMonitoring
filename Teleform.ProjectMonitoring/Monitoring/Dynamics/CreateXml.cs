using System;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using DynamicCardModel;

namespace MonitoringMinProm.DynamicCard
{
    public class CreateXml
    {
        private string conString = @"Current Language = Russian; data source=STEND\SQLEXPRESS_12; Initial Catalog = MinProm; User Id=sa; Password=345; Asynchronous Processing=true";
        private XElement xml;
        private XElement xmlHeader;

        public void Xml()
        {
            var ent = "";
            Card entityModel;

            using (var conn = new SqlConnection(conString))
            using (var cmd = new SqlCommand("SELECT [model].[xmlEntityAttributes]('_Accomplice')", conn))
            {
                try
                {
                    conn.Open();
                    ent = cmd.ExecuteScalar().ToString();
                    entityModel = new Card(ent);
                }
                catch (Exception ex)
                {
                    throw new ArgumentNullException("Ошибка при вызове хранимой процедуры [dbo].[SavebObject]", ex.Message);
                }
                conn.Close();
            }

            var s = SerializationToXml(entityModel, "1", "312");

        }

#warning статический userID
        public string SerializationToXml(Card entityModel, string id = null, string userID = "0")
        {
            xmlHeader = new XElement("bObject", new XAttribute("entity", entityModel.TableName),
                                                new XAttribute("userID", userID), new XElement("attributes"));

            if (!string.IsNullOrEmpty(id))
                xmlHeader.Add(new XAttribute("objID", id));

            var i = 0;

            foreach (var field in entityModel.FieldList)
            {
                xml = new XElement("attribute", new XAttribute("name", field.Name),
                    new XAttribute("alias", field.Alias),
                    new XAttribute("value", field.Value == null ? "" : field.Value),
                    new XAttribute("allowNulls", field.IsNullable),
                    new XAttribute("type", field.Type),
                    new XAttribute("IsEditable", field.IsEditable),
                    new XAttribute("IsComputed", field.IsComputed));
                    xmlHeader.Element("attributes").Add(xml);
            }

            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xmlHeader);

            return doc.ToString();

        }


    }
}