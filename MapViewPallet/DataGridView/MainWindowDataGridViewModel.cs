using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MapViewPallet.DataGridView
{


    public class MainWindowDataGridViewModel
    {
        public ICollectionView Stations_DGV { get; private set; }

        //***************VARIABLES*********************
        public List<DgvStation> _stations;
        public MainWindowDataGridViewModel()
        {
            _stations = new List<DgvStation>();
            Stations_DGV = CollectionViewSource.GetDefaultView(_stations);
        }

        public void AddItem(DgvStation x)
        {
            _stations.Add(x);
            Stations_DGV.Refresh();
        }

    }
}
