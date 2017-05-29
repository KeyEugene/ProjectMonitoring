using DynamicCardModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace MonitoringMinProm.DynamicCard
{
    public partial class DynamicCard : CompositeControl
    {
        /// <summary>
        /// Формирование модели динамической карточки.
        /// </summary>
        private void FormCardModel()
        {
            var xmlstring = String.Empty;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["stend"].ConnectionString))
            using (var cmd = new SqlCommand(string.Format("select [model].[xmlEntityAttributes]('{0}')", this.EntityID), conn))
            {
                conn.Open();
                xmlstring = cmd.ExecuteScalar().ToString();
            }



//            if (this.ConstraintID == null && this.CurrentRegime != PageRegime.ReadOnly) throw new ArgumentNullException("Не указан идентификатор ссылки к бизнес-объекту.");

            this.Card = new Card(xmlstring);
        }


        /// <summary>
        /// Заполняет модель текущего бизнес-объекта значениями.
        /// </summary>
        /// <param name="id">Идентификатор экземпляра бизнес-объекта.</param>
        private void FillCardValues(string id)
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["stend"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.Connection = conn;

            cmd.CommandText = "EXEC [report].[getBObjectData] @entity, NULL, @cyr=0, @flTitle=2, @instances=@instance";
            cmd.Parameters.AddRange(new SqlParameter[] {
                new SqlParameter { ParameterName = "entity", DbType = DbType.String, Value = this.Card.TableName },
                new SqlParameter { ParameterName = "instance", DbType = DbType.String, Value = this.InstanceID }
            });

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count == 0)
                throw new Exception(string.Format("Не удалось найти экземпляр данных в таблице: \"{0}\"", this.Card.TableAlias));

            var titleRow = dt.Rows.OfType<DataRow>().First();

            cmd = new SqlCommand(string.Format("SELECT * FROM [{0}] WHERE [objID]={1}", this.Card.TableName, this.InstanceID), conn);
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
                throw new Exception(string.Format("Не удалось найти экземпляр данных в таблице: \"{0}\"", this.Card.TableAlias));

            var dataRow = dt.Rows.OfType<DataRow>().First();

            foreach (var f in this.Card.FieldList)
            {
                if (f.FieldType == FieldType.FiniteField)
                {
                    var val = Convert.IsDBNull(titleRow[f.Name]) == true ?
                        "" : titleRow[f.Name].ToString();

                    if (!string.IsNullOrEmpty(val))
                        if (f.Type == "date" || f.Type == "smalldatetime" || f.Type == "datetime")
                        {
                            val = val.TrimEnd(new char[] { '0', ':', ' ' });

                            var datearr = val.Split('.');
                            var dd = datearr[0];
                            var mm = datearr[1];
                            var yy = datearr[2];

                            val = yy + "-" + mm + "-" + dd;
                        }

                    f.Title = f.Value = val;
                }
                else if (f.FieldType == FieldType.ConstraintField)
                {
                    f.Value = Convert.IsDBNull(dataRow[f.Name]) == true ?
                        "" : dataRow[f.Name].ToString();

                    var title = Convert.IsDBNull(titleRow[f.RelationName]) == true ?
                        "" : titleRow[f.RelationName].ToString();

                    f.Title = title.Trim(new char[] { '~', ' ' });
                }
            }

        }


        /// <summary>
        /// Заполняет модель текущего бизнес-объекта значениями, в режиме добавления.
        /// </summary>
        private void FillConstraintField()
        {
#if false
            if (Page.Request.QueryString["entity"] == Page.Request.QueryString["constraint"])
                return;
#endif

            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["stend"].ConnectionString);
            var cmd = new SqlCommand("SELECT [refTbl], [pCol], [rCol], [name] FROM [model].[ForeignKeysColumns] WHERE [objID]=@constraintID", conn);
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "constraintID",
                DbType = System.Data.DbType.Int64,
                Value = this.ConstraintID
            });
            var dt = new DataTable();
            var da = new SqlDataAdapter(cmd);
            da.Fill(dt);

#warning Да простит меня число 255 и присвятой intel!
            var constraintStart = dt.Rows.OfType<DataRow>().First()["rCol"].ToString().TrimEnd(new char[] { 'I', 'D' });

            foreach (DataRow r in dt.Rows)
            {
                var field = this.Card.FieldList.First(f => f.Name == (string)r["pCol"]);
                //field.Value = this.InstanceID;

                var query = string.Format("SELECT [{0}] FROM [{1}] WHERE [objID]={2}",
                    (string)r["rCol"],
                    (string)r["refTbl"],
                    this.InstanceID);


                using (cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    field.Value = cmd.ExecuteScalar().ToString();
                    conn.Close();
                }


                cmd = new SqlCommand("EXEC [report].[getBObjectData] @table, @cyr=0, @flTitle=1, @instances=@inst", conn);
                cmd.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter
                    {
                        ParameterName = "table",
                        DbType = DbType.String,
                        Value = (string)r["refTbl"]
                    },
                    new SqlParameter
                    {
                        ParameterName = "inst",
                        DbType = System.Data.DbType.String,
                        Value = this.InstanceID,
                        IsNullable = true
                    }
                });

                dt = new DataTable();
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0) throw new Exception("Не удалось получить данные по объекту.");

                var titleRow = dt.Rows.OfType<DataRow>().First();

                var title = "";
                var RefColumns = dt.Columns.OfType<DataColumn>().Where(col => col.ColumnName.ToLower().Contains(constraintStart));
                var BaseColumns = dt.Columns.OfType<DataColumn>().Where(col => !col.ColumnName.ToLower().Contains(constraintStart));


                if (r["rCol"].ToString().ToLower() == "objid")
                {
                    foreach (var col in BaseColumns)
                        title += titleRow[col.ColumnName].ToString() + " ~ ";
                }
                else
                {
                    foreach (var col in RefColumns)
                        title += titleRow[col.ColumnName].ToString() + " ~ ";
                }

                var n = field.Name;
                field.Title = title.TrimEnd(new char[] { '~', ' ' });

                field.IsEditable = false;
            }

        }


        /// <summary>
        /// Устанавливает полям карточки режим редактирования.
        /// </summary>
        private void SetEditableProperties()
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["stend"].ConnectionString);
            var dt = new DataTable();
            var cmd = new SqlCommand("select [col] from [model].[tablecolumnudp] where [tbl] = @tbl and [isindexed] = 1", conn);
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "tbl",
                DbType = DbType.String,
                Value = this.Card.TableName
            });

            var da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if (dt.Rows.Count == 0) throw new Exception("Не найдено ни одного PrimaryKey.");

            if (this.CurrentRegime == PageRegime.ReadOnly)
                foreach (var f in this.Card.FieldList)
                    f.IsEditable = false;
            else if (this.CurrentRegime == PageRegime.Edit)
                foreach (DataRow r in dt.Rows)
                    this.Card.FieldList.First(f => f.Name == (string)r["col"]).IsEditable = false;

            else if (this.CurrentRegime == PageRegime.Insert)
            {
            }
        }
    }
}