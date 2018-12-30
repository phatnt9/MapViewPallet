using MapViewPallet.MiniForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static MapViewPallet.Global_Object;

namespace MapViewPallet.Shape
{
    public class PalletShape : Border
    {
        private double palletMargin = 0.05; //metters

        private dtPallet pPallet;
        public dtPallet pallet { get => pPallet; set => pPallet = value; }

        public string name = "";


        public PalletShape(string name)
        {
            this.name = name;
            // Specific Size of Pallet
            Margin = new Thickness(palletMargin / Global_Object.resolution);
            //Padding = new Thickness(palletPadding / Global_Object.resolution);
            // Style Pallet Border
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new Thickness(0.3);
            CornerRadius = new CornerRadius(1.3);
            // Background for Pallet
            //Bitmap bmp = (Bitmap)MapViewPallet.Properties.Resources.ResourceManager.GetObject(typePallet);
            //ImageBrush img = new ImageBrush();
            //img.ImageSource = ImageSourceForBitmap(bmp);
            //Background = img;
            //=============================
            StatusChanged("normal");

            Label lbPallet = new Label();
            lbPallet.Width = this.Width;
            lbPallet.FontSize = 3;
            lbPallet.Margin = new Thickness(-4);
            lbPallet.Content = this.name.Split('x')[1] +"-"+ this.name.Split('x')[2];

            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            rectangle.Width = 2;
            rectangle.Height = 2;
            rectangle.Fill = new SolidColorBrush(Colors.Red);


            StackPanel stackPanel = new StackPanel();
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;
            stackPanel.Children.Add(lbPallet);

            Child = stackPanel;

            // Event handler
            MouseDown += PalletMouseDown;
          
        }


        /// <summary>
        /// Free LightGray, Plan OrangeYellow, Wait Green
        /// </summary>
        /// <param name="status"></param>
        public void StatusChanged (string status)
        {
            /*Dispatcher.BeginInvoke(
        new ThreadStart(() => EmployeesDataGrid.DataContext = employeesView));*/
            Dispatcher.BeginInvoke(
           new ThreadStart(() => 
           {
               switch (status)
               {
                   case "F":
                       {
                           Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F8FFE5"));
                           break;
                       }
                   case "P":
                       {
                           Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1B9AAA"));
                           break;
                       }
                   case "W":
                       {
                           Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00FF99"));
                           break;
                       }
                   default:
                       {
                           Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EF476F"));
                           break;
                       }

               }
           }));
        }

        private void PalletMouseDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(""+name);
        }
        
        //\\\\\\\\\\\\Action\\\\\\\\\\\\\\\
       
        //\\\\\\\\\\\\Others\\\\\\\\\\\\\\\
        private ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { }
        }
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        private void ChangeToolTipContent(object sender, ToolTipEventArgs e)
        {
            ToolTip = "Name: " + 
                "\n Start: " +
                " \n End: " +
                " \n Rotate: ";
        }
    }
}
