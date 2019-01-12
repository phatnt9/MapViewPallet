using Newtonsoft.Json;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class DeviceList
    {
        public List<dtDevice> listDevices;

        public DeviceList()
        {
            listDevices = new List<dtDevice>();
        }

        public bool GetDevicesList()
        {
            try
            {
                listDevices.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/getListDevice");
                request.Method = "GET";
                request.ContentType = @"application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();

                    DataTable devices = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in devices.Rows)
                    {
                        dtDevice tempDevice = new dtDevice
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),

                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            deviceName = dr["deviceName"].ToString()
                        };
                        if (AddDevice(tempDevice))
                        {
                            tempDevice.GetDeviceProductsList();
                        }
                    }
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool AddDevice(dtDevice checkDevice)
        {
            foreach (dtDevice item in listDevices)
            {
                if (item.deviceId == checkDevice.deviceId)
                {
                    return false;
                }
            }
            listDevices.Add(checkDevice);
            return true;
        }
        
        public int GetProductIdByDeviceProductId(int deviceProductId)
        {
            foreach (dtDevice device in listDevices)
            {
                foreach (dtDeviceProduct deviceProduct in device.deviceProducts)
                {
                    if (deviceProduct.deviceProductId == deviceProductId)
                    {
                        return deviceProduct.productId;
                    }
                }
            }
            return -1;
        }

        public int GetDeviceIdByDeviceProductId(int deviceProductId)
        {
            foreach (dtDevice device in listDevices)
            {
                foreach (dtDeviceProduct deviceProduct in device.deviceProducts)
                {
                    if (deviceProduct.deviceProductId == deviceProductId)
                    {
                        return deviceProduct.deviceId;
                    }
                }
            }
            return -1;
        }
    }
}
