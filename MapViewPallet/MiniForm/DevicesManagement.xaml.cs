using MapViewPallet.MiniForm.MicsWpfForm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MapViewPallet.MiniForm
{
    /// <summary>
    /// Interaction logic for DevicesManagement.xaml
    /// </summary>
    public partial class DevicesManagement : Window
    {
        public ManagementModel managementModel;


        public DevicesManagement()
        {
            InitializeComponent();
            DeviceManagementTabControl.SelectionChanged += DeviceManagementTabControl_SelectionChanged;

            //*****************************************************************

            DevicesListDg.SelectedCellsChanged += DevicesListDg_SelectedCellsChanged;
            ProductsListDg.SelectedCellsChanged += ProductsListDg_SelectedCellsChanged;
            BuffersListDg.SelectedCellsChanged += BuffersListDg_SelectedCellsChanged;
            DevicesListDg2.SelectedCellsChanged += DevicesListDg2_SelectedCellsChanged;

            //*****************************************************************

            managementModel = new ManagementModel(this);
            DataContext = managementModel;

            //*****************************************************************

            DevicesListDg.SelectionMode =
                DeviceProductsListDg.SelectionMode =
                DeviceBuffersListDg.SelectionMode =
                ProductsListDg.SelectionMode =
                ProductDetailsListDg.SelectionMode =
                BuffersListDg.SelectionMode =
                PalletsListDg.SelectionMode =
                DevicesListDg2.SelectionMode = 
                DevicePalletsListDg.SelectionMode = 
                DataGridSelectionMode.Single;
            DevicesListDg.SelectionUnit =
                DeviceProductsListDg.SelectionUnit =
                DeviceBuffersListDg.SelectionUnit =
                ProductsListDg.SelectionUnit =
                ProductDetailsListDg.SelectionUnit =
                BuffersListDg.SelectionUnit =
                PalletsListDg.SelectionUnit =
                DevicesListDg2.SelectionUnit = 
                DevicePalletsListDg.SelectionUnit = 
                DataGridSelectionUnit.FullRow;

        }

        private void DevicesListDg2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DevicesListDg2.SelectedItem != null)
            {
                Console.WriteLine(managementModel.devicesList);
                dtDevice temp = DevicesListDg2.SelectedItem as dtDevice;
                MaxBaytb.Text = temp.maxBay.ToString();
                MaxRowtb.Text = temp.maxRow.ToString();
                managementModel.ReloadListDevicePallets(temp.deviceId);

            }
        }

        private void BuffersListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (BuffersListDg.SelectedItem != null)
            {
                dtBuffer temp = BuffersListDg.SelectedItem as dtBuffer;
                managementModel.ReloadListPallets(temp.bufferId);
            }
        }

        private void ProductsListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (ProductsListDg.SelectedItem != null)
            {
                dtProduct temp = ProductsListDg.SelectedItem as dtProduct;
                managementModel.ReloadListProductDetails(temp.productId);
            }
        }

        private void DevicesListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DevicesListDg.SelectedItem != null)
            {
                dtDevice temp = DevicesListDg.SelectedItem as dtDevice;
                managementModel.ReloadListDeviceProducts(temp.deviceId);
                managementModel.ReloadListDeviceBuffers(temp.deviceId);
            }
        }




        //****************************************************************************************

        private void DeviceManagementTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TabControl)
            {
                switch (((e.Source as System.Windows.Controls.TabControl).SelectedIndex))
                {
                    case 0:
                        {
                            managementModel.ReloadListDevices(((e.Source as System.Windows.Controls.TabControl).SelectedIndex));
                            break;
                        }
                    case 1:
                        {
                            managementModel.ReloadListProducts();
                            break;
                        }
                    case 2:
                        {
                            managementModel.ReloadListBuffers();
                            break;
                        }
                    case 3:
                        {
                            managementModel.ReloadListDevices(((e.Source as System.Windows.Controls.TabControl).SelectedIndex));
                            break;
                        }
                }
            }
        }

        private void DeviceBuffersListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (DevicesListDg.SelectedItem != null)
            {
                SaveData_tab1(false);
            }
            UpdateTab1(false);
        }

        //****************************************************************************************

        private void SaveData_tab1(bool isDeviceProduct)
        {
            managementModel.UpdateDataStatus("Đang cập nhật...");
            dtDevice device = GetDataSave();
            string jsonData = JsonConvert.SerializeObject(device);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/updateDevice");
            request.Method = "POST";
            request.ContentType = "application/json";

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
                int result = 0;
                int.TryParse(reader.ReadToEnd(), out result);
            }
            managementModel.UpdateDataStatus("Sẵn sàng");
        }

        private void SaveData_tab2()
        {
            dtProductDetail selectedProductDetail = (ProductDetailsListDg.SelectedItem as dtProductDetail);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/insertUpdateProductDetail");
            request.Method = "POST";
            request.ContentType = @"application/json";

            dtProductDetail productDetail = new dtProductDetail();
            productDetail.productId = ((ProductsListDg.SelectedItem) as dtProduct).productId;
            productDetail.productDetailId = selectedProductDetail.productDetailId;
            productDetail.productDetailName = selectedProductDetail.productDetailName;
            productDetail.creUsrId = selectedProductDetail.creUsrId;
            productDetail.updUsrId = selectedProductDetail.updUsrId;

            string jsonData = JsonConvert.SerializeObject(productDetail);

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
                int result = 0;
                int.TryParse(reader.ReadToEnd(), out result);
            }
        }

        private dtDevice GetDataSave()
        {
            dtDevice deviceData = new dtDevice();
            if (DevicesListDg.SelectedItem == null)
            {
                return deviceData;
            }
            int deviceID = 0;
            int.TryParse((DevicesListDg.SelectedItem as dtDevice).deviceId.ToString(), out deviceID);
            deviceData.deviceId = deviceID;
            deviceData.deviceName = (DevicesListDg.SelectedItem as dtDevice).deviceName.ToString().Trim();
            deviceData.creUsrId = Global_Object.userLogin;
            deviceData.updUsrId = Global_Object.userLogin;
            List<dtDeviceProduct> deviceProducts = new List<dtDeviceProduct>();
            foreach (dtDeviceProduct item in managementModel.deviceProductsList)
            {
                if (item.checkStatus)
                {
                    dtDeviceProduct deviceProductTemp = new dtDeviceProduct();
                    deviceProductTemp.deviceProductId = item.deviceProductId;
                    deviceProductTemp.productId = item.productId;
                    deviceProductTemp.checkStatus = item.checkStatus;
                    deviceProductTemp.checkStatus = item.checkStatus;
                    deviceProducts.Add(deviceProductTemp);

                }
            }
            deviceData.deviceProducts = deviceProducts;

            List<dtDeviceBuffer> deviceBuffers = new List<dtDeviceBuffer>();
            foreach (dtDeviceBuffer item in managementModel.deviceBuffersList)
            {
                if (item.checkStatus)
                {
                    dtDeviceBuffer deviceBuffer = new dtDeviceBuffer();
                    deviceBuffer.deviceBufferId = item.deviceBufferId;
                    deviceBuffer.bufferId = item.bufferId;
                    deviceBuffer.checkStatus = item.checkStatus;
                    deviceBuffer.bufferSort = item.bufferSort;
                    deviceBuffers.Add(deviceBuffer);
                }
            }
            deviceData.deviceBuffers = deviceBuffers;
            return deviceData;
        }

        public void UpdateTab1(bool isAddDevice)
        {
            managementModel.UpdateDataStatus("Đang cập nhật...");
            if (isAddDevice)
            {
                managementModel.ReloadListDevices(-1);
            }
            else
            {
                if (DevicesListDg.HasItems)
                {
                    if (DevicesListDg.SelectedItem == null)
                    {
                        DevicesListDg.SelectedItem = DevicesListDg.Items[0];
                        DevicesListDg.ScrollIntoView(DevicesListDg.SelectedItem);
                    }
                    else
                    {
                        managementModel.ReloadListDeviceProducts((DevicesListDg.SelectedItem as dtDevice).deviceId);
                        managementModel.ReloadListDeviceBuffers((DevicesListDg.SelectedItem as dtDevice).deviceId);
                    }
                }
                else
                {
                    managementModel.ReloadListDevices(0);
                }
            }
            managementModel.UpdateDataStatus("Sẵn sàng");
        }

        public void UpdateTab2(bool isAddProduct)
        {
            managementModel.UpdateDataStatus("Đang cập nhật...");
            if (isAddProduct)
            {
                managementModel.ReloadListProducts();
            }
            else
            {
                if (ProductsListDg.HasItems)
                {
                    if (ProductsListDg.SelectedItem == null)
                    {
                        ProductsListDg.SelectedItem = ProductsListDg.Items[0];
                        ProductsListDg.ScrollIntoView(ProductsListDg.SelectedItem);
                    }
                    else
                    {
                        managementModel.ReloadListProductDetails((ProductsListDg.SelectedItem as dtProduct).productId);
                    }
                }
                else
                {
                    managementModel.ReloadListProducts();
                }
            }
            managementModel.UpdateDataStatus("Sẵn sàng");
        }

        public void UpdateTab3(bool isAddBuffer)
        {
            managementModel.UpdateDataStatus("Đang cập nhật...");
            if (isAddBuffer)
            {
                managementModel.ReloadListBuffers();
            }
            else
            {
                if (BuffersListDg.HasItems)
                {
                    if (BuffersListDg.SelectedItem == null)
                    {
                        BuffersListDg.SelectedItem = BuffersListDg.Items[0];
                        BuffersListDg.ScrollIntoView(BuffersListDg.SelectedItem);
                    }
                    else
                    {
                        managementModel.ReloadListPallets((BuffersListDg.SelectedItem as dtBuffer).bufferId);

                    }
                }
                else
                {
                    managementModel.ReloadListBuffers();
                }
            }
            managementModel.UpdateDataStatus("Sẵn sàng");
        }

        //****************************************************************************************

        private void Btn_Save_tab1_Click(object sender, RoutedEventArgs e)
        {
            managementModel.UpdateDataStatus("Đang cập nhật...");
            foreach (dtDevice dr in managementModel.devicesList)
            {
                if ((dr.deviceName == "") || String.IsNullOrEmpty(dr.deviceName))
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "Devices Name", "Devices Name"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                dr.deviceName = dr.deviceName.ToString().Trim();
                dr.updUsrId = Global_Object.userLogin;
            }
            List<dynamic> postApiBody = new List<dynamic>();
            foreach (dtDevice dr in managementModel.devicesList)
            {
                dynamic postApiBodyChild = new JObject();
                postApiBodyChild.creUsrId = dr.creUsrId;
                postApiBodyChild.creDt = dr.creDt;
                postApiBodyChild.updUsrId = dr.updUsrId;
                postApiBodyChild.updDt = dr.updDt;
                postApiBodyChild.deviceId = dr.deviceId;
                postApiBodyChild.deviceName = dr.deviceName;
                //postApiBodyChild.deviceProducts = dr.deviceProducts;
                //postApiBodyChild.deviceBuffers = dr.deviceBuffers;
                postApiBody.Add(postApiBodyChild);
            }
            string jsonData = JsonConvert.SerializeObject(postApiBody, Formatting.Indented);
            //jsonData = jsonData.Trim();
            //jsonData = jsonData.Replace("  ", "");
            //jsonData = jsonData.Trim(new Char[] { ' '});
            jsonData = jsonData.Replace("\r\n", "");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/updateListNameDevice");
            request.Method = "POST";
            request.ContentType = "application/json";

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
                int result = 0;
                int.TryParse(reader.ReadToEnd(), out result);
                if (result == 1)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DevicesListDg.SelectedItem = DevicesListDg.Items[0];
                    DevicesListDg.ScrollIntoView(DevicesListDg.SelectedItem);
                }
                else if (result == -2)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Devices Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            managementModel.UpdateDataStatus("Sẵn sàng");
        }

        private void Btn_Exit_tab1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void Btn_Add_Device_Click(object sender, RoutedEventArgs e)
        {
            AddDeviceForm form = new AddDeviceForm(this);
            form.ShowDialog();
        }

        private void Btn_Refresh_Device_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab1(true);
        }

        private void Btn_Refresh_DeviceProduct_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab1(false);
        }

        private void Btn_Refresh_DeviceBuffer_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab1(false);
        }

        private void Btn_Save_DeviceProduct_Click(object sender, RoutedEventArgs e)
        {
            SaveData_tab1(true);
            UpdateTab1(false);
        }

        private void Btn_Save_DeviceBuffer_Click(object sender, RoutedEventArgs e)
        {
            SaveData_tab1(false);
            UpdateTab1(false);
        }

        private void DeviceProductCbRow_Click(object sender, RoutedEventArgs e)
        {

            if (DevicesListDg.SelectedItem != null)
            {
                SaveData_tab1(true);
            }
            UpdateTab1(false);

        }

        private void DeviceBufferCbRow_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesListDg.SelectedItem != null)
            {
                SaveData_tab1(false);
            }
            UpdateTab1(false);
        }

        //****************************************************************************************

        private void Btn_Exit_tab2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Add_Product_Click(object sender, RoutedEventArgs e)
        {
            AddProductForm form = new AddProductForm(this);
            form.ShowDialog();
        }

        private void Btn_Add_ProductDetail_Click(object sender, RoutedEventArgs e)
        {
            AddProductDetailForm form = new AddProductDetailForm(this);
            form.ShowDialog();
        }


        private void Btn_Refresh_Product_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab2(true);
        }

        private void Btn_Refresh_ProductDetail_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab2(false);
        }


        //****************************************************************************************


        private void Btn_Exit_tab3_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Add_Buffer_Click(object sender, RoutedEventArgs e)
        {
            AddBufferForm form = new AddBufferForm(this);
            form.ShowDialog();

        }


        private void Btn_Refresh_Buffer_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab3(true);
        }

        private void Btn_Refresh_Pallet_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab3(false);
        }




        private void ProductDetailsListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string productDetailName = ((e.EditingElement as System.Windows.Controls.TextBox).Text);
            if (ProductsListDg.SelectedItem != null)
            {
                dtProductDetail selectedProductDetail = (ProductDetailsListDg.SelectedItem as dtProductDetail);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/insertUpdateProductDetail");
                request.Method = "POST";
                request.ContentType = @"application/json";

                dtProductDetail productDetail = new dtProductDetail();
                productDetail.productId = ((ProductsListDg.SelectedItem) as dtProduct).productId;
                productDetail.productDetailId = selectedProductDetail.productDetailId;
                productDetail.productDetailName = productDetailName;
                productDetail.creUsrId = selectedProductDetail.creUsrId;
                productDetail.updUsrId = selectedProductDetail.updUsrId;

                string jsonData = JsonConvert.SerializeObject(productDetail);

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
                }
            }
            UpdateTab2(false);
        }

        private void Btn_Delete_Product_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListDg.SelectedItem != null)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/deleteProduct");
                request.Method = "DELETE";
                request.ContentType = @"application/json";

                dynamic postApiBody = new JObject();
                postApiBody.productId = (ProductsListDg.SelectedItem as dtProduct).productId;
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
                }
            }
            UpdateTab2(true);
        }

        private void Btn_Delete_ProductDetail_Click(object sender, RoutedEventArgs e)
        {
            if (ProductDetailsListDg.SelectedItem != null)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/deleteProductDetail");
                request.Method = "DELETE";
                request.ContentType = @"application/json";

                dynamic postApiBody = new JObject();
                postApiBody.productDetailId = (ProductDetailsListDg.SelectedItem as dtProductDetail).productDetailId;
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
                }
            }
            UpdateTab2(false);
        }

        private void Btn_Delete_Device_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesListDg.SelectedItem != null)
            {
                if (System.Windows.Forms.MessageBox.Show
                    (
                    string.Format(Global_Object.messageDeleteConfirm, "Device"),
                    Global_Object.messageTitileWarning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                    )
                {

                    List<dtDevice> listDelete = new List<dtDevice>();
                    dtDevice device = DevicesListDg.SelectedItem as dtDevice;
                    listDelete.Add(device);
                    string jsonData = JsonConvert.SerializeObject(listDelete);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/deleteDevice");
                    request.Method = "DELETE";
                    request.ContentType = "application/json";

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
                        int result = 0;
                        int.TryParse(reader.ReadToEnd(), out result);
                        if (result == 1)
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            managementModel.ReloadListDevices(0);
                        }
                        else if (result == 2)
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteUse, "Devices", "Other Screen"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void DevicesListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            dtDevice temp = (sender as System.Windows.Controls.DataGrid).SelectedItem as dtDevice;
            string deviceName = ((e.EditingElement as System.Windows.Controls.TextBox).Text);
            List<dtDevice> devices = new List<dtDevice>();
            temp.deviceName = deviceName;
            devices.Add(temp);

            if (devices.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string jsonData = JsonConvert.SerializeObject(devices);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/updateListNameDevice");
            request.Method = "POST";
            request.ContentType = "application/json";

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
                int result = 0;
                int.TryParse(reader.ReadToEnd(), out result);
                if (result == 1)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    managementModel.ReloadListDevices(0);
                }
                else if (result == -2)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Devices Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
    }
}
