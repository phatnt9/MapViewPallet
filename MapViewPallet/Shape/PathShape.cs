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
        public string _name;
        public Properties pathProperties;
        public Grid pathGrid;
        public Point _start { get; set; }
        public Point _end { get; set; }
        public double _strokeThickness = 10;
        public SolidColorBrush _stroke = new SolidColorBrush(Colors.Black);
        public Path _shape;
        public Ellipse _pointHead;
        public Ellipse _pointTail;
        public Polygon _arrow;
        public Point _middle;
        public Canvas canvas;
        public PathShape(Canvas canvas, Point Start, Point End)
        {
            pathGrid = new Grid();
            _shape = new Path();
            _pointHead = new Ellipse();
            _pointTail = new Ellipse();
            _arrow = new Polygon();


            double x1 = Math.Min(Start.X, End.X);
            double y1 = Math.Min(Start.Y, End.Y);
            Width = 50;// Math.Max(_start.X, _end.X) - Math.Min(_start.X, _end.X);
            Height = 50;// Math.Max(_start.Y, _end.Y) - Math.Min(_start.Y, _end.Y);
            BorderBrush = new SolidColorBrush(Colors.Green);
            BorderThickness = new Thickness(2);
            
            this.canvas = canvas;
            pathGrid.Children.Add(_shape);
            pathGrid.Children.Add(_pointHead);
            pathGrid.Children.Add(_pointTail);
            pathGrid.Children.Add(_arrow);
            Child = pathGrid;
            Move(new Point(x1, y1));
            this.canvas.Children.Add(this);
            pathProperties = new Properties(this);
        }

        private void Path_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mouseWasDownOn = e.Source as FrameworkElement;
            Console.WriteLine("chuot phai"+ Global_Object.Foo(sender));
        }

        public virtual void Draw()
        {
        }

        public void Move(Point pos)
        {
            TranslateTransform a = new TranslateTransform(pos.X, pos.Y);
            RotateTransform b = new RotateTransform(45);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(a);
            //myTransformGroup.Children.Add(b);
            RenderTransform = myTransformGroup;
        }

        public void ShowPropertiesGrid ()
        {

        }

        [CategoryAttribute("ID Settings"), DescriptionAttribute("")]
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
