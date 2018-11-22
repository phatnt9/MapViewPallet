using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapViewPallet.Shape
{
    class PalletViewControlService
    {
        //=================VARIABLE==================
        private int stationCount = 0;
        //---------------MAP-------------------
        private MainWindow mainWindow;
        private Canvas map;
        private ScaleTransform scaleTransform;
        private TranslateTransform translateTransform;
        private TreeView mainTreeView;
        //---------------DRAW-------------------
        //private Point roundedMousePos = new Point();
        private Point startPoint;
        private Point draw_StartPoint;
        private Point originalPoint;
        PathShape pathTemp;
        //---------------POINT OF VIEW-------------------
        private string selectedItemName = "";
        private string hoveringItemName = "";
        private double slidingScale;
        
        //---------------OBJECT-------------------
        public SortedDictionary<string, PathShape> list_Path;
        public SortedDictionary<string, StationShape> list_Station;
        //double yDistanceBottom, xDistanceLeft, yDistanceTop, xDistanceRight;
        //---------------MICS-------------------
        //private Ellipse cursorPoint = new Ellipse();
        //======================MAP======================
        public PalletViewControlService(MainWindow mainWinDowIn, TreeView mainTreeViewIn)
        {
            mainWindow = mainWinDowIn;
            mainTreeView = mainTreeViewIn;
            map = mainWindow.map;
            scaleTransform = mainWindow.canvasScaleTransform;
            translateTransform = mainWindow.canvasTranslateTransform;
            list_Path = new SortedDictionary<string, PathShape>();
            list_Station = new SortedDictionary<string, StationShape>();
            //==========EVENT==========
            map.MouseDown += Map_MouseDown;
            map.MouseWheel += Map_Zoom;
            map.MouseMove += Map_MouseMove;
            map.SizeChanged += Map_SizeChanged;
            map.MouseLeftButtonDown += Map_MouseLeftButtonDown;
            map.MouseRightButtonDown += Map_MouseRightButtonDown;
            map.MouseLeftButtonUp += Map_MouseLeftButtonUp;
            mainWindow.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            mainWindow.clipBorder.SizeChanged += ClipBorder_SizeChanged;

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
        private void ReCenterMapCanvas ()
        {
            double MapWidthScaled = (map.Width * scaleTransform.ScaleX);
            double MapHeightScaled = (map.Height * scaleTransform.ScaleY);
            double ClipBorderWidth = (mainWindow.clipBorder.ActualWidth);
            double ClipBorderHeight = (mainWindow.clipBorder.ActualHeight);

            double xlim;
            double ylim;
            //==========================================================
            if (ClipBorderWidth < map.Width)
            {
                xlim = (map.Width * (scaleTransform.ScaleX - 1)) / 2;
            }
            else
            {
                xlim = Math.Abs((MapWidthScaled - ClipBorderWidth) / 2);
            }

            if (ClipBorderHeight < map.Height)
            {
                ylim = (map.Height * (scaleTransform.ScaleY - 1)) / 2;
            }
            else
            {
                ylim = Math.Abs((MapHeightScaled - ClipBorderHeight) / 2);
            }
            //==========================================================
            if (ClipBorderWidth > map.Width)
            {
                translateTransform.X = 0;
            }
            else
            {
                translateTransform.X = ((xlim) - (MapWidthScaled - ClipBorderWidth - xlim)) / 2;
            }
            if (ClipBorderHeight > map.Height)
            {
                translateTransform.Y = 0;
            }
            else
            {
                translateTransform.Y = ((ylim) - (MapHeightScaled - ClipBorderHeight - ylim)) / 2;
            }
        }

        //////////////////////////////////////////////////////
        //EVENT========EVENT========EVENT========EVENT========
        //////////////////////////////////////////////////////

        private void ClipBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine("ClipBorder_SizeChanged");
            ReCenterMapCanvas();
        }
        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string elementName = (e.OriginalSource as FrameworkElement).Name;
        }
        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string elementName = (e.OriginalSource as FrameworkElement).Name;
            ToggleSelectedPath(selectedItemName);
            selectedItemName = elementName;
            Console.WriteLine(elementName);
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
            double edgeW = (scaleTransform.ScaleX + slidingScale) * map.Width;
            double edgeH = (scaleTransform.ScaleY + slidingScale) * map.Height;
            if (((edgeW>1)||(edgeH>1))&&((edgeW<(map.Width*10))||(edgeH < (map.Height * 10))))
            {
                scaleTransform.ScaleX = scaleTransform.ScaleY += slidingScale;
            }
            ReCenterMapCanvas();
        }
        private void Map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Global_Object.OriginPoint.X = map.Width * 0.5;
            Global_Object.OriginPoint.Y = map.Height * 0.5;
            mainWindow.rect.RenderTransform = new TranslateTransform(Global_Object.OriginPoint.X, Global_Object.OriginPoint.Y);
            mainWindow.robot.RenderTransform = new TranslateTransform(Global_Object.OriginPoint.X, Global_Object.OriginPoint.Y);
            Point backGroundTransform = Global_Object.LaserOriginalCoor;
            double X = Global_Object.OriginPoint.X - backGroundTransform.X;
            double Y = Global_Object.OriginPoint.Y - backGroundTransform.Y;
            map.Background.Transform = new TranslateTransform(X, Y);
            ReCenterMapCanvas();
        }
        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            //Get mouse props
            Point mousePos = e.GetPosition(map);
            var mouseWasDownOn = (e.Source as FrameworkElement);
            hoveringItemName = mouseWasDownOn.Name;
            //mainWindow.MouseCoor.Content = (mousePos.X- Global_Object.OriginPoint.X).ToString("0.0") + " " + (Global_Object.OriginPoint.Y - mousePos.Y ).ToString("0.0");
            mainWindow.MouseCoor.Content = (Global_Object.CoorLaser(mousePos).X).ToString("0.00") + " " + (Global_Object.CoorLaser(mousePos).Y).ToString("0.00");
            //mainWindow.MouseCoor.Content = ((mousePos).X.ToString("0.0") + " " + (mousePos).Y.ToString("0.0"));

            //Console.WriteLine("============================================");
            //Console.WriteLine("MousePos:  (" + mousePos.X + "," + mousePos.Y + ")");
            //Console.WriteLine("CoorLaser:  (" + Global_Object.CoorLaser(mousePos).X.ToString("0.0") + "," + Global_Object.CoorLaser(mousePos).Y.ToString("0.0")+")");
            //Console.WriteLine("CoorCanvas:  ("+ Global_Object.CoorCanvas(Global_Object.CoorLaser(mousePos)).X.ToString("0.0") + "," + Global_Object.CoorCanvas(Global_Object.CoorLaser(mousePos)).Y.ToString("0.0") + ")");
            ////
            // POINT OF VIEW
            //
            if ((mainWindow.drag))
            {
                if (!map.IsMouseCaptured) return;
                Vector moveVector = startPoint - e.GetPosition(mainWindow.clipBorder);
                double xCoor = originalPoint.X - moveVector.X;
                double yCoor = originalPoint.Y - moveVector.Y;
                
                double MapWidthScaled = (map.Width * scaleTransform.ScaleX);
                double MapHeightScaled = (map.Height * scaleTransform.ScaleY);
                double ClipBorderWidth = (mainWindow.clipBorder.ActualWidth);
                double ClipBorderHeight = (mainWindow.clipBorder.ActualHeight);

                double xlim;
                double ylim;
                if (ClipBorderWidth < map.Width)
                {
                    xlim = (map.Width * (scaleTransform.ScaleX - 1)) / 2;
                }
                else
                {
                    xlim = Math.Abs((MapWidthScaled - ClipBorderWidth) / 2);
                }

                if (ClipBorderHeight < map.Height)
                {
                    ylim = (map.Height * (scaleTransform.ScaleY - 1)) / 2;
                }
                else
                {
                    ylim = Math.Abs((MapHeightScaled - ClipBorderHeight) / 2);
                }

                if (ClipBorderWidth > map.Width)
                {
                    if ((xCoor >= (-xlim)) && (xCoor <= (xlim)))
                    {
                        translateTransform.X = xCoor;
                    }
                }
                else
                {
                    if (ClipBorderWidth < MapWidthScaled)
                    {
                        if ((xCoor <= (xlim)) && (xCoor >= -(MapWidthScaled - ClipBorderWidth - xlim)))
                        {
                            translateTransform.X = xCoor;
                        }
                    }
                    else
                    {
                        if ((xCoor >= (xlim)) && (xCoor <= -(MapWidthScaled - ClipBorderWidth - xlim)))
                        {
                            translateTransform.X = xCoor;
                        }
                    }
                }
                if (ClipBorderHeight > map.Height)
                {
                    if ((yCoor >= (-ylim)) && (yCoor <= (ylim)))
                    {
                        translateTransform.Y = yCoor;
                    }
                }
                else
                {
                    if (ClipBorderHeight < MapHeightScaled)
                    {
                        if ((yCoor <= (ylim)) && (yCoor >= -(MapHeightScaled - ClipBorderHeight - ylim)))
                        {
                            translateTransform.Y = yCoor;
                        }
                    }
                    else
                    {
                        if ((yCoor >= (ylim)) && (yCoor <= -(MapHeightScaled - ClipBorderHeight - ylim)))
                        {
                            translateTransform.Y = yCoor;
                        }
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
                            if (list_Path.ContainsKey(selectedItemName))
                            {
                                list_Path[selectedItemName].Remove();
                                //list_Path.Remove(selectedItemName);
                                Console.WriteLine("Remove: " + selectedItemName + "-Count: " + list_Path.Count);
                            }
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
                            stationTemp = new StationShape(map, "MIX" + stationCount, 2, 7, 0, "normal");
                            stationCount++;
                            stationTemp.Move(mousePos);
                            //map.Children.Add(stationTemp);
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
                    pathTemp.RemoveHandle += PathRemove;
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
                        if ((elementName != "") && ((type == "StraightPath") || (elementName.Split('x')[0] == "StraightPath")))
                        {
                            draw_StartPoint = list_Path[elementName].props.eightCorner[4];
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
                        pathTemp.props._oriMousePos = pathTemp.props.eightCorner[0];
                        pathTemp.props._desMousePos = pathTemp.props.eightCorner[4];
                        list_Path.Add(pathTemp.Name, pathTemp);
                    }
                    break;
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_FINISH:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_P1;
                        Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL; //stop draw
                        pathTemp.props._oriMousePos = pathTemp.props.eightCorner[7];
                        pathTemp.props._desMousePos = pathTemp.props.eightCorner[5];
                        list_Path.Add(pathTemp.Name, pathTemp);
                    }
                    break;
                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_FINISH:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_P1;
                        Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL; //stop draw
                        pathTemp.props._oriMousePos = pathTemp.props.eightCorner[7];
                        pathTemp.props._desMousePos = pathTemp.props.eightCorner[5];
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
                            pathTemp.props._oriMousePos = pathTemp.props.eightCorner[7];
                            pathTemp.props._desMousePos = pathTemp.props.eightCorner[5];
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

        public void PathRemove(string name)
        {
            if (list_Path.ContainsKey(name))
            {
                list_Path.Remove(name);
                Console.WriteLine("Remove: " + selectedItemName + "-Count: " + list_Path.Count);
            }
        }

        public void StationRemove(string name)
        {
            if (list_Station.ContainsKey(name))
            {
                list_Station.Remove(name);
                Console.WriteLine("Remove: " + selectedItemName + "-Count: " + list_Station.Count);
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
                                pathTemp.ReDraw(draw_StartPoint, list_Path[elementName].props.eightCorner[0]);
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


//Console.WriteLine("clipBorderWidth: " + ClipBorderWidth);
//Console.WriteLine("map.Width: " + map.Width);
//Console.WriteLine("map.Width-Scaled: " + MapWidthScaled);
//Console.WriteLine("xlim: " + xlim +"-"+ (-(MapWidthScaled - ClipBorderWidth - xlim)));
//Console.WriteLine("translateTransform: " + translateTransform.X);

//Console.WriteLine("clipBorderHeight: " + ClipBorderHeight);
//Console.WriteLine("map.Height: " + map.Height);
//Console.WriteLine("map.Height-Scaled: " + MapHeightScaled);
//Console.WriteLine("ylim: " + ylim + "-" + (-(MapHeightScaled - ClipBorderHeight - ylim)));
//Console.WriteLine("translateTransform: " + translateTransform.Y);
//Console.WriteLine("Middle Y: "+((ylim) - (MapHeightScaled - ClipBorderHeight - ylim)) / 2);
//Console.WriteLine((ClipBorderHeight > map.Height)? "ClipBorderHeight > map.Height" : "ClipBorderHeight < map.Height");


//yDistanceBottom = (((mainWindow.clipBorder.ActualHeight / 2) - (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
//xDistanceRight = ((mainWindow.clipBorder.ActualWidth / 2 - (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
//yDistanceTop = (((mainWindow.clipBorder.ActualHeight / 2) + (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
//xDistanceLeft = ((mainWindow.clipBorder.ActualWidth / 2 + (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));



//map.ContextMenu = new ContextMenu();
////===================================
//MenuItem editItem = new MenuItem();
//editItem.Header = "Option 1";
////editItem.Click += EditMenu;
////===================================
//MenuItem removeItem = new MenuItem();
//removeItem.Header = "Option 2";
////removeItem.Click += RemoveMenu;
//map.ContextMenu.Items.Add(editItem);
//map.ContextMenu.Items.Add(removeItem);
