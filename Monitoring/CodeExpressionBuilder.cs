using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Compilation;

namespace Teleform.ProjectMonitoring
{
    /// <summary>
    /// Класс, который позволяет брать значения из ConnectionString's (Web.config) 
    /// ( Example : ConfigurationManager.ConnectionStrings["Server"].ConnectionString)
    /// </summary>
    public class CodeExpressionBuilder : ExpressionBuilder
    {
        public override System.CodeDom.CodeExpression GetCodeExpression(System.Web.UI.BoundPropertyEntry entry, 
            object parsedData, ExpressionBuilderContext context)
        {
            return new System.CodeDom.CodeSnippetExpression(entry.Expression);
        }
    }
}