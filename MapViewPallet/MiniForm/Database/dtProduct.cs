using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;

namespace MapViewPallet.MiniForm
{
    public class dtProduct : userModel
    {
        private int pProductId;
        private string pProductName;
        private string pImageProductUrl;
        private string pImageProductUrlOld;
        private List<dtProductDetail> pProductDetails;
        private string pPathFile;

        public int productId { get => pProductId; set => pProductId = value; }
        public string productName { get => pProductName; set => pProductName = value; }
        public List<dtProductDetail> productDetails { get => pProductDetails; set => pProductDetails = value; }
        public string imageProductUrl { get => pImageProductUrl; set { pImageProductUrl = value; RaisePropertyChanged("imageProductUrl"); } }
        public string pathFile { get => pPathFile; set { pPathFile = value; RaisePropertyChanged("pathFile"); } }
        public string imageProductUrlOld { get => pImageProductUrlOld; set { pImageProductUrlOld = value; RaisePropertyChanged("imageProductUrlOld"); } }


        public dtProduct()
        {
            
        }

        public void GetProductDetailsList()
        {
            try
            {
                productDetails = new List<dtProductDetail>();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/getListProductDetailByProductId");
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

                        }
                    }
                }
            }
            catch
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
