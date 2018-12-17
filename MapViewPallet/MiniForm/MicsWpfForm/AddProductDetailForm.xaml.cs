using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for AddProductDetailForm.xaml
    /// </summary>
    public partial class AddProductDetailForm : Window
    {
        public class AddProductDetailModel : NotifyUIBase
        {
            private string pProductDetailNameDuplicate = "Ready";
            public string productDetailNameDuplicate
            {
                get { return pProductDetailNameDuplicate; }
                set
                {
                    if (pProductDetailNameDuplicate != value)
                    {
                        pProductDetailNameDuplicate = value;
                        RaisePropertyChanged("productDetailNameDuplicate");
                    }
                }
            }
        }

        DevicesManagement devicesManagement;

        public AddProductDetailModel addProductDetailModel;

        public AddProductDetailForm(DevicesManagement devicesManagement)
        {
            InitializeComponent();
            this.devicesManagement = devicesManagement;
            addProductDetailModel = new AddProductDetailModel { productDetailNameDuplicate = "Ready" };
            DataContext = addProductDetailModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/insertUpdateProductDetail");
            request.Method = "POST";
            request.ContentType = @"application/json";
            dtProductDetail productDetail = new dtProductDetail();
            productDetail.productId = ((devicesManagement.ProductsListDg.SelectedItem) as dtProduct).productId;
            productDetail.productDetailId = 0;
            productDetail.productDetailName = this.productDetailNametb.Text;
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
                switch (result)
                {
                    case "-1":
                        {
                            addProductDetailModel.productDetailNameDuplicate = "Mã sản phẩm đã tồn tại!";
                            break;
                        }
                    default:
                        {
                            //CreateNewProduct(productNametb.Text.Trim());
                            Close();
                            break;
                        }
                }
            }
        }
    }
}
