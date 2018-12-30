using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm.Database
{
    public class dtDevicePallet : userModel
    {
        private int pDevicePalletId;
        private int pDeviceId;
        private string pDevicePalletName;
        private int pRow;
        private int pBay;
        private string pDataPallet;
        private string pImageDeviceUrl;

        public int devicePalletId { get => pDevicePalletId; set => pDevicePalletId = value; }
        public int deviceId { get => pDeviceId; set => pDeviceId = value; }
        public string devicePalletName { get => pDevicePalletName; set => pDevicePalletName = value; }
        public int row { get => pRow; set => pRow = value; }
        public int bay { get => pBay; set => pBay = value; }
        public string dataPallet { get => pDataPallet; set { pDataPallet = value; RaisePropertyChanged("dataPallet"); } }
        public string imageDeviceUrl { get => pImageDeviceUrl; set => pImageDeviceUrl = value; }
    }
}
