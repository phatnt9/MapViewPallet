using MapViewPallet.MiniForm;
using MapViewPallet.Shape;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class MainWindow : Window
    {
        //=================VARIABLE==================
        public Point renderTransformOrigin = new Point(0, 0);

        public bool drag = true;
        bool play = false;
        Point transform = new Point(0, 0);
        private PalletViewControlService palletViewEventControl;
        System.Media.SoundPlayer snd;
        //public event Action<double, double> LoadMapSizeHandle;
        //=================METHOD==================


        public MainWindow()
        {
            InitializeComponent();
            canvasMatrixTransform = new MatrixTransform(1, 0, 0, -1, 0, 0);
            ImageBrush img = LoadImage("Map");
            
            map.Width = img.ImageSource.Width;
            map.Height = img.ImageSource.Height;
            map.Background = img;
            palletViewEventControl = new PalletViewControlService(this);
            //btn_AddRect.Background = LoadImage("Pallet0");
            //btn_moverect.Background = LoadImage("Pallet1");
            //btn_normal.Background = LoadImage("Pallet2");
            snd = new System.Media.SoundPlayer();
            

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
        

        public Point Transformations(Point origin, Point transform, double degrees)
        {
            //double angle = Math.PI * degrees / 180.0;
            //origin.X += transform.X;
            //origin.Y += transform.Y;
            //origin.X = origin.X * Math.Cos(angle) - origin.Y * Math.Sin(angle);
            //origin.Y = origin.X * Math.Sin(angle) + origin.Y * Math.Cos(angle);
            return origin;
        }

        private void btn_DrawStraight_Click(object sender, RoutedEventArgs e)
        {
            drag = true;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._DRAWING;
            
            ////=================DRAW TEST====================
            string name = "Sheet1";
            string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            "C:\\Users\\Charlie\\Desktop\\test.xls" +
                            ";Extended Properties='Excel 12.0 XML;HDR=YES;';";

            OleDbConnection con = new OleDbConnection(constr);
            OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
            con.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            DataTable data = new DataTable();
            sda.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                PathShape tempPath;
                double oriX = double.Parse(row.Field<string>("ORIGINAL").Split(',')[0]);
                double oriY = double.Parse(row.Field<string>("ORIGINAL").Split(',')[1]);
                Point ori = new Point(oriX, oriY);
                double desX = double.Parse(row.Field<string>("DESTINATION").Split(',')[0]);
                double desY = double.Parse(row.Field<string>("DESTINATION").Split(',')[1]);
                Point des = new Point(desX, desY);
                
                // ... Write value of first field as integer.
                if (row.Field<string>("TYPE") == "CurvePath")
                {
                    bool Curve = bool.Parse(row.Field<string>("CURVE"));
                    tempPath = new CurvePath(map, ori, des, Curve);

                }
                else
                {
                    tempPath = new StraightPath(map, ori, des);
                }
                tempPath.RemoveHandle += palletViewEventControl.PathRemove;
                palletViewEventControl.list_Path.Add(tempPath.Name, tempPath);
            }

            //====================================
        }

        
        

        private void btn_Warning_Click(object sender, RoutedEventArgs e)
        {
            //if (!play)
            //{
            //    Stream str = Properties.Resources.tennessee_whiskey;
            //    snd.Stream = str;
            //    snd.PlayLooping();
            //    play = true;
            //}
            //else
            //{
            //    snd.Stop();
            //    play = false;
            //}

        }

        private void btn_HandDrawCurveUp_Click(object sender, RoutedEventArgs e)
        {
            drag = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_P1;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        }

        private void btn_HandDrawCurveDown_Click(object sender, RoutedEventArgs e)
        {
            drag = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_P1;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        }
        
        

        private void btn_LoadExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable data = new DataTable();
                data = Global_Object.LoadExcelFile();
                foreach (DataRow row in data.Rows)
                {
                    PathShape tempPath;
                    double oriX = double.Parse(row.Field<string>("ORIGINAL").Split(',')[0]) / Global_Object.resolution;
                    double oriY = double.Parse(row.Field<string>("ORIGINAL").Split(',')[1]) / Global_Object.resolution;
                    Point ori = Global_Object.CoorCanvas(new Point(oriX, oriY));

                    double desX = double.Parse(row.Field<string>("DESTINATION").Split(',')[0]) / Global_Object.resolution;
                    double desY = double.Parse(row.Field<string>("DESTINATION").Split(',')[1]) / Global_Object.resolution;
                    Point des = Global_Object.CoorCanvas(new Point(desX, desY));

                    // ... Write value of first field as integer.
                    if (row.Field<string>("TYPE") == "CurvePath")
                    {
                        bool Curve = bool.Parse(row.Field<string>("CURVE"));
                        tempPath = new CurvePath(map, ori, des, Curve);
                    }
                    else
                    {
                        tempPath = new StraightPath(map, ori, des);
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
                    double oriX = double.Parse(row.Field<string>("POSITION").Split(',')[0]) / Global_Object.resolution;
                    double oriY = double.Parse(row.Field<string>("POSITION").Split(',')[1]) / Global_Object.resolution;
                    Point ori = Global_Object.CoorCanvas(new Point(oriX, oriY));

                    int lines = int.Parse(row.Field<string>("LINES"));
                    int pallets = int.Parse(row.Field<string>("PALLETS"));
                    double rotate = double.Parse(row.Field<string>("ROTATE"));
                    tempStation = new StationShape(map, "acb", lines, pallets, rotate, "Pallet2");
                    // ... Write value of first field as integer.
                    tempStation.ReDraw(ori);
                    tempStation.RemoveHandle += palletViewEventControl.StationRemove;
                    palletViewEventControl.list_Station.Add(tempStation.Name, tempStation);
                }
            }
            catch { }
        }


        private void btn_StraightPath_Click_on(object sender, RoutedEventArgs e)
        {
            drag = false;
            //btn_JoinPath.IsChecked = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_STRAIGHT_P1;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        }

        private void btn_JoinPath_Click_on(object sender, RoutedEventArgs e)
        {
            drag = false;
            //btn_StraightPath.IsChecked = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_P1;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        }

        private void btn_Normal_Click(object sender, RoutedEventArgs e)
        {
            drag = true;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._NORMAL;
        }

        private void ChangeMapSize(object sender, RoutedEventArgs e)
        {
            //MapResize resizeForm = new MapResize(map.Width, map.Height);
            //resizeForm.ResizeHandle += CanvasResizeHandle;
            //resizeForm.Show();
        }

        private void CanvasResizeHandle(double Width, double Height)
        {
            if (map.Width < Width)
            {
                map.Width = Width;
            }
            if (map.Height < Height)
            {
                map.Height = Height;
            }
        }
        

        private void btn_ExtendMap_Click(object sender, RoutedEventArgs e)
        {
            //map.Width += 100;
            //map.Height += 100;
        }

        private void btn_ShrinkMap_Click(object sender, RoutedEventArgs e)
        {
            //if ((map.Width - 100) > 1)
            //{
            //    map.Width -= 100;
            //}
            //if ((map.Height - 100) > 1)
            //{
            //    map.Height -= 100;
            //}
        }

        private void btn_AddStation_Click(object sender, RoutedEventArgs e)
        {
            drag = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._ADD_STATION;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        }

        


    }

}
