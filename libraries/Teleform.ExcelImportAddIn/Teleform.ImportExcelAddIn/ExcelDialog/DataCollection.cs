using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teleform.Reporting;
using Teleform.Reporting.constraint;
using Teleform.Reporting.Providers;

namespace ExcelDialog
{
    using Teleform.ImportExcelAddIn;
    using Constraint = Teleform.Reporting.constraint.Constraint;
    partial class EntityDesigner
    {

        private void ParsersExcel()
        {
            var ExcelApplication = Globals.ThisAddIn.Application;

            var address = ExcelApplication.ActiveCell.Address.Split(new char[] { '$', ':', ' ' }).Where(x => x != "").ToArray();
            numberRow = ExcelApplication.ActiveCell.Row;
            numberColumn = ExcelApplication.ActiveCell.Column;

            if (address.Count() == 2)
                adressCell = address[0] + address[1];

            oldValueCell = ExcelApplication.ActiveSheet.Cells(numberRow, numberColumn).Value; //Получение информации из поределенной ячейки
            NameColumn = ExcelApplication.ActiveSheet.Cells(1, numberColumn).Value;
            //            var id = ExcelApplication.ActiveCell.ID;
            templateName = ExcelApplication.ActiveSheet.Name;

            if (string.IsNullOrEmpty(NameColumn) || numberColumn == 0 || numberRow == 0 || string.IsNullOrEmpty(templateName))
                throw new Exception("Не удалось получить соответствующие данные из Excel-файла");

            if (numberRow == 1)
            {
                MessageBox.Show("Выберите поле ниже названия колонок.");
                throw new Exception();
            }
        }

        private static void FillSchema()
        {
        connectToServer:
            var webQuery = Environment.GetEnvironmentVariable("officeaddinserver");
            //webQuery = "http://localhost:25000/monitoring/get.schema.aspx";
            var provider = new XmlSchemaProvider(webQuery);

            try
            {
                Schema = provider.GetInstance();
            }
            catch (WebException)
            {
                var result = System.Windows.Forms.MessageBox.Show("Не удаётся подключиться к серверу мониторинга проектов.",
                    "Уведомление",
                    System.Windows.Forms.MessageBoxButtons.RetryCancel,
                    System.Windows.Forms.MessageBoxIcon.Warning);

                if (result == System.Windows.Forms.DialogResult.Retry)
                    goto connectToServer;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Ошибка", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void GetTemplate()
        {
            var dt = GetDataTable(string.Concat("SELECT [objID] FROM [model].[R$Template] WHERE [name] = '", templateName, "'"));

            if (dt.Rows.Count == 0)
                new Exception("We have not this Template :" + templateName);
            try
            {
                template = Storage.Select<Template>(dt.Rows[0][0].ToString());
            }
            catch (Exception)
            {
                MessageBox.Show(string.Concat("Шаблона с иминем ", templateName, " не существует."));
                throw new Exception(string.Concat("Шаблона с иминем ", templateName, " не существует."));
            }

        }

        private List<ListItem> CreateListItems(DataTable dt)
        {
            if (dt == null)
                return null;

            var listItem = new List<ListItem>(dt.Rows.Count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var value = dt.Rows[i]["objID"].ToString();
                var sb = new StringBuilder();

                for (int j = 1; j < dt.Columns.Count; j++)
                    sb.Append(dt.Rows[i][dt.Columns[j]] + ", ");

                sb.Remove(sb.ToString().Count() - 2, 2);

                listItem.Add(new ListItem(value, sb.ToString()));
            }
            return listItem;
        }

        private Constraint GetConstraint(string templateFieldAttributeFPath)
        {
            string fPathWithoutObjID = "";
            if (templateFieldAttributeFPath.Contains("objID") || templateFieldAttributeFPath.Contains("objid"))
             fPathWithoutObjID = templateFieldAttributeFPath.Remove(templateFieldAttributeFPath.Count() - 6, 6); // Отсикаем ObjID
            else if (templateFieldAttributeFPath.Contains("name"))
                fPathWithoutObjID = templateFieldAttributeFPath.Remove(templateFieldAttributeFPath.Count() - 5, 5);
            return template.Entity.Constraints.FirstOrDefault(x => x.ConstraintName == fPathWithoutObjID);
        }

        private string GetObjID(string cell)
        {
            var startIndex = cell.LastIndexOf('|');

            return cell = cell.Remove(0, (startIndex + 1));
        }

        public static DataTable GetDataTable(string query)
        {
            try
            {
                var da = new SqlDataAdapter(query, new SqlConnection(Storage.ConnectionString));
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                throw new Exception("Не удалось сохранить значения в таблицу", ex.InnerException);
            }
        }
    }
}
