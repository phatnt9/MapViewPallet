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
        public List<Device> listDevices;
        public DeviceList()
        {
            listDevices = new List<Device>();
            GetDevicesList();
        }

        public void GetDevicesList()
        {
            try
            {
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
                        Device tempDevice = new Device
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
            }
            catch (Exception ex)
            {

            }
        }

        public bool AddDevice(Device checkDevice)
        {
            foreach (Device item in listDevices)
            {
                if (item.deviceId == checkDevice.deviceId)
                {
                    return false;
                }
            }
            listDevices.Add(checkDevice);
            return true;
        }


        public int GetListIndexByDeviceId (int deviceId)
        {
            try
            {
                for (int i=0;i< listDevices.Count;i++)
                {
                    if (listDevices[i].deviceId == deviceId)
                    {
                        return i;
                    }
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        public Device Get_Device_By_DeviceId(int deviceId)
        {
            foreach (Device device in listDevices)
            {
                if (device.deviceId == deviceId)
                {
                    return device;
                }
            }
            return null;
        }

        public DeviceProduct GetDeviceProductByProductId(int deviceId, int productId)
        {
            Device deviceTemp = Get_Device_By_DeviceId(deviceId);
            foreach (DeviceProduct deviceProduct in deviceTemp.listDeviceProduct)
            {
                if (deviceProduct.productId == productId)
                {
                    return deviceProduct;
                }
            }
            return null;
        }
        public ProductDetail GetProductDetailByProductDetailId (int deviceId, int productId, int productDetailId)
        {
            DeviceProduct deviceProductTemp = GetDeviceProductByProductId(deviceId, productId);
            foreach (ProductDetail productDetail in deviceProductTemp.listProductDetails)
            {
                if (productDetail.productDetailId == productDetailId)
                {
                    return productDetail;
                }
            }
            return null;
        }

        public Device GetDeviceByDeviceProductId (int deviceProductId)
        {
            foreach (Device device in listDevices)
            {
                foreach (DeviceProduct deviceProduct in device.listDeviceProduct)
                {
                    if (deviceProduct.deviceProductId == deviceProductId)
                    {
                        return device;
                    }
                }
            }
            return null;
        }

        public int GetProductIdByDeviceProductId(int deviceProductId)
        {
            foreach (Device device in listDevices)
            {
                foreach (DeviceProduct deviceProduct in device.listDeviceProduct)
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
            foreach (Device device in listDevices)
            {
                foreach (DeviceProduct deviceProduct in device.listDeviceProduct)
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
