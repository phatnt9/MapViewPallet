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
            this.direction = direction;
            RenderTransformOrigin = new Point(0, 1);
            Control = new Point();
            Draw();
        }
        
        public override void Draw()
        {
            Width = Global_Object.LengthBetweenPoints(_oriMousePos, _desMousePos);
            Height = Width* rate;
            if(Height > 20)
            {
                Height = 20;
            }
            _start.X = 0;
            _start.Y = Height-2;
            _end.X = Width;
            _end.Y = Height;
            //Control point of Path
            xDiff = _desMousePos.X - _oriMousePos.X;
            yDiff = _desMousePos.Y - _oriMousePos.Y;
            Control.X = Width / 2;
            Control.Y = -Height / 1.5;
            //Middle point of curve path
            //-----------Middle point-------------
            _middle.X = (1 - t) * (1 - t) * (1 - t) * _start.X + 3 * (1 - t) * (1 - t) * t * _start.X + 3 * (1 - t) * t * t * Control.X + t * t * t * _end.X;
            _middle.Y = (1 - t) * (1 - t) * (1 - t) * _start.Y + 3 * (1 - t) * (1 - t) * t * _start.Y + 3 * (1 - t) * t * t * Control.Y + t * t * t * _end.Y;
            //--------------------------------
            // Point at _start and _end
            _pointTail.RenderTransform = new TranslateTransform(Width / 2, Height / 2);
            _pointHead.RenderTransform = new TranslateTransform(-Width / 2, Height / 2);
            //Arrow show direction
            //3 Point of Triangle
            PointCollection points = new PointCollection(3);
            points.Add(new Point(_middle.X - sizeArrow, _middle.Y - sizeArrow));
            points.Add(new Point(_middle.X - sizeArrow, _middle.Y + sizeArrow));
            points.Add(new Point(_middle.X + sizeArrow + 1, _middle.Y));
            _arrow.Points = points;
            //Position the Path
            bezierSegment.Point1 = _start;
            bezierSegment.Point2 = Control;
            bezierSegment.Point3 = _end;
            bezierSegment.IsStroked = true;
            if(pathSegments.Count>0)
            {
                pathSegments[0] = bezierSegment;
            }
            else
            {
                Console.WriteLine("curve add");
                pathSegments.Add(bezierSegment);
            }
            pathFigure.StartPoint = _start;
            //Render Path
            rotate = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
            myRotateTransform.Angle = rotate;
            myTranslate = new TranslateTransform(_oriMousePos.X, _oriMousePos.Y - (Height));
            myTransformGroup.Children[1] = myTranslate;
        }
    }
}
