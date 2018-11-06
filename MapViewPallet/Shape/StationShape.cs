using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace MapViewPallet.Shape
{
    public class StationShape:Border
    {
        Grid stationGrid;

        public StationShape ()
        {
        }

        public StationShape (string stationName, int lines, int pallets_per_line, string typePallet)
        {
            Padding = new Thickness(0);
            Name = stationName;
            BorderBrush = new SolidColorBrush(Colors.Red);
            BorderThickness = new Thickness(0.1);
            stationGrid = new Grid();
            for (int lineIndex=0; lineIndex<lines; lineIndex++)
            {
                ColumnDefinition colTemp = new ColumnDefinition();
                colTemp.Name = Name + "x" + lineIndex;
                stationGrid.ColumnDefinitions.Add(colTemp);
            }
            for (int palletIndex = 0; palletIndex < pallets_per_line; palletIndex++)
            {
                RowDefinition rowTemp = new RowDefinition();
                rowTemp.Name = Name + "x" + palletIndex;
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
            Child = stationGrid;
        }

        public void Move(Point pos)
        {
            TranslateTransform a = new TranslateTransform(pos.X, pos.Y);
            RotateTransform b = new RotateTransform(45);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(a);
            //myTransformGroup.Children.Add(b);
            RenderTransform = myTransformGroup;
        }
    }
}
