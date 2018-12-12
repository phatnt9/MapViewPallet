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


        public int planId { get => pPlanId; set { pPlanId = value; NotifyPropertyChanged("planId"); } }
        public int deviceProductId { get => pDeviceProductId; set { pDeviceProductId = value; NotifyPropertyChanged("deviceProductId"); } }
        public int timeWorkId { get => pTimeWorkId; set { pTimeWorkId = value; NotifyPropertyChanged("timeWorkId"); } }
        public int productDetailId {get => pProductDetailId;set{pProductDetailId = value;NotifyPropertyChanged("productDetailId");}}
        public int palletAmount {get => pPalletAmount;set{pPalletAmount = value;NotifyPropertyChanged("palletAmount");}}
        public int palletUse { get => pPalletUse; set { pPalletUse = value; NotifyPropertyChanged("palletUse"); } }
        public int palletMiss { get => pPalletMiss; set { pPalletMiss = value; NotifyPropertyChanged("palletMiss"); } }
        public int deviceId { get => pDeviceId; set { pDeviceId = value; NotifyPropertyChanged("deviceId"); } }
        public int productId { get => pProductId; set { pProductId = value; NotifyPropertyChanged("productId"); } }
        public string activeDate { get => pActiveDate; set { pActiveDate = value; NotifyPropertyChanged("activeDate"); } }





        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Private Helpers
        private void NotifyPropertyChanged(string propertyName)
        {
            //Console.WriteLine(propertyName);
            if (PropertyChanged != null)
            {
                //Console.WriteLine("in");
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                //Console.WriteLine(propertyName);
            }
        }
        #endregion
    }
}
