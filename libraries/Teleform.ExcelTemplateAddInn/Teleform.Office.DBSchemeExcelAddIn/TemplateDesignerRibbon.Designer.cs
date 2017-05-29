namespace Teleform.Office.DBSchemeExcelAddIn
{
    partial class TemplateDesignerRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public TemplateDesignerRibbon(): base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.TabTemplateDesigner = this.Factory.CreateRibbonTab();
            this.Group = this.Factory.CreateRibbonGroup();
            this.GroupDialogs = this.Factory.CreateRibbonGroup();
            this.toggleInsertMode = this.Factory.CreateRibbonToggleButton();
            this.btnDesignerForm = this.Factory.CreateRibbonButton();
            this.TabTemplateDesigner.SuspendLayout();
            this.Group.SuspendLayout();
            this.GroupDialogs.SuspendLayout();
            // 
            // TabTemplateDesigner
            // 
            this.TabTemplateDesigner.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;

#warning HERE
            //this.TabTemplateDesigner.Groups.Add(this.Group);

            this.TabTemplateDesigner.Groups.Add(this.GroupDialogs);
            this.TabTemplateDesigner.Label = "Конструктор шаблона";
            this.TabTemplateDesigner.Name = "TabTemplateDesigner";
            // 
            // Group
            // 

#warning HERE
            //this.Group.Items.Add(this.toggleInsertMode);
            //this.Group.Label = "Режим добавления";
            //this.Group.Name = "Group";

            // 
            // GroupDialogs
            // 
            this.GroupDialogs.Items.Add(this.btnDesignerForm);
            this.GroupDialogs.Label = "Диалоги добавления";
            this.GroupDialogs.Name = "GroupDialogs";
            // 
            // toggleInsertMode
            // 

#warning AND HERE
            //this.toggleInsertMode.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            //this.toggleInsertMode.Label = "Добавление атрибутов";
            //this.toggleInsertMode.Name = "toggleInsertMode";
            //this.toggleInsertMode.ShowImage = true;
            //this.toggleInsertMode.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.toggleInsertMode_Click);
            // 

            // btnDesignerForm
            // 
            this.btnDesignerForm.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnDesignerForm.Label = "Конструктор шаблона";
            this.btnDesignerForm.Name = "btnDesignerForm";
            this.btnDesignerForm.ShowImage = true;
            this.btnDesignerForm.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDesignerForm_Click);
            // 
            // TemplateDesignerRibbon
            // 
            this.Name = "TemplateDesignerRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.TabTemplateDesigner);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.TemplateDesignerRibbon_Load);
            this.TabTemplateDesigner.ResumeLayout(false);
            this.TabTemplateDesigner.PerformLayout();
            this.Group.ResumeLayout(false);
            this.Group.PerformLayout();
            this.GroupDialogs.ResumeLayout(false);
            this.GroupDialogs.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab TabTemplateDesigner;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Group;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton toggleInsertMode;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup GroupDialogs;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDesignerForm;
    }

    partial class ThisRibbonCollection
    {
        internal TemplateDesignerRibbon TemplateDesignerRibbon
        {
            get { return this.GetRibbon<TemplateDesignerRibbon>(); }
        }
    }
}
