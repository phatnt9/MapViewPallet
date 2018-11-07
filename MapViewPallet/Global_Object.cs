using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapViewPallet
{
    public static class Global_Object
    {
        public static string Foo<T>(T parameter) { return typeof(T).Name; }
        public static SortedDictionary<string,StationShape> list_Station;
        public static double palletWidth = 13;
        public static double palletHeight = 15;
        public static double LengthBetweenPoints(Point pt1, Point pt2)
        {
            //Calculate the distance between the both points
            //for both axes separately.
            double dblDistX = Math.Abs(pt1.X - pt2.X);
            double dblDistY = Math.Abs(pt1.Y - pt2.Y);

            //Calculate the length of a line traveling from pt1 to pt2
            //(according to Pythagoras).
            double dblHypotenuseLength = Math.Sqrt(
               dblDistX * dblDistX
               +
               dblDistY * dblDistY
             );

            return dblHypotenuseLength;
        }

    }
}
