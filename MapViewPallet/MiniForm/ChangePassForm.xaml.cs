using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace MapViewPallet.MiniForm
{
    /// <summary>
    /// Interaction logic for ChangePassForm.xaml
    /// </summary>
    public partial class ChangePassForm : Window
    {
        public ChangePassForm(string cultureName = null)
        {
            Loaded += ChangePassForm_Loaded;
            InitializeComponent();
            ApplyLanguage(cultureName);
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

        private void ChangePassForm_Loaded(object sender, RoutedEventArgs e)
        {
            this.userNametb.Text = Global_Object.userName;
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PasswordCurrenttb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.passwordNewtb.SelectAll();
                this.passwordNewtb.Focus();
            }
        }

        private void PasswordNewtb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.passwordNewConfirmtb.SelectAll();
                this.passwordNewConfirmtb.Focus();
            }
        }

        private void PasswordNewConfirmtb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btn_save_Click(sender, e);
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(passwordNewtb.Password) || String.IsNullOrEmpty(passwordNewConfirmtb.Password))
            {
                System.Windows.Forms.MessageBox.Show("Password không được để trống!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (String.IsNullOrEmpty(passwordNewtb.Password))
                {
                    passwordNewtb.Focus();
                }
                else
                {
                    passwordNewConfirmtb.Focus();
                }
                return;
            }

            if (passwordNewtb.Password != passwordNewConfirmtb.Password)
            {
                System.Windows.Forms.MessageBox.Show("New Password don't match the Confirm New Password!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordNewtb.SelectAll();
                passwordNewtb.Focus();
                return;
            }

            dtUser user = new dtUser();
            user.userId = Global_Object.userLogin;
            user.userName = this.userNametb.Text;
            user.userAuthor = Global_Object.userAuthor;
            user.userPasswordOld = this.passwordCurrenttb.Password;
            user.userPassword = this.passwordNewtb.Password;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "user/changPasswordUser");
            request.Method = "POST";
            request.ContentType = @"application/json";
            string jsonData = JsonConvert.SerializeObject(user);
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonData);
            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Flush();
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                int result = int.Parse(reader.ReadToEnd());

                if (result == 1)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else if (result == -2)
                {
                    System.Windows.Forms.MessageBox.Show("Password is correct!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    passwordCurrenttb.SelectAll();
                    passwordCurrenttb.Focus();
                    return;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Change Password fail!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


        }
    }
}
