namespace ExcelDialog
{
    partial class EntityForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.nameColumn = new System.Windows.Forms.Label();
            this.CountObjects = new System.Windows.Forms.Label();
            this.CreateNew = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // filterTextBox
            // 
            this.filterTextBox.BackColor = System.Drawing.SystemColors.Menu;
            this.filterTextBox.Location = new System.Drawing.Point(94, 31);
            this.filterTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(569, 24);
            this.filterTextBox.TabIndex = 0;
            this.filterTextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Palatino Linotype", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(22, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Фильтр : ";
            // 
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listBox.FormattingEnabled = true;
            this.listBox.ItemHeight = 17;
            this.listBox.Location = new System.Drawing.Point(19, 63);
            this.listBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(644, 289);
            this.listBox.TabIndex = 2;
            this.listBox.DoubleClick += new System.EventHandler(this.button_Ok_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(240, 367);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 30);
            this.button1.TabIndex = 3;
            this.button1.Text = "Добавить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button_Ok_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(372, 367);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 30);
            this.button2.TabIndex = 4;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // nameColumn
            // 
            this.nameColumn.AutoSize = true;
            this.nameColumn.Font = new System.Drawing.Font("Palatino Linotype", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nameColumn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.nameColumn.Location = new System.Drawing.Point(227, 9);
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.Size = new System.Drawing.Size(64, 17);
            this.nameColumn.TabIndex = 6;
            this.nameColumn.Text = "qwerqwer";
            // 
            // CountObjects
            // 
            this.CountObjects.AutoSize = true;
            this.CountObjects.Font = new System.Drawing.Font("Palatino Linotype", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CountObjects.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.CountObjects.Location = new System.Drawing.Point(32, 367);
            this.CountObjects.Name = "CountObjects";
            this.CountObjects.Size = new System.Drawing.Size(0, 17);
            this.CountObjects.TabIndex = 7;
            // 
            // CreateNew
            // 
            this.CreateNew.AutoSize = true;
            this.CreateNew.Location = new System.Drawing.Point(22, 9);
            this.CreateNew.Name = "CreateNew";
            this.CreateNew.Size = new System.Drawing.Size(157, 17);
            this.CreateNew.TabIndex = 8;
            this.CreateNew.TabStop = true;
            this.CreateNew.Text = "Создание нового объекта.";
            this.CreateNew.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CreateNew_LinkClicked);
            // 
            // EntityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.ClientSize = new System.Drawing.Size(677, 413);
            this.Controls.Add(this.CreateNew);
            this.Controls.Add(this.CountObjects);
            this.Controls.Add(this.nameColumn);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.filterTextBox);
            this.Font = new System.Drawing.Font("Palatino Linotype", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "EntityForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label nameColumn;
        private System.Windows.Forms.Label CountObjects;
        private System.Windows.Forms.LinkLabel CreateNew;
    }
}

