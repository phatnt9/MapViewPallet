using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

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
        
        DevicesManagement devicesManagement;

        public AddProductModel addProductModel;

        public AddProductForm(DevicesManagement devicesManagement, string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            this.devicesManagement = devicesManagement;
            addProductModel = new AddProductModel { productNameDuplicate = "Ready" };
            DataContext = addProductModel;
            Loaded += AddProductForm_Loaded;
        }

        private void AddProductForm_Loaded(object sender, RoutedEventArgs e)
        {
            productNametb.Focus();
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
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/insertUpdateProduct");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dtProduct productNew = new dtProduct();
                productNew.productName = productNametb.Text.Trim();
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
                                break;
                            }
                        default:
                            {
                                addProductModel.productNameDuplicate = "Tên sản phẩm đã tồn tại!";
                                break;
                            }
                    }
                }
                devicesManagement.UpdateTab3(true);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

        }

        private void ProductNametb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }
    }
}
