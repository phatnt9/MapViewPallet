using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MapViewPallet.MiniForm
{
    /// <summary>
    /// Interaction logic for OperationControl.xaml
    /// </summary>
    ///


    public partial class PlanControl : Window
    {
        //=================VARIABLE==================
        PlanControlModel operation_model;
        private int _runningShift;
        public int runningShift { get => _runningShift; set => _runningShift = value; }
        

        public PlanControl(string cultureName = null)
        {
            runningShift = 1;
            InitializeComponent();
            ApplyLanguage(cultureName);
            pCalendar.Loaded += pCalendarLoaded;
            //===============TabControlShift========
            TabControlShift.SelectionChanged += TabControlShift_SelectionChanged;
            //===============DataGridView1========
            operation_model = new PlanControlModel(this);
            DataContext = operation_model;
        }

        public void ApplyLanguage(string cultureName = null)
        {
            if (cultureName != null)
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);

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
            //Console.WriteLine("pCalendarLoaded");
            pCalendar.SelectedDate = DateTime.Now;
            if (TabControlShift.IsLoaded)
            {
                TabControlShift.SelectedIndex = 0;
            }
        }
        
        private void TabControlShift_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TabControl)
            {
                if (pCalendar.SelectedDate != null)
                {
                    System.Windows.Controls.TabControl temp = e.Source as System.Windows.Controls.TabControl;
                    //Console.WriteLine("Ca:" + (temp.SelectedIndex + 1));
                    operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
                }
                else
                {
                    pCalendar.SelectedDate = DateTime.Now;
                }
            }
        }

        private void PCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pCalendar.SelectedDate != null)
            {
                switch (UpdateDateStatus((DateTime)pCalendar.SelectedDate))
                {
                    case -1: { Shift1Dgv.IsReadOnly = Shift2Dgv.IsReadOnly = Shift3Dgv.IsReadOnly = false; break; }
                    default: { Shift1Dgv.IsReadOnly = Shift2Dgv.IsReadOnly = Shift3Dgv.IsReadOnly = false; break; }
                }
                if (TabControlShift.IsLoaded)
                {
                    operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
                }
            }
        }
        
        private void Btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if ((pCalendar.SelectedDate != null) &&
                (TabControlShift.SelectedIndex>=0) &&
                (TabControlShift.IsLoaded))
            {

                operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
            }
        }
        
        /// <summary>
        /// -1: Ngày hôm trước trở đi, 0: Ngày hôm nay, 1: Ngày hôm sau trở đi
        /// </summary>
        /// <param name="pDate"></param>
        /// <returns></returns>
        public int UpdateDateStatus(DateTime pDate)
        {
            string ngay = "";
            switch (pDate.DayOfWeek)
            {
                case DayOfWeek.Monday: { ngay = "Thứ Hai"; break; }
                case DayOfWeek.Tuesday: { ngay = "Thứ Ba"; break; }
                case DayOfWeek.Wednesday: { ngay = "Thứ Tư"; break; }
                case DayOfWeek.Thursday: { ngay = "Thứ Năm"; break; }
                case DayOfWeek.Friday: { ngay = "Thứ Sáu"; break; }
                case DayOfWeek.Saturday: { ngay = "Thứ Bảy"; break; }
                case DayOfWeek.Sunday: { ngay = "Chủ Nhật"; break; }
                default: { ngay = "Chủ Nhật"; break; }
            }
            if (DateTime.Now.ToShortDateString() == pDate.ToShortDateString())
            {
                //DateTimeBorder.Background = new SolidColorBrush(Colors.LightGreen);
                pCalendar.Background = new SolidColorBrush(Colors.LightGreen);
                //lb_Date.Foreground = new SolidColorBrush(Colors.Black);
                return 0;
            }
            else
            {
                if (DateTime.Now.CompareTo(pDate) == 1)
                {
                    //DateTimeBorder.Background = new SolidColorBrush(Colors.IndianRed);
                    pCalendar.Background = new SolidColorBrush(Colors.IndianRed);
                    //lb_Date.Foreground = new SolidColorBrush(Colors.Black);
                    return -1;
                }
                else
                {
                    //DateTimeBorder.Background = new SolidColorBrush(Colors.LightGray);
                    pCalendar.Background = new SolidColorBrush(Colors.LightGray);
                    //lb_Date.Foreground = new SolidColorBrush(Colors.Black);
                    return 1;
                }
            }
        }
        
        private void Btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            operation_model.UpdateAllCurrentPlansToDb();
            operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
        }
        
        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(operation_model.deviceList);
            foreach (Plan plan in operation_model.BasePlans1)
            {
                string print = "\"" + plan.deviceName + "\"   deviceProductId:" + plan.deviceProductId + "   productDetailId:" + plan.productDetailId + "   palletAmount:" + plan.palletAmount + "   " + plan.productName;//productDetailName
                Console.WriteLine(print);
            }
            Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "plan/createPlanPallet");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.activeDate = activeDate;
                postApiBody.timeWorkId = TabControlShift.SelectedIndex+1;
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
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
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

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "plan/deleteListPlan");
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
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if ((pCalendar.SelectedDate != null) && (TabControlShift.SelectedIndex >= 0) && (TabControlShift.IsLoaded))
                        {

                            operation_model.CreateListPlansFromShift((DateTime)pCalendar.SelectedDate, TabControlShift.SelectedIndex + 1);
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
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

        }
    }

}
