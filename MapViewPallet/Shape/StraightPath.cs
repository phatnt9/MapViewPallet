using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapViewPallet.Shape
{
    public class StraightPath : PathShape
    {
        public Line shape;
        public Ellipse point_Start;
        public Ellipse point_End;
        public Polygon arrow;
        public Point middle;
        public StraightPath(Canvas canvas) : base(canvas)
        {
            shape = new Line();
            point_Start = new Ellipse();
            point_End = new Ellipse();
            arrow = new Polygon();
            canvas.Children.Add(shape);
            canvas.Children.Add(point_Start);
            canvas.Children.Add(point_End);
            canvas.Children.Add(arrow);
        }

        public void Copy (StraightPath copy)
        {
            Name = copy.Name;
            shape = copy.shape;
            point_Start = copy.point_Start;
            point_End = copy.point_End;
            arrow = copy.arrow;
            middle = copy.middle;
        }

        public void DrawAxis(Point Start, Point End, Color color)
        {
            canvas.Children.Remove(point_End);
            //Middle point of straight path
            StartPoint = Start;
            EndPoint = End;
            // Point at Start and End

            point_End.Fill = new SolidColorBrush(color);
            point_End.Width = 6;
            point_End.Height = 6;
            point_End.RenderTransform = new TranslateTransform(End.X - 3, End.Y - 3);
            //Arrow
            arrow.Fill = new SolidColorBrush(color);
            arrow.Stroke = new SolidColorBrush(color);
            double xDiff = End.X - Start.X;
            double yDiff = End.Y - Start.Y;
            double rotate = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            RotateTransform myRotateTransform = new RotateTransform(rotate, End.X, End.Y);
            TranslateTransform myTranslate = new TranslateTransform(0, 0);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslate);
            arrow.RenderTransform = myTransformGroup;

            //3 Point of Triangle
            PointCollection points = new PointCollection(3);
            points.Add(new Point(End.X - 3, End.Y - 3));
            points.Add(new Point(End.X - 3, End.Y + 3));
            points.Add(new Point(End.X + 4, End.Y));
            arrow.Points = points;


            //Position the Path
            shape.X1 = Start.X;
            shape.Y1 = Start.Y;
            shape.X2 = End.X;
            shape.Y2 = End.Y;
            shape.Stroke = new SolidColorBrush(color); ;
            shape.StrokeThickness = StrokeThickness;
        }

        public void Draw(Point Start, Point End)
        {
            //Middle point of straight path
            StartPoint = Start;
            EndPoint = End;
            middle.X = (Start.X + End.X) / 2;
            middle.Y = (Start.Y + End.Y) / 2;

            // Point at Start and End

            point_Start.Fill = new SolidColorBrush(Colors.Red);
            point_End.Fill = new SolidColorBrush(Colors.Red);
            point_Start.Width = point_End.Width = 6;
            point_Start.Height = point_End.Height = 6;
            point_End.RenderTransform = new TranslateTransform(End.X - 3, End.Y - 3);
            point_Start.RenderTransform = new TranslateTransform(Start.X - 3, Start.Y - 3);

            //Arrow show direction
           
            arrow.Fill = new SolidColorBrush(Colors.Green);
            arrow.Stroke = Stroke;
            double xDiff = End.X - Start.X;
            double yDiff = End.Y - Start.Y;
            double rotate = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            RotateTransform myRotateTransform = new RotateTransform(rotate, middle.X, middle.Y);
            TranslateTransform myTranslate = new TranslateTransform(0, 0);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslate);
            arrow.RenderTransform = myTransformGroup;

            //3 Point of Triangle
            PointCollection points = new PointCollection(3);
            points.Add(new Point(middle.X - 3, middle.Y - 3));
            points.Add(new Point(middle.X - 3, middle.Y + 3));
            points.Add(new Point(middle.X + 4, middle.Y));
            arrow.Points = points;


            //Position the Path
            shape.X1 = Start.X;
            shape.Y1 = Start.Y;
            shape.X2 = End.X;
            shape.Y2 = End.Y;
            shape.Stroke = Stroke;
            shape.StrokeThickness = StrokeThickness;

            //Draw to Canvas
            

            //MessageBox.Show(middle.X + "  " + middle.Y);
        }

        public void remove()
        {
            canvas.Children.Remove(shape);
            canvas.Children.Remove(point_Start);
            canvas.Children.Remove(point_End);
            canvas.Children.Remove(arrow);
        }

       
    }
}
