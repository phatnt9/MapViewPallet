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
        public CurvePath(Canvas canvas) : base(canvas)
        {
            shape = new Path();
        }
        public void Draw(Point Start, Point End, string Direction)
        {
            Ellipse point_Start = new Ellipse();
            Ellipse point_End = new Ellipse();
            point_Start.Fill = new SolidColorBrush(Colors.Red);
            point_End.Fill = new SolidColorBrush(Colors.Red);
            point_Start.Width = point_End.Width = 6;
            point_Start.Height = point_End.Height = 6;
            point_End.RenderTransform = new TranslateTransform(End.X - 3, End.Y - 3);
            point_Start.RenderTransform = new TranslateTransform(Start.X - 3, Start.Y - 3);

            Point Middle;
            if (Start.Y > End.Y)
            {
                Middle = new Point((Direction == "up") ? Start.X : End.X, (Direction == "up") ? End.Y : Start.Y);
            }
            else
            {
                Middle = new Point((Direction == "up") ? End.X : Start.X, (Direction == "up") ? Start.Y : End.Y);
            }

            BezierSegment bezierSegment = new BezierSegment(Start, Middle, End, true);
            PathSegmentCollection pathSegments = new PathSegmentCollection();
            pathSegments.Add(bezierSegment);
            PathFigure pathFigure = new PathFigure(Start, pathSegments,false);
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            shape.Data = pathGeometry;
            shape.Stroke = Stroke;
            shape.StrokeThickness = StrokeThickness;

            canvas.Children.Add(shape);
            canvas.Children.Add(point_Start);
            canvas.Children.Add(point_End);
        }
    }
}
