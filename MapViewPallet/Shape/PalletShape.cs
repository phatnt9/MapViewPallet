using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapViewPallet.Shape
{
    class PalletShape : Border
    {
        private double palletWidth = 1; //metters
        private double palletHeight = 1.2; //metters
        private double palletMargin = 0.1; //metters
        //private double palletPadding = 0.5; //metters

        public PalletShape(string typePallet)
        {
            // Specific Size of Pallet
            Width = palletWidth / Global_Object.resolution; //in pixel
            Height = palletHeight / Global_Object.resolution; //in pixel
            Margin = new Thickness(palletMargin / Global_Object.resolution);
            //Padding = new Thickness(palletPadding / Global_Object.resolution);
            // Style Pallet
            BorderBrush = new SolidColorBrush(Colors.Green);
            BorderThickness = new Thickness(0);
            // Background for Pallet
            Bitmap bmp = (Bitmap)MapViewPallet.Properties.Resources.ResourceManager.GetObject(typePallet);
            ImageBrush img = new ImageBrush();
            img.ImageSource = ImageSourceForBitmap(bmp);
            Background = img;
            // Event handler
            MouseDown += PalletMouseDown;
        }

        private void PalletMouseDown(object sender, MouseButtonEventArgs e)
        {
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

    }
}
