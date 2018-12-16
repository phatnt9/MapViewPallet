using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class Plan : dtPlan
    {

        private List<dtProductDetail> pListProductDetails;
        public List<dtProductDetail> listProductDetails { get => pListProductDetails; set { pListProductDetails = value; RaisePropertyChanged("listProductDetails"); } }

        public Plan()
        {
            listProductDetails = new List<dtProductDetail>();
        }
        
    }
}
