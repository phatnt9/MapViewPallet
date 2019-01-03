using MapViewPallet.MiniForm.Database;
using MapViewPallet.MiniForm.MicsWpfForm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        public DevicesManagementModel managementModel;

        MainWindow mainWindow;
        int loadTab = 0;


        public DevicesManagement(MainWindow mainWindow, int loadtab, string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            Loaded += DevicesManagement_Loaded;
            this.mainWindow = mainWindow;
            loadTab = loadtab;
            DeviceManagementTabControl.SelectionChanged += DeviceManagementTabControl_SelectionChanged;

            //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

            DevicesListDg.SelectedCellsChanged += DevicesListDg_SelectedCellsChanged;
            DevicesListDg2.SelectedCellsChanged += DevicesListDg2_SelectedCellsChanged;
            ProductsListDg.SelectedCellsChanged += ProductsListDg_SelectedCellsChanged;
            BuffersListDg.SelectedCellsChanged += BuffersListDg_SelectedCellsChanged;
            DevicePalletsListDg.SelectedCellsChanged += DevicePalletsListDg_SelectedCellsChanged;


            DevicesListDg2.MouseDoubleClick += DevicesListDg2_MouseDoubleClick;
            ProductsListDg.MouseDoubleClick += ProductsListDg_MouseDoubleClick;


            DevicesListDg2.CellEditEnding += DevicesListDg2_CellEditEnding;
            ProductsListDg.CellEditEnding += ProductsListDg_CellEditEnding;
            BuffersListDg.CellEditEnding += BuffersListDg_CellEditEnding;
            DeviceBuffersListDg.CellEditEnding += DeviceBuffersListDg_CellEditEnding;
            ProductDetailsListDg.CellEditEnding += ProductDetailsListDg_CellEditEnding;

            //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

            managementModel = new DevicesManagementModel(this);
            DataContext = managementModel;
            //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
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
            //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        }

        public void ApplyLanguage(string cultureName = null)
        {
            if (cultureName != null)
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);

            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "vi-VN":
                    dict.Source = new Uri("..\\Lang\\Vietnamese.xaml", UriKind.Relative);
                    break;
                // ...
                default:
                    dict.Source = new Uri("..\\Lang\\English.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void DevicesManagement_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeTabIndex(loadTab);
        }





        //**********************************************

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

        public void ChangeTabIndex(int x)
        {
            Dispatcher.BeginInvoke((Action)(() => DeviceManagementTabControl.SelectedIndex = x));
        }



        //********************************************************TAB 0******************************************************

        private void DevicesListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DevicesListDg.SelectedItem != null)
            {
                dtDevice temp = DevicesListDg.SelectedItem as dtDevice;
                managementModel.ReloadListDeviceProducts(temp.deviceId);
                managementModel.ReloadListDeviceBuffers(temp.deviceId);
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

        private void DeviceBufferCbRow_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesListDg.SelectedItem != null)
            {
                SaveData_tab1(false);
            }
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

        private void SaveData_tab1(bool isDeviceProduct)
        {
            if (DevicesListDg.SelectedItem == null)
            {
                return;
            }

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
                    int sort = 1;
                    int.TryParse(item.bufferSort.ToString(), out sort);
                    deviceBuffer.bufferSort = sort;
                    deviceBuffers.Add(deviceBuffer);
                }
            }
            deviceData.deviceBuffers = deviceBuffers;
            return deviceData;
        }
        //********************************************************TAB 1******************************************************

        private void DevicesListDg2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DevicesListDg2.SelectedItem != null)
            {
                dtDevice temp = DevicesListDg2.SelectedItem as dtDevice;
                MaxBaytb.Text = temp.maxBay.ToString();
                MaxRowtb.Text = temp.maxRow.ToString();
                managementModel.ReloadListDevicePallets(temp.deviceId);
            }
        }

        private void DevicesListDg2_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                //DataGridCellInfo dgci = (DataGridCellInfo)(DevicesListDg2).CurrentCell;
                switch (e.Column.DisplayIndex)
                {
                    case 1:
                        {
                            dtDevice temp = (sender as System.Windows.Controls.DataGrid).SelectedItem as dtDevice;
                            string deviceName = ((e.EditingElement as System.Windows.Controls.TextBox).Text.Trim());
                            List<dtDevice> devices = new List<dtDevice>();
                            temp.deviceName = deviceName;
                            temp.updUsrId = Global_Object.userLogin;
                            devices.Add(temp);

                            if (devices.Count == 0)
                            {
                                System.Windows.Forms.MessageBox.Show
                                    (
                                    String.Format(Global_Object.messageNoDataSave),
                                    Global_Object.messageTitileWarning,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                    );
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
                                    System.Windows.Forms.MessageBox.Show
                                        (
                                        String.Format(Global_Object.messageSaveSucced),
                                        Global_Object.messageTitileInformation,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information
                                        );

                                    managementModel.ReloadListDevices(DeviceManagementTabControl.SelectedIndex);
                                }
                                else if (result == -2)
                                {
                                    System.Windows.Forms.MessageBox.Show
                                        (
                                        String.Format(Global_Object.messageDuplicated, "Devices Name"),
                                        Global_Object.messageTitileError,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error
                                        );
                                }
                                else
                                {
                                    System.Windows.Forms.MessageBox.Show
                                        (
                                        String.Format(Global_Object.messageSaveFail),
                                        Global_Object.messageTitileError,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error
                                        );
                                }
                            }
                            UpdateTab2(true);
                            break;
                        }
                }

            }
            catch
            {

            }
        }

        private void DevicesListDg2_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridCellInfo dgci = (DataGridCellInfo)((System.Windows.Controls.DataGrid)sender).CurrentCell;
            switch (dgci.Column.DisplayIndex)
            {
                case 2:
                    {
                        OpenFileDialog fileDialog = new OpenFileDialog();
                        fileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
                        if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            (DevicesListDg2.SelectedItem as dtDevice).pathFile = fileDialog.FileName;
                            (DevicesListDg2.SelectedItem as dtDevice).imageDeviceUrl = Path.GetFileName(fileDialog.FileName);
                        }
                        break;
                    }
            }

            if (uploadFileDevices() == 0)
            {
                System.Windows.Forms.MessageBox.Show
                    (
                    String.Format(Global_Object.messageSaveFail),
                    Global_Object.messageTitileError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                return;
            }
        }

        private void Btn_Add_Device_Click(object sender, RoutedEventArgs e)
        {
            AddDeviceForm form = new AddDeviceForm(this, Thread.CurrentThread.CurrentCulture.ToString());
            form.ShowDialog();
        }

        private void Btn_Delete_Device_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesListDg2.SelectedItem != null)
            {
                if (System.Windows.Forms.MessageBox.Show
                    (
                    String.Format(Global_Object.messageDeleteConfirm, "Device"),
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

        private void Btn_Save_Devices_Click(object sender, RoutedEventArgs e)
        {
            List<dtDevice> devices = new List<dtDevice>();
            foreach (dtDevice dr in managementModel.devicesList)
            {
                if (dr.deviceName.ToString().Trim() != dr.deviceNameOld.ToString().Trim()
                    || (((dr.pathFile != null) ? dr.pathFile.ToString() : "").Trim() != ""))
                {
                    if (String.IsNullOrEmpty(dr.deviceName.ToString()) || dr.deviceName.ToString().Trim() == "")
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "Devices Name", "Devices Name"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    dtDevice device = new dtDevice();
                    device.deviceId = int.Parse(dr.deviceId.ToString());
                    device.deviceName = dr.deviceName.ToString().Trim();
                    device.imageDeviceUrl = dr.imageDeviceUrl.ToString();
                    device.imageDeviceUrlOld = dr.imageDeviceUrlOld.ToString();
                    device.updUsrId = Global_Object.userLogin;

                    devices.Add(device);
                }
            }

            if (devices.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (uploadFileDevices() == 0)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    return;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void Btn_Apply_DeviceBufferData_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesListDg2.SelectedItem != null)
            {
                int maxBay = 0;
                int.TryParse(this.MaxBaytb.Text, out maxBay);
                int maxRow = 0;
                int.TryParse(this.MaxRowtb.Text, out maxRow);

                managementModel.ReloadListDevicePallets((DevicesListDg2.SelectedItem as dtDevice).deviceId);

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
                    for (int r = 0; r < maxRow; r++)
                    {
                        for (int b = 0; b < maxBay; b++)
                        {
                            dtDevicePallet dr = new dtDevicePallet
                            {
                                devicePalletName = "Pallet-" + r + "-" + b,
                                row = r,
                                bay = b,
                                deviceId = selectedDevice.deviceId,

                                dataPallet = "{\"line\":{\"x\":0.0,\"y\":0.0,\"angle\":0.0},\"pallet\":{\"row\":0.0,\"bay\":0.0,\"direct\":0.0}}",
                                creUsrId = Global_Object.userLogin,
                                updUsrId = Global_Object.userLogin
                            };

                            foreach (dtDevicePallet drOld in devicePalletsListOld)
                            {
                                int rowOld = 0;
                                int bayOld = 0;

                                int.TryParse(drOld.row.ToString(), out rowOld);
                                int.TryParse(drOld.bay.ToString(), out bayOld);

                                if (r == rowOld)
                                {
                                    if (bayOld == b)
                                    {
                                        dr.devicePalletId = drOld.devicePalletId;
                                        dr.dataPallet = drOld.dataPallet;
                                        break;
                                    }
                                }
                                else
                                {
                                    //break;
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

        //********************************************************TAB 2******************************************************

        private void ProductsListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (ProductsListDg.SelectedItem != null)
            {
                dtProduct temp = ProductsListDg.SelectedItem as dtProduct;
                managementModel.ReloadListProductDetails(temp.productId);
            }
        }

        private void ProductsListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                string productName = ((e.EditingElement as System.Windows.Controls.TextBox).Text.Trim());
                if (ProductsListDg.SelectedItem != null)
                {
                    dtProduct selectedProduct = (ProductsListDg.SelectedItem as dtProduct);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/insertUpdateProduct");
                    request.Method = "POST";
                    request.ContentType = @"application/json";

                    dtProduct product = new dtProduct();
                    product.productId = ((ProductsListDg.SelectedItem) as dtProduct).productId;
                    product.productName = productName;
                    product.imageProductUrl = (ProductsListDg.SelectedItem as dtProduct).imageProductUrl.ToString();
                    product.imageProductUrlOld = (ProductsListDg.SelectedItem as dtProduct).imageProductUrlOld.ToString();
                    product.productDetails = new List<dtProductDetail>();
                    product.creUsrId = Global_Object.userLogin;
                    product.updUsrId = Global_Object.userLogin;

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

        private void ProductDetailsListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                string productDetailName = ((e.EditingElement as System.Windows.Controls.TextBox).Text.Trim());
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
                    productDetail.creUsrId = Global_Object.userLogin;
                    productDetail.updUsrId = Global_Object.userLogin;

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

        private void ProductsListDg_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridCellInfo dgci = (DataGridCellInfo)((System.Windows.Controls.DataGrid)sender).CurrentCell;
            switch (dgci.Column.DisplayIndex)
            {
                case 2:
                    {
                        OpenFileDialog fileDialog = new OpenFileDialog();
                        fileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
                        if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {

                            (ProductsListDg.SelectedItem as dtProduct).pathFile = fileDialog.FileName;
                            (ProductsListDg.SelectedItem as dtProduct).imageProductUrl = Path.GetFileName(fileDialog.FileName);
                        }
                        break;
                    }
            }

            if (uploadFileProducts() == 0)
            {
                System.Windows.Forms.MessageBox.Show
                    (
                    String.Format(Global_Object.messageSaveFail),
                    Global_Object.messageTitileError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                return;
            }
        }

        private void Btn_Add_Product_Click(object sender, RoutedEventArgs e)
        {
            AddProductForm form = new AddProductForm(this, Thread.CurrentThread.CurrentCulture.ToString());
            form.ShowDialog();
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

        private void Btn_Refresh_ProductDetail_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab3(false);
        }

        private void Btn_Refresh_Product_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab3(true);
        }

        private void Btn_Add_ProductDetail_Click(object sender, RoutedEventArgs e)
        {
            AddProductDetailForm form = new AddProductDetailForm(this, Thread.CurrentThread.CurrentCulture.ToString());
            form.ShowDialog();
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

        private void Btn_Save_Products_Click(object sender, RoutedEventArgs e)
        {
            List<dtProduct> products = new List<dtProduct>();
            foreach (dtProduct dr in managementModel.productsList)
            {
                if (((dr.pathFile != null) ? dr.pathFile.ToString() : "").Trim() != "")
                {

                    dtProduct product = new dtProduct();
                    product.productId = int.Parse(dr.productId.ToString());
                    product.productName = dr.productName.ToString().Trim();
                    product.imageProductUrl = dr.imageProductUrl.ToString();
                    product.imageProductUrlOld = dr.imageProductUrlOld.ToString();
                    product.updUsrId = Global_Object.userLogin;

                    products.Add(product);
                }
            }

            if (products.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (uploadFileProducts() == 0)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string jsonData = JsonConvert.SerializeObject(products);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/insertUpdateListProduct");
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
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Products Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        //********************************************************TAB 3******************************************************

        private void BuffersListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (BuffersListDg.SelectedItem != null)
            {
                dtBuffer temp = BuffersListDg.SelectedItem as dtBuffer;
                Console.WriteLine(temp.bufferData);
                dynamic bufferData = JsonConvert.DeserializeObject(temp.bufferData);
                bufferX.Text = (bufferData != null) ? (((double)bufferData.x).ToString()) : "0";
                bufferY.Text = (bufferData != null) ? (((double)bufferData.y).ToString()) : "0";
                bufferA.Text = (bufferData != null) ? (((double)bufferData.angle).ToString()) : "0";
                managementModel.ReloadListPallets(temp.bufferId);
            }
        }


        private void BuffersListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                dtBuffer temp = (sender as System.Windows.Controls.DataGrid).SelectedItem as dtBuffer;
                temp.updUsrId = Global_Object.userLogin;
                string bufferCellEdit = ((e.EditingElement as System.Windows.Controls.TextBox).Text.Trim());
                List<dtBuffer> buffers = new List<dtBuffer>();
                switch (e.Column.DisplayIndex)
                {
                    case 1:
                        {
                            temp.bufferName = bufferCellEdit;
                            break;
                        }
                    case 2:
                        {
                            temp.maxBay = int.Parse(bufferCellEdit);
                            break;
                        }
                    case 3:
                        {
                            temp.maxRow = int.Parse(bufferCellEdit);
                            break;
                        }
                    case 5:
                        {
                            temp.bufferCheckIn = bufferCellEdit;
                            break;
                        }
                }
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
                        //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
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


        private void DevicePalletsListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DevicePalletsListDg.SelectedItem != null)
            {
                dtDevicePallet devicePallet = DevicePalletsListDg.SelectedItem as dtDevicePallet;
                dynamic devicePalletData = JsonConvert.DeserializeObject(devicePallet.dataPallet);
                palletLineX.Text = (devicePalletData != null) ? (((double)devicePalletData.line.x).ToString()) : "0";
                palletLineY.Text = (devicePalletData != null) ? (((double)devicePalletData.line.y).ToString()) : "0";
                palletLineA.Text = (devicePalletData != null) ? (((double)devicePalletData.line.angle).ToString()) : "0";
                palletPalletD.Text = (devicePalletData != null) ? (((double)devicePalletData.pallet.direct).ToString()) : "0";
            }
        }

        private void Btn_Refresh_Buffer_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab4(true);
        }

        private void Btn_Add_Buffer_Click(object sender, RoutedEventArgs e)
        {
            AddBufferForm form = new AddBufferForm(this, Thread.CurrentThread.CurrentCulture.ToString());
            form.ShowDialog();

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

        private void Btn_SetBufferData_Click(object sender, RoutedEventArgs e)
        {
            //if (BuffersListDg.SelectedItem != null)
            //{
            //    dtBuffer temp = BuffersListDg.SelectedItem as dtBuffer;
            //    Console.WriteLine(temp.bufferData);
            //    dynamic bufferData = JsonConvert.DeserializeObject(temp.bufferData);
            //    dynamic postApiBody = new JObject();
            //    postApiBody.productId = productId;
            //    managementModel.ReloadListPallets(temp.bufferId);
            //}

            //try
            {
                dtBuffer buffer = BuffersListDg.SelectedItem as dtBuffer;
                List<dtBuffer> buffers = new List<dtBuffer>();

                dynamic postApiBody = new JObject();
                postApiBody.x = double.Parse((bufferX.Text != "") ? bufferX.Text : "0");
                postApiBody.y = double.Parse((bufferY.Text != "") ? bufferY.Text : "0");
                postApiBody.angle = double.Parse((bufferA.Text != "") ? bufferA.Text : "0");
                string jsonBufferData = JsonConvert.SerializeObject(postApiBody);
                buffer.bufferData = jsonBufferData;

                buffers.Add(buffer);

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
            //catch
            {

            }
        }


        private void Btn_Save_Pallet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //dtPallet temp = (sender as System.Windows.Controls.DataGrid).SelectedItem as dtPallet;
                //temp.updUsrId = Global_Object.userLogin;
                //string palletCellEdit = ((e.EditingElement as System.Windows.Controls.TextBox).Text.Trim());
                List<dtPallet> pallets = new List<dtPallet>();
                //switch (e.Column.DisplayIndex)
                //{
                //    case 4:
                //        {
                //            temp.dataPallet = palletCellEdit;
                //            break;
                //        }
                //}
                foreach (dtPallet pallet in managementModel.palletsList)
                {
                    pallet.updUsrId = Global_Object.userLogin;
                    pallets.Add(pallet);
                }

                if (pallets.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string jsonData = JsonConvert.SerializeObject(pallets);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "pallet/updateListPalletData");
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
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Pallets Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                UpdateTab4(false);
            }
            catch
            {

            }
        }

        private void Btn_Refresh_Pallet_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab4(false);
        }
        //****************************************************************************************

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
            mainWindow.canvasControlService.ReloadAllStation();
            managementModel.UpdateDataStatus("Sẵn sàng");
        }

        //****************************************************************************************

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
                    string initiateDataPallet = "{\"line\":{\"x\":0.0,\"y\":0.0,\"angle\":0.0},\"pallet\":{\"row\":" + (double)dr.row + ",\"bay\":" + (double)dr.bay + ",\"direct\":0.0}}";
                    devicePallet.dataPallet = (dr.dataPallet != null) ? dr.dataPallet.ToString() : initiateDataPallet;
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

        //*******************************************************************************************************************
        private void Btn_Exit_Device_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Exit_Buffer_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Exit_DevicePallet_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Exit_Product_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //*******************************************************************************************************************

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void BufferX_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void BufferY_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void BufferA_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }


        private void MaxBaytb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void MaxRowtb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private int uploadFileProducts()
        {
            List<string> files = new List<string>();
            foreach (dtProduct dr in managementModel.productsList)
            {
                if (((dr.pathFile != null) ? dr.pathFile.ToString() : "") != "")
                {
                    files.Add(dr.pathFile.ToString());
                }
            }
            if (files.Count > 0)
            {
                return UploadFilesToRemoteUrl(Global_Object.url + "upload", files);
            }
            return 1;
        }

        private int uploadFileDevices()
        {
            List<string> files = new List<string>();
            foreach (dtDevice dr in managementModel.devicesList)
            {
                if (((dr.pathFile != null) ? dr.pathFile.ToString() : "") != "")
                {
                    files.Add(dr.pathFile.ToString());
                }
            }
            if (files.Count > 0)
            {
                return UploadFilesToRemoteUrl(Global_Object.url + "upload", files);
            }
            return 1;
        }

        public int UploadFilesToRemoteUrl(string url, List<string> files, NameValueCollection formFields = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" +
                                    boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            Stream memStream = new System.IO.MemoryStream();

            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                    boundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "--");


            string formdataTemplate = "\r\n--" + boundary +
                                        "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            if (formFields != null)
            {
                foreach (string key in formFields.Keys)
                {
                    string formitem = string.Format(formdataTemplate, key, formFields[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }

            string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                "Content-Type: application/octet-stream\r\n\r\n";

            for (int i = 0; i < files.Count; i++)
            {
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header = string.Format(headerTemplate, "files", files[i]);
                var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);

                using (var fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            request.ContentLength = memStream.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }
            int result = 0;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                int.TryParse(reader.ReadToEnd(), out result);
            }
            return result;
        }

        private void Btn_Save_Buffer_Click(object sender, RoutedEventArgs e)
        {


        }

        private void BufferReturnCbRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //temp.updUsrId = Global_Object.userLogin;
                List<dtBuffer> buffers = new List<dtBuffer>();
                foreach (dtBuffer buffer in managementModel.buffersList)
                {
                    buffer.updUsrId = Global_Object.userLogin;
                    buffers.Add(buffer);
                }

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
                        //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void Btn_SetPalletDataPallet_Click(object sender, RoutedEventArgs e)
        {
            if ((DeviceManagementTabControl.SelectedIndex == 1) && (DevicePalletsListDg.SelectedItem != null))
            {
                if ((palletLineX.Text.Trim() == "") ||
                    (palletLineY.Text.Trim() == "") ||
                    (palletLineA.Text.Trim() == "") ||
                    (palletPalletD.Text.Trim() == ""))
                {
                    return;
                }
                List<dtDevicePallet> devicePallets = new List<dtDevicePallet>();
                dtDevicePallet devicePallet = DevicePalletsListDg.SelectedItem as dtDevicePallet;

                string initiateDataPallet =
                    "{\"line\":{\"x\":" + double.Parse(palletLineX.Text) +
                    ",\"y\":" + double.Parse(palletLineY.Text) +
                    ",\"angle\":" + double.Parse(palletLineA.Text) +
                    "},\"pallet\":{\"row\":" + (double)devicePallet.row +
                    ",\"bay\":" + (double)devicePallet.bay +
                    ",\"direct\":" + double.Parse(palletPalletD.Text) +
                    "}}";

                //devicePallet.dataPallet = (devicePallet.dataPallet != null) ? devicePallet.dataPallet.ToString() : initiateDataPallet;
                devicePallet.dataPallet = initiateDataPallet;

                devicePallet.creUsrId = Global_Object.userLogin;
                devicePallet.updUsrId = Global_Object.userLogin;

                foreach (dtDevicePallet dr in managementModel.devicePalletsList)
                {
                    dtDevicePallet devicePalletold = new dtDevicePallet();

                    devicePalletold.devicePalletId = int.Parse(dr.devicePalletId.ToString());
                    devicePalletold.devicePalletName = dr.devicePalletName.ToString();
                    devicePalletold.deviceId = int.Parse(dr.deviceId.ToString());
                    devicePalletold.row = int.Parse(dr.row.ToString());
                    devicePalletold.bay = int.Parse(dr.bay.ToString());
                    string initiateDataPalletOld = "{\"line\":{\"x\":0.0,\"y\":0.0,\"angle\":0.0},\"pallet\":{\"row\":" + (double)dr.row + ",\"bay\":" + (double)dr.bay + ",\"direct\":0.0}}";
                    devicePalletold.dataPallet = (dr.dataPallet != null) ? dr.dataPallet.ToString() : initiateDataPalletOld;
                    devicePalletold.creUsrId = Global_Object.userLogin;
                    devicePalletold.updUsrId = Global_Object.userLogin;

                    if (devicePalletold.devicePalletId != devicePallet.devicePalletId)
                    {
                        devicePallets.Add(devicePalletold);
                    }

                }

                devicePallets.Add(devicePallet);

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


        private void PalletLine_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }


        


    }
}
