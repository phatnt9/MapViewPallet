using System;

namespace DTO
{
    public class RptProcessChargeDTO
    {
        private string robotNameID;
        private string chargeID;
        private string robotAgentContent;
        private string chargeContent;
        private string status;
        private DateTime dptUpd;

        public string RobotNameID { get { return robotNameID; } set { robotNameID = value; } }
        public string ChargeID { get { return chargeID; } set { chargeID = value; } }
        public string RobotAgentContent { get { return robotAgentContent; } set { robotAgentContent = value; } }
        public string ChargeContent { get { return chargeContent; } set { chargeContent = value; } }
        public string Status { get { return status; } set { status = value; } }
        public DateTime DptUpd { get { return dptUpd; } set { dptUpd = value; } }
    }
}
