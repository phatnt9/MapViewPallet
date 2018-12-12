using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class dtDeviceBuffer : userModel
    {
        private int pDeviceBufferId;
        private int pDeviceId;
        private string pBufferId;
        private int pBufferSort;
        private bool pcheckStatus;

        public int deviceBufferId { get => pDeviceBufferId; set => pDeviceBufferId = value; }
        public int deviceId { get => pDeviceId; set => pDeviceId = value; }
        public string bufferId { get => pBufferId; set => pBufferId = value; }
        public int bufferSort { get => pBufferSort; set => pBufferSort = value; }
        public bool checkStatus { get => pcheckStatus; set => pcheckStatus = value; }
    }
}
