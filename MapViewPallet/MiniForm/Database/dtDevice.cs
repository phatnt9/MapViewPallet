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
    public class dtDevice : userModel
    {
        private int pDeviceId;
        private string pDeviceName;
        private string pDeviceNameOld;
        private string pImageDeviceUrl;
        private string pImageDeviceUrlOld;
        private int pMaxRow;
        private int pMaxBay;
        private string pPathFile;
        private List<dtDeviceProduct> pDeviceProducts;
        private List<dtDeviceBuffer> pDeviceBuffers;

        public int deviceId { get => pDeviceId; set => pDeviceId = value; }
        public string deviceName { get => pDeviceName; set => pDeviceName = value; }
        public List<dtDeviceProduct> deviceProducts { get => pDeviceProducts; set => pDeviceProducts = value; }
        public List<dtDeviceBuffer> deviceBuffers { get => pDeviceBuffers; set => pDeviceBuffers = value; }
        public string deviceNameOld { get => pDeviceNameOld; set => pDeviceNameOld = value; }
        public int maxRow { get => pMaxRow; set => pMaxRow = value; }
        public int maxBay { get => pMaxBay; set => pMaxBay = value; }
        public string imageDeviceUrl { get => pImageDeviceUrl; set { pImageDeviceUrl = value; RaisePropertyChanged("imageDeviceUrl"); } }
        public string pathFile { get => pPathFile; set { pPathFile = value; RaisePropertyChanged("pathFile"); } }
        public string imageDeviceUrlOld { get => pImageDeviceUrlOld; set { pImageDeviceUrlOld = value; RaisePropertyChanged("imageDeviceUrlOld"); } }



        public dtDevice()
        {
            deviceProducts = new List<dtDeviceProduct>();
            deviceBuffers = new List<dtDeviceBuffer>();
        }

        public void GetDeviceProductsList()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "product/getListDeviceProductAllByDeviceId");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.deviceId = deviceId;
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
                        dtDeviceProduct tempDeviceProduct = new dtDeviceProduct
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            deviceProductId = int.Parse(dr["deviceProductId"].ToString()),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            productId = int.Parse(dr["productId"].ToString()),
                            productName = dr["productName"].ToString(),
                            checkStatus = bool.Parse(dr["checkStatus"].ToString()),
                            imageDeviceUrl = dr["imageDeviceUrl"].ToString(),
                            imageProductUrl = dr["imageProductUrl"].ToString()
                        };
                        if (AddDeviceProduct(tempDeviceProduct))
                        {
                            tempDeviceProduct.GetProductDetailsList();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public bool AddDeviceProduct(dtDeviceProduct deviceProduct)
        {
            foreach (dtDeviceProduct item in deviceProducts)
            {
                if (item.deviceProductId == deviceProduct.deviceProductId)
                {
                    return false;
                }
            }
            deviceProducts.Add(deviceProduct);
            return true;
        }
    }
}
