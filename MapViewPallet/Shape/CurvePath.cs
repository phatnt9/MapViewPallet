using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapViewPallet.Shape
{
    public class CurvePath: PathShape
    {
        Point Control;
        bool direction; // true= Up, false = Down
        public CurvePath(Canvas canvas, Point Start, Point End, bool direction) : base(canvas, Start, End)
        {
            this.direction = direction;
            _name = "CurvePath-" + Global_Mouse.EncodeTransmissionTimestamp();
            //Draw(Start, End);
        }
        public void Draw(Point Start, Point End)
        {
            _start = Start;
            _end = End;
            //Control point of Path
            if (_start.Y > _end.Y)
            {
                Control = new Point(direction ? _start.X : _end.X, direction ? _end.Y : _start.Y);
            }
            else
            {
                Control = new Point(direction ? _end.X : _start.X, direction ? _start.Y : _end.Y);
            }
            //Middle point of curve path
            //-----------Middle point-------------
            double t = 0.65;
            _middle.X = (1 - t) * (1 - t) * (1 - t) * _start.X + 3 * (1 - t) * (1 - t) * t * _start.X + 3 * (1 - t) * t * t * Control.X + t * t * t * _end.X;
            _middle.Y = (1 - t) * (1 - t) * (1 - t) * _start.Y + 3 * (1 - t) * (1 - t) * t * _start.Y + 3 * (1 - t) * t * t * Control.Y + t * t * t * _end.Y;
            //--------------------------------


            // Point at _start and _end
            _pointHead.Fill = new SolidColorBrush(Colors.Red);
            _pointTail.Fill = new SolidColorBrush(Colors.Red);
            _pointHead.Width = _pointTail.Width = 6;
            _pointHead.Height = _pointTail.Height = 6;
            _pointTail.RenderTransform = new TranslateTransform(_end.X - 3, _end.Y - 3);
            _pointHead.RenderTransform = new TranslateTransform(_start.X - 3, _start.Y - 3);

            //Arrow show direction
            _arrow.Fill = new SolidColorBrush(Colors.Green);
            _arrow.Stroke = _stroke;
            double xDiff = _end.X - _start.X;
            double yDiff = _end.Y - _start.Y;
            double rotate = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            RotateTransform myRotateTransform = new RotateTransform(rotate, _middle.X, _middle.Y);
            TranslateTransform myTranslate = new TranslateTransform(0, 0);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslate);
            _arrow.RenderTransform = myTransformGroup;

            //3 Point of Triangle
            PointCollection points = new PointCollection(3);
            points.Add(new Point(_middle.X - 3, _middle.Y - 3));
            points.Add(new Point(_middle.X - 3, _middle.Y + 3));
            points.Add(new Point(_middle.X + 4, _middle.Y));
            _arrow.Points = points;


            
            BezierSegment bezierSegment = new BezierSegment(_start, Control, _end, true);
            PathSegmentCollection pathSegments = new PathSegmentCollection();
            pathSegments.Add(bezierSegment);
            PathFigure pathFigure = new PathFigure(_start, pathSegments,false);
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            _shape.Data = pathGeometry;
            _shape.Stroke = _stroke;
            _shape.StrokeThickness = _strokeThickness;

            
        }
    }
}
