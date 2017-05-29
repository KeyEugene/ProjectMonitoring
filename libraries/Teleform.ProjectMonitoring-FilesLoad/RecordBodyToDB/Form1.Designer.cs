namespace RecordBodyToDB
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.filesCount = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NotFoundTextBox = new System.Windows.Forms.TextBox();
            this.YFoundTextBox = new System.Windows.Forms.TextBox();
            this.NewCreateTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxSaveNew = new System.Windows.Forms.CheckBox();
            this.checkBoxUpdateOld = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.авторизацияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(465, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Найту папку";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(447, 20);
            this.textBox1.TabIndex = 1;
            // 
            // filesCount
            // 
            this.filesCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filesCount.AutoSize = true;
            this.filesCount.Location = new System.Drawing.Point(462, 131);
            this.filesCount.Name = "filesCount";
            this.filesCount.Size = new System.Drawing.Size(7, 13);
            this.filesCount.TabIndex = 2;
            this.filesCount.Text = "\r\n";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(419, 84);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 44);
            this.button2.TabIndex = 3;
            this.button2.Text = "Старт записи";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(15, 667);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Несохраненные документы :";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(14, 396);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(409, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Документы, по-которым файлы были сохранены или перезаписаны :";
            // 
            // NotFoundTextBox
            // 
            this.NotFoundTextBox.Location = new System.Drawing.Point(15, 688);
            this.NotFoundTextBox.Multiline = true;
            this.NotFoundTextBox.Name = "NotFoundTextBox";
            this.NotFoundTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.NotFoundTextBox.Size = new System.Drawing.Size(556, 217);
            this.NotFoundTextBox.TabIndex = 8;
            // 
            // YFoundTextBox
            // 
            this.YFoundTextBox.Location = new System.Drawing.Point(12, 414);
            this.YFoundTextBox.Multiline = true;
            this.YFoundTextBox.Name = "YFoundTextBox";
            this.YFoundTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.YFoundTextBox.Size = new System.Drawing.Size(559, 233);
            this.YFoundTextBox.TabIndex = 9;
            // 
            // NewCreateTextBox
            // 
            this.NewCreateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NewCreateTextBox.Location = new System.Drawing.Point(12, 162);
            this.NewCreateTextBox.Multiline = true;
            this.NewCreateTextBox.Name = "NewCreateTextBox";
            this.NewCreateTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.NewCreateTextBox.Size = new System.Drawing.Size(559, 219);
            this.NewCreateTextBox.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(15, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(188, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Новые, созданные документы :";
            // 
            // checkBoxSaveNew
            // 
            this.checkBoxSaveNew.AutoSize = true;
            this.checkBoxSaveNew.Location = new System.Drawing.Point(15, 67);
            this.checkBoxSaveNew.Name = "checkBoxSaveNew";
            this.checkBoxSaveNew.Size = new System.Drawing.Size(301, 17);
            this.checkBoxSaveNew.TabIndex = 12;
            this.checkBoxSaveNew.Text = "Создавать новые документы, если такие не найдены.";
            this.checkBoxSaveNew.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpdateOld
            // 
            this.checkBoxUpdateOld.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUpdateOld.AutoSize = true;
            this.checkBoxUpdateOld.Location = new System.Drawing.Point(15, 91);
            this.checkBoxUpdateOld.Name = "checkBoxUpdateOld";
            this.checkBoxUpdateOld.Size = new System.Drawing.Size(379, 17);
            this.checkBoxUpdateOld.TabIndex = 13;
            this.checkBoxUpdateOld.Text = "Перезаписывать данные о документе, если файл документа не пуст.";
            this.checkBoxUpdateOld.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.авторизацияToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(587, 24);
            this.menuStrip1.TabIndex = 14;
            // 
            // авторизацияToolStripMenuItem
            // 
            this.авторизацияToolStripMenuItem.Name = "авторизацияToolStripMenuItem";
            this.авторизацияToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
            this.авторизацияToolStripMenuItem.Text = "Авторизация";
            this.авторизацияToolStripMenuItem.Click += new System.EventHandler(this.авторизацияToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(587, 918);
            this.Controls.Add(this.checkBoxUpdateOld);
            this.Controls.Add(this.checkBoxSaveNew);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NewCreateTextBox);
            this.Controls.Add(this.YFoundTextBox);
            this.Controls.Add(this.NotFoundTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.filesCount);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Сохранение документов";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label filesCount;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox NotFoundTextBox;
        private System.Windows.Forms.TextBox YFoundTextBox;
        private System.Windows.Forms.TextBox NewCreateTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxSaveNew;
        private System.Windows.Forms.CheckBox checkBoxUpdateOld;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem авторизацияToolStripMenuItem;
    }
}

