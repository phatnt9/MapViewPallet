using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using MapViewPallet.MiniForm;
using System.Threading;

namespace MapViewPallet.Shape
{
    public class StationShape : Border
    {

        //public event Action<string> RemoveHandle;
        //int StaticWidth = 16;
        //int StaticHeight = 18;
        public class Props
        {
            //public bool isSelected;
            //public bool isHovering;

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
            //SHAPE
            ToolTip = "";
            ToolTipOpening += ChangeToolTipContent;
            //BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF1F1F"));
            BorderBrush = new SolidColorBrush(Colors.Lime);
            BorderThickness = new Thickness(1, 0, 1, 1);
            CornerRadius = new CornerRadius(0);
            RenderTransformOrigin = new Point(0, 0);
            Background = new SolidColorBrush(Colors.Transparent);

            //PROPERTIES
            props = new Props();
            props.bufferDb = buffer;
            props._stationGrid = new Grid();
            props._stationInfoBorder = new Border();
            props._stationInfoBorder.Background = new SolidColorBrush(Colors.Red);
            props._stationInfoBorder.CornerRadius = new CornerRadius(1.3);
            props._stationInfoBorder.Height = 5;
            Grid.SetColumn(props._stationInfoBorder, 0);
            //Name = props.bufferDb.bufferName.Trim().Replace(" ", ""); //Object name

            ContextMenu = new ContextMenu();

            //MenuItem propertiesItem = new MenuItem();
            //propertiesItem.SetResourceReference(MenuItem.HeaderProperty, "Station_Menu_Item_Properties");
            //propertiesItem.Click += PropertiesMenu;

            MenuItem editItem = new MenuItem();
            editItem.SetResourceReference(MenuItem.HeaderProperty, "Station_Menu_Item_Edit");
            editItem.Click += EditMenu;

            MenuItem removeItem = new MenuItem();
            removeItem.SetResourceReference(MenuItem.HeaderProperty, "Station_Menu_Item_Remove");
            removeItem.Click += RemoveMenu;

            MenuItem rotateItem = new MenuItem();
            rotateItem.SetResourceReference(MenuItem.HeaderProperty, "Station_Menu_Item_Rotate");
            rotateItem.Click += RotateMenu;

            //ContextMenu.Items.Add(propertiesItem);
            ContextMenu.Items.Add(editItem);
            ContextMenu.Items.Add(rotateItem);
            //ContextMenu.Items.Add(removeItem);
            //====================EVENT=====================
            MouseDown += StationShape_MouseDown;
            //MouseLeave += MouseLeaveStation;
            //MouseMove += MouseHoverStation;
            //MouseLeftButtonDown += MouseLeftButtonDownStation;
            //MouseRightButtonDown += MouseRightButtonDownStation;
            //===================CREATE=====================
            Width = MapViewPallet.Properties.Settings.Default.palletWidth * props.bufferDb.maxBay;
            Height = MapViewPallet.Properties.Settings.Default.palletHeight * props.bufferDb.maxRow;
            props.NameID = props.bufferDb.bufferName; //label
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
                //colTemp.Name = Name + "xL" + bayIndex;
                props._stationGrid.ColumnDefinitions.Add(colTemp);

                // Create BorderLine
                Border borderLine = new Border();
                Grid.SetColumn(borderLine, bayIndex);
                //Create GridLine
                Grid gridLine = new Grid();
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
                    //rowTemp.Name = Name + "xR" + rowIndex;
                    //rowTemp.MinHeight = 10;
                    gridLine.RowDefinitions.Add(rowTemp);
                    //=============
                    //if(rowIndex>0)
                    {
                        dynamic bufferData = JsonConvert.DeserializeObject(props.bufferDb.bufferData);
                        //PalletShape palletTemp = new PalletShape(Name + "x" + lineIndex + "x" + palletIndex);
                        PalletShape palletTemp = new PalletShape(
                            "Pallet"
                            + "x" +
                            ((bufferData.arrange == "littleEndian") ? (props.bufferDb.maxBay - bayIndex - 1) : bayIndex)
                            + "x" + rowIndex);

                        Grid.SetRow(palletTemp, rowIndex);
                        gridLine.Children.Add(palletTemp);
                        props._palletList.Add(palletTemp.name, palletTemp);
                    }
                }
            }
            //==================SPECIALPOINT===================
            //props._eightCorner = new List<Point>();
            //for (int i = 0; i < 8; i++)
            //{
            //    Point temp = new Point();
            //    props._eightCorner.Add(temp);
            //}

