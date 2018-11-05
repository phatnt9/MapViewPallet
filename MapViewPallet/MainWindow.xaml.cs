using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        Point transform = new Point(10, 10);
        double angle = 50;
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

            double axisLenght = 30;
            Point X1 = new Point(0, 0);
            Point X2 = new Point(X1.X+ axisLenght, X1.Y);
            Point Y1 = new Point(0, 0);
            Point Y2 = new Point(Y1.X, Y1.Y + axisLenght);
            StraightPath xAxis = new StraightPath(map);
            xAxis.DrawAxis(Transformations(X1, transform, angle), Transformations(X2, transform, angle),Colors.Red);
            
            StraightPath yAxis = new StraightPath(map);
            yAxis.DrawAxis(Transformations(Y1, transform, angle), Transformations(Y2, transform, angle), Colors.Blue);

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
            double angle = Math.PI * degrees / 180.0;
            origin.X += transform.X;
            origin.Y += transform.Y;
            origin.X = origin.X * Math.Cos(angle) - origin.Y * Math.Sin(angle);
            origin.Y = origin.X * Math.Sin(angle) + origin.Y * Math.Cos(angle);
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
            Console.WriteLine(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            Console.WriteLine(Math.Atan2(yDiff, xDiff));

            CurvePath b0 = new CurvePath(map);
            b0.Draw(Transformations(pointArray[0], transform, angle), Transformations(pointArray[1], transform, angle), "up");

            StraightPath a0 = new StraightPath(map);
            a0.Draw(Transformations(pointArray[1], transform, angle), Transformations(pointArray[2], transform, angle));

            StraightPath a1 = new StraightPath(map);
            a1.Draw(Transformations(pointArray[2], transform, angle), Transformations(pointArray[3], transform, angle));
            StraightPath a2 = new StraightPath(map);
            a2.Draw(Transformations(pointArray[3], transform, angle), Transformations(pointArray[4], transform, angle));

            CurvePath b1 = new CurvePath(map);
            b1.Draw(Transformations(pointArray[4], transform, angle), Transformations(pointArray[5], transform, angle), "up");

            StraightPath a3 = new StraightPath(map);
            a3.Draw(Transformations(pointArray[5], transform, angle), Transformations(pointArray[6], transform, angle));
            StraightPath a4 = new StraightPath(map);
            a4.Draw(Transformations(pointArray[6], transform, angle), Transformations(pointArray[7], transform, angle));

            CurvePath b2 = new CurvePath(map);
            b2.Draw(Transformations(pointArray[7], transform, angle), Transformations(pointArray[8], transform, angle), "ups");

            StraightPath a5 = new StraightPath(map);
            a5.Draw(Transformations(pointArray[8], transform, angle), Transformations(pointArray[9], transform, angle));
            StraightPath a6 = new StraightPath(map);
            a6.Draw(Transformations(pointArray[9], transform, angle), Transformations(pointArray[10], transform, angle));
            StraightPath a7 = new StraightPath(map);
            a7.Draw(Transformations(pointArray[10], transform, angle), Transformations(pointArray[11], transform, angle));

            CurvePath b3 = new CurvePath(map);
            b3.Draw(Transformations(pointArray[11], transform, angle), Transformations(pointArray[12], transform, angle), "ups");

            StraightPath a8 = new StraightPath(map);
            a8.Draw(Transformations(pointArray[12], transform, angle), Transformations(pointArray[13], transform, angle));
            StraightPath a9 = new StraightPath(map);
            a9.Draw(Transformations(pointArray[13], transform, angle), Transformations(pointArray[0], transform, angle));
            //MessageBox.Show("ss");
            //a9.remove();
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

        }

        private void btn_HandDrawCurveDown_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
