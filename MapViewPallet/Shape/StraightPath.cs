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
            lineSegment.IsStroked = true;
            Name = "StraightPathx" + Global_Mouse.EncodeTransmissionTimestamp();
            props.name = Name;
            props._shape.Name = Name;
            props._pointHead.Name = Name;
            props._pointTail.Name = Name;
            props._arrow.Name = Name;
            RenderTransformOrigin = new Point(0, 0.5);
            Height = 20;
            props._start.X = 0;
            props._start.Y = (Height / 2);
            props._end.Y = (Height / 2);
            Draw();
        }
        
        public override void Draw()
        {
            Width = Global_Object.LengthBetweenPoints(props._oriMousePos, props._desMousePos);
            props._end.X = Width;
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
            props.points[0] = (new Point(props._middle.X - props.sizeArrow, props._middle.Y - props.sizeArrow));
            props.points[1] = (new Point(props._middle.X - props.sizeArrow, props._middle.Y + props.sizeArrow));
            props.points[2] = (new Point(props._middle.X + props.sizeArrow + 1, props._middle.Y));
            props._arrow.Points = props.points;
            //Position the Path
            lineSegment.Point = props._end;
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
            props.rotate = angle;
            props.myRotateTransform.Angle = props.rotate;
            props.myTranslate = new TranslateTransform(props._oriMousePos.X, props._oriMousePos.Y - (Height/2));
            props.myTransformGroup.Children[1] = props.myTranslate;
            // SPECIAL POINTS
            props.eightCorner[0] = CoorPointAtBorder(new Point((0),(Height/2)));          //mid-left
            props.eightCorner[1] = CoorPointAtBorder(new Point((0),(0)));                 //top-left
            props.eightCorner[2] = CoorPointAtBorder(new Point((Width/2),(0)));           //top-mid
            props.eightCorner[3] = CoorPointAtBorder(new Point((Width),(0)));             //top-right
            props.eightCorner[4] = CoorPointAtBorder(new Point((Width),(Height/2)));      //mid-right
            props.eightCorner[5] = CoorPointAtBorder(new Point((Width),(Height)));        //bot-right
            props.eightCorner[6] = CoorPointAtBorder(new Point((Width/2),(Height)));      //bot-mid
            props.eightCorner[7] = CoorPointAtBorder(new Point((0),(Height)));            //bot-left
        }

        public Point CoorPointAtBorder (Point pointOnBorder)
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
