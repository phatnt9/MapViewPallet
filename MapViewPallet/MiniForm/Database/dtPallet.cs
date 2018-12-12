using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class dtPallet : userModel
    {
        private int pPalletId;
        private int pDeviceBufferId;
        private string pBufferId;
        private int pPlanId;
        private int pRow;
        private int pBay;
        private string pDataPallet;
        private string pPalletStatus;

        public int palletId { get => pPalletId; set => pPalletId = value; }
        public int deviceBufferId { get => pDeviceBufferId; set => pDeviceBufferId = value; }
        public int planId { get => pPlanId; set => pPlanId = value; }
        public int row { get => pRow; set => pRow = value; }
        public int bay { get => pBay; set => pBay = value; }
        public string dataPallet { get => pDataPallet; set => pDataPallet = value; }
        public string palletStatus { get => pPalletStatus; set => pPalletStatus = value; }
        public string bufferId { get => pBufferId; set => pBufferId = value; }
    }
}
