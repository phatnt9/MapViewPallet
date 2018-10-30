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
        private double slidingScale;
        private System.Windows.Point startPoint;
        private System.Windows.Point originalPoint;
        private bool mouseMove = true;
        public enum STATECTRL_MOUSEDOWN
        {
            STATECTRL_NORMAL,
            STATECTRL_ADD_STATION
        }
        public STATECTRL_MOUSEDOWN valstatectrl_md = STATECTRL_MOUSEDOWN.STATECTRL_NORMAL;

        public MainWindow()
        {
            InitializeComponent();
            Bitmap bmp = Properties.Resources.mapbackground;
            ImageBrush img = new ImageBrush();
            img.ImageSource = ImageSourceForBitmap(bmp);
            map.Background = img;
        }

        private void clipBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            double zoomDirection = e.Delta > 0 ? 1 : -1;
            slidingScale = 0.1 * zoomDirection;
            if (((canvasScaleTransform.ScaleY + slidingScale) >= 0.6) && ((canvasScaleTransform.ScaleY + slidingScale) <= 7))
            {
                canvasScaleTransform.ScaleX = canvasScaleTransform.ScaleY += slidingScale;
            }

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

        private void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.ToString() == "System.Windows.Controls.Canvas")
            {
                map.CaptureMouse();
                startPoint = e.GetPosition(clipBorder);
                originalPoint = new System.Windows.Point(canvasTranslateTransform.X, canvasTranslateTransform.Y);
            }
            if (valstatectrl_md == STATECTRL_MOUSEDOWN.STATECTRL_ADD_STATION)
            {
                var mouseWasDownOn = e.Source as FrameworkElement;
                System.Windows.Point pp = e.GetPosition(map);
                StationShape sts = null;
                sts = new StationShape("MIX0", 3, 6, "Pallet2");
                sts.Move(pp.X, pp.Y);
                map.Children.Add(sts);
            }
        }

        private void btn_AddRect_Click(object sender, RoutedEventArgs e)
        {
            if (valstatectrl_md == STATECTRL_MOUSEDOWN.STATECTRL_NORMAL)
            {
                mouseMove = false;
                valstatectrl_md = STATECTRL_MOUSEDOWN.STATECTRL_ADD_STATION;
            }
            else
            {
                mouseMove = true;
                valstatectrl_md = STATECTRL_MOUSEDOWN.STATECTRL_NORMAL;
            }

        }

        private void map_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseMove)
            {
                if (!map.IsMouseCaptured) return;
                //RobotStore

                Vector moveVector = startPoint - e.GetPosition(clipBorder);
                double xCoor = originalPoint.X - moveVector.X;
                double yCoor = originalPoint.Y - moveVector.Y;
                canvasTranslateTransform.X = originalPoint.X - moveVector.X;
                canvasTranslateTransform.Y = originalPoint.Y - moveVector.Y;
                Console.WriteLine(canvasTranslateTransform.X);
                Console.WriteLine(canvasTranslateTransform.Y);
                Console.WriteLine(canvasScaleTransform.ScaleX);
                Console.WriteLine(canvasScaleTransform.ScaleY);
                Console.WriteLine("/////////////////////////");
                //double verticalLimimit = (((map.Height - clipBorder.ActualHeight) / 2) + 20)* (canvasScaleTransform.ScaleX*3);
                //double horizontalLimit = (((map.Width - clipBorder.ActualWidth) / 2) + 20)* (canvasScaleTransform.ScaleY*3);
                ////Console.WriteLine(verticalLimimit);
                //if (((xCoor < horizontalLimit) && (xCoor > -horizontalLimit)))
                //{
                //    canvasTranslateTransform.X = originalPoint.X - moveVector.X;
                //}
                //else
                //{
                //    if (originalPoint.X > horizontalLimit)
                //    {
                //        canvasTranslateTransform.X = horizontalLimit;
                //    }
                //    if (originalPoint.X < -horizontalLimit)
                //    {
                //        canvasTranslateTransform.X = horizontalLimit;
                //    }
                //}
                //if (((yCoor < verticalLimimit) && (yCoor > -verticalLimimit)))
                //{
                //    canvasTranslateTransform.Y = originalPoint.Y - moveVector.Y;
                //}
                //else
                //{
                //    if (originalPoint.Y > verticalLimimit)
                //    {
                //        canvasTranslateTransform.Y = verticalLimimit;
                //    }
                //    if (originalPoint.Y < -verticalLimimit)
                //    {
                //        canvasTranslateTransform.Y = verticalLimimit;
                //    }
                //}



            }
        }

        private void map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            map.ReleaseMouseCapture();
        }
    }
}
