using System.Text.RegularExpressions;
using System.Windows;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for BufferSettingForm.xaml
    /// </summary>
    public partial class BufferSettingForm : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        public BufferSettingForm()
        {
            InitializeComponent();
            Loaded += BufferSettingForm_Loaded;
        }

        private void BufferSettingForm_Loaded(object sender, RoutedEventArgs e)
        {
            bufferWidth.Text = Properties.Settings.Default["palletWidth"].ToString();
            bufferHeight.Text = Properties.Settings.Default["palletHeight"].ToString();
            bufferPadding.Text = Properties.Settings.Default["palletPadding"].ToString();
            bufferMargin.Text = Properties.Settings.Default["palletMargin"].ToString();
        }

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double width = 0;
            double height = 0;
            if(double.TryParse(bufferWidth.Text.ToString().Trim().Replace(" ", ""),out width))
            {
                if (width.ToString().Trim() != "")
                {
                    Properties.Settings.Default.palletWidth = width;
                    Properties.Settings.Default.Save();
                }
            }
            if(double.TryParse(bufferHeight.Text.ToString().Trim().Replace(" ", ""),out height))
            {
                if (height.ToString().Trim() != "")
                {
                    Properties.Settings.Default.palletHeight = height;
                    Properties.Settings.Default.Save();
                }
            }

        }
        

        private void Buffer_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
