using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Collections.Generic;

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
            public TranslateTransform myTranslate;
            public TransformGroup myTransformGroup;
            public RotateTransform myRotateTransform;
            public List<Point> eightCorner;
        }
        public Properties stationProperties;
        public Props props;

        public StationShape(Canvas pCanvas, string stationName, int lines, int pallets_per_line, string typePallet)
        {
            ToolTip = "";
            Name = "Stationx" + Global_Mouse.EncodeTransmissionTimestamp(); //Object name
            ContextMenu = new ContextMenu();
            //===================================
            MenuItem editItem = new MenuItem();
            editItem.Header = "Edit";
            editItem.Click += EditMenu;
            //===================================
            MenuItem removeItem = new MenuItem();
            removeItem.Header = "Remove";
            removeItem.Click += RemoveMenu;
            ContextMenu.Items.Add(editItem);
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
            RenderTransformOrigin = new Point(0.5, 0.5);
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
            for (int palletIndex = 0; palletIndex < props.palletsPerLine; palletIndex++)
            {
                RowDefinition rowTemp = new RowDefinition();
                rowTemp.Name = Name + "xR" + palletIndex;
                props.stationGrid.RowDefinitions.Add(rowTemp);
            }
            //Add Pallet to Grid
            for (int lineIndex = 0; lineIndex < props.lines; lineIndex++)
            {
                for (int palletIndex = 0; palletIndex < props.palletsPerLine; palletIndex++)
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
            Child = props.stationGrid;
            props.myTransformGroup.Children.Add(props.myRotateTransform);
            props.myTransformGroup.Children.Add(props.myTranslate);
            RenderTransform = props.myTransformGroup;
            props.rotate = 0;
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

        private void EditMenu(object sender, RoutedEventArgs e)
        {
            stationProperties.ShowDialog();
        }

        private void RemoveMenu(object sender, RoutedEventArgs e)
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
    }
}
