using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapViewPallet.MiniForm.MicsWpfForm
{

    /// <summary>
    /// Interaction logic for AddBufferForm.xaml
    /// </summary>
    /// 
    public partial class AddBufferForm : Window
    {
        DevicesManagement devicesManagement;

        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        public class AddBufferModel : NotifyUIBase
        {
            private string pBufferNameDuplicate = "Ready";
            public string bufferNameDuplicate
            {
                get { return pBufferNameDuplicate; }
                set
                {
                    if (pBufferNameDuplicate != value)
                    {
                        pBufferNameDuplicate = value;
                        RaisePropertyChanged("bufferNameDuplicate");
                    }
                }
            }
        }

        public AddBufferModel addBufferModel;

        public AddBufferForm(DevicesManagement devicesManagement, string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            this.devicesManagement = devicesManagement;
            addBufferModel = new AddBufferModel { bufferNameDuplicate = "Ready" };
            DataContext = addBufferModel;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.bufferNametb.Text) || this.bufferNametb.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show
                    (
                    String.Format(Global_Object.messageValidate, "Buffer", "Buffer"), 
                    Global_Object.messageTitileWarning, 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning
                    );
                this.bufferNametb.Focus();
                return;
            }

            if (string.IsNullOrEmpty(this.bufferMaxBaytb.Text) || this.bufferMaxBaytb.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show
                    (
                    String.Format(Global_Object.messageValidate, "Max Bay", "Max Bay"),
                    Global_Object.messageTitileWarning,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
                this.bufferMaxBaytb.Focus();
                return;
            }

            if (string.IsNullOrEmpty(this.bufferMaxRowtb.Text) || this.bufferMaxRowtb.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show
                    (
                    String.Format(Global_Object.messageValidate, "Max Row", "Max Row"),
                    Global_Object.messageTitileWarning,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
                this.bufferMaxRowtb.Focus();
                return;
            }

            int maxRow = 0;
            int maxBay = 0;

            int.TryParse(this.bufferMaxRowtb.Text.Trim().Replace(" ", ""), out maxRow);
            int.TryParse(this.bufferMaxBaytb.Text.Trim().Replace(" ", ""), out maxBay);

            if (maxRow == 0)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidateNumber, "Max Row", "greater", "0"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.bufferMaxRowtb.Focus();
                return;
            }

            if (maxBay == 0)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidateNumber, "Max Bay", "greater", "0"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.bufferMaxBaytb.Focus();
                return;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "buffer/insertBuffer");
            request.Method = "POST";
            request.ContentType = @"application/json";
            dtBuffer buffer = new dtBuffer();
            buffer.bufferName = this.bufferNametb.Text.Trim();
            buffer.maxRow = maxRow;
            buffer.maxBay = maxBay;
            buffer.bufferReturn = (bool)bufferReturnCb.IsChecked;
            buffer.bufferData = "{\"x\":\"0\",\"y\":\"0\",\"angle\":\"0\"}";
            buffer.bufferCheckIn = "{\"x\":\"0\",\"y\":\"0\",\"angle\":\"0\"}";
            buffer.creUsrId = Global_Object.userLogin;
            buffer.updUsrId = Global_Object.userLogin;

            string jsonData = JsonConvert.SerializeObject(buffer);

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

                if (result > 0)
                {
                    devicesManagement.managementModel.ReloadListBuffers();
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (result == -2)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Buffer"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.bufferNametb.Focus();
                    return;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            devicesManagement.UpdateTab4(true);
        }
        

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void BufferMaxBaytb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void BufferMaxRowtb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }


    }
}
