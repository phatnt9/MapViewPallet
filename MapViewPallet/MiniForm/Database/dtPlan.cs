using System.Collections.Generic;

namespace MapViewPallet.MiniForm
{
    public class dtPlan : userModel
    {

        private int pPlanId;
        private int pTimeWorkId;
        private int pProductDetailId;
        private int pPalletAmount;
        private int pPalletUse;
        private int pPalletMiss;
        private string pActiveDate;
        private int pDeviceId;
        private int pProductId;
        private string pDeviceName;
        private string pProductName;
        private string pProductDetailName;
        private string pPalletStatus;
        private List<dtBuffer> pBuffers;

        public int planId { get => pPlanId; set => pPlanId = value; }
        public int deviceProductId { get; set; }
        public int timeWorkId { get => pTimeWorkId; set => pTimeWorkId = value; }
        public int productDetailId { get => pProductDetailId; set => pProductDetailId = value; }
        public int palletAmount { get => pPalletAmount; set => pPalletAmount = value; }
        public int palletUse { get => pPalletUse; set => pPalletUse = value; }
        public int palletMiss { get => pPalletMiss; set => pPalletMiss = value; }
        public string activeDate { get => pActiveDate; set => pActiveDate = value; }
        public int deviceId { get => pDeviceId; set => pDeviceId = value; }
        public int productId { get => pProductId; set => pProductId = value; }
        public List<dtBuffer> buffers { get => pBuffers; set => pBuffers = value; }
        public string deviceName { get => pDeviceName; set => pDeviceName = value; }
        public string productName { get => pProductName; set => pProductName = value; }
        public string productDetailName { get => pProductDetailName; set => pProductDetailName = value; }
        public string palletStatus { get => pPalletStatus; set => pPalletStatus = value; }




    }
}
