using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapViewPallet.Shape
{
    public class CurvePath: PathShape
    {

        double rate = 0.3;
        double t = 0.65;
        Point Control;
        BezierSegment bezierSegment;
        bool direction; // true= Up, false = Down
        public CurvePath(Canvas canvas, Point Start, Point End, bool direction) : base(canvas, Start, End)
        {
            
            bezierSegment = new BezierSegment();
            Name = "CurvePathx" + Global_Mouse.EncodeTransmissionTimestamp();
            props.name = Name;
            props._shape.Name = Name;
            props._pointHead.Name = Name;
            props._pointTail.Name = Name;
            props._arrow.Name = Name;
            this.direction = direction;
            RenderTransformOrigin = new Point(0, 1);
            Control = new Point();
            Draw();
        }

        public override void Draw()
        {
            Width = Global_Object.LengthBetweenPoints(props._oriMousePos, props._desMousePos);
            Height = Width* rate;
            if(Height > 20)
            {
                Height = 20;
            }
            props._start.X = 0;
            props._start.Y = Height;
            props._end.X = Width;
            props._end.Y = Height;
            //Control point of Path
            props.xDiff = props._desMousePos.X - props._oriMousePos.X;
            props.yDiff = props._desMousePos.Y - props._oriMousePos.Y;
            Control.X = Width / 2;
            Control.Y = -Height / 1.5;
            //Middle point of curve path
            //-----------Middle point-------------
            props._middle.X = (1 - t) * (1 - t) * (1 - t) * props._start.X + 3 * (1 - t) * (1 - t) * t * props._start.X + 3 * (1 - t) * t * t * Control.X + t * t * t * props._end.X;
            props._middle.Y = (1 - t) * (1 - t) * (1 - t) * props._start.Y + 3 * (1 - t) * (1 - t) * t * props._start.Y + 3 * (1 - t) * t * t * Control.Y + t * t * t * props._end.Y;
            //--------------------------------
            // Point at _start and _end
            props._pointTail.RenderTransform = new TranslateTransform(Width / 2, Height / 2);
            props._pointHead.RenderTransform = new TranslateTransform(-Width / 2, Height / 2);
            //Arrow show direction
            //3 Point of Triangle
            PointCollection points = new PointCollection(3);
            points.Add(new Point(props._middle.X - props.sizeArrow, props._middle.Y - props.sizeArrow));
            points.Add(new Point(props._middle.X - props.sizeArrow, props._middle.Y + props.sizeArrow));
            points.Add(new Point(props._middle.X + props.sizeArrow + 1, props._middle.Y));
            props._arrow.Points = points;
            //Position the Path
            bezierSegment.Point1 = props._start;
            bezierSegment.Point2 = Control;
            bezierSegment.Point3 = props._end;
            bezierSegment.IsStroked = true;
            if(props.pathSegments.Count>0)
            {
                props.pathSegments[0] = bezierSegment;
            }
            else
            {
                props.pathSegments.Add(bezierSegment);
            }
            props.pathFigure.StartPoint = props._start;
            //Render Path
            props.rotate = (Math.Atan2(props.yDiff, props.xDiff) * 180.0 / Math.PI);
            props.myRotateTransform.Angle = props.rotate;
            props.myTranslate = new TranslateTransform(props._oriMousePos.X, props._oriMousePos.Y - (Height));
            props.myTransformGroup.Children[1] = props.myTranslate;
        }
    }
}
