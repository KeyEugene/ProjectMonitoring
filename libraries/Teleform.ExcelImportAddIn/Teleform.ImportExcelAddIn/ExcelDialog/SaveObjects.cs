using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelDialog
{
    public class SaveObjects
    {
        public static void Serialize(string name, object obj)
        {
            FileStream fileStream = new FileStream(string.Concat(name, ".dat"), FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fileStream, obj);
            }
            catch (Exception e)
            {
                MessageBox.Show("Faild to serialize. Reason: " + e.Message);
                throw;
            }
            fileStream.Close();
        }

        public static object Deserialize(string name)
        {
            object o = null;
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(string.Concat(name, ".dat"), FileMode.Open);
            }
            catch (Exception)
            {
                return null;
            }

            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                o = formatter.Deserialize(fileStream);
            }
            catch (Exception e)
            {
                MessageBox.Show("Faild to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fileStream.Close();
            }
            return o;
        }
    }
}
