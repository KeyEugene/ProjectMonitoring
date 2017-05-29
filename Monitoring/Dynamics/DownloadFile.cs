using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace MonitoringMinProm.DynamicCard
{
    public partial class DynamicCard : CompositeControl
    {
        void downloadButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;

            var field = this.Card.FieldList.First(f => f.Name == button.Attributes["data-field"]);

            var c  = new SqlConnection(ConfigurationManager.ConnectionStrings["stend"].ConnectionString);
#warning динамика, такая динамика(предполагается, что в таблице есть mimeTypeID)
            var ad = new SqlDataAdapter("DECLARE @query VARCHAR(MAX) = 'SELECT [A].[body], [MT].[mime] "+                                             "FROM ' +  @table + ' [A] "+
                                               "JOIN [MimeType] [MT] ON [MT].[objID] = [A].[mimeTypeID] "+
                                               "WHERE [A].[objID] =' +  @id; EXEC(@query)", c);
            ad.SelectCommand.Parameters.Add("id", SqlDbType.VarChar).Value = this.InstanceID;
            ad.SelectCommand.Parameters.Add("table", SqlDbType.VarChar).Value = this.Card.TableName;

            var dt = new DataTable();
            ad.Fill(dt);

            if (dt.Rows.Count == 0)
                throw new Exception("Документ не загружен");

            var bytes = (byte[])dt.Rows[0]["body"];
            var mime = dt.Rows[0]["mime"];
            //всегда идет с расширением
            var fileName = field.Title;

            var response = this.Page.Response;
            response.Clear();
            response.ContentType = mime.ToString();
            response.AddHeader("content-disposition", string.Format("attachment;fileName={0}", fileName));
            response.BinaryWrite(bytes);
            response.Flush();
            response.End();
        }
    }
}