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
            props.name = Name;
            props._shape.Name = Name;
            props._pointHead.Name = Name;
            props._pointTail.Name = Name;
            props._arrow.Name = Name;
            RenderTransformOrigin = new Point(0, 0.5);
            Draw();
        }
        
        public override void Draw()
        {
            Width = Global_Object.LengthBetweenPoints(props._oriMousePos, props._desMousePos);
            Height = 20;
            props._start.X = -5;
            props._start.Y = (Height / 2) ;
            props._end.X = Width;
            props._end.Y = (Height / 2) ;
            //Rotate param
            props.xDiff = props._desMousePos.X - props._oriMousePos.X;
            props.yDiff = props._desMousePos.Y - props._oriMousePos.Y;
            //Middle point of straight path
            props._middle.X = (props._start.X + props._end.X) / 2;
            props._middle.Y = (props._start.Y + props._end.Y) / 2;
            // Point at _start and _end
            props._pointHead.RenderTransform = new TranslateTransform(-(Width / 2), 0);
            props._pointTail.RenderTransform = new TranslateTransform((Width / 2), 0);
            //Arrow show direction
            //3 Point of Triangle
            PointCollection points = new PointCollection(3);
            points.Add(new Point(props._middle.X - props.sizeArrow, props._middle.Y - props.sizeArrow));
            points.Add(new Point(props._middle.X - props.sizeArrow, props._middle.Y + props.sizeArrow));
            points.Add(new Point(props._middle.X + props.sizeArrow + 1, props._middle.Y));
            props._arrow.Points = points;
            //Position the Path
            lineSegment.Point = props._end;
            lineSegment.IsStroked = true;
            if(props.pathSegments.Count>0)
            {

                props.pathSegments[0] = lineSegment;
            }
            else
            {
                props.pathSegments.Add(lineSegment);
            }
            props.pathFigure.StartPoint = props._start;

            //Render Path
            props.rotate = (Math.Atan2(props.yDiff, props.xDiff) * 180.0 / Math.PI);
            props.myRotateTransform.Angle = props.rotate;
            props.myTranslate = new TranslateTransform(props._oriMousePos.X, props._oriMousePos.Y - (Height/2));
            props.myTransformGroup.Children[1] = props.myTranslate;
        }

        public void remove()
        {
            props.canvas.Children.Remove(this);
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
