using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapViewPallet
{
    public class MainTreeView : TreeView
    {
        TreeViewItem robotList;
        TreeViewItem stationList;


        class RobotItem: TreeViewItem
        {
            public RobotItem()
            {

            }
        }

        class StationItem
        {
            StationShape station;
            public string Title { get; set; }
            public StationItem(StationShape pstation)
            {
                
                Items = new ObservableCollection<MenuItem>();
                station = pstation;
                Title = "sdasd";
            }

            public ObservableCollection<MenuItem> Items { get; set; }
        }

        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }

            public string Title { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }

        public MainTreeView()
        {
            //===========TEMPLATE==========
            HierarchicalDataTemplate dataTemplate = new HierarchicalDataTemplate();
            dataTemplate.DataType = "{x:Type self:StationItem}";
            this.ItemTemplate = new DataTemplate();
            //=============================
            Name = "mainTreeView";
            Background = new SolidColorBrush(Colors.White);
            BorderThickness = new Thickness(0);
            //=============================
            robotList = new TreeViewItem();
            Image imgR = new Image();
            TextBlock tbR = new TextBlock();
            StackPanel robotStackPanel = new StackPanel();
            //=====
            robotList.IsExpanded = true;
            robotStackPanel.Orientation = Orientation.Horizontal;
            imgR = LoadImage("pallet");
            imgR.Width = 15;
            tbR.Text = "Robot";
            //=====
            robotStackPanel.Children.Add(imgR);
            robotStackPanel.Children.Add(tbR);
            robotList.Header = robotStackPanel;
            //=============================
            stationList = new TreeViewItem();
            Image imgS = new Image();
            TextBlock tbS = new TextBlock();
            StackPanel stationStackPanel = new StackPanel();
            //=====
            stationList.IsExpanded = true;
            stationStackPanel.Orientation = Orientation.Horizontal;
            imgS = LoadImage("pallet");
            imgS.Width = 15;
            tbS.Text = "Station";
            //=====
            stationStackPanel.Children.Add(imgS);
            stationStackPanel.Children.Add(tbS);
            stationList.Header = stationStackPanel;

            //====================
            Items.Add(robotList);
            Items.Add(stationList);
        }


        public void AddRobot()
        {

        }

        public void AddStation(StationShape pstation)
        {
            stationList.Items.Add(new StationItem(pstation));
            //StackPanel stackPanel = new StackPanel();
            //Image img = new Image();
            //TextBlock tb = new TextBlock();
            ////=====
            //tb.Text = pstation.Name;
            //stackPanel.Orientation = Orientation.Horizontal;
            //img = LoadImage("pallet");
            //img.Width = 15;
            ////=====
            //stackPanel.Children.Add(img);
            //stackPanel.Children.Add(tb);
            ////=====
            //TreeViewItem temp = new TreeViewItem()
            //{
            //    Header = stackPanel
            //};
            //stationList.Items.Add(temp);
        }

        public Image LoadImage(string name)
        {
            System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject(name);
            Image img = new Image();
            img.Source = ImageSourceForBitmap(bmp);
            return img;
        }

        public ImageSource ImageSourceForBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { }
        }

    }
}
