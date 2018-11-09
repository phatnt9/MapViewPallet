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
            props._start.X = 0;
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
            double angle = (Math.Atan2(props.yDiff, props.xDiff) * 180.0 / Math.PI);
            switch (angle)
            {
                case double n when (n >= -22.5 && n < 22.5):
                    {
                        props.rotate = 0;
                        //Console.WriteLine(angle + "-" + props.rotate);
                        break;
                    }
                case double n when (n >= 22.5 && n < 67.5):
                    {
                        props.rotate = 45;
                        break;
                    }
                case double n when (n >= 67.5 && n < 112.5):
                    {
                        props.rotate = 90;
                        break;
                    }
                case double n when (n >= 112.5 && n < 157.5):
                    {
                        props.rotate = 135;
                        break;
                    }
                case double n when ((n >= 157.5 && n < 180)|| (n <= -157.5 && n > -179.9)):
                    {
                        props.rotate = 180;
                        break;
                    }
                case double n when (n >= -157.5 && n < -112.5):
                    {
                        props.rotate = -135;
                        break;
                    }
                case double n when (n >= -112.5 && n < -67.5):
                    {
                        props.rotate = -90;
                        break;
                    }
                case double n when (n >= -67.5 && n < -22.5):
                    {
                        props.rotate = -45;
                        break;
                    }
                default:
                    {
                        props.rotate = 45;
                        break;
                    }
            }
            props.myRotateTransform.Angle = props.rotate;
            
            props.myTranslate = new TranslateTransform(props._oriMousePos.X, props._oriMousePos.Y - (Height/2));
            //Console.WriteLine(props._oriMousePos.X.ToString("0.") + "-" + (props._oriMousePos.Y - (Height / 2)).ToString("0."));
            props.myTransformGroup.Children[1] = props.myTranslate;
            
            props.eightCorner[0].RenderTransform = RenderPointAtBorder(new Point((0),(Height/2)));          //mid-left
            props.eightCorner[1].RenderTransform = RenderPointAtBorder(new Point((0),(0)));                 //top-left
            props.eightCorner[2].RenderTransform = RenderPointAtBorder(new Point((Width/2),(0)));           //top-mid
            props.eightCorner[3].RenderTransform = RenderPointAtBorder(new Point((Width),(0)));             //top-right
            props.eightCorner[4].RenderTransform = RenderPointAtBorder(new Point((Width),(Height/2)));      //mid-right
            props.eightCorner[5].RenderTransform = RenderPointAtBorder(new Point((Width),(Height)));        //bot-right
            props.eightCorner[6].RenderTransform = RenderPointAtBorder(new Point((Width/2),(Height)));      //bot-mid
            props.eightCorner[7].RenderTransform = RenderPointAtBorder(new Point((0),(Height)));            //bot-left
            
        }

        public TranslateTransform RenderPointAtBorder (Point pointOnBorder)
        {
            double xDiff = (pointOnBorder.X) - (props._start.X);
            double yDiff = (pointOnBorder.Y) - (props._start.Y);
            double rad1 = (props.rotate * Math.PI) / 180;
            double rad2 = (Math.Atan2(yDiff, xDiff));
            double L = Global_Object.LengthBetweenPoints(props._start, pointOnBorder);
            double x1 = props._oriMousePos.X + ((L * Math.Cos(rad1 + rad2)));
            double y1 = props._oriMousePos.Y + ((L * Math.Sin(rad1 + rad2)));
            return new TranslateTransform(x1, y1);
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
