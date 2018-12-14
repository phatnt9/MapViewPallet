using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MapViewPallet.MiniForm
{
    public class dtPlan : userModel
    {

        private int pPlanId;
        private int pDeviceProductId;
        private int pTimeWorkId;
        private int pProductDetailId;
        private int pPalletAmount;
        private int pPalletUse;
        private int pPalletMiss;
        private string pActiveDate;
        private int pDeviceId;
        private int pProductId;
        private List<dtBuffer> pBuffers;


        public int planId { get => pPlanId; set { pPlanId = value; RaisePropertyChanged("planId"); } }
        public int deviceProductId { get => pDeviceProductId; set { pDeviceProductId = value; RaisePropertyChanged("deviceProductId"); } }
        public int timeWorkId { get => pTimeWorkId; set { pTimeWorkId = value; RaisePropertyChanged("timeWorkId"); } }
        public int productDetailId {get => pProductDetailId;set{pProductDetailId = value; RaisePropertyChanged("productDetailId");}}
        public int palletAmount {get => pPalletAmount;set{pPalletAmount = value; RaisePropertyChanged("palletAmount");}}
        public int palletUse { get => pPalletUse; set { pPalletUse = value; RaisePropertyChanged("palletUse"); } }
        public int palletMiss { get => pPalletMiss; set { pPalletMiss = value; RaisePropertyChanged("palletMiss"); } }
        public string activeDate { get => pActiveDate; set { pActiveDate = value; RaisePropertyChanged("activeDate"); } }
        public int deviceId { get => pDeviceId; set { pDeviceId = value; RaisePropertyChanged("deviceId"); } }
        public int productId { get => pProductId; set { pProductId = value; RaisePropertyChanged("productId"); } }
        public List<dtBuffer> buffers { get => pBuffers; set { pBuffers = value; RaisePropertyChanged("buffers"); } }





        //#region INotifyPropertyChanged Members
        //public event PropertyChangedEventHandler PropertyChanged;
        //#endregion

        //#region Private Helpers
        //private void NotifyPropertyChanged(string propertyName)
        //{
        //    //Console.WriteLine(propertyName);
        //    if (PropertyChanged != null)
        //    {
        //        //Console.WriteLine("in");
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //        //Console.WriteLine(propertyName);
        //    }
        //}
        //#endregion
    }
}
