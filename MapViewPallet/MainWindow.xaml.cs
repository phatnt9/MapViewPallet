﻿using MapViewPallet.MiniForm;
using MapViewPallet.MiniForm.MicsWpfForm;
using MapViewPallet.Shape;
using MapViewPallet.View;
using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapViewPallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //=================VARIABLE==================
        public System.Timers.Timer stationTimer;

        public bool drag = true;
        public CanvasControlService canvasControlService;

        //public WaitServerForm waitServerForm;
        public LoginForm loginForm;

        //public PlanControl planControl;
        public MVVM_PlanManagement planControl_MVVM;
        public DevicesManagement devicesManagement;
        public UserManagement userManagement;
        public Statistics statistics;

        public MainWindow()
        {
            InitializeComponent();
            ApplyLanguage();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            Closed += MainWindow_Closed;
            //==============TreeView=============
            //===================================
            //canvasMatrixTransform = new MatrixTransform(1, 0, 0, -1, 0, 0);

            //ImageBrush img = LoadImage("Map_aTan___Copy4___Copy");
            ImageBrush img = LoadImage("mapPCLDuy");
            map.Width = img.ImageSource.Width;
            map.Height = img.ImageSource.Height;
            map.Background = img;

            canvasControlService = new CanvasControlService(this);
            

            //===============DataGridView========

            DataContext = canvasControlService;
            stationTimer = new System.Timers.Timer();
            SetTimerInterval(stationTimer);
            stationTimer.Elapsed += OnTimedRedrawStationEvent;
            stationTimer.AutoReset = true;
        }

        public void SetTimerInterval(System.Timers.Timer timer)
        {
            bool backToWork = false;
            if (timer.Enabled)
            {
                backToWork = true;
                timer.Stop();
            }
            timer.Interval = Properties.Settings.Default.bufferRefreshInterval;
            if (backToWork)
            {
                timer.Start();
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
        }

        private void DevicesManagement_Closed(object sender, EventArgs e)
        {
            devicesManagement = null;
        }
        
        private void PlanControl_MVVM_Closed(object sender, EventArgs e)
        {
            planControl_MVVM = null;
        }

        private void UserManagement_Closed(object sender, EventArgs e)
        {
            userManagement = null;
        }
        

        private void LoginForm_Closed(object sender, EventArgs e)
        {
            loginForm = null;
        }

        private void ApplyLanguage(string cultureName = null)
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

            // check/uncheck the language menu items based on the current culture
            foreach (var item in languageMenuItem.Items)
            {
                MenuItem menuItem = item as MenuItem;
                if (menuItem.Tag.ToString() == Thread.CurrentThread.CurrentCulture.Name)
                {
                    menuItem.IsChecked = true;
                }
                else
                {
                    menuItem.IsChecked = false;
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = sender as MenuItem;
                ApplyLanguage(menuItem.Tag.ToString());
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void CenterWindowOnScreen()
        {
            try
            {
                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                double windowWidth = this.Width;
                double windowHeight = this.Height;
                this.Left = (screenWidth / 2) - (windowWidth / 2);
                this.Top = (screenHeight / 2) - (windowHeight / 2);
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CenterWindowOnScreen();
                myManagementWindow.Visibility = Visibility.Hidden;
                loginForm = new LoginForm(Thread.CurrentThread.CurrentCulture.ToString());
                loginForm.Closed += LoginForm_Closed;
                //stationTimer.Enabled = false;
                stationTimer.Stop();
                loginForm.ShowDialog();
                if (Global_Object.userAuthor <= 2)
                {
                    //stationTimer.Enabled = true;
                    stationTimer.Start();
                    myManagementWindow.Visibility = Visibility.Visible;
                    Dispatcher.BeginInvoke(new ThreadStart(() =>
                    {
                        canvasControlService.ReloadAllStation();
                    }));
                    tb_refreshBufferInterval.Text = Properties.Settings.Default["bufferRefreshInterval"].ToString();
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void OnTimedRedrawStationEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (Global_Object.ServerAlive())
                {
                    BackgroundWorker workerRedrawStation = new BackgroundWorker();
                    workerRedrawStation.DoWork += WorkerRedrawStation_DoWork;
                    workerRedrawStation.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
            finally
            {
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    lb_timerStatus.Content = "Timer is running";
                }));
            }
        }

        private void WorkerRedrawStation_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                canvasControlService.RedrawAllStation(canvasControlService.GetDataAllStation());
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public ImageBrush LoadImage(string name)
        {
            try
            {
                System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject(name);
                ImageBrush img = new ImageBrush();
                img.ImageSource = ImageSourceForBitmap(bmp);
                return img;
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
                return new ImageBrush();
            }
        }

        public ImageSource ImageSourceForBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { }
        }
        

        private void btn_ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangePassForm changePassForm = new ChangePassForm(Thread.CurrentThread.CurrentCulture.ToString());
                changePassForm.ShowDialog();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myManagementWindow.Visibility = Visibility.Hidden;

                Global_Object.userAuthor = -2;
                Global_Object.userLogin = -2;
                Global_Object.userName = "";

                loginForm = new LoginForm(Thread.CurrentThread.CurrentCulture.ToString());
                loginForm.Closed += LoginForm_Closed;
                //stationTimer.Enabled = false;
                stationTimer.Stop();
                loginForm.ShowDialog();
                if (Global_Object.userLogin <= 2)
                {
                    myManagementWindow.Visibility = Visibility.Visible;
                    Dispatcher.BeginInvoke(new ThreadStart(() =>
                    {
                        canvasControlService.ReloadAllStation();
                        //stationTimer.Enabled = true;
                        stationTimer.Start();
                    }));
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Reloadallstation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                canvasControlService.ReloadAllStation();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Btn_MapOnOff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (map.Background.Opacity == 0)
                {
                    map.Background.Opacity = 100;
                    return;
                }
                map.Background.Opacity = 0;
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Btn_PlanManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((planControl_MVVM == null))
                {
                    planControl_MVVM = new MVVM_PlanManagement();
                    planControl_MVVM.Closed += PlanControl_MVVM_Closed; ;
                    planControl_MVVM.Show();
                }
                else
                {
                    planControl_MVVM.Focus();
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Btn_OperationManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(new CheckAuthorityForm().ShowDialog() == true)
                {
                    if ((devicesManagement == null))
                    {
                        devicesManagement = new DevicesManagement(this, 0, Thread.CurrentThread.CurrentCulture.ToString());
                        devicesManagement.Closed += DevicesManagement_Closed;
                        devicesManagement.Show();
                    }
                    else
                    {
                        devicesManagement.ChangeTabIndex(0);
                        devicesManagement.Focus();
                    }
                }
                
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        

        private void Btn_UserManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new CheckAuthorityForm().ShowDialog() == true)
                {
                    if ((userManagement == null))
                    {
                        userManagement = new UserManagement(Thread.CurrentThread.CurrentCulture.ToString());
                        userManagement.Closed += UserManagement_Closed;
                        userManagement.Show();
                    }
                    else
                    {
                        userManagement.Focus();
                    }
                }
                   
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        

        private void MoveBuffer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new CheckAuthorityForm().ShowDialog() == true)
                {
                    drag = false;
                    Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT_MOVE_STATION;
                    Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION;
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void btn_AboutUs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                About frm = new About();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                BufferSettingForm bufferSettingForm = new BufferSettingForm(this);
                bufferSettingForm.Show();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Btn_applyRefreshInterval_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double interval = 2000;
                if (double.TryParse(tb_refreshBufferInterval.Text.ToString().Trim().Replace(" ", ""), out interval))
                {
                    if (interval.ToString().Trim() != "")
                    {
                        Properties.Settings.Default.bufferRefreshInterval = interval;
                    }
                }
                Properties.Settings.Default.Save();
                SetTimerInterval(stationTimer);
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
            finally
            {
                tb_refreshBufferInterval.Text = Properties.Settings.Default["bufferRefreshInterval"].ToString();
            }
        }

        private void Btn_stopRefreshInterval_Click(object sender, RoutedEventArgs e)
        {
            stationTimer.Stop();
            if (!stationTimer.Enabled)
            {
                lb_timerStatus.Content = "Timer is stopped";
            }
        }

        private void Btn_startRefreshInterval_Click(object sender, RoutedEventArgs e)
        {
            stationTimer.Start();
            if (stationTimer.Enabled)
            {
                lb_timerStatus.Content = "Timer is started";
            }
        }

        private void Btn_manualRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Global_Object.ServerAlive())
                {
                    BackgroundWorker workerRedrawStation = new BackgroundWorker();
                    workerRedrawStation.DoWork += WorkerRedrawStation_DoWork;
                    workerRedrawStation.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
            finally
            {
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    lb_timerStatus.Content = "Manual Refresh";
                }));
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F5)
            {
                try
                {
                    if (Global_Object.ServerAlive())
                    {
                        BackgroundWorker workerRedrawStation = new BackgroundWorker();
                        workerRedrawStation.DoWork += WorkerRedrawStation_DoWork;
                        workerRedrawStation.RunWorkerAsync();
                    }
                }
                catch (Exception ex)
                {
                    logFile.Error(ex.Message);
                }
                finally
                {
                    Dispatcher.BeginInvoke(new ThreadStart(() =>
                    {
                        lb_timerStatus.Content = "Manual Refresh";
                    }));
                }
            }
        }
    }
}