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
            Bitmap bmp = Properties.Resources.mapbackground;
            ImageBrush img = new ImageBrush();
            img.ImageSource = ImageSourceForBitmap(bmp);
            map.Width = img.ImageSource.Width;
            map.Height = img.ImageSource.Height;
            map.Background = img;
            palletViewEventControl = new PalletViewControlService(this);
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

        //private void clipBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    double zoomDirection = e.Delta > 0 ? 1 : -1;
        //    slidingScale = 0.1 * zoomDirection;
        //    if (((canvasScaleTransform.ScaleY + slidingScale) >= 0.6) && ((canvasScaleTransform.ScaleY + slidingScale) <= 7))
        //    {
        //        canvasScaleTransform.ScaleX = canvasScaleTransform.ScaleY += slidingScale;
        //    }
        //}
        //private void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    string elementName = (e.OriginalSource as FrameworkElement).Name;
        //    Console.WriteLine(elementName);
        //    if (e.Source.ToString() == "System.Windows.Controls.Canvas")
        //    {
        //        map.CaptureMouse();
        //        startPoint = e.GetPosition(clipBorder);
        //        originalPoint = new System.Windows.Point(canvasTranslateTransform.X, canvasTranslateTransform.Y);
        //    }
        //    Statectrl_md(e);
        //    if (valstatectrl_mousedown == STATECTRL_MOUSEDOWN.STATECTRL_ADD_STATION)
        //    {
        //        var mouseWasDownOn = e.Source as FrameworkElement;
        //        System.Windows.Point pp = e.GetPosition(map);
        //        StationShape sts = null;
        //        sts = new StationShape("MIX0", 2, 7, "Pallet2");
        //        sts.Move(pp.X, pp.Y);
        //        map.Children.Add(sts);
        //    }
        //}
        //private void map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    map.ReleaseMouseCapture();
        //}
        //private void map_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (mouseMove)
        //    {
        //        if (!map.IsMouseCaptured) return;
        //        //RobotStore
        //        Vector moveVector = startPoint - e.GetPosition(clipBorder);
        //        double xCoor = originalPoint.X - moveVector.X;
        //        double yCoor = originalPoint.Y - moveVector.Y;
        //        //canvasTranslateTransform.X = originalPoint.X - moveVector.X;
        //        //canvasTranslateTransform.Y = originalPoint.Y - moveVector.Y;
        //        //Console.WriteLine(canvasTranslateTransform.X + ":" + canvasTranslateTransform.Y);
        //        //Console.WriteLine(canvasScaleTransform.ScaleX);
        //        //Console.WriteLine(clipBorder.ActualWidth + "-" + clipBorder.ActualHeight);
        //        //Console.WriteLine("/////////////////////////");
        //        double verticalLimimit = (((map.Height * canvasScaleTransform.ScaleX - clipBorder.ActualHeight) / 2) + 20);
        //        double horizontalLimit = (((map.Width * canvasScaleTransform.ScaleX - clipBorder.ActualWidth) / 2) + 20);
        //        //Console.WriteLine(verticalLimimit);
        //        if (((xCoor < horizontalLimit) && (xCoor > -horizontalLimit)))
        //        {
        //            canvasTranslateTransform.X = originalPoint.X - moveVector.X;
        //        }
        //        else
        //        {
        //            if (originalPoint.X > horizontalLimit)
        //            {
        //                canvasTranslateTransform.X = horizontalLimit;
        //            }
        //            if (originalPoint.X < -horizontalLimit)
        //            {
        //                canvasTranslateTransform.X = horizontalLimit;
        //            }
        //        }
        //        if (((yCoor < verticalLimimit) && (yCoor > -verticalLimimit)))
        //        {
        //            canvasTranslateTransform.Y = originalPoint.Y - moveVector.Y;
        //        }
        //        else
        //        {
        //            if (originalPoint.Y > verticalLimimit)
        //            {
        //                canvasTranslateTransform.Y = verticalLimimit;
        //            }
        //            if (originalPoint.Y < -verticalLimimit)
        //            {
        //                canvasTranslateTransform.Y = verticalLimimit;
        //            }
        //        }
        //    }
        //    if (!mouseMove)
        //    {
        //        Statectrl_mm(e);
        //    }
        //}
        //=================PROCESS===============
        //void Statectrl_md(MouseButtonEventArgs e)
        //{
        //    if (e.ClickCount == 2)
        //    {
        //        //EditObject();
        //    }
        //    System.Windows.Point pp = e.GetPosition(map);
        //    var mouseWasDownOn = e.Source as FrameworkElement;
        //    switch (valstatectrl_mousedown)
        //    {
        //        case STATECTRL_MOUSEDOWN.STATECTRL_KEEP_IN_OBJECT:
        //            if (mouseWasDownOn != null)
        //            {
        //                string elementName = mouseWasDownOn.Name;
        //                if (elementName != "")
        //                {
        //                    valstatectrl_mousemove = STATECTRL_MOUSEMOVE.STATECTRL_MOVE_STATION;
        //                    valstatectrl_mousedown = STATECTRL_MOUSEDOWN.STATECTRL_GET_OUT_OBJECT;
        //                }
        //            }
        //            break;
        //        default:
        //            {
        //                break;
        //            }
        //    }
        //}
        //void Statectrl_mm(MouseEventArgs e)
        //{
        //    System.Windows.Point pp = e.GetPosition(map);
        //    var mouseWasDownOn = e.Source as FrameworkElement;
        //    switch (valstatectrl_mousemove)
        //    {
        //        case STATECTRL_MOUSEMOVE.STATECTRL_MOVE_STATION:
        //            {
        //                //StationShape x = new StationShape();
        //                //x = (StationShape)map.Children[0];
        //                //x.Move(pp.X, pp.Y);
        //                break;
        //            }
        //        default:
        //            {
        //                break;
        //            }
        //    }
        //}
    }
}
