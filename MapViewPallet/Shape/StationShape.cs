using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.ComponentModel;

namespace MapViewPallet.Shape
{
    public class StationShape : Border
    {
        public event Action<string> RemoveHandle;

        public struct Props
        {
            private string _name; //Considered as label
            private int _bays;
            private int _rows;
            public bool isSelected;
            public bool isHovering;
            public Canvas _canvas;
            public Grid _stationGrid; // Grid to hold all Pallet in Station
            public Point _posision; // Where station will be render,only accept Cavnas Coordinate
            public double _rotate; // Station rotate

            public Border _stationInfomation;
            public TranslateTransform _myTranslate;
            public TransformGroup _myTransformGroup;
            public RotateTransform _myRotateTransform;
            public List<Point> _eightCorner;
            public SortedDictionary<string, PalletShape> _palletList;

            [CategoryAttribute("ID Settings"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public string NameID
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }

            [CategoryAttribute("Special Point"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point Point0
            {
                get
                {
                    return Global_Object.CoorLaser(_eightCorner[0]);
                }
            }

            [CategoryAttribute("Special Point"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point Point1
            {
                get
                {
                    return Global_Object.CoorLaser(_eightCorner[1]);
                }
            }

            [CategoryAttribute("Special Point"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point Point2
            {
                get
                {
                    return Global_Object.CoorLaser(_eightCorner[2]);
                }
            }

            [CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point Position
            {
                get
                {
                    return Global_Object.CoorLaser(_posision);
                }
            }

            [CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public double Rotation
            {
                get
                {
                    return _rotate;
                }
                set
                {
                    _rotate = value;
                }
            }

            [CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public int Bays
            {
                get
                {
                    return _bays;
                }
                set
                {
                    _bays = value;
                }
            }

            [CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public int Rows
            {
                get
                {
                    return _rows;
                }
                set
                {
                    _rows = value;
                }
            }




        }
        public Properties stationProperties;
        public Props props;

        public StationShape(Canvas pCanvas, string stationName, int bays, int rows, double rotate, string status)
        {
            Background = new SolidColorBrush(Colors.Transparent);
            //ToolTip = "";
            //ToolTipOpening += ChangeToolTipContent;
            props._stationInfomation = new Border();
            props._stationInfomation.Background = new SolidColorBrush(Colors.Red);
            props._stationInfomation.CornerRadius = new CornerRadius(1.3);
            props._stationInfomation.Height = 5;
            Grid.SetColumn(props._stationInfomation, 0);

            Name = "Stationx" + Global_Mouse.EncodeTransmissionTimestamp(); //Object name
            //Name = stationName; //Object name
            ContextMenu = new ContextMenu();
            //===================================
            MenuItem editItem = new MenuItem();
            editItem.Header = "Edit";
            editItem.Click += EditMenu;
            //===================================
            MenuItem rotateItem = new MenuItem();
            rotateItem.Header = "Rotate";
            rotateItem.Click += RotateMenu;
            //===================================
            MenuItem removeItem = new MenuItem();
            removeItem.Header = "Remove";
            removeItem.Click += RemoveMenu;
            ContextMenu.Items.Add(editItem);
            ContextMenu.Items.Add(rotateItem);
            ContextMenu.Items.Add(removeItem);
            //====================EVENT=====================
            //MouseLeave += MouseLeaveStation;
            //MouseMove += MouseHoverStation;
            //MouseLeftButtonDown += MouseLeftButtonDownStation;
            //MouseRightButtonDown += MouseRightButtonDownStation;
            //===================CREATE=====================
            props.NameID = stationName; //label
            props.Bays = bays;
            props.Rows = rows;
            Width = 11 * props.Bays;
            Height = 13 * props.Rows;
            props._palletList = new SortedDictionary<string, PalletShape>();
            props._myRotateTransform = new RotateTransform();
            props._myTranslate = new TranslateTransform();
            props._myTransformGroup = new TransformGroup();
            RenderTransformOrigin = new Point(0, 0);
            stationProperties = new Properties(this);
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new Thickness(0.3);
            CornerRadius = new CornerRadius(1.2);
            props._stationGrid = new Grid();
            


            //Add Pallet to Grid
            for (int lineIndex = 0; lineIndex < props.Bays; lineIndex++) //Column Index
            {
                //Create a Col
                ColumnDefinition colTemp = new ColumnDefinition();
                colTemp.Name = Name + "xL" + lineIndex;
                props._stationGrid.ColumnDefinitions.Add(colTemp);
                //Create GridLine
                Grid gridLine = new Grid();
                // Create BorderLine
                Border borderLine = new Border();
                Grid.SetColumn(borderLine, lineIndex);
                borderLine.Child = gridLine;
                //
                props._stationGrid.Children.Add(borderLine);
                if (lineIndex > 0)
                {
                    borderLine.BorderBrush = new SolidColorBrush(Colors.Black);
                    borderLine.BorderThickness = new Thickness(0.3, 0, 0, 0);
                }
                //Add Pallet to GridPallet ==> add GridPallet to BorderLine
                for (int palletIndex = 0; palletIndex < props.Rows; palletIndex++) //Row Index, start from 1, Row 0 use for Infomation
                {
                    //Create Rows for Col
                    RowDefinition rowTemp = new RowDefinition();
                    rowTemp.Name = Name + "xR" + palletIndex;
                    //rowTemp.MinHeight = 10;
                    gridLine.RowDefinitions.Add(rowTemp);
                    //=============

                    PalletShape palletTemp = new PalletShape(Name + "x" + lineIndex + "x" + palletIndex, status);
                    Grid.SetRow(palletTemp, palletIndex);
                    gridLine.Children.Add(palletTemp);
                    props._palletList.Add(palletTemp.Name, palletTemp);

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
            props._myTransformGroup.Children.Add(props._myTranslate);
            RenderTransform = props._myTransformGroup;
            props._rotate = rotate;
            props._myRotateTransform.Angle = props._rotate;
            props._myTranslate = new TranslateTransform(props._posision.X, props._posision.Y);
            props._myTransformGroup.Children[1] = props._myTranslate;
            props._canvas = pCanvas;
            props._canvas.Children.Add(this);
        }

        public void ReDraw(Point position)
        {

            props._posision = new Point(position.X, position.Y);
            Draw();
        }

        public void Draw()
        {
            props._myTranslate = new TranslateTransform(props._posision.X, props._posision.Y);
            props._myTransformGroup.Children[1] = props._myTranslate;
            // SPECIAL POINTS
            double width = 0.11285714 * props.Bays;
            double height = 0.11285714 * props.Bays;
            props._eightCorner[0] = CoorPointAtBorder(new Point((0), (Height / 2)));          //mid-left
            props._eightCorner[1] = CoorPointAtBorder(new Point((0), (0)));                 //top-left
            props._eightCorner[2] = CoorPointAtBorder(new Point((Width / 2), (0)));           //top-mid
            props._eightCorner[3] = CoorPointAtBorder(new Point((Width), (0)));             //top-right
            props._eightCorner[4] = CoorPointAtBorder(new Point((Width), (Height / 2)));      //mid-right
            props._eightCorner[5] = CoorPointAtBorder(new Point((Width), (Height)));        //bot-right
            props._eightCorner[6] = CoorPointAtBorder(new Point((Width / 2), (Height)));      //bot-mid
            props._eightCorner[7] = CoorPointAtBorder(new Point((0), (Height)));            //bot-left
        }

        public void EditMenu(object sender, RoutedEventArgs e)
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
            props._myTranslate = new TranslateTransform(pos.X, pos.Y);
            props._myRotateTransform = new RotateTransform(0);
            props._myTransformGroup.Children[1] = props._myTranslate;
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
    }
}
