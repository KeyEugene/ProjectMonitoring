using ExcelTemplateDesigner;

namespace Teleform.Office.DBSchemeExcelAddIn
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }



        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            var ribbon = new TemplateDesignerRibbon();
            ribbon.toggleInsertMode_Clicked += new System.Action(ribbon_toggleInsertMode_Clicked);
            ribbon.btnDesignerForm_Clicked += new System.Action(ribbon_btnDesignerForm_Clicked);

            return Globals.Factory.GetRibbonFactory().
                CreateRibbonManager(new Microsoft.Office.Tools.Ribbon.IRibbonExtension[] { ribbon });
        }

        void ribbon_btnDesignerForm_Clicked()
        {
            TemplateDesigner.Show(Globals.ThisAddIn.Application);
        }

        void ribbon_toggleInsertMode_Clicked()
        {
            if (TemplateDesigner.IsHidden) return;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
