using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    /// Interaction logic for SetupIpAndPort.xaml
    /// </summary>
    public partial class SetupIpAndPort : Window
    {
        private static readonly Regex _ipRegex = new Regex(
            @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\." +
            @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\." +
            @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\." +
            @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
        private static readonly Regex _portRegex = new Regex(
            @":(6553[0-5]|655[0-2][0-9]\d|65[0-4](\d){2}|6[0-4](\d){3}|[1-5](\d){4}|[1-9](\d){0,3})");


        public SetupIpAndPort(string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            Loaded += SetupIpAndPort_Loaded;
        }

        private void SetupIpAndPort_Loaded(object sender, RoutedEventArgs e)
        {
            tb_ip.Text = Properties.Settings.Default.serverIp;
            tb_port.Text = Properties.Settings.Default.serverPort;
            tb_serverRobotip.Text = Properties.Settings.Default.serverReturnIp;
            tb_ip.Focus();
        }

        public void ApplyLanguage(string cultureName = null)
        {
            if (cultureName != null)
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);

            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "vi-VN":
                    dict.Source = new Uri("..\\Lang\\Vietnamese.xaml", UriKind.Relative);
                    break;
                // ...
                default:
                    dict.Source = new Uri("..\\Lang\\English.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private static bool IsIpAllowed(string text)
        {
            return !_ipRegex.IsMatch(text);
        }

        private static bool IsPortAllowed(string text)
        {
            return !_portRegex.IsMatch(text);
        }

        public bool IsValidateIP(string Address)
        {
            //Match pattern for IP address    
            string Pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            //Regular Expression object    
            Regex check = new Regex(Pattern);

            //check to make sure an ip address was provided    
            if (string.IsNullOrEmpty(Address))
                //returns false if IP is not provided    
                return false;
            else
                //Matching the pattern    
                return check.IsMatch(Address, 0);
        }

        public bool IsValidatePort(string Port)
        {
            //Match pattern for IP address    
            string Pattern = @"^([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$";
            //Regular Expression object    
            Regex check = new Regex(Pattern);

            //check to make sure an ip address was provided    
            if (string.IsNullOrEmpty(Port))
                //returns false if IP is not provided    
                return false;
            else
                //Matching the pattern    
                return check.IsMatch(Port, 0);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsIpAllowed(e.Text);
        }

        private void Tb_port_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsPortAllowed(e.Text);
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidateIP(tb_ip.Text))
            {
                Properties.Settings.Default.serverIp = tb_ip.Text;
                Properties.Settings.Default.Save();
            }
            if (IsValidateIP(tb_serverRobotip.Text))
            {
                Properties.Settings.Default.serverReturnIp = tb_serverRobotip.Text;
                Properties.Settings.Default.Save();
            }
            if (IsValidatePort(tb_port.Text))
            {
                Properties.Settings.Default.serverPort = tb_port.Text;
                Properties.Settings.Default.Save();
            }

            Console.WriteLine(Properties.Settings.Default.serverIp);
            Console.WriteLine(Properties.Settings.Default.serverPort);
            Console.WriteLine(Properties.Settings.Default.serverReturnIp);
            Close();
        }
    }
}
