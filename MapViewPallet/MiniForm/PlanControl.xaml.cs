using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        PlanModel operation_model;
        private int _runningShift;
        public int runningShift { get => _runningShift; set => _runningShift = value; }
        

        public PlanControl()
        {
            runningShift = 1;
            InitializeComponent();
            Loaded += FormLoad;
            pCalendar.Loaded += pCalendarLoadedEvent;
            //===============DataGridView1========
            Shift1Dgv.SelectionMode = Shift2Dgv.SelectionMode = Shift3Dgv.SelectionMode = DataGridSelectionMode.Single;
            Shift1Dgv.SelectionUnit = Shift2Dgv.SelectionUnit = Shift3Dgv.SelectionUnit = DataGridSelectionUnit.FullRow;
            operation_model = new PlanModel(this);
            DataContext = operation_model;
            pCalendar.SelectionMode = CalendarSelectionMode.SingleDate;
            UpdateDateStatus(DateTime.Now);
            
        }

        private void pCalendarLoadedEvent(object sender, RoutedEventArgs e)
        {
            if (pCalendar.SelectedDate == null)
            {
                //Console.WriteLine("Tao Ngay");
                pCalendar.SelectedDate = DateTime.Now;
            }
        }

        private void PCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pCalendar.SelectedDate != null)
            {
                //Console.WriteLine("PCalendar_SelectedDatesChanged");
                int dateStatus = UpdateDateStatus((DateTime)pCalendar.SelectedDate);
                //**************************
                DateTime selectedDate = (DateTime)pCalendar.SelectedDate;
                string date = selectedDate.Year + "-" + selectedDate.Month + "-" + selectedDate.Day.ToString("00.");
                //operation_model.CreateListPlansFromDeviceList(selectedDate); // For selected day
                //**************************
                switch (dateStatus)
                {
                    case -1: { Shift1Dgv.IsEnabled = false; Shift2Dgv.IsEnabled = false; Shift3Dgv.IsEnabled = false; break; }
                    case 0: { goto default; }
                    case 1: { goto default; }
                    default: { Shift1Dgv.IsEnabled = true; Shift2Dgv.IsEnabled = true; Shift3Dgv.IsEnabled = true; break; }
                }
            }
            operation_model.CreateListPlansFromDeviceList((DateTime)pCalendar.SelectedDate);
            operation_model.RefreshData();
        }


        private void FormLoad(object sender, RoutedEventArgs e)
        {
            operation_model.deviceList.GetDevicesList();
            //Thread.Sleep(1000);
            //Mặc định kiểm tra plan của ngày hôm nay đã 
            //operation_model.RefreshData();
            //Console.WriteLine("FormLoad");
            //if (pCalendar.SelectedDate == null)
            //{
            //    pCalendar.SelectedDate = DateTime.Now;
            //}
            //operation_model.CreateListPlansFromDeviceList();
            //if ((DateTime)pCalendar.SelectedDate != null)
            //{
            //operation_model.CheckPlans();
            //Kiểm tra có dữ liệu cho ngày đã chọn trong lịch hay chưa,
            // Có thì load
            // Chưa thì cho tạo mới

            //}
        }

        

        

        private void Btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            //operation_model.CreateListPlansFromDeviceList((DateTime)pCalendar.SelectedDate);
            //operation_model.RefreshData();
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            operation_model.basePlans1.Clear();
            operation_model.basePlans2.Clear();
            operation_model.basePlans3.Clear();
            operation_model.RefreshData();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(operation_model.deviceList);
            foreach (Plan plan in operation_model.basePlans1)
            {
                string print = "\"" + plan.deviceName + "\"   deviceProductId:" + plan.deviceProductId + "   productDetailId:" + plan.productDetailId + "   palletAmount:" + plan.palletAmount + "   " + plan.productName;//productDetailName
                Console.WriteLine(print);
            }
            //Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //foreach (Plan plan in operation_model.basePlans1)
            //{
            //    string print = "\"" + plan.deviceName + "\"   deviceProductId:" + plan.deviceProductId + "   productDetailId:" + plan.productDetailId + "   palletAmount:" + plan.palletAmount + "   " + plan.productName;//productDetailName
            //    Console.WriteLine(print);
            //}
            //Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //foreach (Plan plan in operation_model.basePlans1)
            //{
            //    string print = "\"" + plan.deviceName + "\"   deviceProductId:" + plan.deviceProductId + "   productDetailId:" + plan.productDetailId + "   palletAmount:" + plan.palletAmount + "   " + plan.productName;//productDetailName
            //    Console.WriteLine(print);
            //}
            Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

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
            lb_Date.Text = ngay + ", " + pDate.Day + " Tháng " + pDate.Month + ", " + pDate.Year;
            if (DateTime.Now.ToShortDateString() == pDate.ToShortDateString())
            {
                DateTimeBorder.Background = new SolidColorBrush(Colors.LightGreen);
                pCalendar.Background = new SolidColorBrush(Colors.LightGreen);
                lb_Date.Foreground = new SolidColorBrush(Colors.Black);
                return 0;
            }
            else
            {
                if (DateTime.Now.CompareTo(pDate) == 1)
                {
                    DateTimeBorder.Background = new SolidColorBrush(Colors.IndianRed);
                    pCalendar.Background = new SolidColorBrush(Colors.IndianRed);
                    lb_Date.Foreground = new SolidColorBrush(Colors.Black);
                    return -1;
                }
                else
                {
                    DateTimeBorder.Background = new SolidColorBrush(Colors.LightGray);
                    pCalendar.Background = new SolidColorBrush(Colors.LightGray);
                    lb_Date.Foreground = new SolidColorBrush(Colors.Black);
                    return 1;
                }
            }
        }

        private void Shift1Dgv_Loaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Shift1Dgv_Loaded");
            //operation_model.RefreshData();
        }

        private void Shift2Dgv_Loaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Shift2Dgv_Loaded");
            //operation_model.RefreshData();
        }

        private void Shift3Dgv_Loaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Shift3Dgv_Loaded");
            //operation_model.CreateListPlansFromDeviceList((DateTime)pCalendar.SelectedDate);
            //operation_model.RefreshData();
        }

        private void Btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            operation_model.UpdateAllCurrentPlansToDb();
        }


        //private void TabControlShift_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Console.WriteLine("TabControlShift_SelectionChanged");
        //    if (e.Source is TabControl)
        //    {
        //        TabControl temp = e.Source as TabControl;
        //        string selectedTabName = (temp.SelectedItem as TabItem).Name;
        //        if (pCalendar.SelectedDate == null)
        //        {
        //            //pCalendar.SelectedDate = DateTime.Now;
        //        }
        //        //DateTime selectedDate = (DateTime)pCalendar.SelectedDate;
        //        //string date = selectedDate.Year + "-" + selectedDate.Month + "-" + selectedDate.Day.ToString("00.");
        //        //Console.WriteLine("Ca: " + selectedTabName + " Ngay: " + date);
        //        //switch (selectedTabName)
        //        //{
        //        //    case "Plan_Shift1":
        //        //        {
        //        //            operation_model.CheckPlans(1, date);
        //        //            break;
        //        //        }
        //        //    case "Plan_Shift2":
        //        //        {
        //        //            operation_model.CheckPlans(2, date);
        //        //            break;
        //        //        }
        //        //    case "Plan_Shift3":
        //        //        {
        //        //            operation_model.CheckPlans(3, date);
        //        //            break;
        //        //        }
        //        //    default:
        //        //        {
        //        //            break;
        //        //        }
        //        //}
        //    }
        //}
    }

}
