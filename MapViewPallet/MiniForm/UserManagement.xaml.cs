using MapViewPallet.MiniForm.MicsWpfForm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace MapViewPallet.MiniForm
{
    
    /// <summary>
    /// Interaction logic for UserManagement.xaml
    /// </summary>
    public partial class UserManagement : Window
    {

        public UserModel userModel;

        public UserManagement(string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            userModel = new UserModel(this);
            DataContext = userModel;
            Loaded += UserManagement_Loaded;

            UsersListDg.SelectionMode = DataGridSelectionMode.Single;
            UsersListDg.SelectionUnit = DataGridSelectionUnit.FullRow;


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

        private void UserManagement_Loaded(object sender, RoutedEventArgs e)
        {
            userModel.ReloadListUsers();
        }

        private void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            AddUserForm frm = new AddUserForm(this, Thread.CurrentThread.CurrentCulture.ToString());
            frm.flgEdit = false;
            frm.ShowDialog();
        }

        private void Btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListDg.SelectedItem == null || UsersListDg.HasItems == false)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNothingSelected), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AddUserForm frm = new AddUserForm(this, UsersListDg.SelectedItem as dtUser, Thread.CurrentThread.CurrentCulture.ToString());
            frm.flgEdit = true;
            frm.ShowDialog();
        }

        private void Btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            userModel.ReloadListUsers();
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListDg.SelectedItem == null || !UsersListDg.HasItems)
            {
                System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNothingSelected), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteConfirm, "User"), Global_Object.messageTitileWarning, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                List<dtUser> listDelete = new List<dtUser>();
                dtUser user = UsersListDg.SelectedItem as dtUser;
                listDelete.Add(user);

                string jsonData = JsonConvert.SerializeObject(listDelete);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "user/deleteListUser");
                request.Method = "DELETE";
                request.ContentType = "application/json";

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
                    int result = 0;
                    int.TryParse(reader.ReadToEnd(), out result);
                    if (result == 1)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        userModel.ReloadListUsers();
                    }
                    else if (result == 2)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteUse, "User", "Other Screen"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDeleteFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }
    }
}
