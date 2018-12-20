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

    public class DgvModel
    {
        public ListCollectionView Stations_DGV { get; private set; }
        public ObservableCollection<StationShape> Stations_Canvas { get; private set; }
        public ObservableCollection<Line> Lines { get; private set; }
        public ObservableCollection<StationShape> StationShapes { get; private set; }

        //***************VARIABLES*********************
        public List<DgvStation> _stationsDgv;
        public List<StationShape> _stations_Canvas;
        public MainWindow mainWindow;

        public DgvModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            Lines = new ObservableCollection<Line>
            {
            new Line { From = new Point(100, 20), To = new Point(180, 180) },
            new Line { From = new Point(180, 180), To = new Point(20, 180) },
            new Line { From = new Point(20, 180), To = new Point(100, 20) },
            new Line { From = new Point(20, 50), To = new Point(180, 150) }
            };

            StationShapes = new ObservableCollection<StationShape>();


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
