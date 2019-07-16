using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for AddDevicesForm.xaml
    /// </summary>
    public partial class AddDeviceForm : Window
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public class AddDeviceModel : NotifyUIBase
        {
            private string pDeviceNameDuplicate = "Ready";

            public string deviceNameDuplicate
            {
                get => pDeviceNameDuplicate;
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

        private DevicesManagement devicesManagement;

        public AddDeviceModel addDeviceModel;

        public AddDeviceForm(DevicesManagement devicesManagement, string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            this.devicesManagement = devicesManagement;
            addDeviceModel = new AddDeviceModel();
            DataContext = addDeviceModel;
            Loaded += AddDeviceForm_Loaded;
        }

        private void AddDeviceForm_Loaded(object sender, RoutedEventArgs e)
        {
            deviceNametb.Focus();
        }

        public void ApplyLanguage(string cultureName = null)
        {
            if (cultureName != null)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                if (string.IsNullOrEmpty(this.deviceNametb.Text) || this.deviceNametb.Text.Trim() == "")
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "Devices Name", "Devices Name"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.deviceNametb.Focus();
                    return;
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "device/insertDevice");
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
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void DeviceNametb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }
    }
}