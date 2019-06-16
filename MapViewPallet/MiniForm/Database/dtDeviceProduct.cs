using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class dtDeviceProduct : userModel
    {
        private int pDeviceProductId;
        private int pDeviceId;
        private int pProductId;
        private string pProductName;
        private bool pcheckStatus;
        private string pImageDeviceUrl;
        private string pImageProductUrl;

        public int deviceProductId { get => pDeviceProductId; set => pDeviceProductId = value; }
        public int deviceId { get => pDeviceId; set => pDeviceId = value; }
        public int productId { get => pProductId; set => pProductId = value; }
        public string productName { get => pProductName; set => pProductName = value; }
        public bool checkStatus { get => pcheckStatus; set => pcheckStatus = value; }
        public string imageDeviceUrl { get => pImageDeviceUrl; set => pImageDeviceUrl = value; }
        public string imageProductUrl { get => pImageProductUrl; set => pImageProductUrl = value; }


        private List<dtProductDetail> pProductDetails;
        public List<dtProductDetail> productDetails { get => pProductDetails; set => pProductDetails = value; }

        public dtDeviceProduct()
        {
            productDetails = new List<dtProductDetail>();
        }

        public void GetProductDetailsList()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "product/getListProductDetailByProductId");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.productId = productId;
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

                    DataTable productDetails = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in productDetails.Rows)
                    {
                        dtProductDetail tempProductDetail = new dtProductDetail
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            productDetailId = int.Parse(dr["productDetailId"].ToString()),
                            productId = int.Parse(dr["productId"].ToString()),
                            productDetailName = dr["productDetailName"].ToString()
                        };
                        if (AddProductDetail(tempProductDetail))
                        {
                            //tempDeviceProduct.GetProductDetailsList();
                        }
                    }
                }
            }
            catch (Exception exc)
            {

            }
        }


        public bool AddProductDetail(dtProductDetail productDetail)
        {
            foreach (dtProductDetail item in productDetails)
            {
                if (item.productDetailId == productDetail.productDetailId)
                {
                    return false;
                }
            }
            productDetails.Add(productDetail);
            return true;
        }


    }
}
