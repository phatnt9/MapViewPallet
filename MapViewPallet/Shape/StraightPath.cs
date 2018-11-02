using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MapViewPallet.Shape
{
   public class StraightPath:PathShape
    {
        public Line shape;
        public StraightPath(Canvas canvas):base(canvas)
        {
            shape = new Line();
        }
        public void Draw (Point Start, Point End)
        {
            shape.X1 = Start.X;
            shape.Y1 = Start.Y;
            shape.X2 = End.X;
            shape.Y2 = End.Y;
            shape.Stroke = Stroke;
            shape.StrokeThickness = StrokeThickness;
            canvas.Children.Add(shape);
        }
    }
}
