using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapViewPallet.MiniForm
{
    public class dtProductDetail : userModel
    {
        private int pProductDetailId;
        private int pProductId;
        private string pProductDetailName;

        public int productDetailId { get => pProductDetailId; set => pProductDetailId = value; }
        public int productId { get => pProductId; set => pProductId = value; }
        public string productDetailName { get => pProductDetailName; set => pProductDetailName = value; }
    }
}
