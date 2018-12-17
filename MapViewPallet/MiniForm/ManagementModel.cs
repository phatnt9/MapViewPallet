using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Data;

namespace MapViewPallet.MiniForm
{
    public class ManagementModel : NotifyUIBase
    {
        private string pStatusData;
        public string statusData { get => pStatusData; set { pStatusData = value; RaisePropertyChanged("statusData"); } }

        DevicesManagement devicesManagement;
        //***************DATA*********************

        public ListCollectionView GroupedDevices { get; private set; }
        public ListCollectionView GroupedDeviceProducts { get; private set; }
        public ListCollectionView GroupedDeviceBuffers { get; private set; }

        public ListCollectionView GroupedProducts { get; private set; }
        public ListCollectionView GroupedProductDetails { get; private set; }

        public ListCollectionView GroupedBuffers { get; private set; }
        public ListCollectionView GroupedPallets { get; private set; }


        //***************VARIABLES*********************

        public List<dtDevice> devicesList;
        public List<DeviceBuffer> deviceBuffersList;
        public List<dtDeviceProduct> deviceProductsList;

        public List<dtProduct> productsList;
        public List<dtProductDetail> productDetailsList;


        public List<dtBuffer> buffersList;
        public List<dtPallet> palletsList;

        public ManagementModel(DevicesManagement devicesManagement)
        {
            UpdateDataStatus("Sẵn sàng");

            this.devicesManagement = devicesManagement;

            devicesList = new List<dtDevice>();
            deviceProductsList = new List<dtDeviceProduct>();
            deviceBuffersList = new List<DeviceBuffer>();
            
            productsList = new List<dtProduct>();
            productDetailsList = new List<dtProductDetail>();

            buffersList = new List<dtBuffer>();
            palletsList = new List<dtPallet>();

            //********************************************************************

            GroupedDevices = (ListCollectionView)CollectionViewSource.GetDefaultView(devicesList);
            GroupedDeviceProducts = (ListCollectionView)CollectionViewSource.GetDefaultView(deviceProductsList);
            GroupedDeviceBuffers = (ListCollectionView)CollectionViewSource.GetDefaultView(deviceBuffersList);

            GroupedProducts = (ListCollectionView)CollectionViewSource.GetDefaultView(productsList);
            GroupedProductDetails= (ListCollectionView)CollectionViewSource.GetDefaultView(productDetailsList);
            
            

            GroupedBuffers = (ListCollectionView)CollectionViewSource.GetDefaultView(buffersList);
            GroupedPallets = (ListCollectionView)CollectionViewSource.GetDefaultView(palletsList);

        }


        //*********************************************************************************************
        
        public void ReloadListDevices()
        {
            try
            {
                UpdateDataStatus("Đang cập nhật...");
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
                        dtDevice tempDevice = new dtDevice
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
                if (GroupedDevices.IsEditingItem)
                    GroupedDevices.CommitEdit();
                if (GroupedDevices.IsAddingNew)
                    GroupedDevices.CommitNew();
                GroupedDevices.Refresh();
                if (devicesManagement.DevicesListDg.HasItems)
                {
                    devicesManagement.DevicesListDg.SelectedItem = devicesManagement.DevicesListDg.Items[0];
                    devicesManagement.DevicesListDg.ScrollIntoView(devicesManagement.DevicesListDg.SelectedItem);
                }
                UpdateDataStatus("Sẵn sàng");

            }
            catch
            {
                UpdateDataStatus("Lỗi");
            }
        }
        
