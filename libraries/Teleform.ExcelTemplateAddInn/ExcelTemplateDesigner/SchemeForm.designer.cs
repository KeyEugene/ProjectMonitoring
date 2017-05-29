namespace Teleform.Office.DBSchemeWordAddIn
{
    partial class SchemeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemeForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.EntityBox = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SelectButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.AttributeListBox = new System.Windows.Forms.ListBox();
            this.FormatListBox = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ExampleLabel = new System.Windows.Forms.Label();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.EntityBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 459F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(587, 528);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // EntityBox
            // 
            this.EntityBox.DisplayMember = "nameT";
            this.EntityBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EntityBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EntityBox.FormattingEnabled = true;
            this.EntityBox.Location = new System.Drawing.Point(3, 2);
            this.EntityBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EntityBox.Name = "EntityBox";
            this.EntityBox.Size = new System.Drawing.Size(581, 24);
            this.EntityBox.TabIndex = 0;
            this.EntityBox.SelectedIndexChanged += new System.EventHandler(this.EntityBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SelectButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 492);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(581, 34);
            this.panel1.TabIndex = 2;
            // 
            // SelectButton
            // 
            this.SelectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SelectButton.Location = new System.Drawing.Point(485, 2);
            this.SelectButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(91, 30);
            this.SelectButton.TabIndex = 1;
            this.SelectButton.Text = "Вставить";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.InsertAttribute);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.00591F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.99409F));
            this.tableLayoutPanel2.Controls.Add(this.AttributeListBox, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.FormatListBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.DescriptionLabel, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 35);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(579, 451);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // AttributeListBox
            // 
            this.AttributeListBox.DisplayMember = "attributeAlias";
            this.AttributeListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AttributeListBox.FormattingEnabled = true;
            this.AttributeListBox.HorizontalScrollbar = true;
            this.AttributeListBox.ItemHeight = 16;
            this.AttributeListBox.Location = new System.Drawing.Point(3, 2);
            this.AttributeListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.AttributeListBox.Name = "AttributeListBox";
            this.AttributeListBox.Size = new System.Drawing.Size(271, 398);
            this.AttributeListBox.TabIndex = 2;
            this.AttributeListBox.SelectedIndexChanged += new System.EventHandler(this.AttributeListBox_SelectedIndexChanged);
            // 
            // FormatListBox
            // 
            this.FormatListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FormatListBox.FormattingEnabled = true;
            this.FormatListBox.HorizontalScrollbar = true;
            this.FormatListBox.ItemHeight = 16;
            this.FormatListBox.Location = new System.Drawing.Point(281, 4);
            this.FormatListBox.Margin = new System.Windows.Forms.Padding(4);
            this.FormatListBox.Name = "FormatListBox";
            this.FormatListBox.Size = new System.Drawing.Size(294, 394);
            this.FormatListBox.TabIndex = 3;
            this.FormatListBox.SelectedIndexChanged += new System.EventHandler(this.FormatListBox_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.ExampleLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(281, 406);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(294, 41);
            this.panel2.TabIndex = 6;
            // 
            // ExampleLabel
            // 
            this.ExampleLabel.AutoSize = true;
            this.ExampleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExampleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ExampleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ExampleLabel.Location = new System.Drawing.Point(0, 0);
            this.ExampleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ExampleLabel.MinimumSize = new System.Drawing.Size(287, 37);
            this.ExampleLabel.Name = "ExampleLabel";
            this.ExampleLabel.Size = new System.Drawing.Size(287, 37);
            this.ExampleLabel.TabIndex = 5;
            this.ExampleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DescriptionLabel.Location = new System.Drawing.Point(4, 402);
            this.DescriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DescriptionLabel.MinimumSize = new System.Drawing.Size(267, 37);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(269, 49);
            this.DescriptionLabel.TabIndex = 5;
            this.DescriptionLabel.Text = "<текст описания>";
            this.DescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SchemeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 528);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "SchemeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Модель";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SchemeForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox EntityBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ListBox AttributeListBox;
        private System.Windows.Forms.ListBox FormatListBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label ExampleLabel;
        private System.Windows.Forms.Label DescriptionLabel;
    }
}