using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WaitServerForm : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public System.Timers.Timer serverTimer;
        MainWindow mainWindow;

        public WaitServerForm(MainWindow mainWindow)
        {
            Console.WriteLine("Tao roi");
            InitializeComponent();
            this.mainWindow = mainWindow;
            serverTimer = new System.Timers.Timer();
            serverTimer.Interval = 5000;
            serverTimer.Elapsed += OnTimedWaitServerEvent;
            serverTimer.AutoReset = true;
            serverTimer.Enabled = false;
            Loaded += WaitServerForm_Loaded;
            Closing += WaitServerForm_Closing;
            
        }

        private void WaitServerForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Dang dong");
        }

        private void WaitServerForm_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            mainWindow.stationTimer.Enabled = false;

            if (mainWindow.planControl != null)
            {
                mainWindow.planControl.Close();
            }
            if (mainWindow.devicesManagement != null)
            {
                mainWindow.devicesManagement.Close();
            }
            if (mainWindow.userManagement != null)
            {
                mainWindow.userManagement.Close();
            }
            if (mainWindow.statistics != null)
            {
                mainWindow.statistics.Close();
            }
            serverTimer.Enabled = true;
        }

        private void OnTimedWaitServerEvent(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (Global_Object.ServerAlive())
                {
                    mainWindow.stationTimer.Enabled = true;
                    serverTimer.Enabled = false;
                    Close();
                }
            });
            
        }
    }
}
