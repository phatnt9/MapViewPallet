using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace MapViewPallet.DataGridView
{
    public class DgvStation : INotifyPropertyChanged
    {
        private string _name;
        private int _bays;
        private int _rows;
        private Point _position;
        private double _rotate;

        [ReadOnlyAttribute(true)]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        [ReadOnlyAttribute(true)]
        public int Bays
        {
            get { return _bays; }
            set
            {
                _bays = value;
                NotifyPropertyChanged("Bays");
            }
        }

        [ReadOnlyAttribute(true)]
        public int Rows
        {
            get { return _rows; }
            set
            {
                _rows = value;
                NotifyPropertyChanged("Rows");
            }
        }

        [ReadOnlyAttribute(true)]
        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                NotifyPropertyChanged("Position");
            }
        }

        [ReadOnlyAttribute(true)]
        public double Angle
        {
            get { return _rotate; }
            set
            {
                _rotate = value;
                NotifyPropertyChanged("Angle");
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
