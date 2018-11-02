using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private PalletViewControlService palletViewEventControl;
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
        }

        public ImageBrush LoadImage (string name)
        {
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(name);
            ImageBrush img = new ImageBrush();
            img.ImageSource = ImageSourceForBitmap(bmp);
            return img;
        }

        public ImageSource ImageSourceForBitmap(Bitmap bmp)
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

        private void btn_DrawStraght_Click(object sender, RoutedEventArgs e)
        {
            drag = true;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._DRAWING;


            //=================DRAW TEST====================
            System.Windows.Point[] pointArray = new System.Windows.Point[16];
            pointArray[0] = new System.Windows.Point(100, 100);
            pointArray[1] = new System.Windows.Point(150, 50);
            pointArray[2] = new System.Windows.Point(200, 50);
            pointArray[3] = new System.Windows.Point(250, 50);
            pointArray[4] = new System.Windows.Point(300, 50);
            pointArray[5] = new System.Windows.Point(350, 100);
            pointArray[6] = new System.Windows.Point(350, 150);
            pointArray[7] = new System.Windows.Point(350, 200);
            pointArray[8] = new System.Windows.Point(300, 250);
            pointArray[9] = new System.Windows.Point(250, 250);
            pointArray[10] = new System.Windows.Point(200, 250);
            pointArray[11] = new System.Windows.Point(150, 250);
            pointArray[12] = new System.Windows.Point(100, 200);
            pointArray[13] = new System.Windows.Point(100, 150);

            pointArray[14] = new System.Windows.Point(450, 50);
            pointArray[15] = new System.Windows.Point(400, 50);
            

            CurvePath b0 = new CurvePath(map);
            b0.Draw(pointArray[0], pointArray[1],"up");

            StraightPath a0 = new StraightPath(map);
            a0.Draw(pointArray[1], pointArray[2]);
            StraightPath a1 = new StraightPath(map);
            a1.Draw(pointArray[2], pointArray[3]);
            StraightPath a2 = new StraightPath(map);
            a2.Draw(pointArray[3], pointArray[4]);

            CurvePath b1 = new CurvePath(map);
            b1.Draw(pointArray[4], pointArray[5], "up");

            StraightPath a3 = new StraightPath(map);
            a3.Draw(pointArray[5], pointArray[6]);
            StraightPath a4 = new StraightPath(map);
            a4.Draw(pointArray[6], pointArray[7]);

            CurvePath b2 = new CurvePath(map);
            b2.Draw(pointArray[7], pointArray[8], "ups");

            StraightPath a5 = new StraightPath(map);
            a5.Draw(pointArray[8], pointArray[9]);
            StraightPath a6 = new StraightPath(map);
            a6.Draw(pointArray[9], pointArray[10]);
            StraightPath a7 = new StraightPath(map);
            a7.Draw(pointArray[10], pointArray[11]);

            CurvePath b3 = new CurvePath(map);
            b3.Draw(pointArray[11], pointArray[12], "ups");

            StraightPath a8 = new StraightPath(map);
            a8.Draw(pointArray[12], pointArray[13]);
            StraightPath a9 = new StraightPath(map);
            a9.Draw(pointArray[13], pointArray[0]);


            StraightPath ax = new StraightPath(map);
            ax.Draw(pointArray[14], pointArray[15]);
            //Arrow show direction
            System.Windows.Point middle = new System.Windows.Point();
            middle.X = (pointArray[14].X + pointArray[15].X) / 2;
            middle.Y = (pointArray[14].Y + pointArray[15].Y) / 2;
            Polygon arrow = new Polygon();
            arrow.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            arrow.Fill = new SolidColorBrush(Colors.Green);
            arrow.Stroke = new SolidColorBrush(Colors.Black);
            RotateTransform myRotateTransform = new RotateTransform();
            myRotateTransform.Angle = 20;
            TranslateTransform myTranslate = new TranslateTransform(0, 0);
            TransformGroup myTransformGroup = new TransformGroup();
            //myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslate);
            
            PointCollection points = new PointCollection(3)
            {
                new System.Windows.Point(middle.X - 2, middle.Y - 2),
                new System.Windows.Point(middle.X - 2, middle.Y + 2),
                new System.Windows.Point(middle.X + 2, middle.Y)
            };
            arrow.Points = points;
            arrow.RenderTransform = myTransformGroup;
            map.Children.Add(arrow);
        }

        private void btn_DrawCurve_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
