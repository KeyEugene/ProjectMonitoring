using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.Reporting.Template
{
    public enum EnumTreeType
    {
        Undefined = 0,
        General,
        Branch,
        Children
    }

    public enum EnumTypeCode
    {
        Undefined,
        TableBased,
        WordBased,
        ExcelBased,
        screenTree,
        crossReport,
        InputExcelBased
    }

    public enum EnumTypeName
    {
        Word,
        Excel,
        ScreenForm,
        ExcelBlank
    }
}
