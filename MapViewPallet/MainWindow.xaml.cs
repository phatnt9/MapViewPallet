using MapViewPallet.Shape;
using System;
using System.IO;
using System.Windows;
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
        public bool drag = true;
        bool play = false;
        Point transform = new Point(0, 0);
        double angle = 0;
        private PalletViewControlService palletViewEventControl;
        System.Media.SoundPlayer snd;
        //=================METHOD==================


        public MainWindow()
        {
            InitializeComponent();
            //ImageBrush img = LoadImage("mapbackground");
            //map.Width = img.ImageSource.Width;
            //map.Height = img.ImageSource.Height;
            //map.Background = img;
            palletViewEventControl = new PalletViewControlService(this);
            btn_AddRect.Background = LoadImage("Pallet0");
            btn_moverect.Background = LoadImage("Pallet1");
            btn_normal.Background = LoadImage("Pallet2");
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
        
        private void btn_AddRect_Click(object sender, RoutedEventArgs e)
        {
            drag = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._ADD_STATION;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        }

        private void btn_moverect_Click(object sender, RoutedEventArgs e)
        {
            drag = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._SLIDE_OBJECT;
        }

        private void btn_normal_Click(object sender, RoutedEventArgs e)
        {
            drag = true;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._NORMAL;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
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
            
            //=================DRAW TEST====================
            Point[] pointArray = new Point[16];
            pointArray[0] = new Point(100, 100);
            pointArray[1] = new Point(150, 50);
            pointArray[2] = new Point(200, 50);
            pointArray[3] = new Point(250, 50);
            pointArray[4] = new Point(300, 50);
            pointArray[5] = new Point(350, 100);
            pointArray[6] = new Point(350, 150);
            pointArray[7] = new Point(350, 200);
            pointArray[8] = new Point(300, 250);
            pointArray[9] = new Point(250, 250);
            pointArray[10] = new Point(200, 250);
            pointArray[11] = new Point(150, 250);
            pointArray[12] = new Point(100, 200);
            pointArray[13] = new Point(100, 150);

            pointArray[14] = new Point(450, 50);
            pointArray[15] = new Point(400, 50);

            double xDiff = pointArray[8].X - pointArray[7].X;
            double yDiff = pointArray[8].Y - pointArray[7].Y;

            CurvePath b0 = new CurvePath(map, Transformations(pointArray[0], transform, angle), Transformations(pointArray[1], transform, angle),true);
            StraightPath a0 = new StraightPath(map, Transformations(pointArray[1], transform, angle), Transformations(pointArray[2], transform, angle));
            StraightPath a1 = new StraightPath(map, Transformations(pointArray[2], transform, angle), Transformations(pointArray[3], transform, angle));
            StraightPath a2 = new StraightPath(map, Transformations(pointArray[3], transform, angle), Transformations(pointArray[4], transform, angle));
            CurvePath b1 = new CurvePath(map, Transformations(pointArray[4], transform, angle), Transformations(pointArray[5], transform, angle), true);
            StraightPath a3 = new StraightPath(map, Transformations(pointArray[5], transform, angle), Transformations(pointArray[6], transform, angle));
            StraightPath a4 = new StraightPath(map, Transformations(pointArray[6], transform, angle), Transformations(pointArray[7], transform, angle));
            CurvePath b2 = new CurvePath(map, Transformations(pointArray[7], transform, angle), Transformations(pointArray[8], transform, angle), false);
            StraightPath a5 = new StraightPath(map, Transformations(pointArray[8], transform, angle), Transformations(pointArray[9], transform, angle));
            StraightPath a6 = new StraightPath(map, Transformations(pointArray[9], transform, angle), Transformations(pointArray[10], transform, angle));
            StraightPath a7 = new StraightPath(map, Transformations(pointArray[10], transform, angle), Transformations(pointArray[11], transform, angle));
            CurvePath b3 = new CurvePath(map, Transformations(pointArray[11], transform, angle), Transformations(pointArray[12], transform, angle), false);
            StraightPath a8 = new StraightPath(map, Transformations(pointArray[12], transform, angle), Transformations(pointArray[13], transform, angle));
            StraightPath a9 = new StraightPath(map, Transformations(pointArray[13], transform, angle), Transformations(pointArray[0], transform, angle));

            palletViewEventControl.list_Path.Add(b0.Name, b0);
            palletViewEventControl.list_Path.Add(b1.Name, b1);
            palletViewEventControl.list_Path.Add(b2.Name, b2);
            palletViewEventControl.list_Path.Add(b3.Name, b3);
            palletViewEventControl.list_Path.Add(a0.Name, a0);
            palletViewEventControl.list_Path.Add(a1.Name, a1);
            palletViewEventControl.list_Path.Add(a2.Name, a2);
            palletViewEventControl.list_Path.Add(a3.Name, a3);
            palletViewEventControl.list_Path.Add(a4.Name, a4);
            palletViewEventControl.list_Path.Add(a5.Name, a5);
            palletViewEventControl.list_Path.Add(a6.Name, a6);
            palletViewEventControl.list_Path.Add(a7.Name, a7);
            palletViewEventControl.list_Path.Add(a8.Name, a8);
            palletViewEventControl.list_Path.Add(a9.Name, a9);

        }
        

        private void btn_HandDrawStraight_Click(object sender, RoutedEventArgs e)
        {
            drag = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_STRAIGHT_P1;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        }
        

        private void btn_Warning_Click(object sender, RoutedEventArgs e)
        {
            if (!play)
            {
                Stream str = Properties.Resources.tennessee_whiskey;
                snd.Stream = str;
                snd.PlayLooping();
                play = true;
            }
            else
            {
                snd.Stop();
                play = false;
            }

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

        private void pathEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pathRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_JoinPath_Click(object sender, RoutedEventArgs e)
        {
            drag = false;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_P1;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
        }
    }
}
