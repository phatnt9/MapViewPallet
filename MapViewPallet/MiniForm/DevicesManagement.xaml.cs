using MapViewPallet.MiniForm.Database;
using MapViewPallet.MiniForm.MicsWpfForm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
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

            ProductsListDg.CellEditEnding += ProductsListDg_CellEditEnding;
            ProductDetailsListDg.CellEditEnding += ProductDetailsListDg_CellEditEnding;
            BuffersListDg.CellEditEnding += BuffersListDg_CellEditEnding; ;
            DeviceBuffersListDg.CellEditEnding += DeviceBuffersListDg_CellEditEnding;
            DevicesListDg2.CellEditEnding += DevicesListDg2_CellEditEnding;

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
                //Console.WriteLine(managementModel.devicesList);
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
                            managementModel.ReloadListDevices(((e.Source as System.Windows.Controls.TabControl).SelectedIndex));
                            break;
                        }
                    case 2:
                        {
                            managementModel.ReloadListProducts();
                            break;
                        }
                    case 3:
                        {
                            managementModel.ReloadListBuffers();
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

        private void ProductDetailsListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
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
                UpdateTab3(false);
            }
            catch
            {

            }
        }

        private void DevicesListDg2_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
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
                        managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
                    }
                    else if (result == -2)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Devices Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                UpdateTab2(true);
            }
            catch
            {

            }
        }

        private void BuffersListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //http://localhost:8081/robot/rest/buffer/updateListBuffer
            try
            {
                dtBuffer temp = (sender as System.Windows.Controls.DataGrid).SelectedItem as dtBuffer;
                string bufferName = ((e.EditingElement as System.Windows.Controls.TextBox).Text);
                List<dtBuffer> buffers = new List<dtBuffer>();
                temp.bufferName = bufferName;
                buffers.Add(temp);

                if (buffers.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string jsonData = JsonConvert.SerializeObject(buffers);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "buffer/updateListBuffer");
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
                    }
                    else if (result == -2)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Buffers Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                UpdateTab4(true);
            }
            catch
            {

            }

        }

        private void ProductsListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                string productName = ((e.EditingElement as System.Windows.Controls.TextBox).Text);
                if (ProductsListDg.SelectedItem != null)
                {
                    dtProduct selectedProduct = (ProductsListDg.SelectedItem as dtProduct);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/insertUpdateProduct");
                    request.Method = "POST";
                    request.ContentType = @"application/json";

                    dtProduct product = new dtProduct();
                    product.productId = ((ProductsListDg.SelectedItem) as dtProduct).productId;
                    product.productName = productName;
                    product.productDetails = new List<dtProductDetail>();
                    product.creUsrId = selectedProduct.creUsrId;
                    product.updUsrId = selectedProduct.updUsrId;

                    string jsonData = JsonConvert.SerializeObject(product);

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
                        }
                        else if (result == -2)
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Product Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                UpdateTab3(true);
            }
            catch
            {

            }
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
                managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
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
                    managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
                }
            }
            managementModel.UpdateDataStatus("Sẵn sàng");
        }

        public void UpdateTab2(bool isAddDevice)
        {
            managementModel.UpdateDataStatus("Đang cập nhật...");
            if (isAddDevice)
            {
                managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
            }
            else
            {
                if (DevicesListDg2.HasItems)
                {
                    if (DevicesListDg2.SelectedItem == null)
                    {
                        DevicesListDg2.SelectedItem = DevicesListDg2.Items[0];
                        DevicesListDg2.ScrollIntoView(DevicesListDg2.SelectedItem);
                    }
                    else
                    {
                        managementModel.ReloadListDevicePallets((DevicesListDg2.SelectedItem as dtDevice).deviceId);
                    }
                }
                else
                {
                    managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
                }
            }
            managementModel.UpdateDataStatus("Sẵn sàng");
        }

        public void UpdateTab3(bool isAddProduct)
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

        public void UpdateTab4(bool isAddBuffer)
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
            UpdateTab3(true);
        }

        private void Btn_Refresh_ProductDetail_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab3(false);
        }


        //****************************************************************************************


        private void Btn_Exit_Buffer_Click(object sender, RoutedEventArgs e)
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
            UpdateTab4(true);
        }

        private void Btn_Refresh_Pallet_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab4(false);
        }


        //****************************************************************************************

        private void Btn_Refresh_Device2_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab1(true);
        }


        private void Btn_Delete_Product_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListDg.SelectedItem != null)
            {
                if (System.Windows.Forms.MessageBox.Show
                    (
                    string.Format(Global_Object.messageDeleteConfirm, "Product"),
                    Global_Object.messageTitileWarning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                    )
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
                        int result = 0;
                        int.TryParse(reader.ReadToEnd(), out result);
                        if (result == 1)
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
                        }
                        else if (result == 2)
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteUse, "Products", "Other Screen"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            UpdateTab3(true);
        }

        private void Btn_Delete_ProductDetail_Click(object sender, RoutedEventArgs e)
        {
            if (ProductDetailsListDg.SelectedItem != null)
            {
                if (System.Windows.Forms.MessageBox.Show
                    (
                    string.Format(Global_Object.messageDeleteConfirm, "Product Detail"),
                    Global_Object.messageTitileWarning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                    )
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
                        int result = 0;
                        int.TryParse(reader.ReadToEnd(), out result);
                        if (result == 1)
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
                        }
                        else if (result == 2)
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteUse, "Product Details", "Other Screen"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            UpdateTab3(false);
        }

        private void Btn_Delete_Device_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesListDg2.SelectedItem != null)
            {
                if (System.Windows.Forms.MessageBox.Show
                    (
                    string.Format(Global_Object.messageDeleteConfirm, "Device"),
                    Global_Object.messageTitileWarning, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                    )
                {

                    List<dtDevice> listDelete = new List<dtDevice>();
                    dtDevice device = DevicesListDg2.SelectedItem as dtDevice;
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
                            managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
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
            UpdateTab2(true);
        }

        private void Btn_Delete_Buffer_Click(object sender, RoutedEventArgs e)
        {
            if (BuffersListDg.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show
                    (
                    String.Format(Global_Object.messageNothingSelected),
                    Global_Object.messageTitileWarning,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
                return;
            }

            if (System.Windows.Forms.MessageBox.Show(
                String.Format(Global_Object.messageDeleteConfirm, "Buffer"),
                Global_Object.messageTitileWarning,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                List<dtBuffer> listDelete = new List<dtBuffer>();
                dtBuffer buffer = BuffersListDg.SelectedItem as dtBuffer;
                listDelete.Add(buffer);

                string jsonData = JsonConvert.SerializeObject(listDelete);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "buffer/deleteListBuffer");
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
                        managementModel.ReloadListBuffers();
                    }
                    else if (result == 2)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteUse, "Buffer", "Other Screen"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

            }
        }

        private void Btn_Apply_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesListDg2.SelectedItem != null)
            {
                int maxBay = 0;
                int.TryParse(this.MaxBaytb.Text, out maxBay);
                int maxRow = 0;
                int.TryParse(this.MaxRowtb.Text, out maxRow);

                List<dtDevicePallet> devicePalletsListOld = new List<dtDevicePallet>();

                foreach (dtDevicePallet item in managementModel.devicePalletsList)
                {
                    devicePalletsListOld.Add(item);
                }


                dtDevice selectedDevice = DevicesListDg2.SelectedItem as dtDevice;

                if (maxBay != int.Parse(selectedDevice.maxBay.ToString()) ||
                   maxRow != int.Parse(selectedDevice.maxRow.ToString()))
                {
                    if (managementModel.devicePalletsList != null)
                    {
                        managementModel.devicePalletsList.Clear();
                    }
                    for (int i = 0; i < maxRow; i++)
                    {
                        for (int j = 0; j < maxBay; j++)
                        {
                            dtDevicePallet dr = new dtDevicePallet
                            {
                                devicePalletName = "Pallet-" + i + "-" + j,
                                row = i,
                                bay = j,
                                deviceId = selectedDevice.deviceId,
                                creUsrId = selectedDevice.creUsrId,
                                updUsrId = selectedDevice.updUsrId
                            };
                            
                            foreach (dtDevicePallet drOld in devicePalletsListOld)
                            {
                                int rowOld = 0;
                                int bayOld = 0;

                                int.TryParse(drOld.row.ToString(), out rowOld);
                                int.TryParse(drOld.bay.ToString(), out bayOld);

                                if (i <= rowOld)
                                {
                                    if (bayOld == j)
                                    {
                                        dr.dataPallet = drOld.dataPallet;
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            managementModel.devicePalletsList.Add(dr);
                        }
                    }
                    if (managementModel.GroupedDevicePallets.IsEditingItem)
                        managementModel.GroupedDevicePallets.CommitEdit();
                    if (managementModel.GroupedDevicePallets.IsAddingNew)
                        managementModel.GroupedDevicePallets.CommitNew();
                    managementModel.GroupedDevicePallets.Refresh();
                }
            }
        }

        private void Btn_Exit_DevicePallet_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Save_DevicePallet_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceManagementTabControl.SelectedIndex == 1)
            {
                List<dtDevicePallet> devicePallets = new List<dtDevicePallet>();
                foreach (dtDevicePallet dr in managementModel.devicePalletsList)
                {
                    dtDevicePallet devicePallet = new dtDevicePallet();

                    devicePallet.devicePalletId = int.Parse(dr.devicePalletId.ToString());
                    devicePallet.devicePalletName = dr.devicePalletName.ToString();
                    devicePallet.deviceId = int.Parse(dr.deviceId.ToString());
                    devicePallet.row = int.Parse(dr.row.ToString());
                    devicePallet.bay = int.Parse(dr.bay.ToString());
                    devicePallet.dataPallet = (dr.dataPallet != null) ? dr.dataPallet.ToString():"" ;
                    devicePallet.creUsrId = Global_Object.userLogin;
                    devicePallet.updUsrId = Global_Object.userLogin;

                    devicePallets.Add(devicePallet);
                }

                if (devicePallets.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string jsonData = JsonConvert.SerializeObject(devicePallets);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/insertUpdateDevicePallet");
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
                        managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        private void Btn_Exit_Product_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