            //==================CHILDREN===================
            //props._stationGrid.Children.Add(props._stationInfoBorder);
            Child = props._stationGrid;
            props._myTransformGroup.Children.Add(props._myRotateTransform);
            props._myTransformGroup.Children.Add(props._myTranslateTransform);
            RenderTransform = props._myTransformGroup;

            SetCoorAndRotate();

            props._canvas = pCanvas;
            props._canvas.Children.Add(this);
        }

        public void SetCoorAndRotate()
        {
            dynamic bufferData = JsonConvert.DeserializeObject(props.bufferDb.bufferData);
            if (bufferData != null)
            {
                props._posision = Global_Object.CoorCanvas(new Point(((bufferData != null) ? (((double)bufferData.x)) : 0), ((bufferData != null) ? (((double)bufferData.y)) : 0)));
                props._rotate = (bufferData != null) ? (((double)bufferData.angle)) : 0;
            }
            props._myRotateTransform.Angle = props._rotate;
            props._myTranslateTransform = new TranslateTransform(props._posision.X, props._posision.Y);
            props._myTransformGroup.Children[0] = props._myRotateTransform;
            props._myTransformGroup.Children[1] = props._myTranslateTransform;

            Width = MapViewPallet.Properties.Settings.Default.palletWidth * props.bufferDb.maxBay;
            Height = MapViewPallet.Properties.Settings.Default.palletHeight * props.bufferDb.maxRow;

        }

        private void StationShape_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //MessageBox.Show("" + Name);
            if (Global_Mouse.ctrl_MouseDown == Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT_MOVE_STATION)
            {
                if (Global_Object.bufferToMove == null)
                {
                    Global_Object.bufferToMove = this;
                }
            }
        }


        public void ReDraw(dtBuffer buffer)
        {
            if ((buffer.bufferId == this.props.bufferDb.bufferId) && (buffer.bufferName == this.props.bufferDb.bufferName))
            {
                if ((this.props.bufferDb.maxBay != buffer.maxBay) || ((this.props.bufferDb.maxRow != buffer.maxRow)))
                {
                    props.bufferDb = buffer;

                    props._palletList.Clear();
                    props._stationGrid.Children.Clear();
                    props._stationGrid.RowDefinitions.Clear();
                    props._stationGrid.ColumnDefinitions.Clear();

                    Width = MapViewPallet.Properties.Settings.Default.palletWidth * props.bufferDb.maxBay;
                    Height = MapViewPallet.Properties.Settings.Default.palletHeight * props.bufferDb.maxRow;


                    for (int bayIndex = 0; bayIndex < props.bufferDb.maxBay; bayIndex++) //Column Index
                    {
                        //Create a Col
                        ColumnDefinition colTemp = new ColumnDefinition();
                        //colTemp.Name = Name + "xL" + bayIndex;
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
                            //rowTemp.Name = Name + "xR" + rowIndex;
                            //rowTemp.MinHeight = 10;
                            gridLine.RowDefinitions.Add(rowTemp);

                            dynamic bufferData = JsonConvert.DeserializeObject(props.bufferDb.bufferData);
                            //PalletShape palletTemp = new PalletShape(Name + "x" + lineIndex + "x" + palletIndex);
                            PalletShape palletTemp = new PalletShape(
                                "Pallet"
                                + "x" +
                                ((bufferData.arrange == "littleEndian") ? (props.bufferDb.maxBay - bayIndex - 1) : bayIndex)
                                + "x" + rowIndex);

                            Grid.SetRow(palletTemp, rowIndex);
                            gridLine.Children.Add(palletTemp);
                            props._palletList.Add(palletTemp.name, palletTemp);

                        }
                    }
                }
                else
                {
                    props.bufferDb = buffer;
                    Dispatcher.BeginInvoke(new ThreadStart(() =>
                    {
                        props._myRotateTransform.Angle = props._rotate;
                        props._myTranslateTransform = new TranslateTransform(props._posision.X, props._posision.Y);
                        props._myTransformGroup.Children[0] = props._myRotateTransform;
                        props._myTransformGroup.Children[1] = props._myTranslateTransform;

                        Width = MapViewPallet.Properties.Settings.Default.palletWidth * props.bufferDb.maxBay;
                        Height = MapViewPallet.Properties.Settings.Default.palletHeight * props.bufferDb.maxRow;

                    }));
                }

            }
            //dynamic bufferData = JsonConvert.DeserializeObject(props.bufferDb.bufferData);
            //props._posision = Global_Object.CoorCanvas(new Point(((bufferData != null) ? (((double)bufferData.x)) : 0), ((bufferData != null) ? (((double)bufferData.y)) : 0)));
            //props._rotate = (bufferData != null) ? (((double)bufferData.angle)) : 0;
            SetCoorAndRotate();
            //if (props.bufferDb.bufferId == 67)
            //{
            //    Console.WriteLine();
            //    foreach(dtPallet pl in props.bufferDb.pallets)
            //    {
            //        Console.WriteLine(pl.bay+"-"+pl.row+"-"+pl.palletStatus);
            //    }
            //}
            UpdateAllPalletStatus(props.bufferDb.pallets);
        }

        public void UpdateAllPalletStatus(List<dtPallet> listPallet)
        {
            BackgroundWorker workerUpdateAllPalletStatus = new BackgroundWorker();
            workerUpdateAllPalletStatus.WorkerSupportsCancellation = true;
            workerUpdateAllPalletStatus.WorkerReportsProgress = true;
            workerUpdateAllPalletStatus.DoWork += WorkerUpdateAllPalletStatus_DoWork;
            workerUpdateAllPalletStatus.ProgressChanged += WorkerUpdateAllPalletStatus_ProgressChanged;
            workerUpdateAllPalletStatus.RunWorkerCompleted += WorkerUpdateAllPalletStatus_RunWorkerCompleted;
            workerUpdateAllPalletStatus.RunWorkerAsync();
        }

        private void WorkerUpdateAllPalletStatus_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void WorkerUpdateAllPalletStatus_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void WorkerUpdateAllPalletStatus_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (dtPallet dr in props.bufferDb.pallets)
            {
                if (props._palletList.ContainsKey("Pallet" + "x" + dr.bay + "x" + dr.row))
                {
                    props._palletList["Pallet" + "x" + dr.bay + "x" + dr.row].StatusChanged(dr);
                }
            }
        }

        public void EditMenu(object sender, RoutedEventArgs e)
        {
            StationEditor stationEditor = new StationEditor(this, Thread.CurrentThread.CurrentCulture.ToString());
            stationEditor.Show();
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

            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                dtBuffer buffer = this.props.bufferDb;
                List<dtBuffer> buffers = new List<dtBuffer>();

                dynamic postApiBody = new JObject();
                Point coorLader = Global_Object.CoorLaser(props._posision);
                postApiBody.x = Math.Round(coorLader.X, 1);
                postApiBody.y = Math.Round(coorLader.Y, 1);
                postApiBody.angle = Math.Round(props._rotate, 1);
                dynamic data = JsonConvert.DeserializeObject(props.bufferDb.bufferData);
                postApiBody.arrange = data.arrange;
                string jsonBufferData = JsonConvert.SerializeObject(postApiBody);
                buffer.bufferData = jsonBufferData;

                buffers.Add(buffer);

                if (buffers.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                string jsonData = JsonConvert.SerializeObject(buffers);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "buffer/updateListBuffer");
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
        }

        public void RemoveMenu(object sender, RoutedEventArgs e)
        {
            Remove();
        }

        private void ChangeToolTipContent(object sender, ToolTipEventArgs e)
        {
            //ToolTip = "Tên: " + props.NameID +
            //    "\n Vị trí: " + Global_Object.CoorLaser(props._posision).X.ToString("0.00") + "," + Global_Object.CoorLaser(props._posision).Y.ToString("0.00") +
            //    " \n Dài: " + Height.ToString("0.00") + "m" +
            //    " \n Rộng: " + Width.ToString("0.00") + "m" +
            //    " \n Góc quay: " + props._rotate;
            ToolTip = "" + props.NameID;
        }

        public void Remove()
        {
            props._canvas.Children.Remove(this);
            //RemoveHandle(props.NameID);
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
