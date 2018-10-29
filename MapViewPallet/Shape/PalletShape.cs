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
    class PalletShape : Border
    {
        readonly MainWindow mainWindow;

        public PalletShape (MainWindow main)
        {
            mainWindow = main;
            Width = 30;
            Height = 30;
            Background = new SolidColorBrush(Colors.Black);
            BorderBrush = new SolidColorBrush(Colors.YellowGreen);
            BorderThickness = new Thickness(3);
        }

        public void AddChildPallet ()
        {
                       
        }

        public void Move (double X, double Y)
        {
            TranslateTransform a = new TranslateTransform(X-15, Y-15);
            RotateTransform b = new RotateTransform(45);

            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(a);
            //myTransformGroup.Children.Add(b);

            RenderTransform = myTransformGroup;



        }
        public Point GetPosition ()
        {
            Point position = new Point();
            TranslateTransform x = new TranslateTransform();
            position = this.TransformToAncestor(mainWindow).Transform(position);
            return position;
        }
        
    }
}
