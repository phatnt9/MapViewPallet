using System;

namespace DTO
{
    public class RptProcessRobotDTO
    {
        private string robotNameID;
        private string processContent;
        private string robotAgentContent;
        private DateTime dateTime;
        private string status;
        private DateTime dptUpd;

        public string RobotNameID { get => robotNameID; set => robotNameID = value; }
        public string ProcessContent { get => processContent; set => processContent = value; }
        public string RobotAgentContent { get => robotAgentContent; set => robotAgentContent = value; }
        public DateTime DateTime { get => dateTime; set => dateTime = value; }
        public string Status { get => status; set => status = value; }
        public DateTime DptUpd { get => dptUpd; set => dptUpd = value; }
    }
}