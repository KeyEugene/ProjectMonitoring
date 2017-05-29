using System;
using Microsoft.Office.Tools.Ribbon;

namespace Teleform.Office.DBSchemeExcelAddIn
{
    public partial class TemplateDesignerRibbon
    {
        public event Action btnDesignerForm_Clicked;
        public event Action toggleInsertMode_Clicked;

        private void TemplateDesignerRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void toggleInsertMode_Click(object sender, RibbonControlEventArgs e)
        {
            if (toggleInsertMode_Clicked != null)
                toggleInsertMode_Clicked();
        }

        private void btnDesignerForm_Click(object sender, RibbonControlEventArgs e)
        {
            if (btnDesignerForm_Clicked != null)
                btnDesignerForm_Clicked();
        }




    }
}
