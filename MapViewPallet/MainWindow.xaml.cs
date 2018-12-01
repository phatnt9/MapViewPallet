﻿using MapViewPallet.DataGridView;
using MapViewPallet.MiniForm;
using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapViewPallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public class Trv_StationGroup
    {
        public Trv_StationGroup()
        {
            Items = new ObservableCollection<Trv_Station>();
            Title = "Stations";
            Icon = "pack://siteoforigin:,,,/Resources/Pallet.png";
        }
        public string Title { get; set; }
        public string Icon { get; set; }
        public ObservableCollection<Trv_Station> Items { get; set; }
    }
    public class Trv_Station
    {
        public Station station;

        public Trv_Station(Station pStation)
        {
            station = pStation;
            Name = station.Name;
            Icon = "pack://siteoforigin:,,,/Resources/Pallet.png";
        }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
    



    public partial class MainWindow : Window
    {
        //=================VARIABLE==================
        public Point renderTransformOrigin = new Point(0, 0);
        public bool drag = true;
        //bool play = false;
        MainWindowDataGridViewModel dgv_model;
        Trv_StationGroup stationGroup;
        List<dynamic> trvGroups;
        Point transform = new Point(0, 0);
        private CanvasControlService palletViewEventControl;
        System.Media.SoundPlayer snd;
        
        
        

        public MainWindow()
        {
            InitializeComponent();
            Robot rbot = new Robot(map);
            rbot.ReDraw(new Point(600, 400), -45);
            rbot.ChangeTask("22");
            //==============TreeView=============
            trvGroups = new List<dynamic>();
            stationGroup = new Trv_StationGroup();
            trvGroups.Add(stationGroup);
            mainTreeView.ItemsSource = trvGroups;
            //===================================
            canvasMatrixTransform = new MatrixTransform(1, 0, 0, -1, 0, 0);
            ImageBrush img = LoadImage("Map");
            map.Width = img.ImageSource.Width;
            map.Height = img.ImageSource.Height;
            map.Background = img;
            palletViewEventControl = new CanvasControlService(this, mainTreeView);
            snd = new System.Media.SoundPlayer();
            //===============DATAGRIDVIEW========
            dgv_model = new MainWindowDataGridViewModel();
            DataContext = dgv_model;
        }

        
        public ImageBrush LoadImage (string name)
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
        

        //private void btn_DrawStraight_Click(object sender, RoutedEventArgs e)
        //{
        //    drag = true;
        //    Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT;
        //    Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._DRAWING;
            
        //    ////=================DRAW TEST====================
        //    string name = "Sheet1";
        //    string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
        //                    "C:\\Users\\Charlie\\Desktop\\test.xls" +
        //                    ";Extended Properties='Excel 12.0 XML;HDR=YES;';";

        //    OleDbConnection con = new OleDbConnection(constr);
        //    OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
        //    con.Open();

        //    OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
        //    DataTable data = new DataTable();
        //    sda.Fill(data);
        //    foreach (DataRow row in data.Rows)
        //    {
        //        PathShape tempPath;
        //        double oriX = double.Parse(row.Field<string>("ORIGINAL").Split(',')[0]);
        //        double oriY = double.Parse(row.Field<string>("ORIGINAL").Split(',')[1]);
        //        Point ori = new Point(oriX, oriY);
        //        double desX = double.Parse(row.Field<string>("DESTINATION").Split(',')[0]);
        //        double desY = double.Parse(row.Field<string>("DESTINATION").Split(',')[1]);
        //        Point des = new Point(desX, desY);
                
        //        // ... Write value of first field as integer.
        //        if (row.Field<string>("TYPE") == "CurvePath")
        //        {
        //            bool Curve = bool.Parse(row.Field<string>("CURVE"));
        //            tempPath = new CurvePath(map, ori, des, Curve);

        //        }
        //        else
        //        {
        //            tempPath = new StraightPath(map, ori, des);
        //        }
        //        tempPath.RemoveHandle += palletViewEventControl.PathRemove;
        //        palletViewEventControl.list_Path.Add(tempPath.Name, tempPath);
        //    }

        //    //====================================
        //}

        
        

        //private void btn_Warning_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (!play)
        //    //{
        //    //    Stream str = Properties.Resources.tennessee_whiskey;
        //    //    snd.Stream = str;
        //    //    snd.PlayLooping();
        //    //    play = true;
        //    //}
        //    //else
        //    //{
        //    //    snd.Stop();
        //    //    play = false;
        //    //}

        //}

        //private void btn_HandDrawCurveUp_Click(object sender, RoutedEventArgs e)
        //{
        //    drag = false;
        //    Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_P1;
        //    Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        //}

        //private void btn_HandDrawCurveDown_Click(object sender, RoutedEventArgs e)
        //{
        //    drag = false;
        //    Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_P1;
        //    Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        //}
        
        public void LoadPath (string Path)
        {
            try
            {
                DataTable data = new DataTable();
                data = Global_Object.LoadExcelFile(Path);
                foreach (DataRow row in data.Rows)
                {
                    Shape.PathShape tempPath;
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
                        tempPath = new Curve(map, ori, des, Curve);
                    }
                    else
                    {
                        tempPath = new Straight(map, ori, des);
                    }
                    tempPath.RemoveHandle += palletViewEventControl.PathRemove;
                    palletViewEventControl.list_Path.Add(tempPath.Name, tempPath);
                    
                }
            }
            catch { }
        }

        public void LoadStation(string Path)
        {
            try
            {
                DataTable data = new DataTable();
                data = Global_Object.LoadExcelFile(Path);
                Console.WriteLine(data.Rows.Count);
                foreach (DataRow row in data.Rows)
                {
                    Station tempStation;
                    string stationName = row.Field<string>("NAME");
                    double oriX = double.Parse(row.Field<string>("POSITION").Split(',')[0]);
                    double oriY = double.Parse(row.Field<string>("POSITION").Split(',')[1]);
                    Point ori = Global_Object.CoorCanvas(new Point(oriX, oriY)); // Change Laser Metter to Canvas Position
                    int lines = int.Parse(row.Field<string>("LINES"));
                    int pallets = int.Parse(row.Field<string>("PALLETS"));
                    double rotate = double.Parse(row.Field<string>("ROTATE"));
                    tempStation = new Station(map, stationName, lines, pallets, rotate, "normal");
                    tempStation.ReDraw(ori);
                    tempStation.RemoveHandle += palletViewEventControl.StationRemove;
                    palletViewEventControl.list_Station.Add(tempStation.Name, tempStation);
                    stationGroup.Items.Add(new Trv_Station(tempStation));
                    dgv_model.AddItem(new DgvStation
                    {
                        Name = tempStation.Name,
                        Bays = tempStation.props.Bays,
                        Rows = tempStation.props.Rows,
                        Position = tempStation.props._posision,
                        Angle = tempStation.props._rotate
                    });
                }
            }
            catch
            {
            }
        }

        private void btn_LoadExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable data = new DataTable();
                data = Global_Object.LoadExcelFile();
                foreach (DataRow row in data.Rows)
                {
                    Shape.PathShape tempPath;
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
                        tempPath = new Curve(map, ori, des, Curve);
                    }
                    else
                    {
                        tempPath = new Straight(map, ori, des);
                    }
                    tempPath.RemoveHandle += palletViewEventControl.PathRemove;
                    palletViewEventControl.list_Path.Add(tempPath.Name, tempPath);
                }
            }
            catch { }
        }

        private void btn_LoadExcel2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable data = new DataTable();
                data = Global_Object.LoadExcelFile();
                foreach (DataRow row in data.Rows)
                {
                    Station tempStation;
                    string stationName = row.Field<string>("NAME");
                    double oriX = double.Parse(row.Field<string>("POSITION").Split(',')[0]);
                    double oriY = double.Parse(row.Field<string>("POSITION").Split(',')[1]);
                    Point ori = Global_Object.CoorCanvas(new Point(oriX, oriY)); // Change Laser Metter to Canvas Position
                    int lines = int.Parse(row.Field<string>("LINES"));
                    int pallets = int.Parse(row.Field<string>("PALLETS"));
                    double rotate = double.Parse(row.Field<string>("ROTATE"));
                    tempStation = new Station(map, stationName, lines, pallets, rotate, "normal");
                    tempStation.ReDraw(ori);
                    tempStation.RemoveHandle += palletViewEventControl.StationRemove;
                    palletViewEventControl.list_Station.Add(tempStation.Name, tempStation);
                    stationGroup.Items.Add(new Trv_Station(tempStation));
                }
            }
            catch { }
        }


        //private void btn_StraightPath_Click_on(object sender, RoutedEventArgs e)
        //{
        //    drag = false;
        //    //btn_JoinPath.IsChecked = false;
        //    Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_STRAIGHT_P1;
        //    Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        //}

        //private void btn_JoinPath_Click_on(object sender, RoutedEventArgs e)
        //{
        //    drag = false;
        //    //btn_StraightPath.IsChecked = false;
        //    Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_P1;
        //    Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        //}

        //private void btn_Normal_Click(object sender, RoutedEventArgs e)
        //{
        //    drag = true;
        //    Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        //    Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._NORMAL;
        //}

        private void ChangeMapSize(object sender, RoutedEventArgs e)
        {
            //MapResize resizeForm = new MapResize(map.Width, map.Height);
            //resizeForm.ResizeHandle += CanvasResizeHandle;
            //resizeForm.Show();
        }
        
        
        

        //private void btn_AddStation_Click(object sender, RoutedEventArgs e)
        //{
        //    drag = false;
        //    Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._ADD_STATION;
        //    Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        //}

        private void clearLog_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void autoScrollLog_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void mainTreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void StationEditMenu_Click(object sender, RoutedEventArgs e)
        {
            Trv_Station temp = ((sender as System.Windows.Controls.MenuItem).DataContext) as Trv_Station;
            temp.station.stationProperties.ShowDialog();
        }

        private void StationRemoveMenu_Click(object sender, RoutedEventArgs e)
        {
            Trv_Station temp = ((sender as System.Windows.Controls.MenuItem).DataContext) as Trv_Station;
            temp.station.Remove();
            stationGroup.Items.Remove(temp);
        }

        private void btn_LoadAll_Click(object sender, RoutedEventArgs e)
        {
            LoadPath(@"C:\Users\Phat\Desktop\seldat\Path.xls");
            LoadStation(@"C:\Users\Phat\Desktop\seldat\StationMain.xls");
        }
    }

}
