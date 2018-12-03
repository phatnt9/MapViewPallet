using MapViewPallet.DataGridView;
using MapViewPallet.MiniForm;
using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapViewPallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public class TrvStationGroup
    {
        public TrvStationGroup()
        {
            Items = new ObservableCollection<TrvStation>();
            Title = "Stations";
            Icon = "pack://siteoforigin:,,,/Resources/Group.png";
        }
        public string Title { get; set; }
        public string Icon { get; set; }
        public ObservableCollection<TrvStation> Items { get; set; }
    }
    public class TrvStation
    {
        public StationShape station;
        public TrvStation(StationShape pStation)
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
        //bool play = false;
        public Point renderTransformOrigin = new Point(0, 0);
        public bool drag = true;
        //Data Grid View
        DgvModel dgv_model;
        string previousStationNameIdDgv;
        string previousStationNameIdTrv;
        //Tree View
        TrvStationGroup stationGroup;
        List<dynamic> trvGroups;
        Point transform = new Point(0, 0);
        private CanvasControlService palletViewEventControl;
        System.Media.SoundPlayer snd;
        OperationControl GiaoDienLapLich;
        
        

        public MainWindow()
        {
            InitializeComponent();
            RobotShape rbot = new RobotShape(map);
            rbot.ReDraw(new Point(600, 400), -45);
            rbot.ChangeTask("22");
            //==============TreeView=============
            trvGroups = new List<dynamic>();
            stationGroup = new TrvStationGroup();
            trvGroups.Add(stationGroup);
            mainTreeView.ItemsSource = trvGroups;
            mainTreeView.GotFocus += mainTreeView_GotFocus;
            mainTreeView.LostFocus += mainTreeView_LostFocus;
            //===================================
            canvasMatrixTransform = new MatrixTransform(1, 0, 0, -1, 0, 0);
            ImageBrush img = LoadImage("Map");
            map.Width = img.ImageSource.Width;
            map.Height = img.ImageSource.Height;
            map.Background = img;
            palletViewEventControl = new CanvasControlService(this, mainTreeView);
            snd = new System.Media.SoundPlayer();
            GiaoDienLapLich = new OperationControl();
            //===============DataGridView========
            StationsDataGrid.CanUserAddRows = false;
            StationsDataGrid.CanUserDeleteRows = false;
            StationsDataGrid.CanUserReorderColumns = false;
            StationsDataGrid.CanUserResizeColumns = false;
            StationsDataGrid.CanUserResizeRows = false;
            StationsDataGrid.SelectionMode = DataGridSelectionMode.Single;
            StationsDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            StationsDataGrid.SelectedCellsChanged += StationsDataGrid_SelectedCellsChanged;
            StationsDataGrid.SelectionChanged += StationsDataGrid_SelectionChanged;
            
            StationsDataGrid.GotFocus += StationsDataGrid_GotFocus;
            StationsDataGrid.LostFocus += StationsDataGrid_LostFocus;
            dgv_model = new DgvModel();
            previousStationNameIdDgv = "";
            previousStationNameIdTrv = "";
            DataContext = dgv_model;
        }
        
        private void testinout(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("test in out");
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
        
        public void LoadPath (string Path)
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
                foreach (DataRow row in data.Rows)
                {
                    StationShape tempStation;
                    string stationName = row.Field<string>("NAME");
                    double oriX = double.Parse(row.Field<string>("POSITION").Split(',')[0]);
                    double oriY = double.Parse(row.Field<string>("POSITION").Split(',')[1]);
                    Point ori = Global_Object.CoorCanvas(new Point(oriX, oriY)); // Change Laser Metter to Canvas Position
                    int lines = int.Parse(row.Field<string>("LINES"));
                    int pallets = int.Parse(row.Field<string>("PALLETS"));
                    double rotate = double.Parse(row.Field<string>("ROTATE"));
                    tempStation = new StationShape(map, stationName, lines, pallets, rotate);
                    tempStation.ReDraw(ori);
                    tempStation.RemoveHandle += palletViewEventControl.StationRemove;
                    palletViewEventControl.list_Station.Add(tempStation.Name, tempStation);
                    stationGroup.Items.Add(new TrvStation(tempStation));
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
                    StationShape tempStation;
                    string stationName = row.Field<string>("NAME");
                    double oriX = double.Parse(row.Field<string>("POSITION").Split(',')[0]);
                    double oriY = double.Parse(row.Field<string>("POSITION").Split(',')[1]);
                    Point ori = Global_Object.CoorCanvas(new Point(oriX, oriY)); // Change Laser Metter to Canvas Position
                    int lines = int.Parse(row.Field<string>("LINES"));
                    int pallets = int.Parse(row.Field<string>("PALLETS"));
                    double rotate = double.Parse(row.Field<string>("ROTATE"));
                    tempStation = new StationShape(map, stationName, lines, pallets, rotate);
                    tempStation.ReDraw(ori);
                    tempStation.RemoveHandle += palletViewEventControl.StationRemove;
                    palletViewEventControl.list_Station.Add(tempStation.Name, tempStation);
                    stationGroup.Items.Add(new TrvStation(tempStation));
                }
            }
            catch { }
        }
        

        private void ChangeMapSize(object sender, RoutedEventArgs e)
        {
            //MapResize resizeForm = new MapResize(map.Width, map.Height);
            //resizeForm.ResizeHandle += CanvasResizeHandle;
            //resizeForm.Show();
        }
        
        
        

        

        private void clearLog_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void autoScrollLog_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void mainTreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void StationPropertiesMenu_Click(object sender, RoutedEventArgs e)
        {
            TrvStation temp = ((sender as MenuItem).DataContext) as TrvStation;
            temp.station.stationProperties.ShowDialog();
        }

        private void StationEditMenu_Click(object sender, RoutedEventArgs e)
        {
            //TrvStation temp = ((sender as System.Windows.Controls.MenuItem).DataContext) as TrvStation;
            //temp.station.Remove();
            //stationGroup.Items.Remove(temp);
        }

        private void btn_LoadAll_Click(object sender, RoutedEventArgs e)
        {
            //LoadPath(@"C:\Users\LI\Desktop\Path.xls");
            //LoadStation(@"C:\Users\LI\Desktop\StationMain.xls");
            string fileName1 = "Path.xls";
            string fileName2 = "StationMain.xls";
            string path1 = Path.Combine(Environment.CurrentDirectory, @"Excels\", fileName1);
            string path2 = Path.Combine(Environment.CurrentDirectory, @"Excels\", fileName2);
            LoadPath(path1);
            LoadStation(path2);
        }

        private void StationsDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
        }

        private void StationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void StationsDataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            DgvStation temp = (sender as DataGrid).CurrentCell.Item as DgvStation;
            if (temp != null)
            {
                Console.WriteLine("In: " + temp.Name + "-" + temp.Bays + "-" + temp.Rows);
                palletViewEventControl.list_Station[temp.Name].SelectedStyle();
                previousStationNameIdDgv = temp.Name;
            }
            else
            {

            }
        }

        private void StationsDataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            DgvStation temp = (sender as DataGrid).CurrentCell.Item as DgvStation;
            if (temp!=null)
            {
                Console.WriteLine("Out: " + temp.Name + "-" + temp.Bays + "-" + temp.Rows);
                palletViewEventControl.list_Station[temp.Name].DeselectedStyle();
            }
            else
            {
                if (previousStationNameIdDgv != "")
                {
                    palletViewEventControl.list_Station[previousStationNameIdDgv].DeselectedStyle();
                }
            }
        }
        
        private void mainTreeView_GotFocus(object sender, RoutedEventArgs e)
        {
            TrvStation temp = mainTreeView.SelectedItem as TrvStation;
            if (temp != null)
            {
                Console.WriteLine("In: " + temp.Name + "-" + temp.station.props.Bays + "-" + temp.station.props.Rows);
                palletViewEventControl.list_Station[temp.Name].SelectedStyle();
                previousStationNameIdTrv = temp.Name;
                temp.station.ChangeStatus(Global_Object.StationState.Error);
            }
            else
            {

            }

        }

        private void mainTreeView_LostFocus(object sender, RoutedEventArgs e)
        {
            TrvStation temp = mainTreeView.SelectedItem as TrvStation;
            if (temp != null)
            {
                Console.WriteLine("Out: " + temp.Name + "-" + temp.station.props.Bays + "-" + temp.station.props.Rows);
                palletViewEventControl.list_Station[temp.Name].DeselectedStyle();
            }
            else
            {
                if (previousStationNameIdTrv != "")
                {
                    palletViewEventControl.list_Station[previousStationNameIdTrv].DeselectedStyle();
                }
            }
        }

        private void btn_LapLich_Click(object sender, RoutedEventArgs e)
        {
            GiaoDienLapLich.Show();
            
        }
    }

}
