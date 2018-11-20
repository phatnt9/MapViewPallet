using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System;
using System.Windows.Input;
using System.Collections.Generic;

namespace MapViewPallet.Shape
{
    public class PathShape: Border
    {
        public event Action<string> RemoveHandle;
        public struct Props
        {
            public string name;
            public int coorStep;
            public bool isSelected;
            public bool isHovering;
            public Canvas canvas;
            public Grid pathGrid;
            public Point _oriMousePos;
            public Point _desMousePos;
            public Point _start;
            public Point _end;
            public Path _shape;
            public Ellipse _pointHead;
            public Ellipse _pointTail;
            public Polygon _arrow;
            public Point _middle;
            public PathFigure pathFigure;
            public PathGeometry pathGeometry;
            public PathSegmentCollection pathSegments;
            public double xDiff;
            public double yDiff;
            public double rotate;
            public double sizeArrow;
            public TranslateTransform myTranslate;
            public TransformGroup myTransformGroup;
            public RotateTransform myRotateTransform;
            public PointCollection points;
            public List<Point> eightCorner;

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

            [CategoryAttribute("Arrow Settings"), DescriptionAttribute(""), ReadOnlyAttribute(false)]
            public Visibility Visible
            {
                get
                {
                    return _arrow.Visibility;
                }
                set
                {
                    _arrow.Visibility = value;
                }
            }

            [CategoryAttribute("Shape Settings"), DescriptionAttribute(""), ReadOnlyAttribute(false)]
            public double Thickness
            {
                get
                {
                    return _shape.StrokeThickness;
                }
                set
                {
                    _shape.StrokeThickness = value;
                    _arrow.StrokeThickness = value;
                }
            }