        public void ReloadListProducts()
        {
            try
            {
                UpdateDataStatus("Đang cập nhật...");
                productsList.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/getListProduct");
                request.Method = "GET";
                request.ContentType = @"application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();

                    DataTable products = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in products.Rows)
                    {
                        dtProduct tempProduct = new dtProduct
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            productId = int.Parse(dr["productId"].ToString()),
                            productName = dr["productName"].ToString()
                        };
                        if (!ContainProduct(tempProduct, productsList))
                        {
                            productsList.Add(tempProduct);
                            //tempProduct.GetProductDetailsList();
                        }
                    }
                }
                if (GroupedProducts.IsEditingItem)
                    GroupedProducts.CommitEdit();
                if (GroupedProducts.IsAddingNew)
                    GroupedProducts.CommitNew();
                GroupedProducts.Refresh();
                if (devicesManagement.ProductsListDg.HasItems)
                {
                    devicesManagement.ProductsListDg.SelectedItem = devicesManagement.ProductsListDg.Items[0];
                    devicesManagement.ProductsListDg.ScrollIntoView(devicesManagement.ProductsListDg.SelectedItem);
                }
                UpdateDataStatus("Sẵn sàng");
            }
            catch
            {
                UpdateDataStatus("Lỗi");
                //Không có device nào
            }
        }

        public void ReloadListBuffers()
        {
            try
            {
                buffersList.Clear();
                UpdateDataStatus("Đang cập nhật...");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "buffer/getListBuffer");
                request.Method = "GET";
                request.ContentType = @"application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();

                    DataTable buffers = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in buffers.Rows)
                    {
                        dtBuffer tempBuffer = new dtBuffer
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            bufferId = int.Parse(dr["bufferId"].ToString()),
                            bufferName = dr["bufferName"].ToString(),
                            maxBay = int.Parse(dr["maxBay"].ToString()),
                            maxBayOld = int.Parse(dr["maxBayOld"].ToString()),
                            maxRow = int.Parse(dr["maxRow"].ToString()),
                            maxRowOld = int.Parse(dr["maxRowOld"].ToString()),
                            bufferCheckIn = dr["bufferCheckIn"].ToString(),
                            bufferNameOld = dr["bufferNameOld"].ToString(),
                            bufferReturn = bool.Parse(dr["bufferReturn"].ToString()),
                        };
                        if (!ContainBuffer(tempBuffer, buffersList))
                        {
                            buffersList.Add(tempBuffer);
                            //tempBuffer.GetPalletsList();
                        }
                    }
                }
                if (GroupedBuffers.IsEditingItem)
                    GroupedBuffers.CommitEdit();
                if (GroupedBuffers.IsAddingNew)
                    GroupedBuffers.CommitNew();
                GroupedBuffers.Refresh();
                if (devicesManagement.BuffersListDg.HasItems)
                {
                    devicesManagement.BuffersListDg.SelectedItem = devicesManagement.BuffersListDg.Items[0];
                    devicesManagement.BuffersListDg.ScrollIntoView(devicesManagement.BuffersListDg.SelectedItem);
                }
                UpdateDataStatus("Sẵn sàng");
            }
            catch
            {
                UpdateDataStatus("Lỗi");
                //Không có device nào
            }
        }

        public void ReloadListDeviceProducts(int deviceId)
        {
            try
            {
                UpdateDataStatus("Đang cập nhật...");
                deviceProductsList.Clear();
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
                        dtDeviceProduct tempDeviceProduct = new dtDeviceProduct
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
                        if (!ContainDeviceProduct(tempDeviceProduct, deviceProductsList))
                        {
                            deviceProductsList.Add(tempDeviceProduct);
                        }
                    }
                }
                if (GroupedDeviceProducts.IsEditingItem)
                    GroupedDeviceProducts.CommitEdit();
                if (GroupedDeviceProducts.IsAddingNew)
                    GroupedDeviceProducts.CommitNew();
                GroupedDeviceProducts.Refresh();
                if (devicesManagement.DeviceProductsListDg.HasItems)
                {
                    devicesManagement.DeviceProductsListDg.SelectedItem = devicesManagement.DeviceProductsListDg.Items[0];
                    devicesManagement.DeviceProductsListDg.ScrollIntoView(devicesManagement.DeviceProductsListDg.SelectedItem);
                }
                UpdateDataStatus("Sẵn sàng");
            }
            catch
            {
                UpdateDataStatus("Lỗi");
            }
        }

        public void ReloadListDeviceBuffers(int deviceId)
        {
            //try
            {
                deviceBuffersList.Clear();
                UpdateDataStatus("Đang cập nhật...");
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
                        if (!ContainDeviceBuffer(tempDeviceBuffer, deviceBuffersList))
                        {
                            deviceBuffersList.Add(tempDeviceBuffer);
                        }
                    }
                }
                if (GroupedDeviceBuffers.IsEditingItem)
                    GroupedDeviceBuffers.CommitEdit();
                if (GroupedDeviceBuffers.IsAddingNew)
                    GroupedDeviceBuffers.CommitNew();
                GroupedDeviceBuffers.Refresh();
                if (devicesManagement.DeviceBuffersListDg.HasItems)
                {
                    devicesManagement.DeviceBuffersListDg.SelectedItem = devicesManagement.DeviceBuffersListDg.Items[0];
                    devicesManagement.DeviceBuffersListDg.ScrollIntoView(devicesManagement.DeviceBuffersListDg.SelectedItem);
                }
                UpdateDataStatus("Sẵn sàng");
            }
            //catch
            {
                UpdateDataStatus("Lỗi");
            }
        }
        
        public void ReloadListProductDetails(int productId)
        {
            //try
            {
                productDetailsList.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/getListProductDetailByProductId");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.productId = productId;
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

                    DataTable productDetails = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in productDetails.Rows)
                    {
                        dtProductDetail tempProductDetail = new dtProductDetail
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            productDetailId = int.Parse(dr["productDetailId"].ToString()),
                            productId = int.Parse(dr["productId"].ToString()),
                            productDetailName = dr["productDetailName"].ToString()
                        };
                        if (!ContainProductDetail(tempProductDetail, productDetailsList))
                        {
                            productDetailsList.Add(tempProductDetail);
                        }
                    }
                }
                if (GroupedProductDetails.IsEditingItem)
                    GroupedProductDetails.CommitEdit();
                if (GroupedProductDetails.IsAddingNew)
                    GroupedProductDetails.CommitNew();
                GroupedProductDetails.Refresh();
                if (devicesManagement.ProductDetailsListDg.HasItems)
                {
                    devicesManagement.ProductDetailsListDg.SelectedItem = devicesManagement.ProductDetailsListDg.Items[0];
                    devicesManagement.ProductDetailsListDg.ScrollIntoView(devicesManagement.ProductDetailsListDg.SelectedItem);
                }
                UpdateDataStatus("Sẵn sàng");
            }
            //catch
            {
                UpdateDataStatus("Lỗi");
            }
        }

        public void ReloadListPallets(int bufferId)
        {

        }
        
        //*********************************************************************************************

        
        public bool ContainDevice(dtDevice tempOpe, List<dtDevice> Base)
        {
            foreach (dtDevice temp in Base)
            {
                if (temp.deviceId > 0)
                {
                    if (temp.deviceId == tempOpe.deviceId)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool ContainProduct(dtProduct tempOpe, List<dtProduct> List)
        {
            foreach (dtProduct temp in List)
            {
                if (temp.productId > 0)
                {
                    if (temp.productId == tempOpe.productId)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        
        public bool ContainBuffer(dtBuffer tempOpe, List<dtBuffer> List)
        {
            foreach (dtBuffer temp in List)
            {
                if (temp.bufferId > 0)
                {
                    if (temp.bufferId == tempOpe.bufferId)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool ContainDeviceProduct(dtDeviceProduct tempOpe, List<dtDeviceProduct> Base)
        {
            foreach (dtDeviceProduct temp in Base)
            {
                if (temp.deviceProductId > 0)
                {
                    if (temp.deviceProductId == tempOpe.deviceProductId)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool ContainDeviceBuffer(DeviceBuffer tempOpe, List<DeviceBuffer> Base)
        {
            foreach (DeviceBuffer temp in Base)
            {
                if (temp.deviceBufferId > 0)
                {
                    if (temp.deviceBufferId == tempOpe.deviceBufferId)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool ContainProductDetail(dtProductDetail tempOpe, List<dtProductDetail> Base)
        {
            foreach (dtProductDetail temp in Base)
            {
                if (temp.productDetailId > 0)
                {
                    if (temp.productDetailId == tempOpe.productDetailId)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        
        //*********************************************************************************************

        public void UpdateDataStatus(string status)
        {
            statusData = status;
        }

    }
}
