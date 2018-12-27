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
        PlanControlModel operation_model;
        private int _runningShift;
        public int runningShift { get => _runningShift; set => _runningShift = value; }
        

        public PlanControl()
        {
            runningShift = 1;
            InitializeComponent();
            pCalendar.Loaded += pCalendarLoaded;
            //===============TabControlShift========
            TabControlShift.SelectionChanged += TabControlShift_SelectionChanged;
            //===============DataGridView1========
            pCalendar.SelectionMode = CalendarSelectionMode.SingleDate;
            Shift1Dgv.SelectionMode = Shift2Dgv.SelectionMode = Shift3Dgv.SelectionMode = DataGridSelectionMode.Single;
            Shift1Dgv.SelectionUnit = Shift2Dgv.SelectionUnit = Shift3Dgv.SelectionUnit = DataGridSelectionUnit.FullRow;
            //===============DataGridView1========
            operation_model = new PlanControlModel(this);
            DataContext = operation_model;
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
            if (e.Source is TabControl)
            {
                if (pCalendar.SelectedDate != null)
                {
                    TabControl temp = e.Source as TabControl;
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
                    case -1: { Shift1Dgv.IsReadOnly = Shift2Dgv.IsReadOnly = Shift3Dgv.IsReadOnly = true; break; }
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

        //private void Btn_Test2_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (Plan item in operation_model.BasePlans1)
        //    {
        //        item.productDetailId++;
        //    }
        //}
    }

}
