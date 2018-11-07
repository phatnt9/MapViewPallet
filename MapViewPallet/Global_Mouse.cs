using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet
{
    public static class Global_Mouse
    {
        public static string EncodeTransmissionTimestamp()
        {
            long shortTicks = (DateTime.Now.Ticks - 631139040000000000L) / 10000L;
            return shortTicks + "";
            //return Convert.ToBase64String(BitConverter.GetBytes(shortTicks)).Substring(0, 7);
        }
        public enum STATE_MOUSEDOWN
        {
            _NORMAL,
            _ADD_STATION,
            _AUTO_DRAW_STRAIGHT,
            _AUTO_DRAW_CURVE,
            _KEEP_IN_OBJECT,
            _GET_OUT_OBJECT,
            _HAND_DRAW_STRAIGHT_P1,
            _HAND_DRAW_CURVEUP_P1,
            _HAND_DRAW_CURVEDOWN_P1,
            _HAND_DRAW_STRAIGHT_FINISH,
            _HAND_DRAW_CURVEUP_FINISH,
            _HAND_DRAW_CURVEDOWN_FINISH,
        }
        public enum STATE_MOUSEMOVE
        {
            _NORMAL,
            _DRAWING,
            _MOVE_STATION,
            _SLIDE_OBJECT,
            _HAND_DRAW_STRAIGHT,
            _HAND_DRAW_CURVE,

        }
        public static STATE_MOUSEDOWN ctrl_MouseDown;
        public static STATE_MOUSEMOVE ctrl_MouseMove;
    }
}
