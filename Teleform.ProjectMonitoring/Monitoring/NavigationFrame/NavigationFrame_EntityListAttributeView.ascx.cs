using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Teleform.ProjectMonitoring.NavigationFrame
{
    public partial class NavigationFrame_EntityListAttributeView : System.Web.UI.UserControl
    {
        public event EventHandler UserControl_InsertInstance_Click;
        public event EventHandler UserControl_DeleteInstance_Click;
        public event EventHandler UserControl_IsEditModeCheckBox_CheckedChanged;
        public event EventHandler UserControl_SaveObjects_OnClick;
        public event EventHandler UserControl_TemplateConstructorButton_Load;
        public event EventHandler UserControl_TemplateConstructorButton_Click;
        public event EventHandler UserControl_TemplateList_SelectedIndexChanged;
        public event EventHandler UserControl_GoToFilterDesignerButton_Click;
        public event EventHandler UserControl_FilterList_SelectedIndexChanged;
        public event EventHandler UserControl_ResetAllFilters_OnClick;
        public event EventHandler UserControl_ResetAllSortings_OnClick;
        public event EventHandler UserControl_ToGroupReportButton_Click;
        public event EventHandler UserControl_CreateExcelReportButton_Click;
        public event EventHandler UserControl_LoadImportFile_Click;

        public event EventHandler UserControl_CreateTemplateButton_Click;
        public event EventHandler UserControl_ReportsTemplatesList_OnSelectedIndexChanged;
        public event EventHandler UserControl_ReportsTemplatesList_OnLoad;
        public event EventHandler UserControl_EditTemplateButton_Click;
        public event EventHandler UserControl_DownloadButton_Click;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void InsertInstance_Click(object sender, EventArgs e)
        {
            if (UserControl_InsertInstance_Click != null)
            {
                UserControl_InsertInstance_Click(sender, e);
            }
        }

        protected void DeleteInstance_Click(object sender, EventArgs e)
        {
            if (UserControl_DeleteInstance_Click != null)
            {
                UserControl_DeleteInstance_Click(sender, e);
            }
        }

        protected void IsEditModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UserControl_IsEditModeCheckBox_CheckedChanged != null)
            {
                UserControl_IsEditModeCheckBox_CheckedChanged(sender, e);
            }
        }

        protected void SaveObjects_OnClick(object sender, EventArgs e)
        {
            if (UserControl_SaveObjects_OnClick != null)
            {
                UserControl_SaveObjects_OnClick(sender, e);
            }
        }

        protected void TemplateConstructorButton_Load(object sender, EventArgs e)
        {
            if (UserControl_TemplateConstructorButton_Load != null)
            {
                UserControl_TemplateConstructorButton_Load(sender, e);
            }
        }

        protected void TemplateConstructorButton_Click(object sender, EventArgs e)
        {
            if (UserControl_TemplateConstructorButton_Click != null)
            {
                UserControl_TemplateConstructorButton_Click(sender, e);
            }
        }

        protected void TemplateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserControl_TemplateList_SelectedIndexChanged != null)
            {
                UserControl_TemplateList_SelectedIndexChanged(sender, e);
            }
        }

        protected void GoToFilterDesignerButton_Click(object sender, EventArgs e)
        {
            if (UserControl_GoToFilterDesignerButton_Click != null)
            {
                UserControl_GoToFilterDesignerButton_Click(sender, e);
            }
        }

        protected void FilterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserControl_FilterList_SelectedIndexChanged != null)
            {
                UserControl_FilterList_SelectedIndexChanged(sender, e);
            }
        }

        protected void ResetAllFilters_OnClick(object sender, EventArgs e)
        {
            if (UserControl_ResetAllFilters_OnClick != null)
            {
                UserControl_ResetAllFilters_OnClick(sender, e);
            }
        }

        protected void ResetAllSortings_OnClick(object sender, EventArgs e)
        {
            if (UserControl_ResetAllSortings_OnClick != null)
            {
                UserControl_ResetAllSortings_OnClick(sender, e);
            }
        }

        protected void ToGroupReportButton_Click(object sender, EventArgs e)
        {
            if (UserControl_ToGroupReportButton_Click != null)
            {
                UserControl_ToGroupReportButton_Click(sender, e);
            }
        }

        protected void CreateExcelReportButton_Click(object sender, EventArgs e)
        {
            if (UserControl_CreateExcelReportButton_Click != null)
            {
                UserControl_CreateExcelReportButton_Click(sender, e);
            }
        }
        protected void LoadImportFile_Click(object sender, EventArgs e)
        {
            if (UserControl_LoadImportFile_Click != null)
            {
                UserControl_LoadImportFile_Click(sender, e);
            }
        }
        
        #region Диалоговое окно создание, редактирование отчетов Word\Excel
        
        protected void CreateTemplateButton_Click(object sender, EventArgs e)
        {
            if (UserControl_CreateTemplateButton_Click != null)
            {
                UserControl_CreateTemplateButton_Click(sender, e);
            }
        }
        protected void ReportsTemplatesList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserControl_ReportsTemplatesList_OnSelectedIndexChanged != null)
            {
                UserControl_ReportsTemplatesList_OnSelectedIndexChanged(sender, e);
            }
        }
        protected void ReportsTemplatesList_OnLoad(object sender, EventArgs e)
        {
            if (UserControl_ReportsTemplatesList_OnLoad != null)
            {
                UserControl_ReportsTemplatesList_OnLoad(sender, e);
            }
        }
        protected void EditTemplateButton_Click(object sender, EventArgs e)
        {
            if (UserControl_EditTemplateButton_Click != null)
            {
                UserControl_EditTemplateButton_Click(sender, e);
            }
        }
        protected void DownloadButton_Click(object sender, EventArgs e)
        {
            if (UserControl_DownloadButton_Click != null)
            {
                UserControl_DownloadButton_Click(sender, e);
            }
        }


        #endregion
    }
}