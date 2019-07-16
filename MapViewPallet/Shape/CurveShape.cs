using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapViewPallet.Shape
{
    public class CurveShape : CanvasPath
    {
        private readonly double rate = 0.3;
        private readonly double t = 0.65;
        private Point controlPoint;
        private BezierSegment bezierSegment;
        private readonly bool direction; // true= Up, false = Down

        public CurveShape(Canvas canvas, Point Start, Point End, bool direction) : base(canvas, Start, End)
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
            controlPoint = new Point();
            Draw();
        }

        public override void Draw()
        {
            Width = Global_Object.LengthBetweenPoints(props._oriMousePos, props._desMousePos);
            Height = Width * rate;
            if (Height > 40)
            {
                Height = 40;
            }
            props._start.X = 0;
            props._start.Y = Height;
            props._end.X = Width;
            props._end.Y = Height;
            //Control point
            props.xDiff = props._desMousePos.X - props._oriMousePos.X;
            props.yDiff = props._desMousePos.Y - props._oriMousePos.Y;
            controlPoint.X = Width / 2;
            controlPoint.Y = -Height / 1.5;
            //Middle point of curve path
            //-----------Middle point-------------
            props._middle.X = (1 - t) * (1 - t) * (1 - t) * props._start.X + 3 * (1 - t) * (1 - t) * t * props._start.X + 3 * (1 - t) * t * t * controlPoint.X + t * t * t * props._end.X;
            props._middle.Y = (1 - t) * (1 - t) * (1 - t) * props._start.Y + 3 * (1 - t) * (1 - t) * t * props._start.Y + 3 * (1 - t) * t * t * controlPoint.Y + t * t * t * props._end.Y;
            //--------------------------------
            // Point at _start and _end
            props._pointTail.RenderTransform = new TranslateTransform(Width / 2, Height / 2);
            props._pointHead.RenderTransform = new TranslateTransform(-Width / 2, Height / 2);
            //Arrow show direction
            //3 Point of Triangle
            props.arrowPoints[0] = (new Point(props._middle.X - props.sizeArrow, props._middle.Y - props.sizeArrow));
            props.arrowPoints[1] = (new Point(props._middle.X - props.sizeArrow, props._middle.Y + props.sizeArrow));
            props.arrowPoints[2] = (new Point(props._middle.X + props.sizeArrow + 1, props._middle.Y));
            props._arrow.Points = props.arrowPoints;
            //Position the Path
            bezierSegment.Point1 = props._start;
            bezierSegment.Point2 = controlPoint;
            bezierSegment.Point3 = props._end;
            bezierSegment.IsStroked = true;
            if (props.pathSegments.Count > 0)
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
            // SPECIAL POINTS
            props.cornerPoints[0] = CoorPointAtBorder(new Point((0), (Height / 2)));          //mid-left
            props.cornerPoints[1] = CoorPointAtBorder(new Point((0), (0)));                 //top-left
            props.cornerPoints[2] = CoorPointAtBorder(new Point((Width / 2), (0)));           //top-mid
            props.cornerPoints[3] = CoorPointAtBorder(new Point((Width), (0)));             //top-right
            props.cornerPoints[4] = CoorPointAtBorder(new Point((Width), (Height / 2)));      //mid-right
            props.cornerPoints[5] = CoorPointAtBorder(new Point((Width), (Height)));        //bot-right
            props.cornerPoints[6] = CoorPointAtBorder(new Point((Width / 2), (Height)));      //bot-mid
            props.cornerPoints[7] = CoorPointAtBorder(new Point((0), (Height)));            //bot-left
        }

        public Point CoorPointAtBorder(Point pointOnBorder)
        {
            double xDiff = (pointOnBorder.X) - (props._start.X);
            double yDiff = (pointOnBorder.Y) - (props._start.Y);
            double rad1 = (props.rotate * Math.PI) / 180;
            double rad2 = (Math.Atan2(yDiff, xDiff));
            double L = Global_Object.LengthBetweenPoints(props._start, pointOnBorder);
            double x1 = props._oriMousePos.X + ((L * Math.Cos(rad1 + rad2)));
            double y1 = props._oriMousePos.Y + ((L * Math.Sin(rad1 + rad2)));
            return new Point(x1, y1);
        }
    }
}