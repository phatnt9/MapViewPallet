using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MapViewPallet
{
    public static class Global_Object
    {
        public static DataTable LoadExcelFile()
        {
            DataTable data = new DataTable();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Excel files (*.xls)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 4;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    Console.WriteLine(filePath);
                    //Read the contents of the file into a stream
                    //var fileStream = openFileDialog.OpenFile();
                    //using (StreamReader reader = new StreamReader(fileStream))
                    //{
                    //    string fileContent = reader.ReadToEnd();
                    //    Console.WriteLine(fileContent);
                    //}
                    string name = "Sheet1";
                    string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                    filePath +
                                    ";Extended Properties='Excel 12.0 XML;HDR=YES;';";

                    OleDbConnection con = new OleDbConnection(constr);
                    OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
                    con.Open();
                    OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
                    sda.Fill(data);
                }
                return data;
            }
        }
        public static string Foo<T>(T parameter) { return typeof(T).Name; }
        public static double palletWidth = 13;
        public static double palletHeight = 15;
        public static double LengthBetweenPoints(Point pt1, Point pt2)
        {
            //Calculate the distance between the both points
            //for both axes separately.
            double dblDistX = Math.Abs(pt1.X - pt2.X);
            double dblDistY = Math.Abs(pt1.Y - pt2.Y);

            //Calculate the length of a line traveling from pt1 to pt2
            //(according to Pythagoras).
            double dblHypotenuseLength = Math.Sqrt(
               dblDistX * dblDistX
               +
               dblDistY * dblDistY
             );

            return dblHypotenuseLength;
        }

    }
}
