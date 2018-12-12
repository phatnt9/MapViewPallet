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
    public class DeviceProduct : dtDeviceProduct
    {
        //#############################
        public List<ProductDetail> listProductDetails;

        

        public DeviceProduct()
        {
            listProductDetails = new List<ProductDetail>();
        }

        public void GetProductDetailsList()
        {
            try
            {
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

                    DataTable deviceProducts = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in deviceProducts.Rows)
                    {
                        ProductDetail tempDeviceProduct = new ProductDetail
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            productDetailId = int.Parse(dr["productDetailId"].ToString()),
                            productId = int.Parse(dr["productId"].ToString()),
                            productDetailName = dr["productDetailName"].ToString()
                        };
                        if (AddProductDetail(tempDeviceProduct))
                        {
                            //tempDeviceProduct.GetProductDetailsList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        

        public bool AddProductDetail(ProductDetail productDetail)
        {
            foreach (ProductDetail item in listProductDetails)
            {
                if (item.productDetailId == productDetail.productDetailId)
                {
                    return false;
                }
            }
            listProductDetails.Add(productDetail);
            return true;
        }

        public ProductDetail Get_ProductDetail_By_ProductDetailId(int productDetailId)
        {
            try
            {
                foreach (ProductDetail productDetail in listProductDetails)
                {
                    if (productDetail.productDetailId == productDetailId)
                    {
                        return productDetail;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
