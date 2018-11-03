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
   public class StraightPath:PathShape
    {
        

        public Line shape;
        Ellipse point_Start;
        Ellipse point_End;
        Polygon arrow;
        Point middle;
        public StraightPath(Canvas canvas):base(canvas)
        {
            shape = new Line();
        }
        public void Draw (Point Start, Point End)
        {
            //Middle point of straight path
            middle.X = (Start.X + End.X) / 2;
            middle.Y = (Start.Y + End.Y) / 2;
            
            // Point at Start and End
            point_Start = new Ellipse();
            point_End = new Ellipse();
            point_Start.Fill = new SolidColorBrush(Colors.Red);
            point_End.Fill = new SolidColorBrush(Colors.Red);
            point_Start.Width = point_End.Width = 6;
            point_Start.Height = point_End.Height = 6;
            point_End.RenderTransform = new TranslateTransform(End.X-3, End.Y-3);
            point_Start.RenderTransform = new TranslateTransform(Start.X-3, Start.Y-3);

            //Arrow show direction
            arrow = new Polygon();
            arrow.Fill = new SolidColorBrush(Colors.Green);
            arrow.Stroke = Stroke;
            double xDiff = End.X - Start.X;
            double yDiff = End.Y - Start.Y;
            double rotate = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            RotateTransform myRotateTransform = new RotateTransform(rotate, middle.X, middle.Y);
            TranslateTransform myTranslate = new TranslateTransform(0,0);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslate);
            arrow.RenderTransform = myTransformGroup;
            
            //3 Point of Triangle
            PointCollection points = new PointCollection(3);
            points.Add(new Point(middle.X-3, middle.Y-3));
            points.Add(new Point(middle.X-3, middle.Y+3));
            points.Add(new Point(middle.X+4, middle.Y));
            arrow.Points = points;


            //Position the Path
            shape.X1 = Start.X;
            shape.Y1 = Start.Y;
            shape.X2 = End.X;
            shape.Y2 = End.Y;
            shape.Stroke = Stroke;
            shape.StrokeThickness = StrokeThickness;

            //Draw to Canvas
            canvas.Children.Add(shape);
            canvas.Children.Add(point_Start);
            canvas.Children.Add(point_End);
            canvas.Children.Add(arrow);

            //MessageBox.Show(middle.X + "  " + middle.Y);
        }
    }
}
