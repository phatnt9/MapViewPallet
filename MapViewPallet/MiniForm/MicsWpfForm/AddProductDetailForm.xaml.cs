﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for AddProductDetailForm.xaml
    /// </summary>
    public partial class AddProductDetailForm : Window
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public class AddProductDetailModel : NotifyUIBase
        {
            private string pProductDetailNameDuplicate = "Ready";

            public string productDetailNameDuplicate
            {
                get => pProductDetailNameDuplicate;
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

        private DevicesManagement devicesManagement;

        public AddProductDetailModel addProductDetailModel;

        public AddProductDetailForm(DevicesManagement devicesManagement, string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            this.devicesManagement = devicesManagement;
            addProductDetailModel = new AddProductDetailModel { productDetailNameDuplicate = "Ready" };
            DataContext = addProductDetailModel;
            Loaded += AddProductDetailForm_Loaded;
        }

        private void AddProductDetailForm_Loaded(object sender, RoutedEventArgs e)
        {
            productDetailNametb.Focus();
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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "product/insertUpdateProductDetail");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dtProductDetail productDetail = new dtProductDetail();
                productDetail.productId = ((devicesManagement.ProductsListDg.SelectedItem) as dtProduct).productId;
                productDetail.productDetailId = 0;
                productDetail.productDetailName = this.productDetailNametb.Text.Trim();
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
                            break;
                        }
                    }
                }
                devicesManagement.UpdateTab3(false);
                productDetailNametb.Focus();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void ProductDetailNametb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }
    }
}