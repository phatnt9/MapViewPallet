using Newtonsoft.Json;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class DeviceList : BridgeClientRequest
    {
        public List<Device> listDevices;
        public DeviceList()
        {
            listDevices = new List<Device>();
            UpdateDevicesList();
        }

        public void UpdateDevicesList()
        {
            GetCallAPI("http://localhost:8081/robot/rest/device/getListDevice");
        }
        public override void ReceiveResponseHandler(string msg)
        {
            dynamic stuff = JsonConvert.DeserializeObject(msg);
            foreach (dynamic item in stuff)
            {
                Device temp = new Device((int)item.deviceId, (string)item.deviceName);
                if (AddDevice(temp))
                {
                    temp.GetDeviceProducts();
                }
            }
        }


        public bool AddDevice(Device device)
        {
            foreach (Device item in listDevices)
            {
                if (item.Id == device.Id)
                {
                    return false;
                }
            }
            listDevices.Add(device);
            return true;
        }
        
    }
}
