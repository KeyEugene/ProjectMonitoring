namespace ExcelDialog.Forms
{
    partial class FormBool
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.NameColumn = new System.Windows.Forms.Label();
            this.comboBoxBool = new System.Windows.Forms.ComboBox();
            this.panelBool = new System.Windows.Forms.Panel();
            this.panelBool.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Palatino Linotype", 9F);
            this.buttonCancel.Location = new System.Drawing.Point(203, 12);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(65, 24);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("Palatino Linotype", 9F);
            this.buttonOk.Location = new System.Drawing.Point(132, 12);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(65, 24);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Принять";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSize = true;
            this.NameColumn.Font = new System.Drawing.Font("Palatino Linotype", 9F);
            this.NameColumn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.NameColumn.Location = new System.Drawing.Point(17, 16);
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.Size = new System.Drawing.Size(86, 17);
            this.NameColumn.TabIndex = 4;
            this.NameColumn.Text = "NameColumn";
            // 
            // comboBoxBool
            // 
            this.comboBoxBool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBool.Location = new System.Drawing.Point(48, 28);
            this.comboBoxBool.Name = "comboBoxBool";
            this.comboBoxBool.Size = new System.Drawing.Size(153, 21);
            this.comboBoxBool.TabIndex = 0;
            this.comboBoxBool.SelectedIndexChanged += new System.EventHandler(this.comboBoxBool_SelectedIndexChanged);
            // 
            // panelBool
            // 
            this.panelBool.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.panelBool.Controls.Add(this.comboBoxBool);
            this.panelBool.Location = new System.Drawing.Point(12, 50);
            this.panelBool.Name = "panelBool";
            this.panelBool.Size = new System.Drawing.Size(260, 200);
            this.panelBool.TabIndex = 7;
            // 
            // FormBool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.panelBool);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.NameColumn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormBool";
            this.Text = "FormBool";
            this.panelBool.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label NameColumn;
        private System.Windows.Forms.ComboBox comboBoxBool;
        private System.Windows.Forms.Panel panelBool;
    }
}