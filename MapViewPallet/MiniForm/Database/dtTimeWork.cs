using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapViewPallet.MiniForm
{
    public class dtTimeWork : userModel
    {
        private int pTimeWorkId;
        private string pTimeWorkName;
        private string pStartTime;
        private string pEndTime;

        public int timeWorkId { get => pTimeWorkId; set => pTimeWorkId = value; }
        public string timeWorkName { get => pTimeWorkName; set => pTimeWorkName = value; }
        public string startTime { get => pStartTime; set => pStartTime = value; }
        public string endTime { get => pEndTime; set => pEndTime = value; }
    }
}
