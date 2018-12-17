using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for AddProductForm.xaml
    /// </summary>
    public partial class AddProductForm : Window
    {
        public class AddProductModel : NotifyUIBase
        {
            private string pProductNameDuplicate = "Ready";
            public string productNameDuplicate
            {
                get { return pProductNameDuplicate; }
                set
                {
                    if (pProductNameDuplicate != value)
                    {
                        pProductNameDuplicate = value;
                        RaisePropertyChanged("productNameDuplicate");
                    }
                }
            }
        }

        public AddProductModel addProductModel;

        public AddProductForm()
        {
            InitializeComponent();
            addProductModel = new AddProductModel { productNameDuplicate = "Ready" };
            DataContext = addProductModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/insertUpdateProduct");
            request.Method = "POST";
            request.ContentType = @"application/json";
            dtProduct productNew = new dtProduct();
            productNew.productName = productNametb.Text;
            productNew.productDetails = new List<dtProductDetail>();
            productNew.creUsrId = Global_Object.userLogin;
            productNew.updUsrId = Global_Object.userLogin;

            string jsonData = JsonConvert.SerializeObject(productNew);

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
                    case "-1":
                        {
                            addProductModel.productNameDuplicate = "Tên sản phẩm đã tồn tại!";
                            break;
                        }
                    case "1":
                        {
                            MessageBox.Show("Thêm sản phẩm thành công!", "Hoàn tất", MessageBoxButton.OK);
                            Close();
                            break;
                        }
                    default:
                        {
                            addProductModel.productNameDuplicate = "Tên sản phẩm đã tồn tại!";
                            break;
                        }
                }
            }
        }
    }
}
