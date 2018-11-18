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
            public string name;
            public bool isSelected;
            public bool isHovering;
            public Canvas canvas;
            public Grid stationGrid;
            public Point _posision;
            public double xDiff;
            public double yDiff;
            public double rotate;
            public double sizeArrow;
            public TranslateTransform myTranslate;
            public TransformGroup myTransformGroup;
            public RotateTransform myRotateTransform;
            public List<Point> eightCorner;
        }
        public Properties stationProperties;
        public Props props;


        public StationShape()
        {
        }

        public StationShape(Canvas pCanvas, string stationName, int lines, int pallets_per_line, string typePallet)
        {
            ToolTip = "";
            Name = "Stationx" + Global_Mouse.EncodeTransmissionTimestamp();
            ContextMenu = new ContextMenu();
            //===================================
            MenuItem editItem = new MenuItem();
            editItem.Header = "Edit";
            //editItem.Click += EditMenu;
            //===================================
            MenuItem removeItem = new MenuItem();
            removeItem.Header = "Remove";
            //removeItem.Click += RemoveMenu;
            ContextMenu.Items.Add(editItem);
            ContextMenu.Items.Add(removeItem);
            //====================EVENT=====================
            //MouseLeave += MouseLeaveStation;
            //MouseMove += MouseHoverStation;
            //MouseLeftButtonDown += MouseLeftButtonDownStation;
            //MouseRightButtonDown += MouseRightButtonDownStation;
            //===================CREATE=====================

            props.myRotateTransform = new RotateTransform();
            props.myTranslate = new TranslateTransform();
            props.myTransformGroup = new TransformGroup();

            Padding = new Thickness(0);
            props.name = stationName;
            BorderBrush = new SolidColorBrush(Colors.Red);
            BorderThickness = new Thickness(0.1);
            props.stationGrid = new Grid();
            for (int lineIndex = 0; lineIndex < lines; lineIndex++)
            {
                ColumnDefinition colTemp = new ColumnDefinition();
                colTemp.Name = Name + "x" + lineIndex;
                props.stationGrid.ColumnDefinitions.Add(colTemp);
            }
            for (int palletIndex = 0; palletIndex < pallets_per_line; palletIndex++)
            {
                RowDefinition rowTemp = new RowDefinition();
                rowTemp.Name = Name + "x" + palletIndex;
                props.stationGrid.RowDefinitions.Add(rowTemp);
            }
            for (int lineIndex = 0; lineIndex < lines; lineIndex++)
            {
                for (int palletIndex = 0; palletIndex < pallets_per_line; palletIndex++)
                {
                    PalletShape borderTemp = new PalletShape(typePallet);
                    borderTemp.Name = Name + "x" + lineIndex + "x" + palletIndex;
                    Grid.SetColumn(borderTemp, lineIndex);
                    Grid.SetRow(borderTemp, palletIndex);
                    props.stationGrid.Children.Add(borderTemp);
                }
            }
            //==================CHILDREN===================
            
            props.myTransformGroup.Children.Add(props.myRotateTransform);
            props.myTransformGroup.Children.Add(props.myTranslate);
            RenderTransform = props.myTransformGroup;
            props.canvas = pCanvas;
            props.eightCorner = new List<Point>();
            for (int i = 0; i < 8; i++)
            {
                Point temp = new Point();
                props.eightCorner.Add(temp);
            }
            Child = props.stationGrid;
            props.canvas.Children.Add(this);
            props.rotate = 0;
            props.myRotateTransform.Angle = props.rotate;
            props.myTranslate = new TranslateTransform(props._posision.X, props._posision.Y);
            props.myTransformGroup.Children[1] = props.myTranslate;

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
