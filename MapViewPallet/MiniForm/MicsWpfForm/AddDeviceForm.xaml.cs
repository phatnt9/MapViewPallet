using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Forms;

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
            if (string.IsNullOrEmpty(this.deviceNametb.Text) || this.deviceNametb.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "Devices Name", "Devices Name"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.deviceNametb.Focus();
                return;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/insertDevice");
            request.Method = "POST";
            request.ContentType = @"application/json";
            dtDevice device = new dtDevice();
            device.deviceName = this.deviceNametb.Text.Trim();
            device.creUsrId = Global_Object.userLogin;
            device.updUsrId = Global_Object.userLogin;
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
                StreamReader readerResult = new StreamReader(responseStream, Encoding.UTF8);
                device = JsonConvert.DeserializeObject<dtDevice>(readerResult.ReadToEnd());

                if (device.deviceId > 0)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (device.deviceId == -2)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Devices Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.deviceNametb.Focus();
                    return;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            devicesManagement.UpdateTab1(true);
        }

        
    }
}

