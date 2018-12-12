using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapViewPallet.MiniForm
{
    public class dtUser : userModel
    {
        private int pPlanId;
        private int pDeviceProductId;
        private int pTimeWorkId;
        private int pProductDetailId;
        private int pPalletAmount;
        private int pPalletUse;
        private int pPalletMiss;
        private DateTime pActiveDate;
        private List<dtUserDevice> pUserDevices;
        private int pFlagModify;

        public int planId { get => pPlanId; set => pPlanId = value; }
        public int deviceProductId { get => pDeviceProductId; set => pDeviceProductId = value; }
        public int timeWorkId { get => pTimeWorkId; set => pTimeWorkId = value; }
        public int productDetailId { get => pProductDetailId; set => pProductDetailId = value; }
        public int palletAmount { get => pPalletAmount; set => pPalletAmount = value; }
        public int palletUse { get => pPalletUse; set => pPalletUse = value; }
        public int palletMiss { get => pPalletMiss; set => pPalletMiss = value; }
        public DateTime activeDate { get => pActiveDate; set => pActiveDate = value; }
        public List<dtUserDevice> userDevices { get => pUserDevices; set => pUserDevices = value; }
        public int flagModify { get => pFlagModify; set => pFlagModify = value; }
    }
}
