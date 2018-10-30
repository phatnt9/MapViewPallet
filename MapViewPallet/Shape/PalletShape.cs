using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapViewPallet.Shape
{
    class PalletShape : Border
    {
        public PalletShape(string typePallet)
        {
            Width = 13;
            Height = 15;
            Margin = new Thickness(2);
            BorderBrush = new SolidColorBrush(Colors.Green);
            BorderThickness = new Thickness(0);
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(typePallet);
            ImageBrush img = new ImageBrush();
            img.ImageSource = ImageSourceForBitmap(bmp);
            Background = img;
        }

        public void ChangeSize (int size)
        {
            Width = size;
            Height = size;
        }

        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally {}
        }

        public void Move(System.Drawing.Point p)
        {
            Canvas.SetLeft(this, p.X);
            Canvas.SetTop(this, p.Y);
        }
        public void Setcol ()
        {

        }

        public void Move (double X, double Y)
        {
            TranslateTransform a = new TranslateTransform(X, Y);
            RotateTransform b = new RotateTransform(45);
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(a);
            //myTransformGroup.Children.Add(b);
            RenderTransform = myTransformGroup;



        }
       /* public Point GetPosition ()
        {
            Point position = new Point();
            TranslateTransform x = new TranslateTransform();
            position = this.TransformToAncestor(mainWindow).Transform(position);
            return position;
        }*/
        
    }
}
