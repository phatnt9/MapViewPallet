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

            managementModel = new ManagementModel(this);
            DataContext = managementModel;

            //*****************************************************************

            DevicesListDg.SelectionMode =
                DeviceProductsListDg.SelectionMode =
                DeviceBuffersListDg.SelectionMode =
                ProductsListDg.SelectionMode =
                ProductDetailsListDg.SelectionMode =
                BuffersListDg.SelectionMode = DataGridSelectionMode.Single;
            DevicesListDg.SelectionUnit =
                DeviceProductsListDg.SelectionUnit =
                DeviceBuffersListDg.SelectionUnit =
                ProductsListDg.SelectionUnit =
                ProductDetailsListDg.SelectionUnit =
                BuffersListDg.SelectionUnit = DataGridSelectionUnit.FullRow;

            //*****************************************************************

            DevicesListDg.SelectedCellsChanged += DevicesListDg_SelectedCellsChanged;
            ProductsListDg.SelectedCellsChanged += ProductsListDg_SelectedCellsChanged;


        }

        private void ProductsListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            dtProduct temp = ProductsListDg.SelectedItem as dtProduct;
            managementModel.ReloadListProductDetails(temp.productId);
        }

        private void DevicesListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            dtDevice temp = DevicesListDg.SelectedItem as dtDevice;
            managementModel.ReloadListDeviceProducts(temp.deviceId);
            managementModel.ReloadListDeviceBuffers(temp.deviceId);
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
                            //Console.WriteLine("Thiết bị");
                            //Console.WriteLine(((e.Source as System.Windows.Controls.TabControl).SelectedIndex));
                            managementModel.ReloadListDevices();
                            break;
                        }
                    case 1:
                        {
                            //Console.WriteLine("Sản phẩm");
                            //Console.WriteLine(((e.Source as System.Windows.Controls.TabControl).SelectedIndex));
                            managementModel.ReloadListProducts();
                            break;
                        }
                    case 2:
                        {
                            //Console.WriteLine("Buffer");
                            //Console.WriteLine(((e.Source as System.Windows.Controls.TabControl).SelectedIndex));
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
                SaveData(false);
            }
            UpdateTab1(false);
        }

        //****************************************************************************************

        private void SaveData(bool isProduct)
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
                UpdateTab1(false);
            }
            Thread.Sleep(150);
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
            foreach (DeviceBuffer item in managementModel.deviceBuffersList)
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
                managementModel.ReloadListDevices();
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
                        if (!DeviceProductsListDg.HasItems)
                        {
                            managementModel.ReloadListDeviceProducts((DevicesListDg.SelectedItem as dtDevice).deviceId);
                        }
                        if (!DeviceBuffersListDg.HasItems)
                        {
                            managementModel.ReloadListDeviceBuffers((DevicesListDg.SelectedItem as dtDevice).deviceId);
                        }
                    }
                }
                else
                {
                    managementModel.ReloadListDevices();
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
                        if (!PalletsListDg.HasItems)
                        {
                            managementModel.ReloadListPallets((BuffersListDg.SelectedItem as dtBuffer).bufferId);
                        }
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

        private void DeviceProductCbRow_Click(object sender, RoutedEventArgs e)
        {

            if (DevicesListDg.SelectedItem != null)
            {
                System.Windows.Controls.CheckBox temp = e.Source as System.Windows.Controls.CheckBox;
                if (DevicesListDg.SelectedItem != null)
                {
                    SaveData(true);
                }
            }
            UpdateTab1(false);

        }
        private void DeviceBufferCbRow_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesListDg.SelectedItem != null)
            {
                SaveData(false);
            }
            UpdateTab1(false);
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

        //****************************************************************************************

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

        //****************************************************************************************

        private void Btn_Test_DeviceBuffer_Click(object sender, RoutedEventArgs e)
        {
            foreach (DeviceBuffer item in managementModel.deviceBuffersList)
            {
                Console.WriteLine(item.bufferName + "-" + item.checkStatus + "-" + item.bufferSort);
            }
        }
        private void Btn_Test_DeviceProduct_Click(object sender, RoutedEventArgs e)
        {
            foreach (dtDeviceProduct item in managementModel.deviceProductsList)
            {
                Console.WriteLine(item.productName + "-" + item.checkStatus);
            }
        }
        private void Btn_Test_Device_Click(object sender, RoutedEventArgs e)
        {
            foreach (dtDevice item in managementModel.devicesList)
            {
                Console.WriteLine(item.deviceName);
            }
        }

        //****************************************************************************************

        private void Btn_Add_Device_Click(object sender, RoutedEventArgs e)
        {
            AddDeviceForm form = new AddDeviceForm();
            form.ShowDialog();
            UpdateTab1(true);
        }

        private void Btn_Add_Buffer_Click(object sender, RoutedEventArgs e)
        {
            //AddBufferForm form = new AddBufferForm();
            //form.ShowDialog();
            //UpdateTab1(true);
        }
        //****************************************************************************************

        private void Btn_Save_DeviceProduct_Click(object sender, RoutedEventArgs e)
        {
            SaveData(true);
            UpdateTab1(false);
        }
        private void Btn_Save_DeviceBuffer_Click(object sender, RoutedEventArgs e)
        {
            SaveData(false);
            UpdateTab1(false);
        }
        private void Btn_Save_Device_Click(object sender, RoutedEventArgs e)
        {
            UpdateTab1(true);
        }

        
    }
}
