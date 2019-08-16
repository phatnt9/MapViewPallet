using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    public class ImportButtonEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!value.ToString().Equals("Ready"))
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    /// <summary>
    /// Interaction logic for ImportPlanForm.xaml
    /// </summary>
    public partial class ImportPlanForm : Window, INotifyPropertyChanged
    {
        public enum AppStatus
        {
            Ready,
            Exporting,
            Importing,
            Completed,
            Finished,
            Cancelled,
            Error,
        }
        private AppStatus _pgbStatus;
        public AppStatus PgbStatus { get => _pgbStatus; set { _pgbStatus = value; RaisePropertyChanged("PgbStatus"); } }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        private BackgroundWorker worker;
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ImportPlanForm()
        {
            InitializeComponent();
            PgbStatus = AppStatus.Ready;
            DataContext = this;
        }
        public void WriteLog(string message)
        {
            rtb_log.AppendText(message);
            rtb_log.AppendText("\u2028");
            rtb_log.ScrollToEnd();
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

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
            object[] argumentBackGroundWorker = new object[3];

            argumentBackGroundWorker[0] = this.txtFile.Text;
            if ((bool)this.chkDeleteInsert.IsChecked)
            {
                argumentBackGroundWorker[1] = 0;
            }
            else if ((bool)this.chkUpdateInsertAddAmount.IsChecked)
            {
                argumentBackGroundWorker[1] = 1;
            }
            else if ((bool)this.chkUpdateInsert.IsChecked)
            {
                argumentBackGroundWorker[1] = 2;
            }
            argumentBackGroundWorker[2] = dtpImport.SelectedDate;
            worker.RunWorkerAsync(argument: argumentBackGroundWorker);
        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WriteLog((string)e.UserState);
            pbStatus.Value = e.ProgressPercentage;
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pbStatus.Value = 0;
            PgbStatus = AppStatus.Ready;
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            PgbStatus = AppStatus.Importing;
            object[] argBackGroundWorker = (object[])e.Argument;
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open((string)argBackGroundWorker[0]);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            try
            {
                List<structExcel> structExcels = new List<structExcel>();

                int rowCount = xlRange.Rows.Count;
                string deviceName = "";
                for (int i = 3; i <= rowCount; i++)
                {
                    //Add 3 lần cho plan returnMain va return401
                    for (int r = 0; r < 3; r++)
                    {
                        if (xlRange.Cells[i, 3] != null && xlRange.Cells[i, 3].Value2 != null)
                        {
                            structExcel structExcel = new structExcel();
                            if (xlRange.Cells[i, 1] != null && xlRange.Cells[i, 1].Value2 != null)
                            {
                                deviceName = xlRange.Cells[i, 1].Value2.ToString();
                            }
                            //Device Name
                            int machinePart = 0;
                            int.TryParse(xlRange.Cells[i, 5].Value2.ToString(), out machinePart);
                            if (machinePart != 0)
                            {
                                string deviceNameRow = ((r == 1) ? "RETURN_401" : ((r == 2) ? "RETURN_MAIN" : deviceName)) + " " + (((r == 1) || (r == 2)) ? 0 : xlRange.Cells[i, 5].Value2.ToString());
                                deviceNameRow = System.Text.RegularExpressions.Regex.Replace(deviceNameRow, @"\s{2,}", " ").ToUpper();
                                structExcel.deviceName = deviceNameRow;

                                //product Name
                                string formula = xlRange.Cells[i, 3].Formula.ToString();
                                formula = formula.ToUpper();
                                formula = formula.Split(',')[0].ToString();
                                formula = formula.Replace("=", "").Replace("VLOOKUP(", "");
                                structExcel.productName = xlRange.get_Range(formula, formula).Value2.ToString();
                                //product Detail Name
                                string productDetailNameRow = xlRange.Cells[i, 3].Value2.ToString() + " " + xlRange.Cells[i, 4].Value2.ToString();
                                productDetailNameRow = System.Text.RegularExpressions.Regex.Replace(productDetailNameRow, @"\s{2,}", " ").ToUpper();
                                structExcel.productDetailName = productDetailNameRow;
                                //Amount Pallet
                                //int palletAmount = 0;
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
                        else
                        {
                            break;
                        }
                    }

                    (sender as BackgroundWorker).ReportProgress((i * 100) / rowCount, ("--Row:" + i));
                }
                
                clImportPlan clImportPlan = new clImportPlan();

                clImportPlan.flgDeleteInsert = (int)argBackGroundWorker[1];
                DateTime? selectedDate = (DateTime)argBackGroundWorker[2];
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

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "plan/importPlan");
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
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            try
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
                    string folderPath = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                    Console.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }
    }
}