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
        private int stationCount = 0;
        private MainWindow mainWindow;
        private Canvas map;
        private ScaleTransform scaleTransform;
        private TranslateTransform translateTransform;
        private Point startPoint;
        private Point draw_StartPoint;
        private Point originalPoint;
        PathShape pathTemp;
        private double zoomInLitmit = 7;
        private double zoomOutLimit;
        private string selectedItemName = "";
        private string hoveringItemName = "";
        private double slidingScale;
        public SortedDictionary<string, PathShape> list_Path;
        public SortedDictionary<string, StationShape> list_Station;
        double yDistanceBottom, xDistanceLeft, yDistanceTop, xDistanceRight;

        public PalletViewControlService(MainWindow mainWinDowIn)
        {
            mainWindow = mainWinDowIn;
            map = mainWindow.map;
            scaleTransform = mainWindow.canvasScaleTransform;
            translateTransform = mainWindow.canvasTranslateTransform;
            list_Path = new SortedDictionary<string, PathShape>();
            list_Station = new SortedDictionary<string, StationShape>();
            //==========EVENT==========
            map.MouseDown += Map_MouseDown;
            map.MouseWheel += Map_Zoom;
            map.MouseMove += Map_MouseMove;
            map.MouseLeftButtonDown += Map_MouseLeftButtonDown;
            map.MouseRightButtonDown += Map_MouseRightButtonDown;
            map.MouseLeftButtonUp += Map_MouseLeftButtonUp;
            mainWindow.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            mainWindow.clipBorder.SizeChanged += ClipBorder_SizeChanged;
            zoomOutLimit = mainWindow.clipBorder.ActualHeight / map.Height;

        }


        //////////////////////////////////////////////////////
        //METHOD======METHOD======METHOD======METHOD======METHOD======
        //////////////////////////////////////////////////////
        private void ToggleSelectedPath(string currentPath)
        {
            if (list_Path.ContainsKey(currentPath))
            {
                list_Path[currentPath].props.isSelected = false;
                list_Path[currentPath].ToggleStyle();

            }
        }


        //////////////////////////////////////////////////////
        //EVENT========EVENT========EVENT========EVENT========
        //////////////////////////////////////////////////////

        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string elementName = (e.OriginalSource as FrameworkElement).Name;
        }
        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string elementName = (e.OriginalSource as FrameworkElement).Name;
            ToggleSelectedPath(selectedItemName);
            selectedItemName = elementName;
            //Console.WriteLine(elementName);
            if ((mainWindow.drag))
            {
                if (e.Source.ToString() == "System.Windows.Controls.Canvas")
                {
                    map.CaptureMouse();
                    startPoint = e.GetPosition(mainWindow.clipBorder);
                    originalPoint = new Point(translateTransform.X, translateTransform.Y);
                }
            }
            if (!mainWindow.drag)
            {
                Statectrl_MouseDown(e);
            }
        }
        private void Map_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            string elementName = (e.OriginalSource as FrameworkElement).Name;
            //selectedItemName = elementName;
        }
        private void Map_Zoom(object sender, MouseWheelEventArgs e)
        {
            Point mousePos = e.GetPosition(map);
            double zoomDirection = e.Delta > 0 ? 1 : -1;

            slidingScale = 0.1 * zoomDirection;
            if (((scaleTransform.ScaleY + slidingScale) >= zoomOutLimit) &&
                ((scaleTransform.ScaleY + slidingScale) <= zoomInLitmit))
            {
                scaleTransform.ScaleX = scaleTransform.ScaleY += slidingScale;
            }

            yDistanceBottom = (((mainWindow.clipBorder.ActualHeight / 2) - (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
            xDistanceRight = ((mainWindow.clipBorder.ActualWidth / 2 - (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
            yDistanceTop = (((mainWindow.clipBorder.ActualHeight / 2) + (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
            xDistanceLeft = ((mainWindow.clipBorder.ActualWidth / 2 + (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
            //YCoor
            if (yDistanceTop > 0)
            {
                translateTransform.Y = translateTransform.Y - yDistanceTop;
            }
            if (yDistanceBottom > 0)
            {
                translateTransform.Y = translateTransform.Y + yDistanceBottom;
            }
            //XCoor
            if ((map.Width * scaleTransform.ScaleX) < mainWindow.clipBorder.ActualWidth)
            {
                translateTransform.X = 0;
            }
            else
            {
                if (xDistanceLeft > 0)
                {
                    translateTransform.X = translateTransform.X - xDistanceLeft;
                }

                if (xDistanceRight > 0)
                {
                    translateTransform.X = translateTransform.X + xDistanceRight;
                }
            }

        }
        private void ClipBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            zoomOutLimit = mainWindow.clipBorder.ActualHeight / map.Height;
            scaleTransform.ScaleX = scaleTransform.ScaleY = zoomOutLimit;
            if (map.Width > (mainWindow.clipBorder.ActualWidth * scaleTransform.ScaleX))
            {
                translateTransform.X = 0;
                translateTransform.Y = 0;
            }
        }
        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(map);
            var mouseWasDownOn = (e.Source as FrameworkElement);
            hoveringItemName = mouseWasDownOn.Name;


            int x = ((int)mousePos.X / 10) * 10;
            int y = ((int)mousePos.Y / 10) * 10;
            mainWindow.MouseCoor.Content = x + " " + y;
            yDistanceBottom = (((mainWindow.clipBorder.ActualHeight / 2) - (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
            xDistanceRight = ((mainWindow.clipBorder.ActualWidth / 2 - (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
            yDistanceTop = (((mainWindow.clipBorder.ActualHeight / 2) + (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
            xDistanceLeft = ((mainWindow.clipBorder.ActualWidth / 2 + (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));

            //Console.WriteLine("===========" + yDistanceTop.ToString("0.") + "===========");
            //Console.WriteLine("===========================");
            //Console.WriteLine("" + xDistanceLeft.ToString("0.") + "=================" + xDistanceRight.ToString("0.") + "");
            //Console.WriteLine("===========================");
            //Console.WriteLine("===========" + yDistanceBottom.ToString("0.") + "===========");
            if ((mainWindow.drag))
            {
                if (!map.IsMouseCaptured) return;
                Vector moveVector = startPoint - e.GetPosition(mainWindow.clipBorder);
                double xCoor = originalPoint.X - moveVector.X;
                double yCoor = originalPoint.Y - moveVector.Y;
                translateTransform.X = xCoor;
                translateTransform.Y = yCoor;
                yDistanceBottom = (((mainWindow.clipBorder.ActualHeight / 2) - (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
                xDistanceRight = ((mainWindow.clipBorder.ActualWidth / 2 - (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
                yDistanceTop = (((mainWindow.clipBorder.ActualHeight / 2) + (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
                xDistanceLeft = ((mainWindow.clipBorder.ActualWidth / 2 + (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
                //YCoor
                if (yDistanceTop > 0)
                {
                    translateTransform.Y = translateTransform.Y - yDistanceTop;
                }
                if (yDistanceBottom > 0)
                {
                    translateTransform.Y = translateTransform.Y + yDistanceBottom;
                }
                //XCoor
                if ((map.Width * scaleTransform.ScaleX) < mainWindow.clipBorder.ActualWidth)
                {
                    translateTransform.X = 0;
                }
                else
                {
                    if (xDistanceLeft > 0)
                    {
                        translateTransform.X = translateTransform.X - xDistanceLeft;
                    }

                    if (xDistanceRight > 0)
                    {
                        translateTransform.X = translateTransform.X + xDistanceRight;
                    }
                }
            }
            if (!mainWindow.drag)
            {
                Statectrl_MouseMove(e);
            }

        }
        private void Map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            map.ReleaseMouseCapture();
        }


        //////////////////////////////////////////////////////
        //PROCESS=====PROCESS=====PROCESS=====PROCESS=========
        //////////////////////////////////////////////////////
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        Normal_mode();
                        break;
                    }
                case Key.Delete:
                    {
                        try
                        {
                            list_Path[selectedItemName].Remove();
                            list_Path.Remove(selectedItemName);
                            Console.WriteLine("Remove: " + selectedItemName + "-Count: " + list_Path.Count);
                        }
                        catch
                        {
                            Console.WriteLine("Nothing to remove");
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        public void Normal_mode()
        {
            mainWindow.drag = true;
            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL;
            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._NORMAL;
        }
        public void Select_mode()
        {
            //valstatectrl_mm = STATECTRL_MOUSEMOVE.STATECTRL_SLIDE_OBJECT;
            //valstatectrl_md = STATECTRL_MOUSEDOWN.STATECTRL_KEEP_IN_OBJECT;
        }
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
                            stationTemp = new StationShape("MIX" + stationCount, 2, 7, "Pallet2");
                            stationTemp.Move(mousePos);
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
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_STRAIGHT_P1:
                    pathTemp = new StraightPath(map, new Point(0, 0), new Point(0, 0));

                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        if (elementName != "")
                        {
                            draw_StartPoint = mousePos;
                            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_STRAIGHT_FINISH;
                            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._HAND_DRAW_STRAIGHT;
                        }
                    }
                    break;
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_P1:
                    pathTemp = new CurvePath(map, new Point(0, 0), new Point(0, 0), true);
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        if (elementName != "")
                        {
                            draw_StartPoint = mousePos;
                            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_FINISH;
                            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._HAND_DRAW_CURVE;
                        }
                    }
                    break;
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_P1:
                    pathTemp = new CurvePath(map, new Point(0, 0), new Point(0, 0), false);
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        if (elementName != "")
                        {
                            draw_StartPoint = mousePos;
                            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_FINISH;
                            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._HAND_DRAW_CURVE;
                        }
                    }
                    break;
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_P1:
                    pathTemp = new CurvePath(map, new Point(0, 0), new Point(0, 0), false);
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        string type = (e.Source.GetType().Name);
                        if ((elementName != "")&&(type == "StraightPath"))
                        {
                            draw_StartPoint = list_Path[elementName].props._desMousePos;
                            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_FINISH;
                            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._HAND_DRAW_JOINPATHS;
                        }
                    }
                    break;
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_STRAIGHT_FINISH:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_STRAIGHT_P1;
                        Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL; //stop draw
                        list_Path.Add(pathTemp.Name, pathTemp);
                    }
                    break;
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_FINISH:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_P1;
                        Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL; //stop draw
                        list_Path.Add(pathTemp.Name, pathTemp);
                    }
                    break;
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_FINISH:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_P1;
                        Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL; //stop draw
                        list_Path.Add(pathTemp.Name, pathTemp);
                    }
                    break;
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_FINISH:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        string type = (e.Source.GetType().Name);
                        if ((elementName != "") && ((type == "StraightPath")||(elementName.Split('x')[0] == "StraightPath")) )
                        {
                            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_P1;
                            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL; //stop draw
                            list_Path.Add(pathTemp.Name, pathTemp);
                        }
                        else
                        {
                            MessageBox.Show("JoinPaths is only accept between two StraightPath! \n And only link Tail-Head");
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
                case Global_Mouse.STATE_MOUSEMOVE._HAND_DRAW_STRAIGHT:
                    {
                        if (Global_Mouse.ctrl_MouseDown == Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_STRAIGHT_FINISH)
                        {
                            pathTemp.ReDraw(draw_StartPoint, mousePos);
                        }
                        break;
                    }
                case Global_Mouse.STATE_MOUSEMOVE._HAND_DRAW_CURVE:
                    {
                        if ((Global_Mouse.ctrl_MouseDown == Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_FINISH) ||
                            (Global_Mouse.ctrl_MouseDown == Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_FINISH))
                        {
                            pathTemp.ReDraw(draw_StartPoint, mousePos);
                        }
                        break;
                    }
                case Global_Mouse.STATE_MOUSEMOVE._HAND_DRAW_JOINPATHS:
                    {
                        if (Global_Mouse.ctrl_MouseDown == Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_FINISH)
                        {
                            string elementName = mouseWasDownOn.Name;
                            string type = (e.Source.GetType().Name);
                            if ((elementName != "") && ((type == "StraightPath") || (elementName.Split('x')[0] == "StraightPath")))
                            {
                                pathTemp.ReDraw(draw_StartPoint, list_Path[elementName].props._oriMousePos);
                            }
                            else if (elementName != pathTemp.Name)
                            {
                                pathTemp.ReDraw(draw_StartPoint, draw_StartPoint);
                            }
                        }
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
