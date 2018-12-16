namespace MapViewPallet.MiniForm
{
    public class dtDeviceBuffer : userModel
    {
        private int pDeviceBufferId;
        private int pDeviceId;
        private int pBufferId;
        private string pBufferName;
        private int pBufferSort;
        private bool pcheckStatus;

        public int deviceBufferId { get => pDeviceBufferId; set => pDeviceBufferId = value; }
        public string bufferName { get => pBufferName; set => pBufferName = value; }
        public int deviceId { get => pDeviceId; set => pDeviceId = value; }
        public int bufferId { get => pBufferId; set => pBufferId = value; }
        public int bufferSort { get => pBufferSort; set => pBufferSort = value; }
        public bool checkStatus { get => pcheckStatus; set => pcheckStatus = value; }
    }
}
