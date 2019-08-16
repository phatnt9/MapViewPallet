using MapViewPallet.MiniForm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MapViewPallet.Shape
{
    public class CanvasControlService : NotifyUIBase
    {
        //=================VARIABLE==================
        private readonly int stationCount = 0;
        private PalletShape _mouseHoverPallet;

        public PalletShape MouseHoverPallet
        {
            get => _mouseHoverPallet;
            set
            {
                if (_mouseHoverPallet != value)
                {
                    _mouseHoverPallet = value;
                    RaisePropertyChanged("MouseHoverPallet");
                }
            }
        }

        //---------------MAP-------------------
        private MainWindow mainWindow;

        private Canvas map;

        private double pZoomScale;

        public double zoomScale
        {
            get => pZoomScale;
            set
            {
                if (pZoomScale != value)
                {
                    pZoomScale = value;
                    pScaleTransform.ScaleX = pScaleTransform.ScaleY = pZoomScale;
                    RaisePropertyChanged("zoomScale");
                }
            }
        }

        private ScaleTransform pScaleTransform;

        public ScaleTransform scaleTransform
        {
            get => pScaleTransform;
            set
            {
                if (pScaleTransform != value)
                {
                    pScaleTransform = value;
                    RaisePropertyChanged("scaleTransform");
                }
            }
        }

        private TranslateTransform translateTransform;

        //private TreeView mainTreeView;
        //---------------DRAW-------------------
        //private Point roundedMousePos = new Point();
        private Point startPoint;

        private Point draw_StartPoint;
        private Point originalPoint;
        private CanvasPath pathTemp;

        //---------------POINT OF VIEW-------------------
        private string selectedItemName = "";

        private string hoveringItemName = "";
        private double slidingScale;

        //---------------OBJECT-------------------
        public SortedDictionary<string, CanvasPath> list_Path;

        public SortedDictionary<string, StationShape> list_Station;
        public SortedDictionary<string, RobotShape> list_Robot;

        
        public CanvasControlService(MainWindow mainWinDowIn)
        {
            mainWindow = mainWinDowIn;
            //mainTreeView = mainTreeViewIn;
            map = mainWindow.map;
            pScaleTransform = mainWindow.canvasScaleTransform;
            translateTransform = mainWindow.canvasTranslateTransform;
            list_Path = new SortedDictionary<string, CanvasPath>();
            list_Station = new SortedDictionary<string, StationShape>();
            list_Robot = new SortedDictionary<string, RobotShape>();
            //==========EVENT==========
            map.MouseDown += Map_MouseDown;
            //map.MouseWheel += Map_Zoom;
            map.MouseMove += Map_MouseMove;
            //map.SizeChanged += Map_SizeChanged;
            map.MouseLeftButtonDown += Map_MouseLeftButtonDown;
            map.MouseRightButtonDown += Map_MouseRightButtonDown;
            map.MouseLeftButtonUp += Map_MouseLeftButtonUp;
            mainWindow.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            //mainWindow.clipBorder.SizeChanged += ClipBorder_SizeChanged;
        }

        //########################################################
        //METHOD======METHOD======METHOD======METHOD======METHOD==
        //########################################################

        private void ToggleSelectedPath(string currentPath)
        {
            if (list_Path.ContainsKey(currentPath))
            {
                list_Path[currentPath].props.isSelected = false;
                list_Path[currentPath].ToggleStyle();
            }
        }

        public void ReCenterMapCanvas()
        {
            double MapWidthScaled = (map.Width * pScaleTransform.ScaleX);
            double MapHeightScaled = (map.Height * pScaleTransform.ScaleY);
            double ClipBorderWidth = (mainWindow.clipBorder.ActualWidth);
            double ClipBorderHeight = (mainWindow.clipBorder.ActualHeight);

            double xlim;
            double ylim;
            //==========================================================
            if (ClipBorderWidth < map.Width)
            {
                xlim = (map.Width * (pScaleTransform.ScaleX - 1)) / 2;
            }
            else
            {
                xlim = Math.Abs((MapWidthScaled - ClipBorderWidth) / 2);
            }

            if (ClipBorderHeight < map.Height)
            {
                ylim = (map.Height * (pScaleTransform.ScaleY - 1)) / 2;
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

        //EVENT========EVENT========EVENT========EVENT========

        private void ClipBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Console.WriteLine("ClipBorder_SizeChanged");
            ReCenterMapCanvas();
        }

        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string elementName = (e.OriginalSource as FrameworkElement).Name;
            Point mousePos = e.GetPosition(map);
            //Console.WriteLine(mousePos.X+"  "+mousePos.Y);
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

                    Point p1 = e.GetPosition(mainWindow.clipBorder);
                    Point p2 = e.GetPosition(mainWindow.map);
                    //Console.WriteLine(p1.X.ToString("0.00") + "-" + p1.Y.ToString("0.00"));
                    //Console.WriteLine(p2.X.ToString("0.00") + "-" + p2.Y.ToString("0.00"));
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
            double edgeW = (pScaleTransform.ScaleX + slidingScale) * map.Width;
            double edgeH = (pScaleTransform.ScaleY + slidingScale) * map.Height;
            if (((edgeW > 1) || (edgeH > 1)) && ((edgeW < (map.Width * 10)) || (edgeH < (map.Height * 10))))
            {
                zoomScale = pScaleTransform.ScaleX = pScaleTransform.ScaleY += slidingScale;
            }

            //Console.WriteLine((map.ActualWidth* pScaleTransform.ScaleX).ToString("0.00")+"-"+ (map.ActualHeight * pScaleTransform.ScaleX).ToString("0.00"));

            //ReCenterMapCanvas();

            //var element = sender as UIElement;
            //var position = e.GetPosition(element);
            //var transform = element.RenderTransform as MatrixTransform;
            ////var matrix = transform.Matrix;
            //var matrix = mainWindow.canvasMatrixTransform.Matrix;
            //Console.WriteLine(matrix.ToString());
            //var scale = e.Delta >= 0 ? 1.1 : (1.0 / 1.1);

            //matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
            //matrix.ScaleAtPrepend(2, 4, 100, 100);
            ////transform.Matrix = matrix;
            //mainWindow.canvasMatrixTransform.Matrix = matrix;
        }

        private void Map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Global_Object.OriginPoint.X = map.Width * 0.5;
            Global_Object.OriginPoint.Y = map.Height * 0.5;
            mainWindow.rect.RenderTransform = new TranslateTransform(Global_Object.OriginPoint.X, Global_Object.OriginPoint.Y);
            //mainWindow.robot.RenderTransform = new TranslateTransform(Global_Object.OriginPoint.X, Global_Object.OriginPoint.Y);
            Point backGroundTransform = Global_Object.LaserOriginalCoor;
            double X = Global_Object.OriginPoint.X - backGroundTransform.X;
            double Y = Global_Object.OriginPoint.Y - backGroundTransform.Y;
            map.Background.Transform = new TranslateTransform(X, Y);
            ReCenterMapCanvas();
        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            //Get mouse props
            //Point mousePos = e.GetPosition(map);
            //mainWindow.DP_PALLETINFO.RenderTransform = new TranslateTransform(mousePos.X+20, mousePos.Y+20);
            var mouseWasDownOn = (e.Source as FrameworkElement);
            //Console.WriteLine(mouseWasDownOn.GetType().ToString());
            try
            {
                Type mouseHoverItemType = mouseWasDownOn.GetType();

                switch (mouseHoverItemType.ToString())
                {
                    case "System.Windows.Controls.TextBlock":
                    {
                        System.Windows.Controls.TextBlock texblock = mouseWasDownOn as System.Windows.Controls.TextBlock;
                        if (texblock.Text.Length > 10)
                        {
                            string[] dateAndCode = mainWindow.palletInfoShow.Text.Split('/');
                            mainWindow.palletInfoShow.Text = "";
                            mainWindow.palletInfoShow.Text =
                                   // " Bay/Row: " + MouseHoverPallet.pallet.bay + "/" + MouseHoverPallet.pallet.row +
                                   // " Item FGS: " + MouseHoverPallet.pallet.productId + "-" + MouseHoverPallet.pallet.productName +
                                   "" + texblock.Text +
                                   "";
                        }
                        break;
                    }
                    case "MapViewPallet.Shape.PalletShape":
                    {
                        MouseHoverPallet = mouseWasDownOn as PalletShape;
                        mainWindow.palletInfoShow.Text = "";
                        mainWindow.palletInfoShow.Text =
                            // " Bay/Row: " + MouseHoverPallet.pallet.bay + "/" + MouseHoverPallet.pallet.row +
                            // " Item FGS: " + MouseHoverPallet.pallet.productId + "-" + MouseHoverPallet.pallet.productName +
                            "" + MouseHoverPallet.pallet.productDetailName +
                            "";
                        break;
                    }
                    default:
                    {
                        mainWindow.palletInfoShow.Text = "";
                        break;
                    }
                }
                
            }
            catch
            {
                mainWindow.palletInfoShow.Text = "";
            }
            //hoveringItemName = mouseWasDownOn.Name;
            //Console.WriteLine(hoveringItemName);
            if (!mainWindow.drag)
            {
                Statectrl_MouseMove(e);
            }
        }

        private void Map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            map.ReleaseMouseCapture();
        }

        //PROCESS=====PROCESS=====PROCESS=====PROCESS=========

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

        private void Statectrl_MouseDown(MouseButtonEventArgs e)
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
                        //StationShape stationTemp = null;
                        ////stationTemp = new StationShape(map, "MIX" + stationCount, 2, 7, 0);
                        //stationCount++;
                        //stationTemp.Move(mousePos);
                        ////map.Children.Add(stationTemp);
                    }
                    break;
                }
                case Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        string type = Global_Object.Foo(mouseWasDownOn);
                        if (elementName != "")
                        {
                            //Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION;
                            //Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._GET_OUT_OBJECT;
                        }
                    }
                    break;

                case Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT_MOVE_STATION:
                    if (Global_Object.bufferToMove != null)
                    {
                        Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION_START;
                        Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._GET_OUT_OBJECT_MOVE_STATION;
                    }
                    break;

                case Global_Mouse.STATE_MOUSEDOWN._GET_OUT_OBJECT_MOVE_STATION:
                    if (Global_Object.bufferToMove != null)
                    {
                        // Cap nhat data
                        if (!Global_Object.ServerAlive())
                        {
                            break;
                        }
                        try
                        {
                            dtBuffer buffer = Global_Object.bufferToMove.props.bufferDb;
                            List<dtBuffer> buffers = new List<dtBuffer>();

                            dynamic postApiBody = new JObject();
                            Point updateBufferDataPoint = Global_Object.CoorLaser(mousePos);

                            postApiBody.x = Math.Round(updateBufferDataPoint.X, 1);
                            postApiBody.y = Math.Round(updateBufferDataPoint.Y, 1);
                            postApiBody.angle = Math.Round(Global_Object.bufferToMove.props._rotate, 1);

                            dynamic data = JsonConvert.DeserializeObject(Global_Object.bufferToMove.props.bufferDb.bufferData);
                            postApiBody.arrange = data.arrange;
                            postApiBody.canOpEdit = (data.canOpEdit == null) ? false : data.returnGate;
                            postApiBody.returnGate = (data.returnGate == null) ? false : data.returnGate;
                            postApiBody.returnMain = (data.returnMain == null) ? false : data.returnMain;
                            postApiBody.return401 = (data.return401 == null) ? false : data.return401;
                            string jsonBufferData = JsonConvert.SerializeObject(postApiBody);
                            buffer.bufferData = jsonBufferData;

                            buffers.Add(buffer);

                            if (buffers.Count == 0)
                            {
                                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                                return;
                            }

                            string jsonData = JsonConvert.SerializeObject(buffers);

                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "buffer/updateListBuffer");
                            request.Method = "POST";
                            request.ContentType = "application/json";

                            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                            Byte[] byteArray = encoding.GetBytes(jsonData);
                            request.ContentLength = byteArray.Length;
                            using (Stream dataStream = request.GetRequestStream())
                            {
                                dataStream.Write(byteArray, 0, byteArray.Length);
                                dataStream.Flush();
                            }

                            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                                int result = 0;
                                int.TryParse(reader.ReadToEnd(), out result);
                                if (result == 1)
                                {
                                    //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else if (result == -2)
                                {
                                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Buffers Name"), Global_Object.messageTitileError, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                }
                                else
                                {
                                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                }
                            }
                            //UpdateTab4(true);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.Message);
                        }
                        Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION;//Do nothing, ready to move
                        Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT_MOVE_STATION;
                        Global_Object.bufferToMove = null;
                    }
                    break;

                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_STRAIGHT_P1:
                    pathTemp = new StraightShape(map, new Point(0, 0), new Point(0, 0));
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
                    pathTemp = new CurveShape(map, new Point(0, 0), new Point(0, 0), true);
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
                    pathTemp = new CurveShape(map, new Point(0, 0), new Point(0, 0), false);
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
                    pathTemp = new CurveShape(map, new Point(0, 0), new Point(0, 0), false);
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        string type = (e.Source.GetType().Name);
                        if ((elementName != "") && ((type == "StraightPath") || (elementName.Split('x')[0] == "StraightPath")))
                        {
                            draw_StartPoint = list_Path[elementName].props.cornerPoints[4];
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
                        pathTemp.props._oriMousePos = pathTemp.props.cornerPoints[0];
                        pathTemp.props._desMousePos = pathTemp.props.cornerPoints[4];
                        list_Path.Add(pathTemp.Name, pathTemp);
                    }
                    break;

                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_FINISH:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEUP_P1;
                        Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL; //stop draw
                        pathTemp.props._oriMousePos = pathTemp.props.cornerPoints[7];
                        pathTemp.props._desMousePos = pathTemp.props.cornerPoints[5];
                        list_Path.Add(pathTemp.Name, pathTemp);
                    }
                    break;

                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_FINISH:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_CURVEDOWN_P1;
                        Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL; //stop draw
                        pathTemp.props._oriMousePos = pathTemp.props.cornerPoints[7];
                        pathTemp.props._desMousePos = pathTemp.props.cornerPoints[5];
                        list_Path.Add(pathTemp.Name, pathTemp);
                    }
                    break;

                case Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_FINISH:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        string type = (e.Source.GetType().Name);
                        if ((elementName != "") && ((type == "StraightPath") || (elementName.Split('x')[0] == "StraightPath")))
                        {
                            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._HAND_DRAW_JOINPATHS_P1;
                            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._NORMAL; //stop draw
                            pathTemp.props._oriMousePos = pathTemp.props.cornerPoints[7];
                            pathTemp.props._desMousePos = pathTemp.props.cornerPoints[5];
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
                //Console.WriteLine("Remove: " + selectedItemName + "-Count: " + list_Path.Count);
            }
        }

        public void StationRemove(string name)
        {
            if (list_Station.ContainsKey(name))
            {
                list_Station.Remove(name);
                //Console.WriteLine("Remove: " + selectedItemName + "-Count: " + list_Station.Count);
            }
        }

        public void ReloadAllStation()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                for (int i = 0; i < list_Station.Count; i++)
                {
                    StationShape temp = list_Station.ElementAt(i).Value;
                    temp.Remove();
                }
                list_Station.Clear();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "buffer/getListBuffer");
                request.Method = "GET";
                request.ContentType = @"application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();

                    DataTable buffers = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in buffers.Rows)
                    {
                        dtBuffer tempBuffer = new dtBuffer
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),

                            bufferId = int.Parse(dr["bufferId"].ToString()),
                            bufferName = dr["bufferName"].ToString(),
                            bufferNameOld = dr["bufferNameOld"].ToString(),
                            bufferCheckIn = dr["bufferCheckIn"].ToString(),
                            bufferData = dr["bufferData"].ToString(),
                            maxBay = int.Parse(dr["maxBay"].ToString()),
                            maxBayOld = int.Parse(dr["maxBayOld"].ToString()),
                            maxRow = int.Parse(dr["maxRow"].ToString()),
                            maxRowOld = int.Parse(dr["maxRowOld"].ToString()),
                            bufferReturn = bool.Parse(dr["bufferReturn"].ToString()),
                            bufferReturnOld = bool.Parse(dr["bufferReturnOld"].ToString()),
                            //pallets
                        };
                        if (list_Station.ContainsKey(tempBuffer.bufferName.ToString().Trim()))
                        {
                            list_Station[tempBuffer.bufferName.ToString().Trim()].props.bufferDb = tempBuffer;
                            Console.WriteLine("Upadte bufferDb station ReloadAllStation:" + tempBuffer.bufferName);
                        }
                        else
                        {
                            StationShape tempStation = new StationShape(map, tempBuffer);
                            //tempStation.ReDraw();
                            //tempStation.RemoveHandle += StationRemove;
                            list_Station.Add(tempStation.props.bufferDb.bufferName.ToString().Trim(), tempStation);
                            //Console.WriteLine("Add them station ReloadAllStation:" + tempBuffer.bufferName);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private void Statectrl_MouseMove(MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(map);
            var mouseWasDownOn = e.Source as FrameworkElement;

            switch (Global_Mouse.ctrl_MouseMove)
            {
                case Global_Mouse.STATE_MOUSEMOVE._NORMAL:
                {
                    break;
                }
                case Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION_START:
                {
                    if (Global_Object.bufferToMove != null)
                    {
                        Global_Object.bufferToMove.Move(mousePos);
                    }
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
                            pathTemp.ReDraw(draw_StartPoint, list_Path[elementName].props.cornerPoints[0]);
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

        //public void RedrawAllStation()
        //{
        //    for (int i = 0; i < list_Station.Count; i++)
        //    {
        //        list_Station.ElementAt(i).Value.ReDraw();
        //    }
        //}

        public void RedrawAllStation(List<dtBuffer> listBuffer)
        {
            //Có lỗi System.InvalidOperationException: 'Collection was modified after the enumerator was instantiated.'
            for (int i = 0; i < list_Station.Count; i++)
            {
                foreach (dtBuffer buffer in listBuffer)
                {
                    if (buffer.bufferId == list_Station.ElementAt(i).Value.props.bufferDb.bufferId)
                    {
                        list_Station.ElementAt(i).Value.ReDraw(buffer);
                    }
                }
            }
        }

        public List<dtBuffer> GetDataAllStation()
        {
            if (!Global_Object.ServerAlive())
            {
                return new List<dtBuffer>();
            }
            try
            {
                List<dtBuffer> listBuffer = new List<dtBuffer>();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "buffer/getListBuffer");
                request.Method = "GET";
                request.ContentType = @"application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();

                    DataTable buffers = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in buffers.Rows)
                    {
                        dtBuffer tempBuffer = new dtBuffer
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),

                            bufferId = int.Parse(dr["bufferId"].ToString()),
                            bufferName = dr["bufferName"].ToString(),
                            bufferNameOld = dr["bufferNameOld"].ToString(),
                            bufferCheckIn = dr["bufferCheckIn"].ToString(),
                            bufferData = dr["bufferData"].ToString(),
                            maxRow = int.Parse(dr["maxRow"].ToString()),
                            maxBay = int.Parse(dr["maxBay"].ToString()),
                            maxRowOld = int.Parse(dr["maxRowOld"].ToString()),
                            maxBayOld = int.Parse(dr["maxBayOld"].ToString()),
                            bufferReturn = bool.Parse(dr["bufferReturn"].ToString()),
                            bufferReturnOld = bool.Parse(dr["bufferReturnOld"].ToString()),
                            //pallets
                        };
                        GetListPallet(tempBuffer);
                        listBuffer.Add(tempBuffer);
                    }
                }
                response.Close();
                return listBuffer;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return new List<dtBuffer>();
            }
        }

        public void GetListPallet(dtBuffer tempBuffer)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/getListPalletBufferId");
                request2.Method = "POST";
                request2.ContentType = @"application/json";

                dynamic postApiBody2 = new JObject();
                postApiBody2.bufferId = tempBuffer.bufferId;
                string jsonData2 = JsonConvert.SerializeObject(postApiBody2);

                System.Text.UTF8Encoding encoding2 = new System.Text.UTF8Encoding();
                Byte[] byteArray2 = encoding2.GetBytes(jsonData2);
                request2.ContentLength = byteArray2.Length;
                using (Stream dataStream2 = request2.GetRequestStream())
                {
                    dataStream2.Write(byteArray2, 0, byteArray2.Length);
                    dataStream2.Flush();
                }
                HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse;
                using (Stream responseStream2 = response2.GetResponseStream())
                {
                    StreamReader reader2 = new StreamReader(responseStream2, Encoding.UTF8);
                    string result2 = reader2.ReadToEnd();

                    DataTable pallets = JsonConvert.DeserializeObject<DataTable>(result2);
                    foreach (DataRow dr2 in pallets.Rows)
                    {
                        dtPallet tempPallet = new dtPallet
                        {
                            creUsrId = int.Parse(dr2["creUsrId"].ToString()),
                            creDt = dr2["creDt"].ToString(),
                            updUsrId = int.Parse(dr2["updUsrId"].ToString()),
                            updDt = dr2["updDt"].ToString(),
                            palletId = int.Parse(dr2["palletId"].ToString()),
                            deviceBufferId = int.Parse(dr2["deviceBufferId"].ToString()),
                            bufferId = int.Parse(dr2["bufferId"].ToString()),
                            planId = int.Parse(dr2["planId"].ToString()),
                            activeDate = dr2["activeDate"].ToString(),
                            row = int.Parse(dr2["row"].ToString()),
                            bay = int.Parse(dr2["bay"].ToString()),
                            dataPallet = dr2["dataPallet"].ToString(),
                            palletStatus = dr2["palletStatus"].ToString(),
                            deviceId = int.Parse(dr2["deviceId"].ToString()),
                            deviceName = dr2["deviceName"].ToString(),
                            productId = int.Parse(dr2["productId"].ToString()),
                            productName = dr2["productName"].ToString(),
                            productDetailId = int.Parse(dr2["productDetailId"].ToString()),
                            productDetailName = dr2["productDetailName"].ToString(),
                        };
                        tempBuffer.pallets.Add(tempPallet);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        public void RedrawAllRobot()
        {
            for (int i = 0; i < list_Robot.Count; i++)
            {
                list_Robot.ElementAt(i).Value.DrawCircle();
                //Thread.Sleep(500);
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