using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MapViewPallet.DataGridView
{
    public class Line
    {
        public Point From { get; set; }

        public Point To { get; set; }
    }

    public class MainWindowModel
    {
        public ListCollectionView Stations_DGV { get; private set; }

        //***************VARIABLES*********************
        public List<DgvStation> _stationsDgv;
        public MainWindow mainWindow;

        public MainWindowModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            
            
            _stationsDgv = new List<DgvStation>();
            Stations_DGV = (ListCollectionView)CollectionViewSource.GetDefaultView(_stationsDgv);
        }

        public void AddItem(DgvStation x)
        {
            _stationsDgv.Add(x);
            Stations_DGV.Refresh();
        }

        



    }
}
