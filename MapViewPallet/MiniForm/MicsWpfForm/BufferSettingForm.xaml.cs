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
using System.Windows.Shapes;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for BufferSettingForm.xaml
    /// </summary>
    public partial class BufferSettingForm : Window
    {
        public BufferSettingForm()
        {
            InitializeComponent();
            Loaded += BufferSettingForm_Loaded;
        }

        private void BufferSettingForm_Loaded(object sender, RoutedEventArgs e)
        {
            bufferMargin.Text = Global_Object.StaticPalletMargin.ToString();
            bufferWidth.Text = Global_Object.StaticPalletWidth.ToString();
            bufferHeight.Text = Global_Object.StaticPalletHeight.ToString();
            bufferPadding.Text = Global_Object.StaticPalletPadding.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double width = 0;
            double height = 0;
            if(double.TryParse(bufferWidth.Text.ToString().Trim().Replace(" ", ""),out width))
            {
                Global_Object.StaticPalletWidth = width;
            }
            if(double.TryParse(bufferHeight.Text.ToString().Trim().Replace(" ", ""),out height))
            {
                Global_Object.StaticPalletHeight = height;
            }

        }
    }
}
