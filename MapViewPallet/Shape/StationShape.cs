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
            public string name; //Considered as label
            public int lines;
            public int palletsPerLine;
            public bool isSelected;
            public bool isHovering;
            public Canvas canvas;
            public Grid stationGrid; // Grid to hold all Pallet in Station
            public Point _posision; // Where station will be render,only accept Cavnas Coordinate
            public double rotate; // Station rotate

            public Border stationInfomation;
            public TranslateTransform myTranslate;
            public TransformGroup myTransformGroup;
            public RotateTransform myRotateTransform;
            public List<Point> eightCorner;
            public SortedDictionary<string, PalletShape> palletList;

            [CategoryAttribute("ID Settings"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public string NameID
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            [CategoryAttribute("Special Point"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point Point0
            {
                get
                {
                    return Global_Object.CoorLaser(eightCorner[0]);
                }
            }

            [CategoryAttribute("Special Point"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point Point1
            {
                get
                {
                    return Global_Object.CoorLaser(eightCorner[1]);
                }
            }

            [CategoryAttribute("Special Point"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point Point2
            {
                get
                {
                    return Global_Object.CoorLaser(eightCorner[2]);
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
                    return rotate;
                }
                set
                {
                    rotate = value;
                }
            }

            [CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public int NumberOfLines
            {
                get
                {
                    return lines;
                }
                set
                {
                    lines = value;
                }
            }

            [CategoryAttribute("Infomation"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public int PalletsPerLine
            {
                get
                {
                    return palletsPerLine;
                }
                set
                {
                    palletsPerLine = value;
                }
            }




        }
        public Properties stationProperties;
        public Props props;

        public StationShape(Canvas pCanvas, string stationName, int lines, int pallets_per_line, double rotate, string status)
        {
            Background = new SolidColorBrush(Colors.Transparent);
            //ToolTip = "";
            //ToolTipOpening += ChangeToolTipContent;
            props.stationInfomation = new Border();
            props.stationInfomation.Background = new SolidColorBrush(Colors.Red);
            props.stationInfomation.CornerRadius = new CornerRadius(1.3);
            props.stationInfomation.Height = 5;
            Grid.SetColumn(props.stationInfomation, 0);

            //Name = "Stationx" + Global_Mouse.EncodeTransmissionTimestamp(); //Object name
            Name = stationName; //Object name
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
            props.name = stationName; //label
            props.lines = lines;
            props.palletsPerLine = pallets_per_line;
            Width = 11 * props.lines;
            Height = 13 * props.palletsPerLine;
            props.palletList = new SortedDictionary<string, PalletShape>();
            props.myRotateTransform = new RotateTransform();
            props.myTranslate = new TranslateTransform();
            props.myTransformGroup = new TransformGroup();
            RenderTransformOrigin = new Point(0, 0);
            stationProperties = new Properties(this);
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new Thickness(0.3);
            CornerRadius = new CornerRadius(1.2);
            props.stationGrid = new Grid();
            //Create Columns
            //for (int lineIndex = 0; lineIndex < props.lines; lineIndex++)
            //{
            //    ColumnDefinition colTemp = new ColumnDefinition();
            //    colTemp.Name = Name + "xL" + lineIndex;
            //    props.stationGrid.ColumnDefinitions.Add(colTemp);
            //}
            ////Create Rows
            //for (int palletIndex = 0; palletIndex < props.palletsPerLine+1; palletIndex++) //Add 1 row for Infomation at top
            //{
            //    RowDefinition rowTemp = new RowDefinition();
            //    rowTemp.Name = Name + "xR" + palletIndex;
            //    props.stationGrid.RowDefinitions.Add(rowTemp);
            //}


            //Add Pallet to Grid
            for (int lineIndex = 0; lineIndex < props.lines; lineIndex++) //Column Index
            {
                //Create a Col
                ColumnDefinition colTemp = new ColumnDefinition();
                colTemp.Name = Name + "xL" + lineIndex;
                props.stationGrid.ColumnDefinitions.Add(colTemp);
                //Create GridLine
                Grid gridLine = new Grid();
                // Create BorderLine
                Border borderLine = new Border();
                Grid.SetColumn(borderLine, lineIndex);
                borderLine.Child = gridLine;
                //
                props.stationGrid.Children.Add(borderLine);
                if (lineIndex > 0)
                {
                    borderLine.BorderBrush = new SolidColorBrush(Colors.Black);
                    borderLine.BorderThickness = new Thickness(0.3, 0, 0, 0);
                }
                //Add Pallet to GridPallet ==> add GridPallet to BorderLine
                for (int palletIndex = 0; palletIndex < props.palletsPerLine; palletIndex++) //Row Index, start from 1, Row 0 use for Infomation
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
                    props.palletList.Add(palletTemp.Name, palletTemp);

                }
            }
            //==================SPECIALPOINT===================
            props.eightCorner = new List<Point>();
            for (int i = 0; i < 8; i++)
            {
                Point temp = new Point();
                props.eightCorner.Add(temp);
            }

            //==================CHILDREN===================
            //props.stationGrid.Children.Add(props.stationInfomation);
            Child = props.stationGrid;
            props.myTransformGroup.Children.Add(props.myRotateTransform);
            props.myTransformGroup.Children.Add(props.myTranslate);
            RenderTransform = props.myTransformGroup;
            props.rotate = rotate;
            props.myRotateTransform.Angle = props.rotate;
            props.myTranslate = new TranslateTransform(props._posision.X, props._posision.Y);
            props.myTransformGroup.Children[1] = props.myTranslate;
            props.canvas = pCanvas;
            props.canvas.Children.Add(this);
        }

        public void ReDraw(Point position)
        {

            props._posision = new Point(position.X, position.Y);
            Draw();
        }

        public void Draw()
        {
            props.myTranslate = new TranslateTransform(props._posision.X, props._posision.Y);
            props.myTransformGroup.Children[1] = props.myTranslate;
            // SPECIAL POINTS
            double width = 0.11285714 * props.lines;
            double height = 0.11285714 * props.lines;
            props.eightCorner[0] = CoorPointAtBorder(new Point((0), (Height / 2)));          //mid-left
            props.eightCorner[1] = CoorPointAtBorder(new Point((0), (0)));                 //top-left
            props.eightCorner[2] = CoorPointAtBorder(new Point((Width / 2), (0)));           //top-mid
            props.eightCorner[3] = CoorPointAtBorder(new Point((Width), (0)));             //top-right
            props.eightCorner[4] = CoorPointAtBorder(new Point((Width), (Height / 2)));      //mid-right
            props.eightCorner[5] = CoorPointAtBorder(new Point((Width), (Height)));        //bot-right
            props.eightCorner[6] = CoorPointAtBorder(new Point((Width / 2), (Height)));      //bot-mid
            props.eightCorner[7] = CoorPointAtBorder(new Point((0), (Height)));            //bot-left
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
            props.rotate += 90;
            if (props.rotate > 360)
            {
                props.rotate -= 360;
            }
            props.myRotateTransform.Angle = props.rotate;
            props.myTransformGroup.Children[0] = props.myRotateTransform;
            RenderTransform = props.myTransformGroup;
        }

        public void RemoveMenu(object sender, RoutedEventArgs e)
        {
            Remove();
        }

        private void ChangeToolTipContent(object sender, ToolTipEventArgs e)
        {
            ToolTip = "Name: " + props.name +
                "\n Position: " + Global_Object.CoorLaser(props._posision).X.ToString("0.00") + "," + Global_Object.CoorLaser(props._posision).Y.ToString("0.00") +
                " \n Width: " + Width.ToString("0.00") + "m" +
                " \n Height: " + Height.ToString("0.00") + "m" +
                " \n Rotate: " + props.rotate;
        }

        public void Remove()
        {
            props.canvas.Children.Remove(this);
            RemoveHandle(props.name);
        }

        public void Move(Point pos)
        {
            props.myTranslate = new TranslateTransform(pos.X, pos.Y);
            props.myRotateTransform = new RotateTransform(0);
            props.myTransformGroup.Children[1] = props.myTranslate;
        }

        public Point CoorPointAtBorder(Point pointOnBorder)
        {
            double xDiff = (pointOnBorder.X) - 0;
            double yDiff = (pointOnBorder.Y) - 0;
            double rad1 = (props.rotate * Math.PI) / 180;
            double rad2 = (Math.Atan2(yDiff, xDiff));
            double L = Global_Object.LengthBetweenPoints(new Point(0, 0), pointOnBorder);
            double x1 = props._posision.X + ((L * Math.Cos(rad1 + rad2)));
            double y1 = props._posision.Y + ((L * Math.Sin(rad1 + rad2)));
            return new Point(x1, y1);
        }
    }
}
