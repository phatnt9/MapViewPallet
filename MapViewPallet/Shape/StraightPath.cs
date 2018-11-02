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
            arrow.Fill = Stroke;
            arrow.Stroke = Stroke;
            RotateTransform myRotateTransform = new RotateTransform();
            myRotateTransform.Angle = 0;
            TranslateTransform myTranslate = new TranslateTransform(middle.X, middle.Y);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslate);
            arrow.RenderTransform = myTransformGroup;
            PointCollection points = new PointCollection(3);
            points.Add(new Point(middle.X-2, middle.Y-2));
            points.Add(new Point(middle.X-2, middle.Y+2));
            points.Add(new Point(middle.X+2, middle.Y));
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
            //canvas.Children.Add(point_Start);
            //canvas.Children.Add(point_End);
            //canvas.Children.Add(arrow);
        }
    }
}
