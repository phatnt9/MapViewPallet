using MapViewPallet.DataGridView;
using MapViewPallet.MiniForm;
using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapViewPallet.MiniForm.MicsWpfForm;
using System.IO;
using System.Media;

namespace MapViewPallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    



    public partial class MainWindow : Window
    {
        private SoundPlayer Player = null;
        //=================VARIABLE==================
        public System.Timers.Timer stationTimer;
        public bool drag = true;
        MainWindowModel mainWindowModel;
        public CanvasControlService canvasControlService;
        System.Media.SoundPlayer snd;

        public WaitServerForm waitServerForm;
        public PlanControl planControl;
        public DevicesManagement devicesManagement;
        public UserManagement userManagement;
        public Statistics statistics;

        public MainWindow()
        {
            InitializeComponent();
            ApplyLanguage();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            //==============TreeView=============
            //===================================
            canvasMatrixTransform = new MatrixTransform(1, 0, 0, -1, 0, 0);

            ImageBrush img = LoadImage("Map_aTan___Copy3");
            map.Width = img.ImageSource.Width;
            map.Height = img.ImageSource.Height;
            map.Background = img;

            canvasControlService = new CanvasControlService(this);

            snd = new System.Media.SoundPlayer();

            //===============DataGridView========

            mainWindowModel = new MainWindowModel(this);
            DataContext = canvasControlService;


            stationTimer = new System.Timers.Timer();
            stationTimer.Interval = 1000;
            stationTimer.Elapsed += OnTimedRedrawStationEvent;
            stationTimer.AutoReset = true;
            //stationTimer.Enabled = true;

            //robotTimer = new System.Timers.Timer();
            //robotTimer.Interval = 50;
            //robotTimer.Elapsed += OnTimedRedrawRobotEvent;
            //robotTimer.AutoReset = true;
            //robotTimer.Enabled = true;

            //Dispatcher.BeginInvoke(new ThreadStart(() =>
            //{
            //    for (int i = 1; i < 5; i++)
            //    {
            //        Random posX = new Random();
            //        RobotShape rbot = new RobotShape(map);
            //        rbot.rad = posX.Next(50, 120);
            //        rbot.org = new Point(600 + posX.Next(10, 50), 386 + posX.Next(10, 50));
            //        rbot.anglestep = posX.NextDouble() + 0.2;
            //        rbot.ReDraw(new Point(0, 0), 0);
            //        //rbot.ChangeTask("22");
            //        palletViewEventControl.list_Robot.Add(i.ToString(), rbot);
            //        Thread.Sleep(100);
            //    }
            //}));
        }

        private void PlayWav(Stream stream, bool play_looping)
        {
            // Stop the player if it is running.
            if (Player != null)
            {
                Player.Stop();
                Player.Dispose();
                Player = null;
            }

            // If we have no stream, we're done.
            if (stream == null) return;

            // Make the new player for the WAV stream.
            Player = new SoundPlayer(stream);

            // Play.
            if (play_looping)
                Player.PlayLooping();
            else
                Player.Play();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Global_Object.musicPlayerOld.StopPlaying();
            Global_Object.musicPlayerOld = null;
        }

        private void DevicesManagement_Closed(object sender, EventArgs e)
        {
            devicesManagement = null;
        }
        
        private void PlanControl_Closed(object sender, EventArgs e)
        {
            planControl = null;
        }

        private void UserManagement_Closed(object sender, EventArgs e)
        {
            userManagement = null;
        }
        
        private void Statistics_Closed(object sender, EventArgs e)
        {
            statistics = null;
        }

        private void ApplyLanguage(string cultureName = null)
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

            // check/uncheck the language menu items based on the current culture
            foreach (var item in languageMenuItem.Items)
            {
                MenuItem menuItem = item as MenuItem;
                if (menuItem.Tag.ToString() == Thread.CurrentThread.CurrentCulture.Name)
                    menuItem.IsChecked = true;
                else
                    menuItem.IsChecked = false;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            ApplyLanguage(menuItem.Tag.ToString());
        }


        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CenterWindowOnScreen();
            myManagementWindow.Visibility = Visibility.Hidden;
            LoginForm frm = new LoginForm(Thread.CurrentThread.CurrentCulture.ToString());
            frm.ShowDialog();
            if (Global_Object.userAuthor <= 2)
            {
                myManagementWindow.Visibility = Visibility.Visible;
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    canvasControlService.ReloadAllStation();
                    stationTimer.Enabled = true;
                }));
            }
        }

        private void OnTimedRedrawRobotEvent(object sender, ElapsedEventArgs e)
        {
            canvasControlService.RedrawAllRobot();
        }

        private void OnTimedRedrawStationEvent(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (waitServerForm == null)
                {
                    if (!Global_Object.ServerAlive())
                    {
                        if (true)
                        {
                            waitServerForm = new WaitServerForm(this);
                            waitServerForm.Closed += WaitServerForm_Closed;
                            waitServerForm.ShowDialog();
                        }
                        return;
                    }
                    canvasControlService.RedrawAllStation(canvasControlService.GetDataAllStation());
                }
            });

            //Dispatcher.BeginInvoke(new ThreadStart(() =>
            //{
            //    if (!Global_Object.ServerAlive())
            //    {
            //        if (waitServerForm == null)
            //        {
            //            if (CloseAllWindows())
            //            {
            //                waitServerForm = new WaitServerForm(this);
            //                waitServerForm.Closed += WaitServerForm_Closed;
            //                waitServerForm.ShowDialog();
            //            }

            //        }
            //        return;
            //    }
            //    canvasControlService.RedrawAllStation(canvasControlService.GetDataAllStation());
            //}));
        }
        
        private void WaitServerForm_Closed(object sender, EventArgs e)
        {
            waitServerForm = null;
            Console.WriteLine("WaitServerForm_Closed and "+ ((waitServerForm==null)?"it is null":"it is not null"));
        }

        public ImageBrush LoadImage(string name)
        {
            System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject(name);
            ImageBrush img = new ImageBrush();
            img.ImageSource = ImageSourceForBitmap(bmp);
            return img;
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

        public void LoadPath(string Path)
        {
            try
            {
                DataTable data = new DataTable();
                data = Global_Object.LoadExcelFile(Path);
                foreach (DataRow row in data.Rows)
                {
                    Shape.CanvasPath tempPath;
                    double oriX = double.Parse(row.Field<string>("ORIGINAL").Split(',')[0]);
                    double oriY = double.Parse(row.Field<string>("ORIGINAL").Split(',')[1]);
                    Point ori = Global_Object.CoorCanvas(new Point(oriX, oriY));

                    double desX = double.Parse(row.Field<string>("DESTINATION").Split(',')[0]);
                    double desY = double.Parse(row.Field<string>("DESTINATION").Split(',')[1]);
                    Point des = Global_Object.CoorCanvas(new Point(desX, desY));

                    // ... Write value of first field as integer.
                    if (row.Field<string>("TYPE") == "CurvePath")
                    {
                        bool Curve = bool.Parse(row.Field<string>("CURVE"));
                        tempPath = new CurveShape(map, ori, des, Curve);
                    }
                    else
                    {
                        tempPath = new StraightShape(map, ori, des);
                    }
                    tempPath.RemoveHandle += canvasControlService.PathRemove;
                    canvasControlService.list_Path.Add(tempPath.Name, tempPath);

                }
            }
            catch { }
        }


        private void btn_LoadExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable data = new DataTable();
                data = Global_Object.LoadExcelFile();
                foreach (DataRow row in data.Rows)
                {
                    Shape.CanvasPath tempPath;
                    double oriX = double.Parse(row.Field<string>("ORIGINAL").Split(',')[0]);
                    double oriY = double.Parse(row.Field<string>("ORIGINAL").Split(',')[1]);
                    Point ori = Global_Object.CoorCanvas(new Point(oriX, oriY));

                    double desX = double.Parse(row.Field<string>("DESTINATION").Split(',')[0]);
                    double desY = double.Parse(row.Field<string>("DESTINATION").Split(',')[1]);
                    Point des = Global_Object.CoorCanvas(new Point(desX, desY));

                    // ... Write value of first field as integer.
                    if (row.Field<string>("TYPE") == "CurvePath")
                    {
                        bool Curve = bool.Parse(row.Field<string>("CURVE"));
                        tempPath = new CurveShape(map, ori, des, Curve);
                    }
                    else
                    {
                        tempPath = new StraightShape(map, ori, des);
                    }
                    tempPath.RemoveHandle += canvasControlService.PathRemove;
                    canvasControlService.list_Path.Add(tempPath.Name, tempPath);
                }
            }
            catch { }
        }



        //private void clearLog_Clicked(object sender, RoutedEventArgs e)
        //{

        //}

        //private void autoScrollLog_Checked(object sender, RoutedEventArgs e)
        //{

        //}

        //private void StationPropertiesMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    TrvStation temp = ((sender as MenuItem).DataContext) as TrvStation;
        //    temp.station.stationProperties.ShowDialog();
        //}

        private void StationEditMenu_Click(object sender, RoutedEventArgs e)
        {
            //TrvStation temp = ((sender as System.Windows.Controls.MenuItem).DataContext) as TrvStation;
            //temp.station.Remove();
            //stationGroup.Items.Remove(temp);
        }

        private void btn_PlanControl_Click(object sender, RoutedEventArgs e)
        {
            if (planControl == null)
            {
                planControl = new PlanControl(Thread.CurrentThread.CurrentCulture.ToString());
                planControl.Closed += PlanControl_Closed;
                planControl.Show();
            }
            else
            {
                planControl.Focus();
            }
        }

        private void btn_DevicesManagement_Click(object sender, RoutedEventArgs e)
        {
            if (devicesManagement == null)
            {
                DevicesManagement devicesManagement = new DevicesManagement(this, 0, Thread.CurrentThread.CurrentCulture.ToString());
                devicesManagement.Closed += DevicesManagement_Closed;
                devicesManagement.Show();
            }
            else
            {
                devicesManagement.ChangeTabIndex(0);
                devicesManagement.Focus();
            }
        }

        private void btn_UsersManagement_Click(object sender, RoutedEventArgs e)
        {
            if (userManagement == null)
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

        

        private void btn_ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePassForm changePassForm = new ChangePassForm(Thread.CurrentThread.CurrentCulture.ToString());
            changePassForm.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            myManagementWindow.Visibility = Visibility.Hidden;

            Global_Object.userAuthor = -2;
            Global_Object.userLogin = -2;
            Global_Object.userName = "";

            LoginForm frm = new LoginForm(Thread.CurrentThread.CurrentCulture.ToString());
            frm.ShowDialog();
            if (Global_Object.userLogin <= 2)
            {
                myManagementWindow.Visibility = Visibility.Visible;
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    canvasControlService.ReloadAllStation();
                }));
            }
        }

        private void Reloadallstation_Click(object sender, RoutedEventArgs e)
        {
            canvasControlService.ReloadAllStation();
        }

        private void Btn_MapOnOff_Click(object sender, RoutedEventArgs e)
        {
            if (map.Background.Opacity == 0)
            {
                map.Background.Opacity = 100;
                return;
            }
            map.Background.Opacity = 0;
        }

        private void Btn_PlanManagement_Click(object sender, RoutedEventArgs e)
        {
            if (planControl == null)
            {
                planControl = new PlanControl(Thread.CurrentThread.CurrentCulture.ToString());
                planControl.Closed += PlanControl_Closed;
                planControl.Show();
            }
            else
            {
                planControl.Focus();
            }
        }

        private void Btn_OperationManagement_Click(object sender, RoutedEventArgs e)
        {
            if (devicesManagement == null)
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

        private void Btn_DeviceManagement_Click(object sender, RoutedEventArgs e)
        {
            if (devicesManagement == null)
            {
                devicesManagement = new DevicesManagement(this, 1, Thread.CurrentThread.CurrentCulture.ToString());
                devicesManagement.Closed += DevicesManagement_Closed;
                devicesManagement.Show();
            }
            else
            {
                devicesManagement.ChangeTabIndex(1);
                devicesManagement.Focus();
            }

        }

        private void Btn_ProductManagement_Click(object sender, RoutedEventArgs e)
        {
            if (devicesManagement == null)
            {
                devicesManagement = new DevicesManagement(this, 2, Thread.CurrentThread.CurrentCulture.ToString());
                devicesManagement.Closed += DevicesManagement_Closed;
                devicesManagement.Show();
            }
            else
            {
                devicesManagement.ChangeTabIndex(2);
                devicesManagement.Focus();
            }

        }

        private void Btn_BufferManagement_Click(object sender, RoutedEventArgs e)
        {
            if (devicesManagement == null)
            {
                devicesManagement = new DevicesManagement(this, 3, Thread.CurrentThread.CurrentCulture.ToString());
                devicesManagement.Closed += DevicesManagement_Closed;
                devicesManagement.Show();
            }
            else
            {
                devicesManagement.ChangeTabIndex(3);
                devicesManagement.Focus();
            }
        }

        private void Btn_UserManagement_Click(object sender, RoutedEventArgs e)
        {
            if (userManagement == null)
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

        private void Btn_MapReCenter_Click(object sender, RoutedEventArgs e)
        {
            canvasControlService.ReCenterMapCanvas();
        }

        private void Btn_Statistics_Click(object sender, RoutedEventArgs e)
        {
            if (statistics == null)
            {
                statistics = new Statistics(Thread.CurrentThread.CurrentCulture.ToString());
                statistics.Closed += Statistics_Closed;
                statistics.Show();
            }
            else
            {
                statistics.Focus();
            }
        }


        private void MoveBuffer_Click(object sender, RoutedEventArgs e)
        {
            drag = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT_MOVE_STATION;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION;
        }

        private void btn_AboutUs_Click(object sender, RoutedEventArgs e)
        {
            About frm = new About();
            frm.ShowDialog();
        }

        private void PlayMusic_Click(object sender, RoutedEventArgs e)
        {
            PlayWav(Properties.Resources.ALARM, true);
        }

        private void StopMusic_Click(object sender, RoutedEventArgs e)
        {
            //Global_Object.StopWarning();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }

}
//public void LoadStation()
//{
//    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "buffer/getListBuffer");
//    request.Method = "GET";
//    request.ContentType = @"application/json";
//    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
//    using (Stream responseStream = response.GetResponseStream())
//    {
//        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
//        string result = reader.ReadToEnd();

//        DataTable buffers = JsonConvert.DeserializeObject<DataTable>(result);

//        foreach (DataRow dr in buffers.Rows)
//        {
//            dtBuffer tempBuffer = new dtBuffer
//            {
//                creUsrId = int.Parse(dr["creUsrId"].ToString()),
//                creDt = dr["creDt"].ToString(),
//                updUsrId = int.Parse(dr["updUsrId"].ToString()),
//                updDt = dr["updDt"].ToString(),
//                bufferId = int.Parse(dr["bufferId"].ToString()),
//                bufferName = dr["bufferName"].ToString(),
//                maxBay = int.Parse(dr["maxBay"].ToString()),
//                maxBayOld = int.Parse(dr["maxBayOld"].ToString()),
//                maxRow = int.Parse(dr["maxRow"].ToString()),
//                maxRowOld = int.Parse(dr["maxRowOld"].ToString()),
//                bufferCheckIn = dr["bufferCheckIn"].ToString(),
//                bufferNameOld = dr["bufferNameOld"].ToString(),
//                bufferData = dr["bufferData"].ToString(),
//                //bufferData = bufferData[i],
//                bufferReturn = bool.Parse(dr["bufferReturn"].ToString()),
//                bufferReturnOld = bool.Parse(dr["bufferReturnOld"].ToString()),
//            };
//            if (!canvasControlService.list_Station.ContainsKey(tempBuffer.bufferId.ToString()))
//            {
//                StationShape tempStation = new StationShape(map, tempBuffer);

//                tempStation.ReDraw();
//                //tempStation.RemoveHandle += palletViewEventControl.StationRemove;
//                //palletViewEventControl.list_Station.Add(tempStation.props.bufferDb.bufferName.ToString().Replace(" ",""), tempStation);
//                canvasControlService.list_Station.Add(tempStation.props.bufferDb.bufferName.ToString().Trim(), tempStation);
//                stationGroup.Items.Add(new TrvStation(tempStation));

//                mainWindowModel.AddItem(new DgvStation
//                {
//                    Name = tempStation.Name,
//                    Bays = tempStation.props.bufferDb.maxBay,
//                    Rows = tempStation.props.bufferDb.maxRow,
//                    Position = tempStation.props._posision,
//                    Angle = tempStation.props._rotate
//                });
//            }

//        }
//    }
//}

//private void btn_LoadAll_Click(object sender, RoutedEventArgs e)
//{
//    canvasControlService.ReloadAllStation();
//    //LoadPath(@"C:\Users\LI\Desktop\Path.xls");
//    //LoadStation(@"C:\Users\LI\Desktop\StationMain.xls");
//    string fileName1 = "Path.xls";
//    //string fileName2 = "StationMain.xls";
//    string path1 = Path.Combine(Environment.CurrentDirectory, @"Excels\", fileName1);
//    //string path2 = Path.Combine(Environment.CurrentDirectory, @"Excels\", fileName2);
//    LoadPath(path1);
//    //LoadStation(path2);
//}

//private void StationsDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
//{

//}

//private void StationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
//{
//}

//private void StationsDataGrid_GotFocus(object sender, RoutedEventArgs e)
//{
//    DgvStation temp = (sender as DataGrid).CurrentCell.Item as DgvStation;
//    if (temp != null)
//    {
//        Console.WriteLine("In: " + temp.Name + "-" + temp.Bays + "-" + temp.Rows);
//        palletViewEventControl.list_Station[temp.Name].SelectedStyle();
//        previousStationNameIdDgv = temp.Name;
//    }
//    else
//    {

//    }
//}

//private void StationsDataGrid_LostFocus(object sender, RoutedEventArgs e)
//{
//    DgvStation temp = (sender as DataGrid).CurrentCell.Item as DgvStation;
//    if (temp != null)
//    {
//        Console.WriteLine("Out: " + temp.Name + "-" + temp.Bays + "-" + temp.Rows);
//        palletViewEventControl.list_Station[temp.Name].DeselectedStyle();
//    }
//    else
//    {
//        if (previousStationNameIdDgv != "")
//        {
//            palletViewEventControl.list_Station[previousStationNameIdDgv].DeselectedStyle();
//        }
//    }
//}

//private void mainTreeView_GotFocus(object sender, RoutedEventArgs e)
//{
//    TrvStation temp = mainTreeView.SelectedItem as TrvStation;
//    if (temp != null)
//    {
//        //Console.WriteLine("In: " + temp.Name + "-" + temp.station.props.Bays + "-" + temp.station.props.Rows);
//        palletViewEventControl.list_Station[temp.Name].SelectedStyle();
//        previousStationNameIdTrv = temp.Name;
//    }
//    else
//    {

//    }

//}

//private void mainTreeView_LostFocus(object sender, RoutedEventArgs e)
//{
//    TrvStation temp = mainTreeView.SelectedItem as TrvStation;
//    if (temp != null)
//    {
//        //Console.WriteLine("Out: " + temp.Name + "-" + temp.station.props.Bays + "-" + temp.station.props.Rows);
//        palletViewEventControl.list_Station[temp.Name].DeselectedStyle();
//    }
//    else
//    {
//        if (previousStationNameIdTrv != "")
//        {
//            palletViewEventControl.list_Station[previousStationNameIdTrv].DeselectedStyle();
//        }
//    }
//}

//public class TrvStationGroup
//{
//    public TrvStationGroup()
//    {
//        Items = new ObservableCollection<TrvStation>();
//        Title = "Stations";
//        Icon = "pack://siteoforigin:,,,/Resources/Group.png";
//    }
//    public string Title { get; set; }
//    public string Icon { get; set; }
//    public ObservableCollection<TrvStation> Items { get; set; }
//}
//public class TrvStation
//{
//    public StationShape station;
//    public TrvStation(StationShape pStation)
//    {
//        station = pStation;
//        Name = station.Name;
//        Icon = "pack://siteoforigin:,,,/Resources/Pallet.png";
//    }

//    public string Name { get; set; }
//    public string Icon { get; set; }
//}

//public System.Timers.Timer robotTimer;
//bool play = false;
//public Point renderTransformOrigin = new Point(0, 0);
//string previousStationNameIdDgv;
//string previousStationNameIdTrv;
//Tree View
//TrvStationGroup stationGroup;
//List<dynamic> trvGroups;
//Point transform = new Point(0, 0);