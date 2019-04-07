using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for ImportPlanForm.xaml
    /// </summary>
    public partial class ImportPlanForm : Window
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public ImportPlanForm()
        {
            InitializeComponent();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtFile.Text))
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "File", "File"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(this.txtFile.Text))
            {
                System.Windows.Forms.MessageBox.Show("File not Exist!", Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(this.txtFile.Text);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            try
            {
                List<structExcel> structExcels = new List<structExcel>();

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;
                string deviceName = "";
                for (int i = 3; i <= rowCount; i++)
                {
                    //Console.WriteLine("row:"+i);
                    if (xlRange.Cells[i, 3] != null && xlRange.Cells[i, 3].Value2 != null)
                    {
                        structExcel structExcel = new structExcel();
                        if (xlRange.Cells[i, 1] != null && xlRange.Cells[i, 1].Value2 != null)
                        {
                            deviceName = xlRange.Cells[i, 1].Value2.ToString();
                            //Console.WriteLine("deviceName:" + deviceName);
                        }
                        //Device Name
                        string deviceNameRow = deviceName + " " + xlRange.Cells[i, 5].Value2.ToString();

                        //Console.WriteLine("deviceNameRow:" + deviceNameRow);
                        deviceNameRow = System.Text.RegularExpressions.Regex.Replace(deviceNameRow, @"\s{2,}", " ").ToUpper();
                        //Console.WriteLine("deviceNameRow2:" + deviceNameRow);
                        structExcel.deviceName = deviceNameRow;

                        //foreach(KeyValuePair<int, string> kvp in Constant.productImage)
                        //{
                        //    if(kvp.Key == int.Parse(xlRange.Cells[i, 5].Value2.ToString()))
                        //    {
                        //        structExcel.imageProductUrl = kvp.Value;
                        //        break;
                        //    }
                        //}

                        //product Name
                        string formula = xlRange.Cells[i, 3].Formula.ToString();
                        formula = formula.ToUpper();
                        formula = formula.Split(',')[0].ToString();
                        formula = formula.Replace("=", "").Replace("VLOOKUP(", "");
                        structExcel.productName = xlRange.get_Range(formula, formula).Value2.ToString();
                        //Console.WriteLine("productName:" + structExcel.productName);
                        //product Detail Name
                        string productDetailNameRow = xlRange.Cells[i, 3].Value2.ToString() + " " + xlRange.Cells[i, 4].Value2.ToString();
                        productDetailNameRow = System.Text.RegularExpressions.Regex.Replace(productDetailNameRow, @"\s{2,}", " ").ToUpper();
                        structExcel.productDetailName = productDetailNameRow;
                        //Console.WriteLine("productDetailName:" + structExcel.productDetailName);
                        //Amount Pallet
                        int palletAmount = 0;
                        //int.TryParse(xlRange.Cells[i, 13].Value2.ToString(), out palletAmount);
                        //structExcel.palletAmount = palletAmount;
                        structExcel.palletAmount = 1;

                        structExcels.Add(structExcel);
                    }
                    else
                    {
                        break;
                    }
                }

                clImportPlan clImportPlan = new clImportPlan();

                if ((bool)this.chkDeleteInsert.IsChecked)
                {
                    clImportPlan.flgDeleteInsert = 0;
                }
                else if ((bool)this.chkUpdateInsertAddAmount.IsChecked)
                {
                    clImportPlan.flgDeleteInsert = 1;
                }
                else if ((bool)this.chkUpdateInsert.IsChecked)
                {
                    clImportPlan.flgDeleteInsert = 2;
                }
                DateTime? selectedDate = dtpImport.SelectedDate;
                string formatted = "";
                if (selectedDate.HasValue)
                {
                    formatted = selectedDate.Value.ToString("yyyy-MM-dd");
                }
                clImportPlan.actveDate = formatted;
                clImportPlan.structExcels = structExcels;
                clImportPlan.creUsrId = Global_Object.userLogin;
                clImportPlan.updUsrId = Global_Object.userLogin;

                string jsonData = JsonConvert.SerializeObject(clImportPlan);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "plan/importPlan");
                request.Method = "POST";
                request.ContentType = "application/json";

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(jsonData);
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Flush();
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    int result = 0;
                    int.TryParse(reader.ReadToEnd(), out result);

                    if (result == 1)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Lỗi nhập File hãy kiểm tra lại!", Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                logFile.Error(ex.Message);
            }

            finally
            {
                xlWorkbook.Close();
                xlApp.Quit();
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChkDeleteInsert_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Console.WriteLine(chkDeleteInsert.IsChecked);
        }

        private void ChkDeleteInsert_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chkDeleteInsert.IsChecked)
            {
                this.chkUpdateInsertAddAmount.IsChecked = false;
                this.chkUpdateInsert.IsChecked = false;
            }
        }

        private void ChkUpdateInsertAddAmount_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chkUpdateInsertAddAmount.IsChecked)
            {
                this.chkDeleteInsert.IsChecked = false;
                this.chkUpdateInsert.IsChecked = false;
            }
        }

        private void ChkUpdateInsert_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)this.chkUpdateInsert.IsChecked)
            {
                this.chkDeleteInsert.IsChecked = false;
                this.chkUpdateInsertAddAmount.IsChecked = false;
            }
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Browse Excel Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "Excel",
                Filter = "All Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtFile.Text = openFileDialog1.FileName;
            }
        }
    }
}
