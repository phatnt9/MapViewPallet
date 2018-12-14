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
        private string pDeviceName;
        private string pProductName;
        
        public string deviceName { get => pDeviceName; set { pDeviceName = value; RaisePropertyChanged("deviceName"); } }
        public string productName { get => pProductName; set { pProductName = value; RaisePropertyChanged("productName"); } }

        private List<ProductDetail> pListProductDetails;
        public List<ProductDetail> listProductDetails { get => pListProductDetails; set { pListProductDetails = value; RaisePropertyChanged("listProductDetails"); } }

        public Plan()
        {
            listProductDetails = new List<ProductDetail>();
        }

        public void UpdateProductDetailId (int productDetailId)
        {
            this.productDetailId = productDetailId;
        }

        //#region INotifyPropertyChanged Members

        //public event PropertyChangedEventHandler PropertyChanged;

        //#endregion

        //#region Private Helpers

        //private void NotifyPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        //#endregion


        //private string pProductDetailName;
        //public string productDetailName
        //{
        //    get => pProductDetailName;
        //    set
        //    {
        //        pProductDetailName = value;
        //        NotifyPropertyChanged("productDetailName");
        //    }
        //}

    }
}
