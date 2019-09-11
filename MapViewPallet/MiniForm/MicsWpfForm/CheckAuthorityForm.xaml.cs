using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    /// <summary>
    /// Interaction logic for CheckAuthorityForm.xaml
    /// </summary>
    public partial class CheckAuthorityForm : Window
    {
        public CheckAuthorityForm()
        {
            InitializeComponent();
        }

        private void Tb_userName_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.tb_password.SelectAll();
                this.tb_password.Focus();
            }
        }

        private void Tb_password_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Btn_login_Click(sender, e);
            }
        }

        private void Btn_login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tb_userName.Text) || this.tb_userName.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "User Name", "User Name"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tb_userName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(this.tb_password.Password) || this.tb_password.Password.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "Password", "Password"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tb_password.Focus();
                return;
            }
            try
            {
                dtUser user = new dtUser();
                user.userName = this.tb_userName.Text;
                user.userPassword = this.tb_password.Password;
                string jsonData = JsonConvert.SerializeObject(user);
                string contentJson = Global_Object.RequestDataAPI(jsonData, "user/getUserInfo", Global_Object.RequestMethod.POST);
                dynamic response = JsonConvert.DeserializeObject(contentJson);
                user = response.ToObject<dtUser>();
                if (user.userAuthor == 0 || user.userAuthor == 1)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    lb_warning.Content = "Please try again";
                }
            }
            catch
            {
                lb_warning.Content = "Wrong User Name or Password";
            }
            
        }
    }
}
