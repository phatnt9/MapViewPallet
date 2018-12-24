using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.ComponentModel;
using static MapViewPallet.Global_Object;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Data;
using MapViewPallet.MiniForm;
using System.Timers;
using System.Threading;

namespace MapViewPallet.Shape
{
    public class StationShape : Border
    {
        string statuspallet = "F";

        //public System.Timers.Timer aTimer;

        public event Action<string> RemoveHandle;

        public class Props
        {
            public bool isSelected;
            public bool isHovering;


            public dtBuffer bufferDb;
            public Canvas _canvas;


            public Border _stationInfoBorder;
            public Grid _stationGrid; // Grid to hold all Pallet in Station
            public Point _posision; // Where station will be render,only accept Cavnas Coordinate
            public double _rotate; // Station rotate


            public TransformGroup _myTransformGroup;
            public TranslateTransform _myTranslateTransform;
            public RotateTransform _myRotateTransform;


            public List<Point> _eightCorner;
            public SortedDictionary<string, PalletShape> _palletList;


            [CategoryAttribute("ID Settings"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public string NameID { get; set; }


            [CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point Position
            {
                get { return Global_Object.CoorLaser(_posision); }
            }


            [CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public double Rotation
            {
                get { return _rotate; }
                set { _rotate = value; }
            }


            //[CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            //public int Bays { get; set; }


            //[CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            //public int Rows { get; set; }

        }
        public Properties stationProperties;
        public Props props;

        //#############---METHOD---############
        public StationShape(Canvas pCanvas, dtBuffer buffer)
        {
            props = new Props();
            props.bufferDb = buffer;


            BorderBrush = new SolidColorBrush(Colors.Transparent);
            BorderThickness = new Thickness(1);
            CornerRadius = new CornerRadius(1.2);
            RenderTransformOrigin = new Point(0, 0);
            props._stationGrid = new Grid();


            Background = new SolidColorBrush(Colors.Transparent);


            ToolTip = "";
            ToolTipOpening += ChangeToolTipContent;


            props._stationInfoBorder = new Border();
            props._stationInfoBorder.Background = new SolidColorBrush(Colors.Red);
            props._stationInfoBorder.CornerRadius = new CornerRadius(1.3);
            props._stationInfoBorder.Height = 5;
            Grid.SetColumn(props._stationInfoBorder, 0);


            //Name = "Stationx" + Global_Mouse.EncodeTransmissionTimestamp(); //Object name
            Name = props.bufferDb.bufferName.Replace(" ", ""); //Object name


            ContextMenu = new ContextMenu();
            //===================================
            MenuItem propertiesItem = new MenuItem();
            propertiesItem.Header = "Thông tin";
            propertiesItem.Click += PropertiesMenu;
            //===================================
            MenuItem editItem = new MenuItem();
            editItem.Header = "Tùy chỉnh";
            editItem.Click += EditMenu;
            //===================================
            MenuItem removeItem = new MenuItem();
            removeItem.Header = "Xóa";
            removeItem.Click += RemoveMenu;
            //===================================
            ContextMenu.Items.Add(propertiesItem);
            ContextMenu.Items.Add(editItem);
            //ContextMenu.Items.Add(rotateItem);
            //ContextMenu.Items.Add(removeItem);
            //====================EVENT=====================
            //MouseLeave += MouseLeaveStation;
            //MouseMove += MouseHoverStation;
            //MouseLeftButtonDown += MouseLeftButtonDownStation;
            //MouseRightButtonDown += MouseRightButtonDownStation;
            //===================CREATE=====================
            props.NameID = props.bufferDb.bufferName; //label
            //props.Bays = props.bufferDb.maxBay;
            //props.Rows = props.bufferDb.maxRow;
            Width = 11 * props.bufferDb.maxBay;
            Height = 13 * props.bufferDb.maxRow;


            props._palletList = new SortedDictionary<string, PalletShape>();


            props._myTransformGroup = new TransformGroup();
            props._myRotateTransform = new RotateTransform();
            props._myTranslateTransform = new TranslateTransform();


            stationProperties = new Properties(this);


            //Add Pallet to Grid
            for (int bayIndex = 0; bayIndex < props.bufferDb.maxBay; bayIndex++) //Column Index
            {
                //Create a Col
                ColumnDefinition colTemp = new ColumnDefinition();
                colTemp.Name = Name + "xL" + bayIndex;
                props._stationGrid.ColumnDefinitions.Add(colTemp);
                //Create GridLine
                Grid gridLine = new Grid();
                // Create BorderLine
                Border borderLine = new Border();
                Grid.SetColumn(borderLine, bayIndex);
                borderLine.Child = gridLine;
                //
                props._stationGrid.Children.Add(borderLine);
                if (bayIndex > 0)
                {
                    borderLine.BorderBrush = new SolidColorBrush(Colors.Black);
                    borderLine.BorderThickness = new Thickness(0.3, 0, 0, 0);
                }
                //Add Pallet to GridPallet ==> add GridPallet to BorderLine
                for (int rowIndex = 0; rowIndex < props.bufferDb.maxRow; rowIndex++) //Row Index, start from 1, Row 0 use for Infomation
                {
                    //Create Rows for Col
                    RowDefinition rowTemp = new RowDefinition();
                    rowTemp.Name = Name + "xR" + rowIndex;
                    //rowTemp.MinHeight = 10;
                    gridLine.RowDefinitions.Add(rowTemp);
                    //=============

                    //PalletShape palletTemp = new PalletShape(Name + "x" + lineIndex + "x" + palletIndex);
                    PalletShape palletTemp = new PalletShape("Pallet" + "x" + bayIndex + "x" + rowIndex);
                    Grid.SetRow(palletTemp, rowIndex);
                    gridLine.Children.Add(palletTemp);
                    props._palletList.Add(palletTemp.name, palletTemp);

                }
            }
            //==================SPECIALPOINT===================
            props._eightCorner = new List<Point>();
            for (int i = 0; i < 8; i++)
            {
                Point temp = new Point();
                props._eightCorner.Add(temp);
            }

            //==================CHILDREN===================
            //props.stationGrid.Children.Add(props.stationInfomation);
            Child = props._stationGrid;
            props._myTransformGroup.Children.Add(props._myRotateTransform);
            props._myTransformGroup.Children.Add(props._myTranslateTransform);
            RenderTransform = props._myTransformGroup;

            dynamic bufferData = JsonConvert.DeserializeObject(props.bufferDb.bufferData);
            if (bufferData != null)
            {
                props._posision = Global_Object.CoorCanvas(new Point((double)bufferData.x, (double)bufferData.y));
                props._rotate = (double)bufferData.a;
            }

            props._myRotateTransform.Angle = props._rotate;
            props._myTranslateTransform = new TranslateTransform(props._posision.X, props._posision.Y);

            props._myTransformGroup.Children[0] = props._myRotateTransform;
            props._myTransformGroup.Children[1] = props._myTranslateTransform;


            props._canvas = pCanvas;
            props._canvas.Children.Add(this);


            //aTimer = new System.Timers.Timer();
            //aTimer.Interval = 1000;
            //aTimer.Elapsed += OnTimedRedrawStationEvent;
            //aTimer.AutoReset = true;
            //aTimer.Enabled = true;
            //Get list pallet
        }



        //private void OnTimedRedrawStationEvent(object sender, ElapsedEventArgs e)
        //{
        //    ReDraw(props._posision);
        //}

        public void ReDraw()
        {
            Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "buffer/getListBuffer");
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
                        if ((tempBuffer.bufferId == this.props.bufferDb.bufferId) && (tempBuffer.bufferName == this.props.bufferDb.bufferName))
                        {
                            if ((this.props.bufferDb.maxBay != tempBuffer.maxBay) || ((this.props.bufferDb.maxRow != tempBuffer.maxRow)))
                            {
                                props.bufferDb = tempBuffer;

                                props._stationGrid.Children.Clear();
                                props._stationGrid.ColumnDefinitions.Clear();
                                props._stationGrid.RowDefinitions.Clear();
                                props._palletList.Clear();

                                Width = 11 * props.bufferDb.maxBay;
                                Height = 13 * props.bufferDb.maxRow;

                                for (int bayIndex = 0; bayIndex < props.bufferDb.maxBay; bayIndex++) //Column Index
                                {
                                    //Create a Col
                                    ColumnDefinition colTemp = new ColumnDefinition();
                                    colTemp.Name = Name + "xL" + bayIndex;
                                    props._stationGrid.ColumnDefinitions.Add(colTemp);
                                    //Create GridLine
                                    Grid gridLine = new Grid();
                                    // Create BorderLine
                                    Border borderLine = new Border();
                                    Grid.SetColumn(borderLine, bayIndex);
                                    borderLine.Child = gridLine;
                                    //
                                    props._stationGrid.Children.Add(borderLine);
                                    if (bayIndex > 0)
                                    {
                                        borderLine.BorderBrush = new SolidColorBrush(Colors.Black);
                                        borderLine.BorderThickness = new Thickness(0.3, 0, 0, 0);
                                    }
                                    //Add Pallet to GridPallet ==> add GridPallet to BorderLine
                                    for (int rowIndex = 0; rowIndex < props.bufferDb.maxRow; rowIndex++) //Row Index, start from 1, Row 0 use for Infomation
                                    {
                                        //Create Rows for Col
                                        RowDefinition rowTemp = new RowDefinition();
                                        rowTemp.Name = Name + "xR" + rowIndex;
                                        //rowTemp.MinHeight = 10;
                                        gridLine.RowDefinitions.Add(rowTemp);
                                        //=============

                                        //PalletShape palletTemp = new PalletShape(Name + "x" + lineIndex + "x" + palletIndex);
                                        PalletShape palletTemp = new PalletShape("Pallet" + "x" + bayIndex + "x" + rowIndex);
                                        Grid.SetRow(palletTemp, rowIndex);
                                        gridLine.Children.Add(palletTemp);
                                        props._palletList.Add(palletTemp.name, palletTemp);

                                    }
                                }
                            }
                            else
                            {
                                props.bufferDb = tempBuffer;
                            }

                        }
                    }
                }
                dynamic bufferData = JsonConvert.DeserializeObject(props.bufferDb.bufferData);
                props._posision = Global_Object.CoorCanvas(new Point(((bufferData != null) ? (((double)bufferData.x)) : 0), ((bufferData != null) ? (((double)bufferData.y)) : 0)));
                props._rotate = (bufferData != null) ? (((double)bufferData.a)) : 0;
                Draw();
            }));
        }

        public void Draw()
        {
            Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "pallet/getListPalletBufferId");
                request.Method = "POST";
                request.ContentType = @"application/json";


                dynamic postApiBody = new JObject();
                postApiBody.bufferId = props.bufferDb.bufferId;
                string jsonData = JsonConvert.SerializeObject(postApiBody);


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
                    string result = reader.ReadToEnd();

                    DataTable pallets = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in pallets.Rows)
                    {
                        dtPallet tempPallet = new dtPallet
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            palletId = int.Parse(dr["palletId"].ToString()),
                            deviceBufferId = int.Parse(dr["deviceBufferId"].ToString()),
                            bufferId = int.Parse(dr["bufferId"].ToString()),
                            planId = int.Parse(dr["planId"].ToString()),
                            row = int.Parse(dr["row"].ToString()),
                            bay = int.Parse(dr["bay"].ToString()),
                            dataPallet = dr["dataPallet"].ToString(),
                            palletStatus = dr["palletStatus"].ToString(),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                        };
                        //props._palletList["Pallet" + "x" + tempPallet.bay + "x" + tempPallet.row].StatusChanged(tempPallet.palletStatus);
                        if (props._palletList.ContainsKey("Pallet" + "x" + tempPallet.bay + "x" + tempPallet.row))
                        {
                            //props._palletList["Pallet" + "x" + tempPallet.bay + "x" + tempPallet.row].StatusChanged(statuspallet);
                            props._palletList["Pallet" + "x" + tempPallet.bay + "x" + tempPallet.row].StatusChanged(tempPallet.palletStatus);
                        }



                    }
                }


                props._myRotateTransform.Angle = props._rotate;
                props._myTranslateTransform = new TranslateTransform(props._posision.X, props._posision.Y);

                props._myTransformGroup.Children[0] = props._myRotateTransform;
                props._myTransformGroup.Children[1] = props._myTranslateTransform;

                // SPECIAL POINTS
                double width = 0.11285714 * props.bufferDb.maxBay;
                double height = 0.11285714 * props.bufferDb.maxRow;
                props._eightCorner[0] = CoorPointAtBorder(new Point((0), (Height / 2)));          //mid-left
                props._eightCorner[1] = CoorPointAtBorder(new Point((0), (0)));                 //top-left
                props._eightCorner[2] = CoorPointAtBorder(new Point((Width / 2), (0)));           //top-mid
                props._eightCorner[3] = CoorPointAtBorder(new Point((Width), (0)));             //top-right
                props._eightCorner[4] = CoorPointAtBorder(new Point((Width), (Height / 2)));      //mid-right
                props._eightCorner[5] = CoorPointAtBorder(new Point((Width), (Height)));        //bot-right
                props._eightCorner[6] = CoorPointAtBorder(new Point((Width / 2), (Height)));      //bot-mid
                props._eightCorner[7] = CoorPointAtBorder(new Point((0), (Height)));            //bot-left
            }));

            if (statuspallet == "F")
            {
                statuspallet = "W";
            }
            else
            {
                statuspallet = "F";
            }
        }

        public void EditMenu(object sender, RoutedEventArgs e)
        {

        }

        public void PropertiesMenu(object sender, RoutedEventArgs e)
        {
            stationProperties.ShowDialog();
        }
        private void RotateMenu(object sender, RoutedEventArgs e)
        {
            //double rotate = props.rotate * Math.PI / 180.0;
            //rotate = (rotate + (Math.PI / 2));
            //props.rotate = (rotate * 180.0 / Math.PI);
            props._rotate += 90;
            if (props._rotate > 360)
            {
                props._rotate -= 360;
            }
            props._myRotateTransform.Angle = props._rotate;
            props._myTransformGroup.Children[0] = props._myRotateTransform;
            RenderTransform = props._myTransformGroup;
        }

        public void RemoveMenu(object sender, RoutedEventArgs e)
        {
            Remove();
        }

        private void ChangeToolTipContent(object sender, ToolTipEventArgs e)
        {
            ToolTip = "Name: " + props.NameID +
                "\n Position: " + Global_Object.CoorLaser(props._posision).X.ToString("0.00") + "," + Global_Object.CoorLaser(props._posision).Y.ToString("0.00") +
                " \n Width: " + Width.ToString("0.00") + "m" +
                " \n Height: " + Height.ToString("0.00") + "m" +
                " \n Rotate: " + props._rotate;
        }

        public void Remove()
        {
            props._canvas.Children.Remove(this);
            RemoveHandle(props.NameID);
        }

        public void Move(Point pos)
        {
            props._myTranslateTransform = new TranslateTransform(pos.X, pos.Y);
            props._myRotateTransform = new RotateTransform(0);
            props._myTransformGroup.Children[1] = props._myTranslateTransform;
        }

        public Point CoorPointAtBorder(Point pointOnBorder)
        {
            double xDiff = (pointOnBorder.X) - 0;
            double yDiff = (pointOnBorder.Y) - 0;
            double rad1 = (props._rotate * Math.PI) / 180;
            double rad2 = (Math.Atan2(yDiff, xDiff));
            double L = Global_Object.LengthBetweenPoints(new Point(0, 0), pointOnBorder);
            double x1 = props._posision.X + ((L * Math.Cos(rad1 + rad2)));
            double y1 = props._posision.Y + ((L * Math.Sin(rad1 + rad2)));
            return new Point(x1, y1);
        }

        public void SelectedStyle()
        {
            BorderBrush = new SolidColorBrush(Colors.Red);
        }
        public void DeselectedStyle()
        {
            BorderBrush = new SolidColorBrush(Colors.Transparent);
        }



    }
}
