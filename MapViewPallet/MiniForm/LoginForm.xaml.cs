using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace MapViewPallet.MiniForm
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        

        public LoginForm()
        {
            InitializeComponent();
            Loaded += LoginForm_Loaded;
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void LoginForm_Loaded(object sender, RoutedEventArgs e)
        {
            CenterWindowOnScreen();
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            userNametb.Focus();
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.userNametb.Text) || this.userNametb.Text.Trim() == "")
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "User Name", "User Name"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.userNametb.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(this.passwordtb.Password) || this.passwordtb.Password.Trim() == "")
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "Password", "Password"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.passwordtb.Focus();
                    return;
                }

                dtUser user = new dtUser();
                user.userName = this.userNametb.Text;
                user.userPassword = this.passwordtb.Password;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "user/getUserInfo");
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
                    string result = reader.ReadToEnd();
                    user = JsonConvert.DeserializeObject<dtUser>(result);

                    if (user.userAuthor == 0 || user.userAuthor == 1 || user.userAuthor == 2)
                    {
                        Global_Object.userAuthor = user.userAuthor;
                        Global_Object.userLogin = user.userId;
                        Global_Object.userName = user.userName;
                        this.Close();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Login Fail!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

            }
            catch
            {

            }
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        
        private void UserNametb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.passwordtb.SelectAll();
                this.passwordtb.Focus();
            }
        }

        private void Passwordtb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btn_login_Click(sender, e);
            }
        }
    }
}
