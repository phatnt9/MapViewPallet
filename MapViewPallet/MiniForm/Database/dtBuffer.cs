using System.Collections.Generic;

namespace MapViewPallet.MiniForm
{
    public class dtBuffer : userModel
    {
        private int pBufferId;
        private string pBufferName;
        private string pBufferNameOld;
        private string pBufferData;
        private string pbufferCheckIn;
        private int pMaxRow;
        private int pMaxBay;
        private int pMaxRowOld;
        private int pMaxBayOld;
        private bool pBufferReturn;
        private bool pBufferReturnOld;
        private List<dtPallet> pPallets;

        public int bufferId { get => pBufferId; set => pBufferId = value; }
        public string bufferName { get => pBufferName; set => pBufferName = value; }
        public int maxRow { get => pMaxRow; set => pMaxRow = value; }
        public int maxBay { get => pMaxBay; set => pMaxBay = value; }
        public string bufferCheckIn { get => pbufferCheckIn; set => pbufferCheckIn = value; }
        public int maxRowOld { get => pMaxRowOld; set => pMaxRowOld = value; }
        public int maxBayOld { get => pMaxBayOld; set => pMaxBayOld = value; }
        public string bufferNameOld { get => pBufferNameOld; set => pBufferNameOld = value; }
        public List<dtPallet> pallets { get => pPallets; set => pPallets = value; }
        public bool bufferReturn { get => pBufferReturn; set { pBufferReturn = value; RaisePropertyChanged("bufferReturn"); } }
        public bool bufferReturnOld { get => pBufferReturnOld; set => pBufferReturnOld = value; }
        public string bufferData { get => pBufferData; set => pBufferData = value; }

        public dtBuffer()
        {
            pPallets = new List<dtPallet>();
        }

        public void GetPalletsList()
        {
        }
    }
}