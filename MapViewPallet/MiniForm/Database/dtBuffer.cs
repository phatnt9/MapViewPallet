using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapViewPallet.MiniForm
{
    public class dtBuffer : userModel
    {
        private string pBufferId;
        private string pBufferIdOld;
        private string pbufferCheckIn;
        private int pMaxRow;
        private int pMaxBay;
        private int pMaxRowOld;
        private int pMaxBayOld;
        private List<dtPallet> pPallets;

        public string bufferId { get => pBufferId; set => pBufferId = value; }
        public int maxRow { get => pMaxRow; set => pMaxRow = value; }
        public int maxBay { get => pMaxBay; set => pMaxBay = value; }
        public string bufferCheckIn { get => pbufferCheckIn; set => pbufferCheckIn = value; }
        public string bufferIdOld { get => pBufferIdOld; set => pBufferIdOld = value; }
        public int maxRowOld { get => pMaxRowOld; set => pMaxRowOld = value; }
        public int maxBayOld { get => pMaxBayOld; set => pMaxBayOld = value; }
        public List<dtPallet> pallets { get => pPallets; set => pPallets = value; }

    }
}
