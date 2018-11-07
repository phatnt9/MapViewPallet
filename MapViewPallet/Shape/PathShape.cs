using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System;
using System.Windows.Input;

namespace MapViewPallet.Shape
{
    public class PathShape: Border
    {
        public Canvas canvas;
        public Properties pathProperties;
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
        public double sizeArrow = 4;
        public TranslateTransform myTranslate;
        public TransformGroup myTransformGroup;
        public RotateTransform myRotateTransform;
        public PathShape(Canvas pCanvas, Point Start, Point End)
        {
            Background = new SolidColorBrush(Colors.Transparent);
            //====================EVENT=====================
            MouseEnter += MouseEnterPath;
            MouseLeave += MouseLeavePath;
            MouseMove += MouseHoverPath;
            MouseLeftButtonDown += MouseLeftButtonDownPath;
            //===================CREATE=====================
            PointCollection points = new PointCollection(3);
            _oriMousePos = new Point(Start.X, Start.Y);
            _desMousePos = new Point(End.X,End.Y);
            _shape = new Path();
            _pointHead = new Ellipse();
            _pointTail = new Ellipse();
            _arrow = new Polygon();
            pathGrid = new Grid();
            pathSegments = new PathSegmentCollection();
            pathFigure = new PathFigure();
            pathGeometry = new PathGeometry();
            myRotateTransform = new RotateTransform();
            myTranslate = new TranslateTransform();
            myTransformGroup = new TransformGroup();
            pathProperties = new Properties(this);
            //===================STYLE=====================
            BorderBrush = new SolidColorBrush(Colors.Transparent);
            BorderThickness = new Thickness(1);
            _pointHead.Width = _pointTail.Width = 6;
            _pointHead.Height = _pointTail.Height = 6;
            _pointHead.Fill = new SolidColorBrush(Colors.Red);
            _pointTail.Fill = new SolidColorBrush(Colors.Red);
            _pointHead.Visibility = Visibility.Hidden;
            _pointTail.Visibility = Visibility.Hidden;
            _arrow.Fill = new SolidColorBrush(Colors.Yellow);
            _arrow.Stroke = new SolidColorBrush(Colors.Gray);
            _shape.Stroke = new SolidColorBrush(Colors.Gray);
            _shape.StrokeThickness = 1;
            //==================CHILDREN===================
            pathFigure.IsClosed = false;
            pathFigure.Segments = pathSegments;
            pathGeometry.Figures.Add(pathFigure);
            _shape.Data = pathGeometry;
            myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslate);
            RenderTransform = myTransformGroup;
            canvas = pCanvas;
            pathGrid.Children.Add(_shape);
            pathGrid.Children.Add(_pointHead);
            pathGrid.Children.Add(_pointTail);
            pathGrid.Children.Add(_arrow);
            Child = pathGrid;
            canvas.Children.Add(this);
        }

        private void MouseLeftButtonDownPath(object sender, MouseButtonEventArgs e)
        {
            
        }


        private void MouseHoverPath(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(canvas);
            _shape.Stroke = new SolidColorBrush(Colors.Black);
            _arrow.Fill = new SolidColorBrush(Colors.Red);
            _arrow.Stroke = new SolidColorBrush(Colors.Black);
            _pointHead.Visibility = Visibility.Visible;
            _pointTail.Visibility = Visibility.Visible;
        }

        private void MouseLeavePath(object sender, MouseEventArgs e)
        {
            _shape.Stroke = new SolidColorBrush(Colors.Gray);
            _arrow.Fill = new SolidColorBrush(Colors.Yellow);
            _arrow.Stroke = new SolidColorBrush(Colors.Gray);
            _pointHead.Visibility = Visibility.Hidden;
            _pointTail.Visibility = Visibility.Hidden;
        }

        private void MouseEnterPath(object sender, MouseEventArgs e)
        {
            
        }

        public void ReDraw(Point Start, Point End)
        {
            _oriMousePos = new Point(Start.X, Start.Y);
            _desMousePos = new Point(End.X, End.Y);
            Draw();
        }

        public virtual void Draw()
        {
        }
        

        public void ShowPropertiesGrid ()
        {

        }

        [CategoryAttribute("ID Settings"), DescriptionAttribute("")]
        public string NameID{get{return Name;}set{Name = value;}}

        [CategoryAttribute("Shape Settings"), DescriptionAttribute("Change stroke color.")]
        public Brush Stroke
        {
            get
            {
                return _shape.Stroke;
            }
            set
            {
                _shape.Stroke = value;
            }
        }





    }
}
