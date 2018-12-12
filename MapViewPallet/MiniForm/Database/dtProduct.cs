using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapViewPallet.MiniForm
{
    public class dtProduct : userModel
    {
        private int pProductId;
        private string pProductName;
        private List<dtProductDetail> pProductDetails;

        public int productId { get => pProductId; set => pProductId = value; }
        public string productName { get => pProductName; set => pProductName = value; }
        public List<dtProductDetail> productDetails { get => pProductDetails; set => pProductDetails = value; }
    }
}
