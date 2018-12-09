using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class Device : BridgeClientRequest
    {
        //########-----VARIABLE-----#######
        public int Id;
        public string Name;
        public List<Product> listProducts; //Cac loai hang dang chay Cap/Bottle/Pouch/Divider/Outer

        //########-----METHOD-----#######
        public Device(int deviceId, string deviceName)
        {
            Id = deviceId;
            Name = deviceName;
            listProducts = new List<Product>();
        }

        public void GetDeviceProducts()
        {
            dynamic postApiBody = new JObject();
            postApiBody.deviceId = Id;
            PostCallAPI("http://localhost:8081/robot/rest/product/getListDeviceProductByDeviceId", JsonConvert.SerializeObject(postApiBody));
        }

        public override void ReceiveResponseHandler(string msg)
        {
            dynamic stuff = JsonConvert.DeserializeObject(msg);
            foreach (dynamic item in stuff)
            {
                Product temp = new Product((int)item.productId, (string)item.productName);
                if (AddProduct(temp))
                {
                    temp.GetProductSerials();
                }
            }
        }
        
        public bool AddProduct(Product product)
        {
            foreach (Product item in listProducts)
            {
                if (item.Id == product.Id)
                {
                    return false;
                }
            }
            listProducts.Add(product);
            return true;
        }
    }
}
