using MapViewPallet.MiniForm.Database;
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

        public ListCollectionView GroupedDevicePallets { get; private set; }


        //***************VARIABLES*********************

        public List<dtDevice> devicesList;
        public List<dtDeviceBuffer> deviceBuffersList;
        public List<dtDeviceProduct> deviceProductsList;


        public List<dtProduct> productsList;
        public List<dtProductDetail> productDetailsList;


        public List<dtBuffer> buffersList;
        public List<dtPallet> palletsList;


        public List<dtDevicePallet> devicePalletsList;

        public ManagementModel(DevicesManagement devicesManagement)
        {
            UpdateDataStatus("Sẵn sàng");

            this.devicesManagement = devicesManagement;

            devicesList = new List<dtDevice>();
            deviceProductsList = new List<dtDeviceProduct>();
            deviceBuffersList = new List<dtDeviceBuffer>();

            productsList = new List<dtProduct>();
            productDetailsList = new List<dtProductDetail>();

            buffersList = new List<dtBuffer>();
            palletsList = new List<dtPallet>();

            devicePalletsList = new List<dtDevicePallet>();
            //********************************************************************

            GroupedDevices = (ListCollectionView)CollectionViewSource.GetDefaultView(devicesList);
            GroupedDeviceProducts = (ListCollectionView)CollectionViewSource.GetDefaultView(deviceProductsList);
            GroupedDeviceBuffers = (ListCollectionView)CollectionViewSource.GetDefaultView(deviceBuffersList);

            GroupedProducts = (ListCollectionView)CollectionViewSource.GetDefaultView(productsList);
            GroupedProductDetails = (ListCollectionView)CollectionViewSource.GetDefaultView(productDetailsList);
            
            GroupedBuffers = (ListCollectionView)CollectionViewSource.GetDefaultView(buffersList);
            GroupedPallets = (ListCollectionView)CollectionViewSource.GetDefaultView(palletsList);


            GroupedDevicePallets = (ListCollectionView)CollectionViewSource.GetDefaultView(devicePalletsList);

        }


        //*********************************************************************************************

        public void ReloadListDevices(int tabIndex)
        {
            Console.WriteLine("ReloadListDevices tab:" + tabIndex);
            //try
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
                            deviceName = dr["deviceName"].ToString(),
                            deviceNameOld = dr["deviceNameOld"].ToString(),
                            maxRow = int.Parse(dr["maxRow"].ToString()),
                            maxBay = int.Parse(dr["maxBay"].ToString()),
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
                switch (tabIndex)
                {
                    case 0:
                        {
                            if (devicesManagement.DevicesListDg.HasItems)
                            {
                                devicesManagement.DevicesListDg.SelectedItem = devicesManagement.DevicesListDg.Items[0];
                                devicesManagement.DevicesListDg.ScrollIntoView(devicesManagement.DevicesListDg.SelectedItem);
                            }
                            break;
                        }
                    case 1:
                        {
                            if (devicesManagement.DevicesListDg2.HasItems)
                            {
                                devicesManagement.DevicesListDg2.SelectedItem = devicesManagement.DevicesListDg2.Items[0];
                                devicesManagement.DevicesListDg2.ScrollIntoView(devicesManagement.DevicesListDg2.SelectedItem);
                            }
                            break;
                        }
                    case 2:
                        {
                            break;
                        }
                    case 3:
                        {
                            break;
                        }
                    default:
                        {
                            if (devicesManagement.DevicesListDg.HasItems)
                            {
                                devicesManagement.DevicesListDg.SelectedItem = devicesManagement.DevicesListDg.Items[0];
                                devicesManagement.DevicesListDg.ScrollIntoView(devicesManagement.DevicesListDg.SelectedItem);
                            }
                            if (devicesManagement.DevicesListDg2.HasItems)
                            {
                                devicesManagement.DevicesListDg2.SelectedItem = devicesManagement.DevicesListDg2.Items[0];
                                devicesManagement.DevicesListDg2.ScrollIntoView(devicesManagement.DevicesListDg2.SelectedItem);
                            }
                            break;
                        }
                }
                
                UpdateDataStatus("Sẵn sàng");

            }
            //catch
            {
                //UpdateDataStatus("Lỗi");
            }
        }

        public void ReloadListProducts()
        {
            //try
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
            //catch
            {
                //UpdateDataStatus("Lỗi");
                //Không có device nào
            }
        }

        public void ReloadListBuffers()
        {
            //try
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
            //catch
            {
                //UpdateDataStatus("Lỗi");
                //Không có device nào
            }
        }

        public void ReloadListDeviceProducts(int deviceId)
        {
            //try
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
            //catch
            {
                //UpdateDataStatus("Lỗi");
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
                        dtDeviceBuffer tempDeviceBuffer = new dtDeviceBuffer
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
                //UpdateDataStatus("Lỗi");
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
            //try
            {
                palletsList.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "pallet/getListPalletBufferId");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.bufferId = bufferId;
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

                    DataTable pallets = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in pallets.Rows)
                    {
                        dtPallet tempPallet = new dtPallet
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            palletId = int.Parse(dr["palletId"].ToString()),
                            deviceBufferId = int.Parse(dr["deviceBufferId"].ToString()),
                            bufferId = int.Parse(dr["bufferId"].ToString()),
                            planId = int.Parse(dr["planId"].ToString()),
                            row = int.Parse(dr["row"].ToString()),
                            bay = int.Parse(dr["bay"].ToString()),
                            dataPallet = dr["dataPallet"].ToString(),
                            palletStatus = dr["palletStatus"].ToString(),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                        };
                        if (!ContainPallet(tempPallet, palletsList))
                        {
                            palletsList.Add(tempPallet);
                        }
                    }
                }
                if (GroupedPallets.IsEditingItem)
                    GroupedPallets.CommitEdit();
                if (GroupedPallets.IsAddingNew)
                    GroupedPallets.CommitNew();
                GroupedPallets.Refresh();
                if (devicesManagement.PalletsListDg.HasItems)
                {
                    devicesManagement.PalletsListDg.SelectedItem = devicesManagement.PalletsListDg.Items[0];
                    devicesManagement.PalletsListDg.ScrollIntoView(devicesManagement.PalletsListDg.SelectedItem);
                }
                UpdateDataStatus("Sẵn sàng");
            }
            //catch
            {
                UpdateDataStatus("Lỗi");
            }
        }

        public void ReloadListDevicePallets (int deviceId)
        {
            //try
            {

                devicePalletsList.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/getListDevicePallet");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dtDevice device = new dtDevice();
                device.deviceId = deviceId;
                string jsonData = JsonConvert.SerializeObject(device);
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

                    DataTable pallets = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in pallets.Rows)
                    {
                        dtDevicePallet tempDevicePallet = new dtDevicePallet
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            devicePalletId = int.Parse(dr["devicePalletId"].ToString()),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            devicePalletName = dr["devicePalletName"].ToString(),
                            row = int.Parse(dr["row"].ToString()),
                            bay = int.Parse(dr["bay"].ToString()),
                            dataPallet = dr["dataPallet"].ToString()
                        };
                        if (!ContainDevicePallet(tempDevicePallet, devicePalletsList))
                        {
                            devicePalletsList.Add(tempDevicePallet);
                        }
                    }
                }
                if (GroupedDevicePallets.IsEditingItem)
                    GroupedDevicePallets.CommitEdit();
                if (GroupedDevicePallets.IsAddingNew)
                    GroupedDevicePallets.CommitNew();
                GroupedDevicePallets.Refresh();
                if (devicesManagement.DevicePalletsListDg.HasItems)
                {
                    devicesManagement.DevicePalletsListDg.SelectedItem = devicesManagement.DevicePalletsListDg.Items[0];
                    devicesManagement.DevicePalletsListDg.ScrollIntoView(devicesManagement.DevicePalletsListDg.SelectedItem);
                }
                UpdateDataStatus("Sẵn sàng");
            }
            //catch
            {
                UpdateDataStatus("Lỗi");
            }
        }
        



        //*********************************************************************************************


        public bool ContainDevice(dtDevice tempOpe, List<dtDevice> List)
        {
            foreach (dtDevice temp in List)
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

        public bool ContainDeviceProduct(dtDeviceProduct tempOpe, List<dtDeviceProduct> List)
        {
            foreach (dtDeviceProduct temp in List)
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

        public bool ContainDeviceBuffer(dtDeviceBuffer tempOpe, List<dtDeviceBuffer> List)
        {
            foreach (dtDeviceBuffer temp in List)
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

        public bool ContainProductDetail(dtProductDetail tempOpe, List<dtProductDetail> List)
        {
            foreach (dtProductDetail temp in List)
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

        public bool ContainPallet(dtPallet tempOpe, List<dtPallet> List)
        {
            foreach (dtPallet temp in List)
            {
                if (temp.palletId > 0)
                {
                    if (temp.palletId == tempOpe.palletId)
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

        public bool ContainDevicePallet(dtDevicePallet tempOpe, List<dtDevicePallet> List)
        {
            foreach (dtDevicePallet temp in List)
            {
                if (temp.devicePalletId > 0)
                {
                    if (temp.devicePalletId == tempOpe.devicePalletId)
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
