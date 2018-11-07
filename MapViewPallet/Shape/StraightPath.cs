using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapViewPallet.Shape
{
    public class StraightPath : PathShape
    {
        LineSegment lineSegment;
        public StraightPath(Canvas canvas, Point Start, Point End) : base(canvas, Start, End)
        {
            lineSegment = new LineSegment();
            Name = "StraightPathx" + Global_Mouse.EncodeTransmissionTimestamp();
            RenderTransformOrigin = new Point(0, 0.5);
            Draw();
        }
        
        public override void Draw()
        {
            Width = Global_Object.LengthBetweenPoints(_oriMousePos, _desMousePos);
            Height = 20;
            _start.X = -5;
            _start.Y = (Height / 2) - 1;
            _end.X = Width;
            _end.Y = (Height / 2) - 1;
            //Rotate param
            xDiff = _desMousePos.X - _oriMousePos.X;
            yDiff = _desMousePos.Y - _oriMousePos.Y;
            //Middle point of straight path
            _middle.X = (_start.X + _end.X) / 2;
            _middle.Y = (_start.Y + _end.Y) / 2;
            // Point at _start and _end
            _pointHead.RenderTransform = new TranslateTransform(-(Width / 2), 0);
            _pointTail.RenderTransform = new TranslateTransform((Width / 2), 0);
            //Arrow show direction
            //3 Point of Triangle
            PointCollection points = new PointCollection(3);
            points.Add(new Point(_middle.X - sizeArrow, _middle.Y - sizeArrow));
            points.Add(new Point(_middle.X - sizeArrow, _middle.Y + sizeArrow));
            points.Add(new Point(_middle.X + sizeArrow + 1, _middle.Y));
            _arrow.Points = points;
            //Position the Path
            lineSegment.Point = _end;
            lineSegment.IsStroked = true;
            if(pathSegments.Count>0)
            {

                pathSegments[0] = lineSegment;
            }
            else
            {
                Console.WriteLine("straight add");
                pathSegments.Add(lineSegment);
            }
            pathFigure.StartPoint = _start;

            //Render Path
            rotate = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            myRotateTransform.Angle = rotate;
            myTranslate = new TranslateTransform(_oriMousePos.X, _oriMousePos.Y - (Height/2));
            myTransformGroup.Children[1] = myTranslate;
        }

        public void remove()
        {
            canvas.Children.Remove(this);
        }
        //public void DrawAxis(Point Start, Point End, Color color)
        //{
        //    canvas.Children.Remove(_pointTail);
        //    //Middle point of straight path
        //    _start = Start;
        //    _end = End;
        //    // Point at Start and End
        //    _pointTail.Fill = new SolidColorBrush(color);
        //    _pointTail.Width = 5;
        //    _pointTail.Height = 5;
        //    _pointTail.RenderTransform = new TranslateTransform(End.X - 3, End.Y - 3);
        //    //Arrow
        //    //3 Point of Triangle
        //    PointCollection points = new PointCollection(3);
        //    points.Add(new Point(_middle.X - 3, _middle.Y - 3));
        //    points.Add(new Point(_middle.X - 3, _middle.Y + 3));
        //    points.Add(new Point(_middle.X + 4, _middle.Y));
        //    _arrow.Points = points;

        //    double xDiff = End.X - Start.X;
        //    double yDiff = End.Y - Start.Y;
        //    double rotate = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
        //    RotateTransform myRotateTransform = new RotateTransform(rotate, End.X, End.Y);
        //    TranslateTransform myTranslate = new TranslateTransform(0, 0);
        //    TransformGroup myTransformGroup = new TransformGroup();
        //    myTransformGroup.Children.Add(myRotateTransform);
        //    myTransformGroup.Children.Add(myTranslate);
        //    _arrow.RenderTransform = myTransformGroup;

        //    //3 Point of Triangle
        //    PointCollection points = new PointCollection(3);
        //    points.Add(new Point(End.X - 3, End.Y - 3));
        //    points.Add(new Point(End.X - 3, End.Y + 3));
        //    points.Add(new Point(End.X + 4, End.Y));
        //    _arrow.Points = points;


        //    //Position the Path
        //    LineSegment lineSegment = new LineSegment(End, true);
        //    PathSegmentCollection pathSegments = new PathSegmentCollection();
        //    pathSegments.Add(lineSegment);
        //    PathFigure pathFigure = new PathFigure(Start, pathSegments, false);
        //    PathGeometry pathGeometry = new PathGeometry();
        //    pathGeometry.Figures.Add(pathFigure);
        //    _shape.Data = pathGeometry;
        //    _shape.Stroke = new SolidColorBrush(color); ;
        //    _shape.StrokeThickness = _strokeThickness;
        //}

    }
}
