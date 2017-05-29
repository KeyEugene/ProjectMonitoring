using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.MicrosoftOffice.ImportFile.Excel
{
    public class ValidationObject
    {
        public bool isError
        {
            get
            {
                if (!string.IsNullOrEmpty(GetNullRow.ToString()) || !string.IsNullOrEmpty(GetErrorOfType.ToString()) || !string.IsNullOrEmpty(GetErrorObjID.ToString()))
                    return true;
                else
                    return false;
            }
        }

        private StringBuilder GetNullRow;
        private StringBuilder GetErrorOfType;
        private StringBuilder GetErrorObjID;

        private byte index = 0;
        private TemplateField field;
        private string cell;

        public ValidationObject()
        {
            GetErrorOfType = new StringBuilder();
            GetNullRow = new StringBuilder();
            GetErrorObjID = new StringBuilder();
        }
        public void StartValidation(ref List<List<string>> excelList, Template template)
        {
            for (byte i = 0; i < template.Fields.Count; i++)
            {
                field = template.Fields[i];

                for (byte j = index = 0; j < excelList.Count; j++)
                {
                    index++;
                    cell = excelList[j][i];

                    var ParentName = template.Entity.SystemName + "_" + template.Entity.SystemName + "/name";

                    if ((field.Name.Contains("/objID") || field.Attribute.Name.Contains("/objID") || field.Attribute.FPath.Contains("/objID") || field.Attribute.FPath.Contains("/objid") || field.Attribute.FPath == ParentName)
                        && !string.IsNullOrEmpty(cell))
                        GetObjID();

                    if (!field.Attribute.IsNullable)
                        if (string.IsNullOrEmpty(cell))
                            GetNullRow.AppendLine(GetNumberCell() + ", ");

                    if (!string.IsNullOrEmpty(cell))
                        CheckType();

                    excelList[j][i] = cell;
                }
            }
        }

        private void CheckType()
        {
            switch (field.Attribute.Type.RuntimeType)
            {
                case "System.Boolean":
                    if (cell.ToLower() == "да")
                        cell = "1";
                    else if (cell.ToLower() == "нет")
                        cell = "0";

                    //bool e;
                    //if (!Boolean.TryParse(cell, out e))
                    //    GetErrorOfType.AppendLine(GetNumberCell() + ", ");
                    break;
                case "System.DateTime":
                    DateTime d;
                    if (!DateTime.TryParse(cell, out d))
                        GetErrorOfType.AppendLine(GetNumberCell() + ", ");
                    else
                    {
                        cell = string.Format("{0:yyyy-MM-dd}", d);
                    }
                    break;
                case "System.Decimal":
                    decimal w;
                    if (!Decimal.TryParse(cell, out w))
                        GetErrorOfType.AppendLine(GetNumberCell() + ", ");
                    break;
                case "System.Double":
                    double q;
                    if (!Double.TryParse(cell, out q))
                        GetErrorOfType.AppendLine(GetNumberCell() + ", ");
                    break;
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.Byte":
                    int i;
                    if (!Int32.TryParse(cell, out i))
                        GetErrorOfType.AppendLine(GetNumberCell() + ", ");
                    break;
                default:
                    break;
            }

        }

        private void GetObjID()
        {
            var startIndex = cell.LastIndexOf('|');
            if (startIndex == -1 || cell.Count() <= startIndex)
                GetErrorObjID.AppendLine(GetNumberCell() + ", ");

            cell = cell.Remove(0, (startIndex + 1));
        }

        public string GetMassegaForError()
        {
            if (!string.IsNullOrEmpty(GetNullRow.ToString()))
                GetNullRow.AppendLine(" - эти поля обезательны для заполнения. ");
            if (!string.IsNullOrEmpty(GetErrorOfType.ToString()))
                GetErrorOfType.AppendLine(" - эти поля не соответсвуют типу. ");
            if (!string.IsNullOrEmpty(GetErrorObjID.ToString()))
                GetErrorObjID.AppendLine(" - в этих полях не удалось найти objID (отсутствует - |). ");

            return string.Concat(GetNullRow, " ", GetErrorOfType, " ", GetErrorObjID.ToString());
        }

        private string GetNumberCell()
        {
            var i = index;
            var up = ExcelParser.GetTheNameOfTheUpperColumn((byte)field.Order);

            return string.Concat(up, (1 + i).ToString());
        }
    }
}
