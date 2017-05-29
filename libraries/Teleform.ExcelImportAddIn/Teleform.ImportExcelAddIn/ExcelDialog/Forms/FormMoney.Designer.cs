namespace ExcelDialog.Forms
{
    partial class FormMoney
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
            this.panelMoney = new System.Windows.Forms.Panel();
            this.labelMaxValue = new System.Windows.Forms.Label();
            this.numeric = new System.Windows.Forms.NumericUpDown();
            this.panelMoney.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Palatino Linotype", 9F);
            this.buttonCancel.Location = new System.Drawing.Point(210, 12);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(65, 24);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("Palatino Linotype", 9F);
            this.buttonOk.Location = new System.Drawing.Point(139, 12);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(65, 24);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "Принять";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSize = true;
            this.NameColumn.Font = new System.Drawing.Font("Palatino Linotype", 9F);
            this.NameColumn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.NameColumn.Location = new System.Drawing.Point(15, 16);
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.Size = new System.Drawing.Size(86, 17);
            this.NameColumn.TabIndex = 7;
            this.NameColumn.Text = "NameColumn";
            // 
            // panelMoney
            // 
            this.panelMoney.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.panelMoney.Controls.Add(this.labelMaxValue);
            this.panelMoney.Controls.Add(this.numeric);
            this.panelMoney.Location = new System.Drawing.Point(12, 50);
            this.panelMoney.Name = "panelMoney";
            this.panelMoney.Size = new System.Drawing.Size(260, 200);
            this.panelMoney.TabIndex = 10;
            // 
            // labelMaxValue
            // 
            this.labelMaxValue.AutoSize = true;
            this.labelMaxValue.Location = new System.Drawing.Point(29, 87);
            this.labelMaxValue.Name = "labelMaxValue";
            this.labelMaxValue.Size = new System.Drawing.Size(0, 13);
            this.labelMaxValue.TabIndex = 4;
            // 
            // numeric
            // 
            this.numeric.Location = new System.Drawing.Point(32, 45);
            this.numeric.Name = "numeric";
            this.numeric.Size = new System.Drawing.Size(200, 20);
            this.numeric.TabIndex = 0;
            // 
            // FormMoney
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.panelMoney);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.NameColumn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMoney";
            this.panelMoney.ResumeLayout(false);
            this.panelMoney.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label NameColumn;
        private System.Windows.Forms.Panel panelMoney;
        private System.Windows.Forms.Label labelMaxValue;
        private System.Windows.Forms.NumericUpDown numeric;
    }
}