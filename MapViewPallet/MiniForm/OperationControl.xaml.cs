using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapViewPallet.MiniForm
{
    /// <summary>
    /// Interaction logic for OperationControl.xaml
    /// </summary>
    ///
    public partial class OperationControl : Window
    {
        //=================VARIABLE==================
        OperationModel operation_model;
        
        

        public OperationControl()
        {
            InitializeComponent();
            Loaded += GoRefreshData;
            //===============DataGridView1========
            Shift1Dgv.SelectionMode = Shift2Dgv.SelectionMode = Shift3Dgv.SelectionMode = DataGridSelectionMode.Single;
            Shift1Dgv.SelectionUnit = Shift2Dgv.SelectionUnit = Shift3Dgv.SelectionUnit = DataGridSelectionUnit.FullRow;
            //Shift1Dgv.SelectedCellsChanged += Shift1Dgv_SelectedCellsChanged;
            //Shift1Dgv.SelectionChanged += Shift1Dgv_SelectionChanged;
            //Shift1Dgv.GotFocus += Shift1Dgv_GotFocus;
            //Shift1Dgv.LostFocus += Shift1Dgv_LostFocus;
            operation_model = new OperationModel();
            DataContext = operation_model;
            pCalendar.SelectionMode = CalendarSelectionMode.SingleDate;
            DateTime tempDT = DateTime.Now;
            RefreshDate(tempDT);
        }

        private void GoRefreshData(object sender, RoutedEventArgs e)
        {
            
            operation_model.RefreshData();
        }

        private void PCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((DateTime)pCalendar.SelectedDate != null)
            {
                RefreshDate((DateTime)pCalendar.SelectedDate);
            }
        }

        public void RefreshDate (DateTime pDate)
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
            lb_Date.Text = ngay + ", "+ pDate.Day + " Tháng " + pDate.Month + ", " + pDate.Year;
            if (DateTime.Now.ToShortDateString() == pDate.ToShortDateString())
            {
                DateTimeBorder.Background = new SolidColorBrush(Colors.LightGreen);
                pCalendar.Background = new SolidColorBrush(Colors.LightGreen);
                lb_Date.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                if (DateTime.Now.CompareTo(pDate) == 1)
                {
                    DateTimeBorder.Background = new SolidColorBrush(Colors.IndianRed);
                    pCalendar.Background = new SolidColorBrush(Colors.IndianRed);
                    lb_Date.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    DateTimeBorder.Background = new SolidColorBrush(Colors.LightGray);
                    pCalendar.Background = new SolidColorBrush(Colors.LightGray);
                    lb_Date.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void Btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            operation_model.RefreshData();
        }
    }
    
}
