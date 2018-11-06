using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapViewPallet.Shape
{
    public class StraightPath : PathShape
    {
        public StraightPath(Canvas canvas, Point Start, Point End) : base(canvas, Start, End)
        {
            //_start = ;
            //_end = ;
            _name = "StraightPath-" + Global_Mouse.EncodeTransmissionTimestamp();
            Height = 20;
            //Draw(Start, End);
        }

        public void Copy (StraightPath copy)
        {
            _name = copy._name;
            _shape = copy._shape;
            _pointHead = copy._pointHead;
            _pointTail = copy._pointTail;
            _arrow = copy._arrow;
            _middle = copy._middle;
        }



        public void DrawAxis(Point Start, Point End, Color color)
        {
            canvas.Children.Remove(_pointTail);
            //Middle point of straight path
            _start = Start;
            _end = End;
            // Point at Start and End

            _pointTail.Fill = new SolidColorBrush(color);
            _pointTail.Width = 5;
            _pointTail.Height = 5;
            _pointTail.RenderTransform = new TranslateTransform(End.X - 3, End.Y - 3);
            //Arrow
            _arrow.Fill = new SolidColorBrush(color);
            _arrow.Stroke = new SolidColorBrush(color);
            double xDiff = End.X - Start.X;
            double yDiff = End.Y - Start.Y;
            double rotate = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            RotateTransform myRotateTransform = new RotateTransform(rotate, End.X, End.Y);
            TranslateTransform myTranslate = new TranslateTransform(0, 0);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myRotateTransform);
            myTransformGroup.Children.Add(myTranslate);
            _arrow.RenderTransform = myTransformGroup;

            //3 Point of Triangle
            PointCollection points = new PointCollection(3);
            points.Add(new Point(End.X - 3, End.Y - 3));
            points.Add(new Point(End.X - 3, End.Y + 3));
            points.Add(new Point(End.X + 4, End.Y));
            _arrow.Points = points;


            //Position the Path
            LineSegment lineSegment = new LineSegment(End, true);
            PathSegmentCollection pathSegments = new PathSegmentCollection();
            pathSegments.Add(lineSegment);
            PathFigure pathFigure = new PathFigure(Start, pathSegments, false);
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            _shape.Data = pathGeometry;
            _shape.Stroke = new SolidColorBrush(color); ;
            _shape.StrokeThickness = _strokeThickness;
        }

        public void Draw(Point Start, Point End)
        {
            //_start = Start;
            //_end = End;
            //Middle point of straight path
            _middle.X = (_start.X + _end.X) / 2;
            _middle.Y = (_start.Y + _end.Y) / 2;

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


            //Position the Path
            LineSegment lineSegment = new LineSegment(_end, true);
            PathSegmentCollection pathSegments = new PathSegmentCollection();
            pathSegments.Add(lineSegment);
            PathFigure pathFigure = new PathFigure(new Point(0,0), pathSegments, false);
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            _shape.Data = pathGeometry;
            _shape.Stroke = _stroke;
            _shape.StrokeThickness = _strokeThickness;
        }

        public void remove()
        {
            canvas.Children.Remove(this);
        }

       
    }
}
