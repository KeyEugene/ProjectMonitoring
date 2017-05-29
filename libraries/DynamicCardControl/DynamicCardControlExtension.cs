using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Teleform.Reporting.DynamicCard
{
    public static class DynamicCardControlExtension
    {
        public static HtmlString DateCalculatorHyperText(this DynamicCardControl card)
        {
            return new HtmlString(@"<details class=""detailsCalc"">

 <summary style='color: gray;'>
            Калькулятор дат:
        </summary>
        <table>
            <tr><td><input type=""date"" id=""SelectedDate"" /></td></tr>
            <tr>
                <td>
                    <input type=""number"" id=""DayNumber"" style=""width:143px"" />
                </td>
            </tr>
            <tr>
                <td>
                    <input type=""date"" id=""resultDate""  readonly=""readonly"" />
                </td>
            </tr>
            <tr>
                <td align=""center"">
                    <input type=""button"" value=""рассчитать"" onclick=""CalculateDate()"" />

                </td>
            </tr>
        </table>
    </details>");
        }
    }
}
