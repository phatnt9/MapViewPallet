namespace MapViewPallet.MiniForm
{
    public class dtPallet : userModel
    {
        private int pPalletId;
        private int pDeviceBufferId;
        private int pBufferId;
        private int pPlanId;
        private int pRow;
        private int pBay;
        private string pDataPallet;
        private string pPalletStatus;
        private int pDeviceId;

        public int palletId { get => pPalletId; set => pPalletId = value; }
        public int deviceBufferId { get => pDeviceBufferId; set => pDeviceBufferId = value; }
        public int planId { get => pPlanId; set => pPlanId = value; }
        public int row { get => pRow; set => pRow = value; }
        public int bay { get => pBay; set => pBay = value; }
        public string dataPallet { get => pDataPallet; set { pDataPallet = value; RaisePropertyChanged("dataPallet"); } }
        public string palletStatus { get => pPalletStatus; set => pPalletStatus = value; }
        public int bufferId { get => pBufferId; set => pBufferId = value; }
        public int deviceId { get => pDeviceId; set => pDeviceId = value; }

    }
}
