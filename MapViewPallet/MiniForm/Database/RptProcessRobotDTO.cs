using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public string RobotNameID { get { return robotNameID; } set { robotNameID = value; } }
        public string ProcessContent { get { return processContent; } set { processContent = value; } }
        public string RobotAgentContent { get { return robotAgentContent; } set { robotAgentContent = value; } }
        public DateTime DateTime { get { return dateTime; } set { dateTime = value; } }
        public string Status { get { return status; } set { status = value; } }
        public DateTime DptUpd { get { return dptUpd; } set { dptUpd = value; } }
    }
}
