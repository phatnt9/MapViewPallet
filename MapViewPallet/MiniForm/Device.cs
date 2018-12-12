using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;

namespace MapViewPallet.MiniForm
{
    public class Device : dtDevice
    {
        //########-----VARIABLE-----#######
        public List<DeviceProduct> listDeviceProduct;
        
        //########-----METHOD-----#######
        public Device()
        {
            listDeviceProduct = new List<DeviceProduct>();
        }

        public void GetDeviceProductsList()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/getListDeviceProductByDeviceId");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.deviceId = deviceId;
                string jsonData = JsonConvert.SerializeObject(postApiBody);
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(jsonData);
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Flush();
                }
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();

                    DataTable deviceProducts = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in deviceProducts.Rows)
                    {
                        DeviceProduct tempDeviceProduct = new DeviceProduct
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            deviceProductId = int.Parse(dr["deviceProductId"].ToString()),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            productId = int.Parse(dr["productId"].ToString()),
                            productName = dr["productName"].ToString(),
                            checkStatus = bool.Parse(dr["checkStatus"].ToString())
                        };
                        if (AddDeviceProduct(tempDeviceProduct))
                        {
                            tempDeviceProduct.GetProductDetailsList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public bool AddDeviceProduct(DeviceProduct deviceProduct)
        {
            foreach (DeviceProduct item in listDeviceProduct)
            {
                if (item.deviceProductId == deviceProduct.deviceProductId)
                {
                    return false;
                }
            }
            listDeviceProduct.Add(deviceProduct);
            return true;
        }

        public DeviceProduct Get_DeviceProduct_By_DeviceProductId(int deviceProductId)
        {
            try
            {
                foreach (DeviceProduct deviceProduct in listDeviceProduct)
                {
                    if (deviceProduct.deviceProductId == deviceProductId)
                    {
                        return deviceProduct;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
