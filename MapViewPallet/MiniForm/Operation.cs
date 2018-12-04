using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    

    public enum PackingMaterials
    {
        Cap,
        Bottle,
        Pouch,
        Outer,
        Divider,
        Empty
    }

    public enum Machines
    {
        JUJENG,
        POSIMAT1, POSIMAT2, POSIMAT3,
        SANGTAO2, SANGTAO3, SANGTAO4,
        MESPACK1, MESPACK2,
        AKASH1, AKASH2, AKASH3,
        VOLPACK, LEEPACK

    }
    public enum Codes
    {
        Cap015246579, Cap115246579, Cap215246579, Cap3, Cap4,
        Bot0, Bot1, Bot2, Bot3, Bot4,
        Pou0, Pou1, Pou215246579, Pou3, Pou4,
        Out0, Out115246579, Out2, Out3, Out4,
        Div0, Div1, Div2, Div315246579, Div4,
        Empty
    }

    public enum ErrorCodes
    {
        ErrorCode
    }

    public class Operation : INotifyPropertyChanged
    {
        private string _machine;
        private PackingMaterials _material;
        private int _quantity;
        private int _buffered;
        private Codes _serials;

        [ReadOnlyAttribute(true)]
        public string Machine
        {
            get { return _machine; }
            set
            {
                _machine = value;
                NotifyPropertyChanged("Machine");
            }
        }

        [ReadOnlyAttribute(true)]
        public PackingMaterials PMs
        {
            get { return _material; }
            set
            {
                _material = value;
                NotifyPropertyChanged("PMs");
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
                NotifyPropertyChanged("Quantity");
            }
        }
        //###################################################
        [ReadOnlyAttribute(false)]
        public Codes Serial
        {
            get
            {
                return _serials;
            }
            set
            {
                _serials = value;
                NotifyPropertyChanged("Serial");
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
                NotifyPropertyChanged("Buffered");
            }
        }





        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
