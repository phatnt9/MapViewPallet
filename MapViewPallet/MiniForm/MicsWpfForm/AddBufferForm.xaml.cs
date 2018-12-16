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
    /// Interaction logic for AddBufferForm.xaml
    /// </summary>
    public partial class AddBufferForm : Window
    {
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

        public AddBufferForm()
        {
            InitializeComponent();
            addBufferModel = new AddBufferModel { bufferNameDuplicate = "Ready" };
            DataContext = addBufferModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateNewBuffer(bufferNametb.Text.Trim());
        }

        public void CreateNewBuffer(string bufferName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/insertBuffer");
            request.Method = "POST";
            request.ContentType = @"application/json";
            dynamic postApiBody = new JObject();
            postApiBody.bufferName = bufferName;
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
                    case "-2":
                        {
                            addBufferModel.bufferNameDuplicate = "Tên buffer đã tồn tại!";
                            break;
                        }
                    default:
                        {
                            //CreateNewProduct(productNametb.Text.Trim());
                            MessageBox.Show("Thêm buffer thành công!", "Hoàn tất", MessageBoxButton.OK);
                            Close();
                            break;
                        }
                }
            }
        }
    }
}
