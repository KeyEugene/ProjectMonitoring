using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.SessionState;

namespace Teleform.ProjectMonitoring
{
    public static class Kernel
    {
        public static string ConnectionString
        {
            get
            {
                return Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString;
            }
        }

        /// <summary>
        /// Получить подпись для объекта из идентификатора.
        /// </summary>
        /// <param name="cell">Ячейка, в которой хранится идентификатор.</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetNameFromID(int objID, string tableName)
        {
            string objCaption;

            string name, obj;



            if (tableName.StartsWith("VO__Division"))
            {
                name = "[Название]";
                obj = "[ИД]";
            } else if (tableName.StartsWith("VO__Person"))
            {
                name = "[ФИО]";
                obj = "[ИД]";
            } else
            {
                name = "[name]";
                obj = "[objID]";
            }


            var connection = new SqlConnection(Kernel.ConnectionString);
            using (var cmd = new SqlCommand(string.Format("SELECT {0} FROM [{1}] WHERE {2} = {3}", name, tableName, obj, objID), connection))
            {
                connection.Open();
                try
                {
                    objCaption = Convert.ToString(cmd.ExecuteScalar());
                } catch
                {
                    throw new Exception("\"Не удалось получить имя объекта из идентификатора.\" Kernel.GetNameFromID(int, string)");
                }
            }

            return objCaption;
        }


    }
}