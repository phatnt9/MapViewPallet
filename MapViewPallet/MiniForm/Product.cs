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
    public class Product : BridgeClientRequest
    {
        public List<string> listSerials;
        public int Id { get; set; }
        public string Name { get; set; }

        public Product(int productId, string productName)
        {
            Id = productId;
            Name = productName;
            listSerials = new List<string>();
        }

        public void GetProductSerials()
        {
            dynamic postApiBody = new JObject();
            postApiBody.productId = Id;
            PostCallAPI("http://localhost:8081/robot/rest/product/getListProductDetailByProductId", JsonConvert.SerializeObject(postApiBody));
        }

        public override void ReceiveResponseHandler(string msg)
        {
            dynamic stuff = JsonConvert.DeserializeObject(msg);
            foreach (dynamic item in stuff)
            {
                if (AddSerial((string)item.productDetailName))
                {
                    Console.WriteLine("");
                }
            }
        }

        public bool AddSerial(string serial)
        {
            foreach (string item in listSerials)
            {
                if (item == serial)
                {
                    return false;
                }
            }
            listSerials.Add(serial);
            return true;
        }
    }
}
