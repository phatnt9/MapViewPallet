using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace MapViewPallet.Shape
{
    class StationShape:Border
    {
        Grid stationGrid;

        public StationShape (string stationName, int lines, int pallets_per_line, string typePallet)
        {
            Padding = new Thickness(0);
            Name = stationName;
            BorderBrush = new SolidColorBrush(Colors.Gray);
            BorderThickness = new Thickness(0.4);
            stationGrid = new Grid();
            for (int lineIndex=0; lineIndex<lines; lineIndex++)
            {
                ColumnDefinition colTemp = new ColumnDefinition();
                colTemp.Name = Name + "xCol" + lineIndex;
                stationGrid.ColumnDefinitions.Add(colTemp);
            }
            for (int palletIndex = 0; palletIndex < pallets_per_line; palletIndex++)
            {
                RowDefinition rowTemp = new RowDefinition();
                rowTemp.Name = Name + "xRow" + palletIndex;
                stationGrid.RowDefinitions.Add(rowTemp);
            }
            for (int lineIndex = 0; lineIndex < lines; lineIndex++)
            {
                for (int palletIndex = 0; palletIndex < pallets_per_line; palletIndex++)
                {
                    PalletShape borderTemp = new PalletShape(typePallet);
                    borderTemp.Name = Name + "x" + lineIndex + "x" + palletIndex;
                    Grid.SetColumn(borderTemp, lineIndex);
                    Grid.SetRow(borderTemp, palletIndex);
                    stationGrid.Children.Add(borderTemp);
                }
            }
            this.Child = stationGrid;
        }

        public void Move(double X, double Y)
        {
            TranslateTransform a = new TranslateTransform(X - 15, Y - 15);
            RotateTransform b = new RotateTransform(45);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(a);
            //myTransformGroup.Children.Add(b);
            RenderTransform = myTransformGroup;
        }
    }
}
