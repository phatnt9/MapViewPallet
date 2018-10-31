using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MapViewPallet.Shape
{
    class PalletViewControlService
    {
        //=================VARIABLE==================
        private MainWindow mainWindow;
        private Canvas map;
        private ScaleTransform scaleTransform;
        private TranslateTransform translateTransform;
        private System.Windows.Point startPoint;
        private System.Windows.Point originalPoint;
        private double zoomInLitmit = 7;
        private double zoomOutLimit;
        private double slidingScale;
        private double verticalLimimit;
        private double horizontalLimit;
        public PalletViewControlService(MainWindow mainWinDowIn)
        {
            mainWindow = mainWinDowIn;
            map = mainWindow.map;
            scaleTransform = mainWindow.canvasScaleTransform;
            translateTransform = mainWindow.canvasTranslateTransform;
            //==========EVENT==========
            map.MouseWheel += Map_Zoom;
            map.MouseMove += Map_MouseMove;
            map.MouseLeftButtonDown += Map_MouseLeftButtonDown;
            map.MouseLeftButtonUp += Map_MouseLeftButtonUp;
            mainWindow.clipBorder.SizeChanged += ClipBorder_SizeChanged;
        }

        private void ClipBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            zoomOutLimit = mainWindow.clipBorder.ActualHeight / map.Height;

        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (mainWindow.drag)
            {
                if (!map.IsMouseCaptured) return;
                Vector moveVector = startPoint - e.GetPosition(mainWindow.clipBorder);
                double xCoor = originalPoint.X - moveVector.X;
                double yCoor = originalPoint.Y - moveVector.Y;
                double ScaleX = scaleTransform.ScaleX;
                double ScaleY = scaleTransform.ScaleY;
                double ActualHeight = mainWindow.clipBorder.ActualHeight;
                double ActualWidth = mainWindow.clipBorder.ActualWidth;
                //verticalLimimit = ((map.Height * ScaleX - ActualHeight) / 2) + 10;
                //horizontalLimit = ((map.Width * ScaleY - ActualWidth) / 2) + 10;
                translateTransform.X = xCoor;
                translateTransform.Y = yCoor;
                //if (((xCoor < horizontalLimit) && (xCoor > -horizontalLimit)))
                //{
                //    translateTransform.X = xCoor;
                //}
                //else
                //{
                //    if (originalPoint.X > horizontalLimit)
                //    {
                //        translateTransform.X = horizontalLimit;
                //    }
                //    if (originalPoint.X < -horizontalLimit)
                //    {
                //        translateTransform.X = -horizontalLimit;
                //    }
                //}
                //if (((yCoor < verticalLimimit) && (yCoor > -verticalLimimit)))
                //{
                //    translateTransform.Y = yCoor;
                //}
                //else
                //{
                //    if (originalPoint.Y > verticalLimimit)
                //    {
                //        translateTransform.Y = verticalLimimit;
                //    }
                //    if (originalPoint.Y < -verticalLimimit)
                //    {
                //        translateTransform.Y = -verticalLimimit;
                //    }
                //}
                int ySign = (Math.Sign(translateTransform.Y));
                int xSign = (Math.Sign(translateTransform.X));
                double yDistance = ((mainWindow.clipBorder.ActualHeight / 2 + Math.Abs(translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
                double xDistance = ((mainWindow.clipBorder.ActualWidth / 2 + Math.Abs(translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));

                if (yDistance > 0)
                {
                    if (ySign > 0)
                        translateTransform.Y = translateTransform.Y - yDistance;
                    else
                        translateTransform.Y = translateTransform.Y + yDistance;
                }
                if (xDistance > 0)
                {
                    if (xSign > 0)
                        translateTransform.X = translateTransform.X - xDistance;
                    else
                        translateTransform.X = translateTransform.X + xDistance;
                }
            }
            if (!mainWindow.drag)
            {
                Statectrl_MouseMove(e);
            }

            //Console.WriteLine(startPoint);
            //Console.WriteLine(originalPoint);
            //Console.WriteLine("//////////////");
        }

        private void Map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            map.ReleaseMouseCapture();
        }

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine(translateTransform.X+"  "+ translateTransform.Y);
            //Console.WriteLine(mainWindow.clipBorder.ActualWidth + "  "+ mainWindow.clipBorder.ActualHeight);
            string elementName = (e.OriginalSource as FrameworkElement).Name;
            //Console.WriteLine(elementName);
            if (e.Source.ToString() == "System.Windows.Controls.Canvas")
            {
                map.CaptureMouse();
                startPoint = e.GetPosition(mainWindow.clipBorder);
                originalPoint = new Point(translateTransform.X, translateTransform.Y);
            }
            Statectrl_MouseDown(e);
        }

        private void Map_Zoom(object sender, MouseWheelEventArgs e)
        {
            Point mousePos = e.GetPosition(map);
            double zoomDirection = e.Delta > 0 ? 1 : -1;
            
            slidingScale = 0.2 * zoomDirection;
            if (((mainWindow.canvasScaleTransform.ScaleY + slidingScale) >= zoomOutLimit) && 
                ((mainWindow.canvasScaleTransform.ScaleY + slidingScale) <= zoomInLitmit))
            {
                mainWindow.canvasScaleTransform.ScaleX = mainWindow.canvasScaleTransform.ScaleY += slidingScale;
            }
            //startPoint = e.GetPosition(mainWindow.clipBorder);
            //originalPoint = new Point(translateTransform.X, translateTransform.Y);

            //Console.WriteLine(mainWindow.canvasScaleTransform.ScaleX);
            //Console.WriteLine(mainWindow.clipBorder.ActualHeight);
            if(true)
            {
                Console.WriteLine(translateTransform.X + "  " + translateTransform.Y);
                //Console.WriteLine(mainWindow.clipBorder.ActualWidth + "  " + mainWindow.clipBorder.ActualHeight);
                //Console.WriteLine((mainWindow.clipBorder.ActualWidth / 2 + (-1)*translateTransform.X) - ((map.Width * scaleTransform.ScaleX) / 2));
                //Console.WriteLine((mainWindow.clipBorder.ActualHeight / 2 + (-1) * translateTransform.Y) - ((map.Height* scaleTransform.ScaleY) / 2));
                //Console.WriteLine(scaleTransform.ScaleY);
                //Console.WriteLine("Zoom out");
            }
            //Console.WriteLine(startPoint);
            //Console.WriteLine(originalPoint);
            //Console.WriteLine("//////////////");

        }

        //=================PROCESS===============
        void Statectrl_MouseDown(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //EditObject();
            }
            Point mousePos = e.GetPosition(map);
            var mouseWasDownOn = e.Source as FrameworkElement;
            switch (Global_Mouse.ctrl_MouseDown)
            {
                case Global_Mouse.STATE_MOUSEDOWN._ADD_STATION:
                    {
                        if (Global_Mouse.ctrl_MouseDown == Global_Mouse.STATE_MOUSEDOWN._ADD_STATION)
                        {
                            StationShape stationTemp = null;
                            stationTemp = new StationShape("MIX0", 2, 7, "Pallet2");
                            stationTemp.Move(mousePos.X, mousePos.Y);
                            map.Children.Add(stationTemp);
                        }
                        break;
                    }
                case Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        if (elementName != "")
                        {
                            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION;
                            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._GET_OUT_OBJECT;
                        }
                    }
                    break;
                default:
                    {
                        break;
                    }
            }
        }
        void Statectrl_MouseMove(MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(map);
            var mouseWasDownOn = e.Source as FrameworkElement;
            switch (Global_Mouse.ctrl_MouseMove)
            {
                case Global_Mouse.STATE_MOUSEMOVE._NORMAL:
                    {
                        break;
                    }
                case Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION:
                    {
                        //StationShape x = new StationShape();
                        //x = (StationShape)map.Children[0];
                        //x.Move(pp.X, pp.Y);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
