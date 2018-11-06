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
        public Point _oriMouse;
        public Point _desMouse;
        public Point _start;
        public Point _end;
        public double _strokeThickness = 1;
        public SolidColorBrush _stroke = new SolidColorBrush(Colors.Black);
        public Path _shape;
        public Ellipse _pointHead;
        public Ellipse _pointTail;
        public Polygon _arrow;
        public Point _middle;
        public Canvas canvas;
        public PathShape(Canvas canvas, Point Start, Point End)
        {


            Ellipse xxx = new Ellipse();
            xxx.Width = 5;
            xxx.Height = 5;



            _oriMouse = new Point(Start.X, Start.Y);
            _desMouse = new Point(End.X,End.Y);

            double x1 = Math.Min(_oriMouse.X, _desMouse.X);
            double y1 = Math.Min(_oriMouse.Y, _desMouse.Y);
            //Move(new Point(x1, y1));
            Width = 50;// Math.Max(_start.X, _end.X) - Math.Min(_start.X, _end.X);
            Height = 50;// Math.Max(_start.Y, _end.Y) - Math.Min(_start.Y, _end.Y);

            BorderBrush = new SolidColorBrush(Colors.Green);
            BorderThickness = new Thickness(1);
            
            pathGrid = new Grid();
            _shape = new Path();
            _pointHead = new Ellipse();
            _pointTail = new Ellipse();
            _arrow = new Polygon();

            double xDiff = _desMouse.X - _oriMouse.X;
            double yDiff = _desMouse.Y - _oriMouse.Y;
            double rotate = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            switch(rotate)
            {

            }
            Console.WriteLine(rotate+"   "+ Math.Atan2(yDiff, xDiff));
            RotateTransform myRotateTransform = new RotateTransform(rotate, _middle.X, _middle.Y);
            TranslateTransform myTranslate = new TranslateTransform(_oriMouse.X, _oriMouse.Y);
            TransformGroup myTransformGroup = new TransformGroup();
            //myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslate);
            RenderTransform = myTransformGroup;

            this.canvas = canvas;
            pathGrid.Children.Add(_shape);
            pathGrid.Children.Add(_pointHead);
            pathGrid.Children.Add(_pointTail);
            pathGrid.Children.Add(_arrow);
            Child = pathGrid;
            this.canvas.Children.Add(this);
            pathProperties = new Properties(this);
        }
        

        public virtual void Draw()
        {
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
