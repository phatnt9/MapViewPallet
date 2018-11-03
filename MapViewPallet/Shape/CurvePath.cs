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
    public class CurvePath: PathShape
    {
        public Path shape;
        Ellipse point_Start;
        Ellipse point_End;
        Point Control;
        Polygon arrow;
        Point middle;
        public CurvePath(Canvas canvas) : base(canvas)
        {
            shape = new Path();
        }
        public void Draw(Point Start, Point End, string Direction)
        {
            //Control point of Path
            if (Start.Y > End.Y)
            {
                Control = new Point((Direction == "up") ? Start.X : End.X, (Direction == "up") ? End.Y : Start.Y);
            }
            else
            {
                Control = new Point((Direction == "up") ? End.X : Start.X, (Direction == "up") ? Start.Y : End.Y);
            }
            //Middle point of straight path
            //-----------Middle point-------------
            double t = 0.6; // given example value
            //middle.X = (1 - t) * (1 - t) * Start.X + 2 * (1 - t) * t * Control.X + t * t * End.X;
            //middle.Y = (1 - t) * (1 - t) * Start.Y + 2 * (1 - t) * t * Control.Y + t * t * End.Y;

            middle.X = (1 - t) * (1 - t) * (1 - t) * Start.X + 3 * (1 - t) * (1 - t) * t * Start.X + 3 * (1 - t) * t * t * Control.X + t * t * t * End.X;
            middle.Y = (1 - t) * (1 - t) * (1 - t) * Start.Y + 3 * (1 - t) * (1 - t) * t * Start.Y + 3 * (1 - t) * t * t * Control.Y + t * t * t * End.Y;
            //--------------------------------


            // Point at Start and End
            point_Start = new Ellipse();
            point_End = new Ellipse();
            point_Start.Fill = new SolidColorBrush(Colors.Red);
            point_End.Fill = new SolidColorBrush(Colors.Red);
            point_Start.Width = point_End.Width = 6;
            point_Start.Height = point_End.Height = 6;
            point_End.RenderTransform = new TranslateTransform(End.X - 3, End.Y - 3);
            point_Start.RenderTransform = new TranslateTransform(Start.X - 3, Start.Y - 3);

            //Arrow show direction
            arrow = new Polygon();
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






            BezierSegment bezierSegment = new BezierSegment(Start, Control, End, true);
            PathSegmentCollection pathSegments = new PathSegmentCollection();
            pathSegments.Add(bezierSegment);
            PathFigure pathFigure = new PathFigure(Start, pathSegments,false);
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            shape.Data = pathGeometry;
            shape.Stroke = Stroke;
            shape.StrokeThickness = StrokeThickness;


            //Draw to Canvas
            canvas.Children.Add(shape);
            canvas.Children.Add(point_Start);
            canvas.Children.Add(point_End);
            canvas.Children.Add(arrow);
        }
    }
}
