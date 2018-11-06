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
        public PalletShape(string typePallet)
        {
            Width = Global_Object.palletWidth;
            Height = Global_Object.palletHeight;
            Margin = new Thickness(2);
            BorderBrush = new SolidColorBrush(Colors.Green);
            BorderThickness = new Thickness(0);
            Bitmap bmp = (Bitmap)MapViewPallet.Properties.Resources.ResourceManager.GetObject(typePallet);
            ImageBrush img = new ImageBrush();
            img.ImageSource = ImageSourceForBitmap(bmp);
            Background = img;
            this.MouseDown += PalletMouseDown;
        }

        private void PalletMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        //\\\\\\\\\\\\Action\\\\\\\\\\\\\\\\\\
        public void Move(double X, double Y)
        {
            TranslateTransform a = new TranslateTransform(X, Y);
            RotateTransform b = new RotateTransform(45);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(a);
            //myTransformGroup.Children.Add(b);
            RenderTransform = myTransformGroup;
        }
        public void ChangeSize(int size)
        {
            Width = size;
            Height = size;
        }
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
