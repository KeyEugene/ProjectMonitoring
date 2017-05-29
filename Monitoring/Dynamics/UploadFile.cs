using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MonitoringMinProm.DynamicCard
{
    public partial class DynamicCard : CompositeControl
    {

        private void UploadFiles()
        {
            foreach (var uploadItem in uploadList)
            {
				if (uploadItem.HasFile)
				{
					// получаем объект поля таблицы, для которой будет совершаться выгрузка документа.
					var field = this.Card.FieldList.First(f => f.Name == uploadItem.Attributes["data-field"]);
					// здесь должно быть определение служебного поля, которое сейчас не предоставляется
					// но мы то с вами знаем, что сейчас оно называется BODY

					// имя таблицы, куда собирается выгружаться файл.
					var tableName = this.Card.TableName;
					// идентификатор объекта, для которого файл будет выгружен.					
					// Это означает, что объект создается и не имеет на текущий момент идентификатора.

					//mimeType загружаемого файла
					var mime = uploadItem.PostedFile.ContentType;

					var query = string.Format("UPDATE [{0}] SET [fileName] = @fileName,"+
                                "[mimeTypeID] = (select ISNULL((SELECT TOP 1 [objID] FROM MimeType WHERE mime = @mime), (SELECT TOP 1 [objID] FROM MimeType WHERE mime = 'application/octet-stream'))) ,"+
                                " [body] = @body WHERE [objID] = {1}", tableName, this.identity);
					using (var c = new SqlConnection((ConfigurationManager.ConnectionStrings["stend"].ConnectionString)))
					{
						var command = new SqlCommand(query, c);
						command.Parameters.Add("fileName", SqlDbType.VarChar).Value = uploadItem.FileName.ToString();
						command.Parameters.Add("body", SqlDbType.Binary).Value = uploadItem.FileBytes;
						command.Parameters.Add("mime", SqlDbType.VarChar).Value = mime;
						try
						{
							c.Open();
							command.ExecuteScalar();
						}
						catch (Exception ex)
						{
							throw new ArgumentNullException("Ошибка при вызове хранимой процедуры", ex.Message);
						}
					}
				
					var fieldfileName = this.Card.FieldList.First(o => o.Name == "fileName");
					fieldfileName.Title = fieldfileName.Value = uploadItem.FileName;
				}
            }
        }
    }
}