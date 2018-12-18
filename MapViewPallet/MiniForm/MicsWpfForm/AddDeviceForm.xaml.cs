using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for AddDevicesForm.xaml
    /// </summary>
    public partial class AddDeviceForm : Window
    {
        public class AddDeviceModel : NotifyUIBase
        {
            private string pDeviceNameDuplicate = "Ready";
            public string deviceNameDuplicate {
                get { return pDeviceNameDuplicate; }
                set
                {
                    if (pDeviceNameDuplicate != value)
                    {
                        pDeviceNameDuplicate = value;
                        RaisePropertyChanged("deviceNameDuplicate");
                    }
                }
            }
        }

        DevicesManagement devicesManagement;

        public AddDeviceModel addDeviceModel;

        public AddDeviceForm(DevicesManagement devicesManagement)
        {
            InitializeComponent();
            this.devicesManagement = devicesManagement;
            addDeviceModel = new AddDeviceModel();
            DataContext = addDeviceModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/checkDuplicateNameDevice");
            request.Method = "POST";
            request.ContentType = @"application/json";
            dynamic postApiBody = new JObject();
            postApiBody.deviceName = deviceNametb.Text.Trim();
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
                switch (result)
                {
                    case "1":
                        {
                            addDeviceModel.deviceNameDuplicate = "Tên thiết bị đã tồn tại!";
                            break;
                        }
                    default:
                        {
                            CreateNewDevice(deviceNametb.Text.Trim());
                            MessageBox.Show("Thêm thiết bị thành công!","Hoàn tất", MessageBoxButton.OK);
                            break;
                        }
                }
            }
            devicesManagement.UpdateTab1(true);
        }

        public void CreateNewDevice(string deviceName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/insertDevice");
            request.Method = "POST";
            request.ContentType = @"application/json";
            dynamic postApiBody = new JObject();
            postApiBody.deviceName = deviceName;
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
    }
}

