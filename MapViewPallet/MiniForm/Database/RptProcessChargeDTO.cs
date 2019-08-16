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

        public string RobotNameID { get => robotNameID; set => robotNameID = value; }
        public string ChargeID { get => chargeID; set => chargeID = value; }
        public string RobotAgentContent { get => robotAgentContent; set => robotAgentContent = value; }
        public string ChargeContent { get => chargeContent; set => chargeContent = value; }
        public string Status { get => status; set => status = value; }
        public DateTime DptUpd { get => dptUpd; set => dptUpd = value; }
    }
}