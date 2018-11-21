using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Shapes;

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
            public Point _posision; // Where station will be render
            public double xDiff;
            public double yDiff;
            public double rotate; // Station rotate
            public Rectangle cornerIndicate;
            public TranslateTransform myTranslate;
            public TransformGroup myTransformGroup;
            public RotateTransform myRotateTransform;
            public List<Point> eightCorner;
        }
        public Properties stationProperties;
        public Props props;

        public StationShape(Canvas pCanvas, string stationName, int lines, int pallets_per_line, double rotate, string typePallet)
        {
            Background = new SolidColorBrush(Colors.Transparent);
            ToolTip = "";
            props.cornerIndicate = new Rectangle();
            props.cornerIndicate.Fill = new SolidColorBrush(Colors.MediumVioletRed);
            props.cornerIndicate.Height = 5;
            Grid.SetColumn(props.cornerIndicate, 0);
            Grid.SetRow(props.cornerIndicate, 0);

            Name = "Stationx" + Global_Mouse.EncodeTransmissionTimestamp(); //Object name
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
            props.myRotateTransform = new RotateTransform();
            props.myTranslate = new TranslateTransform();
            props.myTransformGroup = new TransformGroup();
            RenderTransformOrigin = new Point(0, 0);
            stationProperties = new Properties(this);
            BorderBrush = new SolidColorBrush(Colors.Red);
            BorderThickness = new Thickness(0.1);
            props.stationGrid = new Grid();
            //Create Columns
            for (int lineIndex = 0; lineIndex < props.lines; lineIndex++)
            {
                ColumnDefinition colTemp = new ColumnDefinition();
                colTemp.Name = Name + "xL" + lineIndex;
                props.stationGrid.ColumnDefinitions.Add(colTemp);
            }
            //Create Rows
            for (int palletIndex = 0; palletIndex < props.palletsPerLine+1; palletIndex++) //Add 1 row for Infomation at top
            {
                RowDefinition rowTemp = new RowDefinition();
                rowTemp.Name = Name + "xR" + palletIndex;
                props.stationGrid.RowDefinitions.Add(rowTemp);
            }
            //Add Pallet to Grid
            for (int lineIndex = 0; lineIndex < props.lines; lineIndex++) //Column Index
            {
                for (int palletIndex = 1; palletIndex < props.palletsPerLine+1; palletIndex++) //Row Index, start from 1, Row 0 use for Infomation
                {
                    PalletShape borderTemp = new PalletShape(typePallet);
                    borderTemp.Name = Name + "x" + lineIndex + "x" + palletIndex;
                    Grid.SetColumn(borderTemp, lineIndex);
                    Grid.SetRow(borderTemp, palletIndex);
                    props.stationGrid.Children.Add(borderTemp);
                }
            }

            props.eightCorner = new List<Point>();
            for (int i = 0; i < 8; i++)
            {
                Point temp = new Point();
                props.eightCorner.Add(temp);
            }
            //==================CHILDREN===================
            props.stationGrid.Children.Add(props.cornerIndicate);
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
        }

        public void EditMenu(object sender, RoutedEventArgs e)
        {
            stationProperties.ShowDialog();
        }
        private void RotateMenu(object sender, RoutedEventArgs e)
        {
            double rotate = props.rotate * Math.PI / 180.0;
            rotate = (rotate + (Math.PI / 2));
            props.rotate = (rotate * 180.0 / Math.PI);
            props.myRotateTransform.Angle = props.rotate;
            props.myTransformGroup.Children[0] = props.myRotateTransform;
            RenderTransform = props.myTransformGroup;
        }

        public void RemoveMenu(object sender, RoutedEventArgs e)
        {
            Remove();
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
            double L = Global_Object.LengthBetweenPoints(new Point(0,0), pointOnBorder);
            double x1 = props._posision.X + ((L * Math.Cos(rad1 + rad2)));
            double y1 = props._posision.Y + ((L * Math.Sin(rad1 + rad2)));
            return new Point(x1, y1);
        }
    }
}
