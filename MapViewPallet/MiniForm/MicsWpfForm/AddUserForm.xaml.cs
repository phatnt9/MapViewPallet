using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

namespace MapViewPallet.MiniForm.MicsWpfForm
{
    public class SimpleAuthor : NotifyUIBase
    {

        private int pUserAuthor;
        private string pUserAuthorName;

        public int userAuthor { get => pUserAuthor; set { pUserAuthor = value; RaisePropertyChanged("userAuthor"); } }
        public string userAuthorName { get => pUserAuthorName; set { pUserAuthorName = value; RaisePropertyChanged("userAuthorName"); } }
    }


    /// <summary>
    /// Interaction logic for AddUserForm.xaml
    /// </summary>
    public partial class AddUserForm : Window
    {
        public bool flgEdit;
        dtUser userEdit;


        UserManagement userManagement;

        public AddUserForm()
        {
            InitializeComponent();
        }

        public AddUserForm(UserManagement userManagement, string cultureName = null)
        {
            Loaded += AddUserForm_Loaded;
            InitializeComponent();
            ApplyLanguage(cultureName);
            this.userManagement = userManagement;
        }

        public AddUserForm(UserManagement userManagement, dtUser userEdit, string cultureName = null)
        {
            Loaded += AddUserForm_Loaded;
            InitializeComponent();
            ApplyLanguage(cultureName);
            this.userManagement = userManagement;
            this.userEdit = userEdit;
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

        private void CmbAuthor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine((cmbAuthor.SelectedItem as SimpleAuthor).userAuthor.ToString());
            if (cmbAuthor.SelectedItem == null)
            {
                return;
            }
            cmbDevice.IsEnabled = false;
            //if ((cmbAuthor.SelectedItem as SimpleAuthor).userAuthor.ToString() == "3")

            //if (((cmbAuthor.SelectedValue==null)? (cmbAuthor.SelectedItem as SimpleAuthor).userAuthor.ToString(): cmbAuthor.SelectedValue.ToString()) == "3")
            if ((cmbAuthor.SelectedItem as SimpleAuthor).userAuthor.ToString() == "3")
            {
                cmbDevice.IsEnabled = true;
            }
            else
            {
                if (cmbDevice.SelectedIndex > 0)
                {
                    cmbDevice.SelectedIndex = 0;
                }
            }
        }

        private void AddUserForm_Loaded(object sender, RoutedEventArgs e)
        {
            CenterWindowOnScreen();
            loadAuthor();
            loadDevice();

            if (flgEdit)
            {
                //this.Title = "Tùy chỉnh";
                //functionTextBlock.Text = "Tùy chỉnh";
                this.userNametb.Text = (userManagement.UsersListDg.SelectedItem as dtUser).userName.ToString();
                this.userNametb.IsReadOnly = true;
                cmbAuthor.SelectedValue = int.Parse((userManagement.UsersListDg.SelectedItem as dtUser).userAuthor.ToString());
                cmbDevice.SelectedValue = (userManagement.UsersListDg.SelectedItem as dtUser).deviceId.ToString();
            }
            else
            {
                this.userNametb.IsReadOnly = false;
                //this.Title = "Thêm mới";
                //functionTextBlock.Text = "Thêm mới";
            }
            userNametb.Focus();
        }

        private void loadAuthor()
        {
            List<SimpleAuthor> dt = new List<SimpleAuthor>();

            if (Global_Object.userAuthor == 0)
            {
                dt.Add(new SimpleAuthor(){userAuthor = 1,userAuthorName = "Admin",});
                dt.Add(new SimpleAuthor(){userAuthor = 2,userAuthorName = "Head of department", });
                dt.Add(new SimpleAuthor(){userAuthor = 3,userAuthorName = "Worker", });
                dt.Add(new SimpleAuthor(){userAuthor = 4,userAuthorName = "Forklift", });
            }
            else if (Global_Object.userAuthor == 1)
            {
                dt.Add(new SimpleAuthor() { userAuthor = 2, userAuthorName = "Head of department", });
                dt.Add(new SimpleAuthor() { userAuthor = 3, userAuthorName = "Worker", });
                dt.Add(new SimpleAuthor() { userAuthor = 4, userAuthorName = "Forklift", });
            }
            else if (Global_Object.userAuthor == 2)
            {
                dt.Add(new SimpleAuthor() { userAuthor = 3, userAuthorName = "Worker", });
                dt.Add(new SimpleAuthor() { userAuthor = 4, userAuthorName = "Forklift", });
            }
            cmbAuthor.ItemsSource = dt;
        }

        private void loadDevice()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                List<dtDevice> dt = new List<dtDevice>();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "device/getListDevice");
                request.Method = "GET";
                request.ContentType = @"application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();

                    DataTable devices = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in devices.Rows)
                    {
                        dtDevice tempDevice = new dtDevice
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            deviceName = dr["deviceName"].ToString()
                        };
                        if (!ContainDevice(tempDevice, dt))
                        {
                            dt.Add(tempDevice);
                        }

                    }
                }
                cmbDevice.ItemsSource = dt;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            
        }

        public bool ContainDevice(dtDevice tempOpe, List<dtDevice> List)
        {
            foreach (dtDevice temp in List)
            {
                if (temp.deviceId > 0)
                {
                    if (temp.deviceId == tempOpe.deviceId)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                if (string.IsNullOrEmpty(this.userNametb.Text) || this.userNametb.Text.Trim() == "")
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "User Name", "User Name"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.userNametb.Focus();
                    return;
                }

                if (!flgEdit)
                {
                    if (string.IsNullOrEmpty(this.userPasswordtb.Text) || this.userPasswordtb.Text.Trim() == "")
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "Password", "Password"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.userPasswordtb.Focus();
                        return;
                    }
                }

                if (cmbAuthor.SelectedValue.ToString() == "3" && cmbDevice.SelectedValue.ToString() == "")
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageValidate, "Device", "Device"), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.cmbDevice.Focus();
                    return;
                }

                dtUser user = new dtUser();
                if (flgEdit)
                {
                    //user.userId = frm.userIdSelect;
                    user.userId = (userManagement.UsersListDg.SelectedItem as dtUser).userId;
                }
                user.userName = this.userNametb.Text.Trim();
                user.userPassword = this.userPasswordtb.Text.Trim();
                user.userAuthor = int.Parse(cmbAuthor.SelectedValue.ToString());
                user.deviceId = (cmbDevice.SelectedValue == null || cmbDevice.SelectedValue.ToString() == "") ? -1 : int.Parse(cmbDevice.SelectedValue.ToString());
                user.creUsrId = Global_Object.userLogin;
                user.updUsrId = Global_Object.userLogin;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "user/insertUpdateUserInfo");
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
                    dtUser dtUser = JsonConvert.DeserializeObject<dtUser>(result);
                    if (dtUser.flagModify == -2)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "User Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.userNametb.Focus();
                        return;
                    }
                    else if (dtUser.flagModify > 0)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //frm.userIdSelect = dtUser.userId;
                        userManagement.userModel.ReloadListUsers();
                    }
                    else if (dtUser.flagModify == -1)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private void Btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
    }
}
