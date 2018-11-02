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
        public void Draw(Point Start, Point End, bool Direction)
        {
            Point Middle = new Point(Direction? End.X: Start.X, Direction ? Start.Y : End.Y);
            BezierSegment bezierSegment = new BezierSegment(Start, Middle, End, true);
            PathSegmentCollection pathSegments = new PathSegmentCollection();
            pathSegments.Add(bezierSegment);
            shape.Stroke = Stroke;
            shape.StrokeThickness = StrokeThickness;
            canvas.Children.Add(shape);
        }
    }
}
