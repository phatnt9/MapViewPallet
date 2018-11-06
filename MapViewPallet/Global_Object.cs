using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet
{
    public static class Global_Object
    {
        public static string Foo<T>(T parameter) { return typeof(T).Name; }
        public static SortedDictionary<string,StationShape> list_Station;
        public static double palletWidth = 13;
        public static double palletHeight = 15;
    }
}
