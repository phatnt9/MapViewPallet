using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class dtDeviceProduct : userModel
    {
        private int pDeviceProductId;
        private int pDeviceId;
        private int pProductId;
        private string pProductName;
        private bool pcheckStatus;

        public int deviceProductId { get => pDeviceProductId; set => pDeviceProductId = value; }
        public int deviceId { get => pDeviceId; set => pDeviceId = value; }
        public int productId { get => pProductId; set => pProductId = value; }
        public string productName { get => pProductName; set => pProductName = value; }
        public bool checkStatus { get => pcheckStatus; set => pcheckStatus = value; }
    }
}
