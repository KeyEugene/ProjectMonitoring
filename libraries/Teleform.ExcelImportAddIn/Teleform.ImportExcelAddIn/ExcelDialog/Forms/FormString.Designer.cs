namespace ExcelDialog.Forms
{
    partial class FormString
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
            this.textBox = new System.Windows.Forms.TextBox();
            this.panelString = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelString.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Palatino Linotype", 9F);
            this.buttonCancel.Location = new System.Drawing.Point(208, 12);
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
            this.buttonOk.Location = new System.Drawing.Point(137, 12);
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
            this.NameColumn.Location = new System.Drawing.Point(21, 16);
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.Size = new System.Drawing.Size(86, 17);
            this.NameColumn.TabIndex = 4;
            this.NameColumn.Text = "NameColumn";
            // 
            // textBox
            // 
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox.Location = new System.Drawing.Point(12, 10);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(238, 160);
            this.textBox.TabIndex = 0;
            // 
            // panelString
            // 
            this.panelString.AutoSize = true;
            this.panelString.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.panelString.Controls.Add(this.label1);
            this.panelString.Controls.Add(this.textBox);
            this.panelString.Location = new System.Drawing.Point(12, 47);
            this.panelString.Name = "panelString";
            this.panelString.Size = new System.Drawing.Size(261, 203);
            this.panelString.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // FormString
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.panelString);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.NameColumn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormString";
            this.panelString.ResumeLayout(false);
            this.panelString.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label NameColumn;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Panel panelString;
        private System.Windows.Forms.Label label1;
    }
}