using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class dtRobotCharge : userModel
    {
        private int _robotChargeId;
        private string _robotTaskId;
        private string _robotId;
        private int _chargeId;
        private int _timeWorkId;
        private string _rcBeginDatetime;
        private string _rcEndDatetime;
        private string _currentBattery;
        private string _robotChargeStatus;
        private string _procedureContent;
        private string _timeCharge;

        public int robotChargeId { get => _robotChargeId; set => _robotChargeId = value; }
        public string robotTaskId { get => _robotTaskId; set => _robotTaskId = value; }
        public int chargeId { get => _chargeId; set => _chargeId = value; }
        public int timeWorkId { get => _timeWorkId; set => _timeWorkId = value; }
        public string rcBeginDatetime { get => _rcBeginDatetime; set => _rcBeginDatetime = value; }
        public string rcEndDatetime { get => _rcEndDatetime; set => _rcEndDatetime = value; }
        public string currentBattery { get => _currentBattery; set => _currentBattery = value; }
        public string robotChargeStatus { get => _robotChargeStatus; set => _robotChargeStatus = value; }
        public string robotId { get => _robotId; set => _robotId = value; }
        public string procedureContent { get => _procedureContent; set => _procedureContent = value; }
        public string timeCharge { get => _timeCharge; set => _timeCharge = value; }
    }
}
