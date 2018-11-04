using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapViewPallet.Shape
{
    public class PathShape
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public double radius;
        public double StrokeThickness = 1;
        public SolidColorBrush Stroke = new SolidColorBrush(Colors.Black);
        public string Name;
        public Canvas canvas;
        public PathShape(Canvas canvas)
        {
            this.canvas = canvas;
        }
        public PathShape()
        {
        }
    }
}
