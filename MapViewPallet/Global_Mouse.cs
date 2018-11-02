using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet
{
    public static class Global_Mouse
    {
        public enum STATE_MOUSEDOWN
        {
            _NORMAL,
            _ADD_STATION,
            _DRAW_STRAIGHT,
            _DRAW_CURVE,
            _KEEP_IN_OBJECT,
            _GET_OUT_OBJECT
        }
        public enum STATE_MOUSEMOVE
        {
            _NORMAL,
            _DRAWING,
            _MOVE_STATION,
            _SLIDE_OBJECT
        }
        public static STATE_MOUSEDOWN ctrl_MouseDown;
        public static STATE_MOUSEMOVE ctrl_MouseMove;
    }
}
