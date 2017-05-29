using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Teleform.ProjectMonitoring.HttpApplication;
using Teleform.Reporting;
using Teleform.Reporting.MicrosoftOffice.ImportFile;
using Teleform.Reporting.MicrosoftOffice.ImportFile.Excel;

namespace Teleform.ProjectMonitoring
{
    public partial class ReportView
    {
        private Template excelTemplate;
        private List<List<string>> excelList;
        private string entityID;

        protected void DownloadImportFile_Click(object sender, EventArgs e)
        {
            var typeFile = new BaseParseFile(ImportFileUpload).GetTypeFile();

            if (typeFile == TypeFile.Excel)
                PreparationExcelObjects();
            else if (typeFile == TypeFile.Word)
                //ToDo: SaveWord();
                new NotImplementedException();

            Storage.ClearInstanceCache(typeof(BusinessContent), entityID);

            entityID = null;

        }

        protected void CancelDownloadImportFile_Click(object sender, EventArgs e)
        {
            //ToDo Somethings
        }

        #region Save excel object's to DataBase
        private void PreparationExcelObjects()
        {
            excelList = new ExcelParser(ImportFileUpload.FileBytes, ImportFileUpload.FileName).StartParse(out excelTemplate);

            if (ValidateExcelObject())
            {
                var xml = SerializationInstance();
                SaveExcelObjects(xml);
            }

            entityID = excelTemplate.Entity.ID.ToString();
            excelList = null; excelTemplate = null;
            ImportFileUpload = null;
            
        }

        private bool ValidateExcelObject()
        {
            var validation = new ValidationObject();
            validation.StartValidation(ref excelList, excelTemplate);

            if (validation.isError)
            {
                var messageLabel = WarningMessageBox.FindControl("WarningLabel") as System.Web.UI.WebControls.Label;
                messageLabel.Text = validation.GetMassegaForError();
                WarningMessageBox.Show();
                return false;
            }
            return true;
        }
        /// <summary>
        /// Preparation Excel xml
        /// </summary>
        private string SerializationInstance()
        {
            var userID = Session["SystemUser.ID"] == null ? "0" : Session["SystemUser.ID"].ToString();
            var serialize = new SerializationExcelObjects(excelTemplate, userID);
            var xml = new StringBuilder();

            for (int i = 0; i < excelList.Count; i++)
            {
                xml.AppendLine(string.Concat("EXEC [model].[BObjectSave] '", serialize.Serialization(excelList[i]), "'"));
            }
            // var trans = Teleform.ProjectMonitoring.admin.Sharing.GetTransactionString(xml.ToString(), "");
            return xml.ToString();
        }

        private void SaveExcelObjects(string xml)
        {
            bool isAllRight = true;

            using (var c = new SqlConnection(Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString))
            using (var command = new SqlCommand(xml, c))
            {
                c.Open();
                SqlTransaction transaction = c.BeginTransaction();

                try
                {
                    command.Transaction = transaction;
                    command.ExecuteScalar();

                    transaction.Commit();
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    isAllRight = false;

                    string duplecateObjects = ErrorHandling(ex.Errors);

                    var messageLabel = WarningMessageBox.FindControl("WarningLabel") as System.Web.UI.WebControls.Label;
                    messageLabel.Text = duplecateObjects;
                    WarningMessageBox.Show();
                }
            }
            if (isAllRight)
            {
                var messageLabel = WarningMessageBox.FindControl("WarningLabel") as System.Web.UI.WebControls.Label;
                messageLabel.Text = "Данные успешно сохранены в тип объекта : " + excelTemplate.Entity.Name;
                WarningMessageBox.Show();
            }
        }

        private string ErrorHandling(SqlErrorCollection sqlErrorCollection)
        {
            var errors = new StringBuilder();
            bool isOneFailConvertDate = true;
            bool isOneDuplicateClarification = true;

            for (int i = 0; i < sqlErrorCollection.Count; i++)
            {
                var error = sqlErrorCollection[i].Message;

                if (error.Contains("duplicate key"))
                {
                    if (isOneDuplicateClarification)
                    {
                        errors.AppendLine("Указанные объекты в базе данных уже существуют : ");
                        isOneDuplicateClarification = false;
                    }

                    var duplicateIndex = sqlErrorCollection[i].Message.IndexOf("is (");

                    if (duplicateIndex != -1)
                    {
                        duplicateIndex = duplicateIndex + 3;
                        error = error.Remove(0, duplicateIndex);
                        errors.AppendLine(error);
                    }
                }
                else if (error.Contains("converting date and/or") && isOneFailConvertDate) //Создание данного объекта невозможно: Conversion failed when converting date and/or time from character string.
                {
                    errors.AppendLine("Неверный формат дат.");
                    isOneFailConvertDate = false;
                }
                else if (error.Contains("Объект с данным именем уже существует."))//Создание данного объекта невозможно: Недопустимая операция. Объект с данным именем уже существует.
                {
                    errors.AppendLine("Объект с данным именем уже существует.");
                    isOneFailConvertDate = false;
                }
            }
            return errors.ToString();
        }


        #endregion

    }
}