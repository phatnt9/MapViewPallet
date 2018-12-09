using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{


    public class Plan : BridgeClientRequest
    {
        private DateTime _date;
        private int _deviceId;
        private string _deviceName;
        private int _productId;
        private string _productName;
        private int _quantity;
        private int _buffered;
        private List<string> _serials;


        public Plan()
        {
            dynamic postApiBody = new JObject();
            postApiBody.deviceId = _deviceId;
            PostCallAPI("http://localhost:8081/robot/rest/product/getListProductDetailByProductId", JsonConvert.SerializeObject(postApiBody));
        }

        public override void ReceiveResponseHandler(string msg)
        {
            dynamic stuff = JsonConvert.DeserializeObject(msg);
            foreach (dynamic item in stuff)
            {
                if ((int)item.productId == 1)
                {

                }
            }
        }
        //private ObservableCollection<Serial> _serials;

        [ReadOnlyAttribute(true)]
        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                _deviceName = value;
            }
        }


        [ReadOnlyAttribute(true)]
        public int DeviceId
        {
            get { return _deviceId; }
            set
            {
                _deviceId = value;
            }
        }

        [ReadOnlyAttribute(true)]
        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
            }
        }

        [ReadOnlyAttribute(true)]
        public int ProductId
        {
            get { return _productId; }
            set
            {
                _productId = value;
            }
        }
        //###################################################
        [ReadOnlyAttribute(false)]
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
            }
        }
        //###################################################
        [ReadOnlyAttribute(false)]
        public List<string> Serials
        {
            get
            {
                return _serials;
            }
            set
            {
                _serials = value;
            }
        }
        //###################################################
        [ReadOnlyAttribute(true)]
        public int Buffered
        {
            get { return _buffered; }
            set
            {
                _buffered = value;
            }
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
    }
}
