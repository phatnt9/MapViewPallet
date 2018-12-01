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
            _stations.Add(
                new DgvStation {
                    Angle = 22,
                    Bays = 3,
                    Name = "sss",
                    Position = new System.Windows.Point(202, 20),
                    Rows = 9
                }
                
                );
            Stations_DGV = CollectionViewSource.GetDefaultView(_stations);
            Stations_DGV.CollectionChanged += asd;

        }

        private void asd(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        public void AddItem(DgvStation x)
        {
            _stations.Add(x);
           
        }

    }
}
