using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapViewPallet.MiniForm
{
    /// <summary>
    /// Interaction logic for DevicesManagement.xaml
    /// </summary>
    public partial class DevicesManagement : Window
    {
        public ManagementModel managementModel;

        public string deviceNameSelect = "";
        private int productIdSelect = -1;
        private string bufferIdSelect = "";

        public DevicesManagement()
        {
            InitializeComponent();
            managementModel = new ManagementModel(this);
            DataContext = managementModel;
            DeviceListDg.SelectionMode = DeviceProductListDg.SelectionMode = BufferListDg.SelectionMode = DataGridSelectionMode.Single;
            DeviceListDg.SelectionUnit = DeviceProductListDg.SelectionUnit = BufferListDg.SelectionUnit = DataGridSelectionUnit.FullRow;

            DeviceListDg.SelectedCellsChanged += DeviceListDg_SelectedCellsChanged;
            DeviceProductListDg.SelectedCellsChanged += DeviceProductListDg_SelectedCellsChanged;

            this.Loaded += devicesManagement_Loaded;

        }

        private void DeviceProductListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DeviceProduct deviceProduct = DeviceProductListDg.SelectedItem as DeviceProduct;
            //Console.WriteLine(deviceProduct.deviceProductId + "-" + deviceProduct.productId + "-" + deviceProduct.productName);
            //managementModel.ReLoadListDeviceBuffers(device.deviceId);
        }

        private void DeviceListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            Device temp = DeviceListDg.SelectedItem as Device;
            //Console.WriteLine(temp.deviceId+"-"+temp.deviceName);
            //Reload lại
            managementModel.ReLoadListDeviceProducts(temp.deviceId);
            managementModel.ReLoadListDeviceBuffers(temp.deviceId);
        }

        private void devicesManagement_Loaded(object sender, RoutedEventArgs e)
        {
            managementModel.LoadListDevices();
            //if (!string.IsNullOrEmpty(deviceNameSelect))
            //{
            //    //DeviceListDg.UnselectAll();
            //    int indexSelect = 0;
            //    //foreach (DataGridViewRow row in DeviceListDg.row)
            //    //{
            //    //}
            //}
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (Device dr in managementModel.devicesList)
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
            foreach (Device dr in managementModel.devicesList)
            {
                dynamic postApiBodyChild = new JObject();
                postApiBodyChild.creUsrId = dr.creUsrId;
                postApiBodyChild.creDt = dr.creDt;
                postApiBodyChild.updUsrId = dr.updUsrId;
                postApiBodyChild.updDt = dr.updDt;
                postApiBodyChild.deviceId = dr.deviceId;
                postApiBodyChild.deviceName = dr.deviceName;
                postApiBodyChild.deviceProducts = dr.deviceProducts;
                postApiBodyChild.deviceBuffers = dr.deviceBuffers;
                postApiBody.Add(postApiBodyChild);
            }
            string jsonData = JsonConvert.SerializeObject(postApiBody, Formatting.Indented);
            //jsonData = jsonData.Trim(new Char[] { ' '});
            //jsonData = jsonData.Replace("  ", "");
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
                    DeviceListDg.SelectedItem = DeviceListDg.Items[0];
                    DeviceListDg.ScrollIntoView(DeviceListDg.SelectedItem);
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

            //Console.WriteLine("");
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeviceListDg_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Refresh_Device_Click(object sender, RoutedEventArgs e)
        {
            managementModel.GroupedDevices.Refresh();
        }

        private void Btn_Refresh_Product_Click(object sender, RoutedEventArgs e)
        {
            managementModel.GroupedProducts.Refresh();
        }

        private void Btn_Refresh_Buffer_Click(object sender, RoutedEventArgs e)
        {
            managementModel.GroupedBuffers.Refresh();
        }

        private void Btn_Test_Buffer_Click(object sender, RoutedEventArgs e)
        {
            foreach (DeviceBuffer item in managementModel.buffersList)
            {
                Console.WriteLine(item.bufferName + "-" + item.checkStatus + "-" + item.bufferSort);
            }
        }

        private void Btn_Test_Product_Click(object sender, RoutedEventArgs e)
        {
            foreach (DeviceProduct item in managementModel.productsList)
            {
                Console.WriteLine(item.productName + "-" + item.checkStatus);
            }
        }

        private void Btn_Test_Device_Click(object sender, RoutedEventArgs e)
        {
            foreach (Device item in managementModel.devicesList)
            {
                Console.WriteLine(item.deviceName);
            }
        }


        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************
        private void ProductCbCol_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Console.WriteLine("ProductCbCol_DataContextChanged");
        }

        private void ProductCbCol_Checked(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("ProductCbCol_Checked");
        }

        private void ProductCbCol_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("ProductCbCol_Click");
        }

        private void ProductCbCol_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Console.WriteLine("ProductCbCol_FocusableChanged");
        }

        private void ProductCbCol_GotFocus(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("ProductCbCol_GotFocus");
        }

        private void ProductCbCol_LostFocus(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("ProductCbCol_LostFocus");
        }



        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************
        private void ProductCbRow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //DeviceProduct temp = e.NewValue as DeviceProduct;
            //Console.WriteLine(temp.productName + "-ProductCbRow_DataContextChanged");
        }

        private void ProductCbRow_Checked(object sender, RoutedEventArgs e)
        {
            //if (DeviceListDg.SelectedItem != null)
            //{
            //    SaveData(true);
            //}
            //System.Windows.Controls.CheckBox temp = e.Source as System.Windows.Controls.CheckBox;
            //DeviceProduct temp1 = temp.DataContext as DeviceProduct;
            //Console.WriteLine(temp1.productName + "-ProductCbRow_Checked:" + temp.IsChecked + "-Device:" + (DeviceListDg.SelectedItem as Device).deviceName);

            //if (DeviceListDg.SelectedItem != null)
            //{
            //    System.Windows.Controls.CheckBox temp = e.Source as System.Windows.Controls.CheckBox;
            //    DeviceProduct temp2 = temp.DataContext as DeviceProduct;
            //    Console.WriteLine(((DeviceListDg.SelectedItem) as Device).deviceName + "-" + temp2.productName + "-[" + temp.IsChecked + "]-ProductCbRow_LostFocus");
            //}
        }

        private void ProductCbRow_Click(object sender, RoutedEventArgs e)
        {

            if (DeviceListDg.SelectedItem != null)
            {
                System.Windows.Controls.CheckBox temp = e.Source as System.Windows.Controls.CheckBox;
                //Console.WriteLine("ProductCbRow_Click-"+temp.IsChecked);
                if (DeviceListDg.SelectedItem != null)
                {
                    SaveData(true);
                }
                //DeviceProduct temp2 = temp.DataContext as DeviceProduct;
                //Console.WriteLine(((DeviceListDg.SelectedItem) as Device).deviceName + "-" + temp2.productName + "-[" + temp.IsChecked + "]-ProductCbRow_LostFocus");
                //SaveData(true);
            }
            UpdateDgv();

        }

        private void ProductCbRow_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Console.WriteLine("ProductCbRow_FocusableChanged");
        }

        private void ProductCbRow_GotFocus(object sender, RoutedEventArgs e)
        {
            //System.Windows.Controls.CheckBox temp = e.Source as System.Windows.Controls.CheckBox;
            //DeviceProduct temp2 = sender
            //Console.WriteLine("ProductCbRow_GotFocus");
        }

        private void ProductCbRow_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DeviceListDg.SelectedItem != null)
            {
                //System.Windows.Controls.CheckBox temp = e.Source as System.Windows.Controls.CheckBox;
                //DeviceProduct temp2 = temp.DataContext as DeviceProduct;
                //Console.WriteLine(((DeviceListDg.SelectedItem) as Device).deviceName + "-" + temp2.productName + "-[" + temp.IsChecked + "]-ProductCbRow_LostFocus");
            }
        }

        private void SaveData(bool isProduct)
        {
            //UpdateDgv();
            dtDevice device = GetDateSave();
            if (DeviceProductListDg.CurrentItem != null)
            {
                productIdSelect = int.Parse((DeviceProductListDg.CurrentItem as DeviceProduct).productId.ToString());
            }
            if (BufferListDg.CurrentItem != null)
            {
                bufferIdSelect = (BufferListDg.CurrentItem as DeviceBuffer).bufferId.ToString();
            }

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
                if (isProduct)
                {
                    //managementModel.ReLoadListDeviceProducts((DeviceListDg.SelectedItem as Device).deviceId);
                    UpdateDgv();
                }
                else
                {
                    //managementModel.ReLoadListDeviceBuffers((DeviceListDg.SelectedItem as Device).deviceId);
                    UpdateDgv();
                }
            }
        }

        private dtDevice GetDateSave()
        {
            dtDevice deviceData = new dtDevice();
            if (DeviceListDg.CurrentCell == null || DeviceListDg.SelectedItem == null || DeviceListDg.CurrentCell.IsValid)
            {
                return deviceData;
            }
            int deviceID = 0;
            int.TryParse((DeviceListDg.SelectedItem as Device).deviceId.ToString(), out deviceID);
            deviceData.deviceId = deviceID;
            deviceData.deviceName = (DeviceListDg.SelectedItem as Device).deviceName.ToString().Trim();
            deviceData.creUsrId = Global_Object.userLogin;
            deviceData.updUsrId = Global_Object.userLogin;
            List<dtDeviceProduct> deviceProducts = new List<dtDeviceProduct>();
            foreach (DeviceProduct item in managementModel.productsList)
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
            foreach (DeviceBuffer item in managementModel.buffersList)
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


        public void UpdateDgv()
        {
            if (DeviceListDg.HasItems)
            {
                if (DeviceListDg.SelectedItem == null)
                {
                    DeviceListDg.SelectedItem = DeviceListDg.Items[0];
                    DeviceListDg.ScrollIntoView(DeviceListDg.SelectedItem);
                }
                else
                {
                    managementModel.ReLoadListDeviceProducts((DeviceListDg.SelectedItem as Device).deviceId);
                    managementModel.ReLoadListDeviceBuffers((DeviceListDg.SelectedItem as Device).deviceId);
                }
            }
            else
            {
                managementModel.LoadListDevices();
            }

        }
        

        private void ProductCbRow_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (DeviceListDg.SelectedItem != null)
            //{
            //    SaveData(true);
            //}
        }

        private void BufferCbRow_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceListDg.SelectedItem != null)
            {
                System.Windows.Controls.CheckBox temp = e.Source as System.Windows.Controls.CheckBox;
                //Console.WriteLine("ProductCbRow_Click-"+temp.IsChecked);
                if (DeviceListDg.SelectedItem != null)
                {
                    SaveData(false);
                }
                //DeviceProduct temp2 = temp.DataContext as DeviceProduct;
                //Console.WriteLine(((DeviceListDg.SelectedItem) as Device).deviceName + "-" + temp2.productName + "-[" + temp.IsChecked + "]-ProductCbRow_LostFocus");
                //SaveData(true);
            }
            UpdateDgv();
        }

        private void BufferListDg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (DeviceListDg.SelectedItem != null)
            {
                //System.Windows.Controls.CheckBox temp = e.Source as System.Windows.Controls.CheckBox;
                //Console.WriteLine("ProductCbRow_Click-"+temp.IsChecked);
                if (DeviceListDg.SelectedItem != null)
                {
                    SaveData(false);
                }
                //DeviceProduct temp2 = temp.DataContext as DeviceProduct;
                //Console.WriteLine(((DeviceListDg.SelectedItem) as Device).deviceName + "-" + temp2.productName + "-[" + temp.IsChecked + "]-ProductCbRow_LostFocus");
                //SaveData(true);
            }
            UpdateDgv();
        }
    }
}
