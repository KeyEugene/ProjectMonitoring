using RecordBodyToDB.ModelName;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordBodyToDB
{
    public class ParseClass
    {
        public List<DataFile> dataFile { get; set; }
        public string connString { get; private set; }
        public FileInfo[] files { get; private set; }

        public List<string> Types = new List<string>();

        public ParseClass(string conn, FileInfo[] file)
        {
            connString = conn;
            files = file;

        }

        public bool Parse()
        {
            dataFile = new List<DataFile>();

            //AddIntoListType();

            foreach (var item in files)
            {
                WorkWithNameFile(item);
            }
            return true;
        }



        private void WorkWithNameFile(FileInfo file)
        {
            string extention = file.Extension;

            try
            {
                var array = Path.GetFileNameWithoutExtension(file.Name).Split(' ');

                if (array.Count() > 0)
                {
                    string count = null;

                    if (array.Count() > 3)
                        count = array[3].ToString();


                    dataFile.Add(new DataFile
                    {
                        NumberContract = array[0].ToString(),
                        NameWork = array[1].ToString(),
                        NameApplication = new nameApplication
                        {
                            ApplicationType = array[2],
                            NumberApplication = count
                        },
                        Extention = extention.ToLower(),
                        Path = file.FullName
                    });
                }
            }
            catch (Exception) { Form1.NotFound.Add(string.Concat(file.FullName)); }

        }

        //private List<int> NumberApplication(string p)
        //{
        //    if (p.Count() == 1)
        //    {
        //        return new List<int> { Convert.ToInt32(p) };
        //    }
        //    else if (p.Count() > 1)
        //    {
        //        List<int> a = new List<int>();
        //        foreach (char item in p)
        //        {
        //            if (!Char.IsPunctuation(item))
        //                a.Add(Convert.ToInt32(item.ToString()));
        //        }
        //        return a;
        //    }
        //    return null;
        //}


    }
}
