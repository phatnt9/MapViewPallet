using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MapViewPallet.MiniForm
{
    public class ManagementModel : NotifyUIBase
    {
        DevicesManagement devicesManagement;
        //***************DATA*********************
        //public ICollectionView GroupedDevices { get; private set; }
        //public ICollectionView GroupedProducts { get; private set; }
        //public ICollectionView GroupedBuffers { get; private set; }

        public ICollectionView GroupedDevices { get; private set; }
        public ICollectionView GroupedProducts { get; private set; }
        public ICollectionView GroupedBuffers { get; private set; }

        //***************VARIABLES*********************

        public List<Device> devicesList;
        public List<DeviceProduct> productsList;
        public List<DeviceBuffer> buffersList;

        public ManagementModel(DevicesManagement devicesManagement)
        {
            this.devicesManagement = devicesManagement;

            devicesList = new List<Device>();
            productsList = new List<DeviceProduct>();
            buffersList = new List<DeviceBuffer>();

            GroupedDevices = new ListCollectionView(devicesList);
            GroupedProducts = new ListCollectionView(productsList);
            GroupedBuffers = new ListCollectionView(buffersList);
            //LoadListDevices();

        }

        public void LoadListDevices()
        {
            try
            {
                devicesList.Clear();
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
                        if (!ContainDevice(tempDevice, devicesList))
                        {
                            devicesList.Add(tempDevice);
                        }
                    }
                }
                GroupedDevices.Refresh();
                if (devicesManagement.DeviceListDg.HasItems)
                {
                    if (devicesManagement.DeviceListDg.SelectedItem == null)
                    {
                        //Console.WriteLine("Select first item!!!");
                        devicesManagement.DeviceListDg.SelectedItem = devicesManagement.DeviceListDg.Items[0];
                        devicesManagement.DeviceListDg.ScrollIntoView(devicesManagement.DeviceListDg.SelectedItem);
                    }
                }
                else
                {
                    //Không có device nào
                }
                //devicesManagement.UpdateDgv();

            }
            catch (Exception ex)
            {
                //Không có device nào
            }
        }



        public void ReLoadListDeviceProducts(int deviceId)
        {
            try
            {
                productsList.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/getListDeviceProductAllByDeviceId");
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
                        if (!ContainDeviceProduct(tempDeviceProduct, productsList))
                        {
                            productsList.Add(tempDeviceProduct);
                        }
                    }
                }
                if (devicesManagement.DeviceProductListDg.SelectedItem == null)
                {
                    //Console.WriteLine("Select first item!!!");
                    devicesManagement.DeviceProductListDg.SelectedItem = devicesManagement.DeviceProductListDg.Items[0];
                    devicesManagement.DeviceProductListDg.ScrollIntoView(devicesManagement.DeviceProductListDg.SelectedItem);
                }
                GroupedProducts.Refresh();
            }
            catch (Exception ex)
            {

            }
        }

        public void ReLoadListDeviceBuffers(int deviceId)
        {
            try
            {
                buffersList.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "buffer/getListDeviceBufferAllByDeviceId");
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

                    DataTable deviceBuffers = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in deviceBuffers.Rows)
                    {
                        DeviceBuffer tempDeviceBuffer = new DeviceBuffer
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            deviceBufferId = int.Parse(dr["deviceBufferId"].ToString()),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            bufferId = int.Parse(dr["bufferId"].ToString()),
                            bufferName = dr["bufferName"].ToString(),
                            bufferSort = int.Parse(dr["bufferSort"].ToString()),
                            checkStatus = bool.Parse(dr["checkStatus"].ToString())
                        };
                        if (!ContainDeviceBuffer(tempDeviceBuffer, buffersList))
                        {
                            buffersList.Add(tempDeviceBuffer);
                        }
                    }
                }
                if (devicesManagement.BufferListDg.SelectedItem == null)
                {
                    //Console.WriteLine("Select first item!!!");
                    devicesManagement.BufferListDg.SelectedItem = devicesManagement.BufferListDg.Items[0];
                    devicesManagement.BufferListDg.ScrollIntoView(devicesManagement.BufferListDg.SelectedItem);
                }
                GroupedBuffers.Refresh();
            }
            catch (Exception ex)
            {

            }
        }

        public bool ContainDevice(Device tempOpe, List<Device> Base)
        {
            foreach (Device temp in Base) { if (temp.deviceId == tempOpe.deviceId) { return true; } }
            return false;
        }

        public bool ContainDeviceProduct(DeviceProduct tempOpe, List<DeviceProduct> Base)
        {
            foreach (DeviceProduct temp in Base) { if (temp.deviceProductId == tempOpe.deviceProductId) { return true; } }
            return false;
        }

        public bool ContainDeviceBuffer(DeviceBuffer tempOpe, List<DeviceBuffer> Base)
        {
            foreach (DeviceBuffer temp in Base) { if (temp.deviceBufferId == tempOpe.deviceBufferId) { return true; } }
            return false;
        }

    }
}
