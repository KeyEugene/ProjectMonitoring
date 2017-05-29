using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecordBodyToDB
{
    enum EnumTextBox
    {
        Ncreate,
        found,
        Nfound
    }
    public partial class Form1 : Form
    {
        private RecordFiles record;
        private ParseClass parse;
        public static string connString; //= @"data source=stend\sqlexpress_12; Initial Catalog = MinPromNew; User Id=sa; Password=345; Asynchronous Processing=true";
        private int count;

        public static List<string> NotFound = new List<string>();
        public static List<string> Found = new List<string>();
        public static List<string> NewCreate = new List<string>();

        public static bool create;
        public static bool edit;

        private Login loginClass;
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            filesCount.ForeColor = Color.Black;
            NotFound.Clear(); Found.Clear(); NewCreate.Clear();

            if (!Login.CheckConnectionString())
            {
                loginClass = new Login();
                loginClass.Show();
                return;
            }
            else
                connString = Login.connString;

            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = folderBrowserDialog1.SelectedPath;
                textBox1.Text = path;

                DirectoryInfo di = new DirectoryInfo(path);
                var files = di.GetFiles();

                count = files.Count();

                if (count == 0)
                {
                    filesCount.Text = "Папка пуста.";
                    return;
                }

                filesCount.Text = count + " файлов ";

                parse = new ParseClass(connString, files);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewCreateTextBox.Text = YFoundTextBox.Text = NotFoundTextBox.Text = string.Empty;

            if (!Login.CheckConnectionString())
            {
                loginClass = new Login();
                loginClass.Show();
                return;
            }

            create = checkBoxSaveNew.Checked;
            edit = checkBoxUpdateOld.Checked;

            if (count == 0)
            {
                filesCount.ForeColor = Color.Red;
                filesCount.Text = "Укажите папку.";
                return;
            }

            if (parse.Parse())
            {
                record = new RecordFiles(parse.dataFile, connString);
            }

            if (NotFound.Count != 0) WorkWithTextBox(NotFound, EnumTextBox.Nfound);
            if (Found.Count != 0) WorkWithTextBox(Found, EnumTextBox.found);
            if (NewCreate.Count != 0) WorkWithTextBox(NewCreate, EnumTextBox.Ncreate);

        }



        private void WorkWithTextBox(List<string> list, EnumTextBox etb)
        {
            int i = 0;
            foreach (var item in list)
            {
                i++;
                switch (etb)
                {
                    case EnumTextBox.Ncreate:
                        NewCreateTextBox.AppendText(i.ToString() + " ) " + item + Environment.NewLine);
                        break;
                    case EnumTextBox.found:
                        YFoundTextBox.AppendText(i.ToString() + " ) " + item + Environment.NewLine);
                        break;
                    case EnumTextBox.Nfound:
                        NotFoundTextBox.AppendText(i.ToString() + " ) " + item + Environment.NewLine);
                        break;
                    default:
                        break;
                }


            }
        }

        private void авторизацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loginClass = new Login();
            loginClass.Show();
        }


    }
}
