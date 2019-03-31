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
        private double palletMargin = 0.1; //metters

        private dtPallet pPallet;
        public dtPallet pallet { get => pPallet; set => pPallet = value; }

        public string name = "";
        public TextBlock lbPallet;
        public TextBlock lbPallet2;

        public PalletShape(string name)
        {
            pallet = new dtPallet();
            this.name = name;
            Name = name;
            // Specific Size of Pallet
            Margin = new Thickness
                (
                (palletMargin / Global_Object.resolution)+0,
                (palletMargin / Global_Object.resolution)+0,
                (palletMargin / Global_Object.resolution)+0,
                (palletMargin / Global_Object.resolution)+0
                );
            //Padding = new Thickness(5,0,5,0);
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
            StatusChanged(new dtPallet());

            lbPallet = new TextBlock();
            lbPallet.TextWrapping = TextWrapping.Wrap;
            lbPallet.Width = this.Width;
            lbPallet.FontSize = 2;
            lbPallet.Text = this.name.Split('x')[1] +"-"+ this.name.Split('x')[2];

            lbPallet2 = new TextBlock();
            lbPallet2.TextWrapping = TextWrapping.Wrap;
            lbPallet2.Width = this.Width;
            lbPallet2.FontSize = 1.5;
            lbPallet2.Text = pallet.productDetailName;

            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            rectangle.Width = 2;
            rectangle.Height = 2;
            rectangle.Fill = new SolidColorBrush(Colors.Red);


            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;
            stackPanel.Children.Add(lbPallet);
            stackPanel.Children.Add(lbPallet2);

            Child = stackPanel;

            // Event handler
            MouseDown += PalletMouseDown;
          
        }


        /// <summary>
        /// Free LightGray, Plan OrangeYellow, Wait Green
        /// </summary>
        /// <param name="status"></param>
        public void StatusChanged(dtPallet pPallet)
        {
            if (this.pallet != null)
            {
                bool replaceStatus = (this.pallet.palletStatus != pPallet.palletStatus) ? true : false;
                bool replaceProductDetailName = (this.pallet.productDetailName != pPallet.productDetailName) ? true : false;
                this.pallet = pPallet;
                if (replaceProductDetailName || replaceStatus)
                {
                    Dispatcher.BeginInvoke(
                   new ThreadStart(() =>
                   {

                       switch (pPallet.palletStatus)
                       {
                           case "F":
                               {
                                   if (replaceProductDetailName)
                                   {
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
                                           }
                                           else
                                           {
                                               lbPallet2.Text = "";
                                           }

                                       }
                                   }
                                   if (replaceStatus)
                                   {
                                       Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F8FFE5"));
                                   }

                                   break;
                               }
                           case "P":
                               {
                                   if (replaceProductDetailName)
                                   {
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
                                           }
                                           else
                                           {
                                               lbPallet2.Text = "";
                                           }

                                       }
                                   }
                                   if (replaceStatus)
                                   {
                                       Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1B9AAA"));
                                   }
                                   break;
                               }
                           case "W":
                               {
                                   if (replaceProductDetailName)
                                   {
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
                                           }
                                           else
                                           {
                                               lbPallet2.Text = "";
                                           }

                                       }
                                   }
                                   if (replaceStatus)
                                   {
                                       Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00FF99"));
                                   }
                                   break;
                               }
                           case "H":
                               {
                                   if (replaceProductDetailName)
                                   {
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
                                           }
                                           else
                                           {
                                               lbPallet2.Text = "";
                                           }

                                       }
                                   }
                                   if (replaceStatus)
                                   {
                                       Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EC9A29"));
                                   }
                                   break;
                               }
                           case "L":
                               {
                                   if (replaceProductDetailName)
                                   {
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
                                           }
                                           else
                                           {
                                               lbPallet2.Text = "";
                                           }

                                       }
                                   }
                                   if (replaceStatus)
                                   {
                                       Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#808080"));
                                   }
                                   break;
                               }
                           default:
                               {
                                   if (replaceProductDetailName)
                                   {
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
                                           }
                                           else
                                           {
                                               lbPallet2.Text = "";
                                           }

                                       }
                                   }
                                   if (replaceStatus)
                                   {
                                       Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF1F1F"));
                                   }
                                   break;
                               }

                       }
                   }));
                }
            }
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
