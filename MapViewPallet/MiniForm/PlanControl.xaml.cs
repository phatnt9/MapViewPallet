using MapViewPallet.MiniForm.MicsWpfForm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace MapViewPallet.MiniForm
{
    internal class dtTempPlan
    {
        public int planId;

        public dtTempPlan(dtPlan plan)
        {
            planId = plan.planId;
        }
    }

    /// <summary>
    /// Interaction logic for OperationControl.xaml
    /// </summary>
    ///

    public partial class PlanControl : Window
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //=================VARIABLE==================
        public PlanControlModel operation_model;

        public PlanControl(string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            operation_model = new PlanControlModel(this);
            DataContext = operation_model;
            pCalendar.Loaded += pCalendarLoaded;
            TabControlShift.SelectionChanged += TabControlShift_SelectionChanged;
            Shift1Dgv.IsTextSearchEnabled = true;
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
        }

        public void ApplyLanguage(string cultureName = null)
        {
            if (cultureName != null)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            }

            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "vi-VN":
                    dict.Source = new Uri("..\\Lang\\Vietnamese.xaml", UriKind.Relative);
                    break;
                // ...
                default:
                    dict.Source = new Uri("..\\Lang\\English.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void pCalendarLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                pCalendar.SelectedDate = DateTime.Now;
                if (TabControlShift.IsLoaded)
                {
                    TabControlShift.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void TabControlShift_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.Source is System.Windows.Controls.TabControl)
                {
                    if (pCalendar.SelectedDate != null)
                    {
                        System.Windows.Controls.TabControl temp = e.Source as System.Windows.Controls.TabControl;
                        //Console.WriteLine("Ca:" + (temp.SelectedIndex + 1));
                        Dispatcher.BeginInvoke(new ThreadStart(() =>
                        {
                            operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
                        }));
                    }
                    else
                    {
                        pCalendar.SelectedDate = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void PCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (pCalendar.SelectedDate != null)
                {
                    switch (UpdateDateStatus((DateTime)pCalendar.SelectedDate))
                    {
                        case -1:
                        { Shift1Dgv.IsReadOnly = false; break; }
                        default:
                        { Shift1Dgv.IsReadOnly = false; break; }
                    }
                    if (TabControlShift.IsLoaded)
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(() =>
                        {
                            operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        /// <summary>
        /// -1: Ngày hôm trước trở đi, 0: Ngày hôm nay, 1: Ngày hôm sau trở đi
        /// </summary>
        /// <param name="pDate"></param>
        /// <returns></returns>
        public int UpdateDateStatus(DateTime pDate)
        {
            try
            {
                string ngay = "";
                switch (pDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                    { ngay = "Thứ Hai"; break; }
                    case DayOfWeek.Tuesday:
                    { ngay = "Thứ Ba"; break; }
                    case DayOfWeek.Wednesday:
                    { ngay = "Thứ Tư"; break; }
                    case DayOfWeek.Thursday:
                    { ngay = "Thứ Năm"; break; }
                    case DayOfWeek.Friday:
                    { ngay = "Thứ Sáu"; break; }
                    case DayOfWeek.Saturday:
                    { ngay = "Thứ Bảy"; break; }
                    case DayOfWeek.Sunday:
                    { ngay = "Chủ Nhật"; break; }
                    default:
                    { ngay = "Chủ Nhật"; break; }
                }
                if (DateTime.Now.ToShortDateString() == pDate.ToShortDateString())
                {
                    pCalendar.Background = new SolidColorBrush(Colors.LightGreen);
                    return 0;
                }
                else
                {
                    if (DateTime.Now.CompareTo(pDate) == 1)
                    {
                        pCalendar.Background = new SolidColorBrush(Colors.IndianRed);
                        return -1;
                    }
                    else
                    {
                        pCalendar.Background = new SolidColorBrush(Colors.LightGray);
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
                return -5;
            }
        }

        private void Btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                operation_model.UpdateAllCurrentPlansToDb();
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
                }));
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_CreatePlanPallet_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                DateTime selectedDate = (DateTime)pCalendar.SelectedDate;
                string activeDate = selectedDate.Year + "-" + selectedDate.Month.ToString("00.") + "-" + selectedDate.Day.ToString("00.");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "plan/createPlanPallet");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.activeDate = activeDate;
                postApiBody.timeWorkId = TabControlShift.SelectedIndex + 1;
                postApiBody.updUsrId = Global_Object.userLogin;
                string jsonData = JsonConvert.SerializeObject(postApiBody);
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
                    string result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void DeletePlan_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                List<Plan> listPlanDelete = new List<Plan>();
                Plan plan = (sender as System.Windows.Controls.Button).DataContext as Plan;
                listPlanDelete.Add(plan);
                string jsonData = JsonConvert.SerializeObject(listPlanDelete);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "plan/deleteListPlan");
                request.Method = "DELETE";
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
                        if ((pCalendar.SelectedDate != null) && (TabControlShift.SelectedIndex >= 0) && (TabControlShift.IsLoaded))
                        {
                            //Dispatcher.BeginInvoke(new ThreadStart(() =>
                            //{
                            //    operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
                            //}));
                        }
                    }
                    else if (result == 2)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteUse, "Devices", "Other Screen"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Btn_importPlan_Click(object sender, RoutedEventArgs e)
        {
            ImportPlanForm importPlanForm = new ImportPlanForm();
            importPlanForm.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                //Console.WriteLine(sender);
                dtPlan selectedPlan = Shift1Dgv.SelectedItem as dtPlan;
                dynamic postApiBody = new JObject();
                postApiBody.timeWorkId = 0;
                postApiBody.activeDate = selectedPlan.activeDate;
                postApiBody.deviceId = selectedPlan.deviceId;
                postApiBody.productId = selectedPlan.productId;
                postApiBody.productDetailId = selectedPlan.productDetailId;
                postApiBody.updUsrId = 1;
                postApiBody.palletAmount = 1;
                string jsonData = JsonConvert.SerializeObject(postApiBody);
                //Console.WriteLine(jsonData);

                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create(@"http://" +
                    Properties.Settings.Default.serverIp + ":" +
                    Properties.Settings.Default.serverPort +
                    @"/robot/rest/" + "plan/createPlanPallet");

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
                    Console.WriteLine("PalletId="+result);
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(operation_model.BasePlans1);
        }

        private void Btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
                }));
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //operation_model.FilterString = tb_searchPlan.Text.Trim();
            var view = operation_model.GroupedDevices_S1;
            view.Filter = (o => (o as Plan).productDetailName.Contains((sender as System.Windows.Controls.TextBox).Text));
            //view.Filter = PlansFilter;
        }

        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cbShowManualPlan.IsChecked = !cbShowManualPlan.IsChecked;
        }

        private void Label_MouseDown2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cbShowReturnPlan.IsChecked = !cbShowReturnPlan.IsChecked;
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show
                        (
                        String.Format(Global_Object.messageDeleteConfirm, "Plans"),
                        Global_Object.messageTitileWarning, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                        )
            {
                try
                {
                    List<dtTempPlan> listToDetele = new List<dtTempPlan>();
                    if (Shift1Dgv.SelectedItems.Count > 0)
                    {
                        foreach (dtPlan plan in Shift1Dgv.SelectedItems)
                        {
                            listToDetele.Add(new dtTempPlan(plan));
                        }
                        string jsonData = JsonConvert.SerializeObject(listToDetele);
                        string contentJson = Global_Object.RequestDataAPI(jsonData, "plan/deleteListPlan", Global_Object.RequestMethod.DELETE);
                        dynamic response = JsonConvert.DeserializeObject(contentJson);

                        if (response != null)
                        {
                            if (response == 1)
                            {
                                System.Windows.MessageBox.Show("Plans deleted successfully!");
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Cannot Delete!");
                            }
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Nothing to delete!");
                    }
                }
                catch (Exception ex)
                {
                    logFile.Error(ex.Message);
                }
            }
        }
    }
}