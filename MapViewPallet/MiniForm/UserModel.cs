using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MapViewPallet.MiniForm
{
    public class UserModel : NotifyUIBase
    {
        private UserManagement userManagement;

        public ListCollectionView GroupedUsers { get; private set; }

        public List<dtUser> usersList;

        public UserModel(UserManagement userManagement)
        {
            this.userManagement = userManagement;
            usersList = new List<dtUser>();
            GroupedUsers = (ListCollectionView)CollectionViewSource.GetDefaultView(usersList);

            UserAuthorCollection cc = Application.Current.Resources["UserAuthorSource"] as UserAuthorCollection;
            cc.Add(new SimpleUser { UserString = "Admin", Id = 1 });
            cc.Add(new SimpleUser { UserString = "Head of department", Id = 2 });
            cc.Add(new SimpleUser { UserString = "Worker", Id = 3 });
            cc.Add(new SimpleUser { UserString = "Forklift", Id = 4 });
            Application.Current.Resources["UserAuthorSource"] = cc;
        }

        public void ReloadListUsers()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                usersList.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "user/getListUser");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dtUser userSend = new dtUser();
                userSend.userAuthor = Global_Object.userAuthor;
                string jsonData = JsonConvert.SerializeObject(userSend);
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

                    DataTable users = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in users.Rows)
                    {
                        dtUser user = new dtUser
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),

                            userId = int.Parse(dr["userId"].ToString()),
                            userName = dr["userName"].ToString(),
                            userPassword = dr["userPassword"].ToString(),
                            userAuthor = int.Parse(dr["userAuthor"].ToString()),
                            //userDevices
                            flagModify = int.Parse(dr["flagModify"].ToString()),
                            userDeviceId = int.Parse(dr["userDeviceId"].ToString()),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            deviceName = dr["deviceName"].ToString(),
                            userPasswordOld = dr["userPasswordOld"].ToString(),
                        };
                        if (!ContainUser(user, usersList))
                        {
                            usersList.Add(user);
                        }
                    }
                }
                if (GroupedUsers.IsEditingItem)
                {
                    GroupedUsers.CommitEdit();
                }

                if (GroupedUsers.IsAddingNew)
                {
                    GroupedUsers.CommitNew();
                }

                GroupedUsers.Refresh();
                if (userManagement.UsersListDg.HasItems)
                {
                    userManagement.UsersListDg.SelectedItem = userManagement.UsersListDg.Items[0];
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        public bool ContainUser(dtUser tempOpe, List<dtUser> List)
        {
            foreach (dtUser temp in List)
            {
                if (temp.userId > 0)
                {
                    if (temp.userId == tempOpe.userId)
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
    }
}