            [CategoryAttribute("Point View"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point OriginalPos
            {
                get
                {
                    return Global_Object.CoorLaser(_oriMousePos);
                }
                set
                {
                    _oriMousePos = Global_Object.CoorCanvas(value);
                }
            }

            [CategoryAttribute("Point View"), DescriptionAttribute(""), ReadOnlyAttribute(true)]
            public Point DestinationPos
            {
                get
                {
                    return Global_Object.CoorLaser(_desMousePos);
                }
                set
                {
                    _desMousePos = Global_Object.CoorCanvas(value);
                }
            }
            



        }
        public Properties pathProperties;
        public Props props;
        //====================METHOD=============================
        public PathShape(Canvas pCanvas, Point Start, Point End)
        {
            ToolTip = "";
            ToolTipOpening += ChangeToolTipContent;
            props.sizeArrow = 1;
            props.coorStep = 10;
            props.isSelected = false;
            props.isHovering = false;
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
            MouseLeave += MouseLeavePath;
            MouseMove += MouseHoverPath;
            MouseLeftButtonDown += MouseLeftButtonDownPath;
            MouseRightButtonDown += MouseRightButtonDownPath;
            //===================CREATE=====================
            props.points = new PointCollection(3);
            for (int i=0;i<3;i++)
            {
                props.points.Add(new Point());
            }
            props._oriMousePos = new Point(Start.X,Start.Y);
            props._desMousePos = new Point(End.X, End.Y);
            props._shape = new Path();
            props._pointHead = new Ellipse();
            props._pointTail = new Ellipse();
            props._arrow = new Polygon();
            props.pathGrid = new Grid();
            props.pathSegments = new PathSegmentCollection();
            props.pathFigure = new PathFigure();
            props.pathGeometry = new PathGeometry();
            props.myRotateTransform = new RotateTransform();
            props.myTranslate = new TranslateTransform();
            props.myTransformGroup = new TransformGroup();
            pathProperties = new Properties(this);
            //===================STYLE=====================
            Background = new SolidColorBrush(Colors.Transparent);
            BorderBrush = new SolidColorBrush(Colors.Transparent);
            BorderThickness = new Thickness(0.1);
            props._pointHead.Width = props._pointTail.Width = 2;
            props._pointHead.Height = props._pointTail.Height = 2;
            props._pointHead.Fill = new SolidColorBrush(Colors.Red);
            props._pointTail.Fill = new SolidColorBrush(Colors.Red);
            props._pointHead.Visibility = Visibility.Hidden;
            props._pointTail.Visibility = Visibility.Hidden;
            props._arrow.Fill = new SolidColorBrush(Colors.Gray);
            props._arrow.Stroke = new SolidColorBrush(Colors.Gray);
            props._arrow.StrokeThickness = 0.7;
            props._shape.Stroke = new SolidColorBrush(Colors.Gray);
            props._shape.StrokeThickness = 0.7;
            //==================CHILDREN===================
            props.pathFigure.IsClosed = false;
            props.pathFigure.Segments = props.pathSegments;
            props.pathGeometry.Figures.Add(props.pathFigure);
            props._shape.Data = props.pathGeometry;
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
            props.pathGrid.Children.Add(props._shape);
            props.pathGrid.Children.Add(props._pointHead);
            props.pathGrid.Children.Add(props._pointTail);
            props.pathGrid.Children.Add(props._arrow);
            Child = props.pathGrid;
            props.canvas.Children.Add(this);
        }

        private void ChangeToolTipContent(object sender, ToolTipEventArgs e)
        {
            ToolTip = "Name: " + props.name + 
                "\n Start: " + props._oriMousePos.X.ToString("0.0") + "," + props._oriMousePos.Y.ToString("0.0") + 
                " \n End: " + props._desMousePos.X.ToString("0.0") + "," + props._desMousePos.Y.ToString("0.0") + 
                " \n Rotate: " + props.rotate;
        }

        private void EditMenu(object sender, RoutedEventArgs e)
        {
            pathProperties.ShowDialog();
        }

        private void RemoveMenu(object sender, RoutedEventArgs e)
        {
            Remove();
        }

        private void MouseRightButtonDownPath(object sender, MouseButtonEventArgs e)
        {
            string elementName = (e.OriginalSource as FrameworkElement).Name;
            Point mousePos = e.GetPosition(props.canvas);
        }

        private void MouseLeftButtonDownPath(object sender, MouseButtonEventArgs e)
        {
            string elementName = (e.OriginalSource as FrameworkElement).Name;
            Point mousePos = e.GetPosition(props.canvas);
            if (!props.isSelected)
            {
                props.isSelected = true;
            }
            else
            {
                props.isSelected = false;
            }
            ToggleStyle();
        }



        public void ToggleStyle ()
        {
            if (props.isSelected || props.isHovering)
            {
                BorderBrush = new SolidColorBrush(Colors.Black);
                props._shape.Stroke = new SolidColorBrush(Colors.Red);
                props._arrow.Fill = new SolidColorBrush(Colors.Red);
                props._arrow.Stroke = new SolidColorBrush(Colors.Red);
                props._pointHead.Visibility = Visibility.Visible;
                props._pointTail.Visibility = Visibility.Visible;
            }
            else
            {
                BorderBrush = new SolidColorBrush(Colors.Transparent);
                props._shape.Stroke = new SolidColorBrush(Colors.Gray);
                props._arrow.Fill = new SolidColorBrush(Colors.Gray);
                props._arrow.Stroke = new SolidColorBrush(Colors.Gray);
                props._pointHead.Visibility = Visibility.Hidden;
                props._pointTail.Visibility = Visibility.Hidden;

            }
        }
        

        private void MouseHoverPath(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(props.canvas);
            props.isHovering = true;
            ToggleStyle();
        }

        private void MouseLeavePath(object sender, MouseEventArgs e)
        {
            props.isHovering = false;
            ToggleStyle();
        }
        

        public void ReDraw(Point Start, Point End)
        {
            props._oriMousePos = new Point(Start.X, Start.Y);
            props._desMousePos = new Point(End.X, End.Y);
            Draw();
        }

        public virtual void Draw()
        {
        }

        public void Remove()
        {
            props.canvas.Children.Remove(this);
            RemoveHandle(props.name);
        }

        public void ShowPropertiesGrid ()
        {

        }
    }
}
