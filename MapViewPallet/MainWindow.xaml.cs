﻿using MapViewPallet.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapViewPallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum STATECTRL_MOUSEDOWN
        {
            STATECTRL_NORMAL,
            STATECTRL_ADD_STATION
        }
        public STATECTRL_MOUSEDOWN valstatectrl_md = STATECTRL_MOUSEDOWN.STATECTRL_NORMAL;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (valstatectrl_md == STATECTRL_MOUSEDOWN.STATECTRL_ADD_STATION)
            {
                var mouseWasDownOn = e.Source as FrameworkElement;
                Point pp = e.GetPosition(map);
                PalletShape rect0 = null;
                rect0 = new PalletShape(this);
                rect0.Move(pp.X, pp.Y);
                map.Children.Add(rect0);
                Console.WriteLine(rect0.GetPosition().ToString());
            }
        }

        private void btn_AddRect_Click(object sender, RoutedEventArgs e)
        {
            if (valstatectrl_md == STATECTRL_MOUSEDOWN.STATECTRL_NORMAL)
            {

                valstatectrl_md = STATECTRL_MOUSEDOWN.STATECTRL_ADD_STATION;
            }
            else
            {
                valstatectrl_md = STATECTRL_MOUSEDOWN.STATECTRL_NORMAL;
            }

        }



    }
}
