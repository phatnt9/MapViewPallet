using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapViewPallet.MiniForm
{
    public class dtDevice : userModel
    {
        private int pDeviceId;
        private string pDeviceName;
        private List<dtDeviceProduct> pDeviceProducts;
        private List<dtDeviceBuffer> pDeviceBuffers;

        public int deviceId { get => pDeviceId; set => pDeviceId = value; }
        public string deviceName { get => pDeviceName; set => pDeviceName = value; }
        public List<dtDeviceProduct> deviceProducts { get => pDeviceProducts; set => pDeviceProducts = value; }
        public List<dtDeviceBuffer> deviceBuffers { get => pDeviceBuffers; set => pDeviceBuffers = value; }
    }
}
