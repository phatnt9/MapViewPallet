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


        private string pDeviceName;
        private int pProductId;
        private string pProductName;
        private int pProductDetailId;
        private string pProductDetailName;


        public string deviceName { get => pDeviceName; set { pDeviceName = value; RaisePropertyChanged("deviceName"); } }
        public int productId { get => pProductId; set { pProductId = value; RaisePropertyChanged("productId"); } }
        public string productName { get => pProductName; set { pProductName = value; RaisePropertyChanged("productName"); } }
        public int productDetailId { get => pProductDetailId; set { pProductDetailId = value; RaisePropertyChanged("productDetailId"); } }
        public string productDetailName { get => pProductDetailName; set { pProductDetailName = value; RaisePropertyChanged("productDetailName"); } }



    }
}
