using System.Collections.Generic;

namespace MapViewPallet.MiniForm
{
    public class dtUser : userModel
    {
        private int pUserId;
        private string pUserName;
        private string pUserPassword;
        private int pUserAuthor;
        private List<dtUserDevice> pUserDevices;
        private int pFlagModify;
        private int pUserDeviceId;
        private int pDeviceId;
        private string pDeviceName;
        private string pUserPasswordOld;

        public int userId { get => pUserId; set => pUserId = value; }
        public string userName { get => pUserName; set => pUserName = value; }
        public string userPassword { get => pUserPassword; set => pUserPassword = value; }
        public int userAuthor { get => pUserAuthor; set => pUserAuthor = value; }
        public List<dtUserDevice> userDevices { get => pUserDevices; set => pUserDevices = value; }
        public int flagModify { get => pFlagModify; set => pFlagModify = value; }
        public int userDeviceId { get => pUserDeviceId; set => pUserDeviceId = value; }
        public int deviceId { get => pDeviceId; set => pDeviceId = value; }
        public string deviceName { get => pDeviceName; set => pDeviceName = value; }
        public string userPasswordOld { get => pUserPasswordOld; set => pUserPasswordOld = value; }
    }
}