using MapViewPallet.MiniForm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FontStyle = System.Windows.FontStyle;

namespace MapViewPallet.Shape
{
    public class PalletShape : Border
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly double palletMargin = 0.1; //metters

        private dtPallet pPallet;
        public dtPallet pallet { get => pPallet; set => pPallet = value; }

        private dtBuffer pBuffer;
        public dtBuffer buffer { get => pBuffer; set => pBuffer = value; }

        public string name = "";
        public Grid myGrid;
        public TextBlock lbPallet;
        public TextBlock lbPallet2;
        public TextBlock lbPallet3;
        public TextBlock lbPallet4;
        public TextBlock lbPallet5;

        public enum ReturnType
        {
            ReturnAreaMain = 13,
            ReturnAreaGate = 18,
            ReturnArea401 = 19
        }

        public enum RequestMethod
        {
            GET,
            POST,
            DELETE
        }

        public PalletShape(dtBuffer buffer, string name)
        {
            this.buffer = buffer;
            pallet = new dtPallet();
            this.name = name;
            Name = name;
            // Specific Size of Pallet
            Margin = new Thickness
                (
                (palletMargin / Global_Object.resolution) + 0,
                (palletMargin / Global_Object.resolution) + 0,
                (palletMargin / Global_Object.resolution) + 0,
                (palletMargin / Global_Object.resolution) + 0
                );
            // Style Pallet Border
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new Thickness(0.3);
            CornerRadius = new CornerRadius(0);

            //Set Grid 3 row 2 col
            myGrid = new Grid();
            ColumnDefinition col0 = new ColumnDefinition();
            ColumnDefinition col1 = new ColumnDefinition();
            RowDefinition row0 = new RowDefinition();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            myGrid.ColumnDefinitions.Add(col0);
            myGrid.ColumnDefinitions.Add(col1);
            myGrid.RowDefinitions.Add(row0);
            myGrid.RowDefinitions.Add(row1);
            myGrid.RowDefinitions.Add(row2);

            
            Border grid0 = new Border();
            grid0.BorderThickness = new Thickness(0,0,0.2,0.2);
            grid0.BorderBrush = new SolidColorBrush(Colors.Black);
            grid0.Padding = new Thickness(0);
            Grid.SetRow(grid0, 0);
            Grid.SetColumn(grid0, 0);

            Border grid1 = new Border();
            grid1.BorderThickness = new Thickness(0, 0, 0, 0.2);
            grid1.BorderBrush = new SolidColorBrush(Colors.Black);
            //grid1.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F8FFE5"));
            grid1.Padding = new Thickness(0);
            Grid.SetRow(grid1, 0);
            Grid.SetColumn(grid1, 1);

            Border grid2 = new Border();
            Grid.SetRow(grid2, 1);
            Grid.SetColumnSpan(grid2, 2);
            Grid.SetColumn(grid2, 0);

            Border grid3 = new Border();
            Grid.SetRow(grid3, 1);
            Grid.SetColumn(grid3, 1);

            Border grid4 = new Border();
            grid4.BorderThickness = new Thickness(0, 0.2, 0.2, 0);
            grid4.BorderBrush = new SolidColorBrush(Colors.Black);
            grid4.Padding = new Thickness(0);
            Grid.SetRow(grid4, 2);
            Grid.SetColumn(grid4, 0);

            Border grid5 = new Border();
            grid5.BorderThickness = new Thickness(0, 0.2, 0, 0);
            grid5.BorderBrush = new SolidColorBrush(Colors.Black);
            grid5.Padding = new Thickness(0);
            Grid.SetRow(grid5, 2);
            Grid.SetColumn(grid5, 1);

            myGrid.Children.Add(grid0);
            myGrid.Children.Add(grid1);
            myGrid.Children.Add(grid2);
            myGrid.Children.Add(grid3);
            myGrid.Children.Add(grid4);
            myGrid.Children.Add(grid5);
            //=============================
            StatusChanged(new dtPallet());

            lbPallet = new TextBlock();
            lbPallet.FontWeight = FontWeights.Bold;
            lbPallet.TextWrapping = TextWrapping.Wrap;
            lbPallet.TextAlignment = TextAlignment.Center;
            lbPallet.Width = grid0.Width;
            lbPallet.FontSize = 2;
            lbPallet.Text = this.name.Split('x')[1] + "-" + this.name.Split('x')[2];
            grid0.Child = lbPallet;

            lbPallet2 = new TextBlock();
            lbPallet2.FontWeight = FontWeights.Medium;
            lbPallet2.FontStyle = FontStyles.Italic;
            lbPallet2.TextWrapping = TextWrapping.Wrap;
            lbPallet2.TextAlignment = TextAlignment.Center;
            lbPallet2.Width = grid2.Width;
            lbPallet2.FontSize = 1.5;
            lbPallet2.Text = pallet.productDetailName == null ? "" : pallet.productDetailName.Substring(0, 8);
            grid2.Child = lbPallet2;

            lbPallet3 = new TextBlock();
            lbPallet3.FontWeight = FontWeights.Bold;
            lbPallet3.TextWrapping = TextWrapping.Wrap;
            lbPallet3.TextAlignment = TextAlignment.Center;
            lbPallet3.Width = this.Width;
            lbPallet3.FontSize = 2;
            if (pallet.dataPallet != null)
            {
                dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                string bayId = palletData.bayId;
                lbPallet3.Text = bayId;
            }
            else
            {
                lbPallet3.Text = "";
            }
            grid1.Child = lbPallet3;

            lbPallet4 = new TextBlock();
            lbPallet4.FontWeight = FontWeights.Bold;
            lbPallet4.TextWrapping = TextWrapping.Wrap;
            lbPallet4.TextAlignment = TextAlignment.Center;
            lbPallet4.Width = this.Width;
            lbPallet4.FontSize = 1.7;
            if (pallet.dataPallet != null)
            {
                dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                string x = palletData.line.x;
                lbPallet4.Text = x;
            }
            else
            {
                lbPallet4.Text = "";
            }
            grid4.Child = lbPallet4;


            lbPallet5 = new TextBlock();
            lbPallet5.FontWeight = FontWeights.Bold;
            lbPallet5.TextWrapping = TextWrapping.Wrap;
            lbPallet5.TextAlignment = TextAlignment.Center;
            lbPallet5.Width = this.Width;
            lbPallet5.FontSize = 1.7;
            if (pallet.dataPallet != null)
            {
                dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                string y = palletData.line.y;
                lbPallet5.Text = y;
            }
            else
            {
                lbPallet5.Text = "";
            }
            grid5.Child = lbPallet5;


            //System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            //rectangle.Width = 2;
            //rectangle.Height = 2;
            //rectangle.Fill = new SolidColorBrush(Colors.Red);

            //StackPanel stackPanel = new StackPanel();
            //stackPanel.Orientation = Orientation.Vertical;
            //stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            //stackPanel.VerticalAlignment = VerticalAlignment.Top;
            //stackPanel.Children.Add(lbPallet3);
            //stackPanel.Children.Add(lbPallet);
            //stackPanel.Children.Add(lbPallet2);

            Child = myGrid;


            ContextMenu = new ContextMenu();
            MenuItem putPallet = new MenuItem();
            putPallet.Header = "Put";
            putPallet.Click += PutPallet;

            MenuItem freePallet = new MenuItem();
            freePallet.Header = "Free";
            freePallet.Click += FreePallet;

            MenuItem lockPallet = new MenuItem();
            lockPallet.Header = "Lock";
            lockPallet.Click += LockPallet;

            MenuItem returnPallet = new MenuItem();
            returnPallet.Header = "Return Main";
            returnPallet.Click += ReturnPallet;

            MenuItem returnPalletGate = new MenuItem();
            returnPalletGate.Header = "Return Gate";
            returnPalletGate.Click += ReturnPalletGate_Click;

            MenuItem returnPallet401 = new MenuItem();
            returnPallet401.Header = "Return 401";
            returnPallet401.Click += ReturnPallet401_Click;

            ContextMenu.Items.Add(putPallet);
            ContextMenu.Items.Add(freePallet);
            ContextMenu.Items.Add(lockPallet);

            dynamic bufferData = JsonConvert.DeserializeObject(buffer.bufferData);
            if ((bufferData.returnGate != null) ? (bool)(bufferData.returnGate) : false)
            {
                ContextMenu.Items.Add(returnPalletGate);
            }

            if ((bufferData.returnMain != null) ? (bool)(bufferData.returnMain) : false)
            {
                ContextMenu.Items.Add(returnPallet);
            }

            if ((bufferData.return401 != null) ? (bool)(bufferData.return401) : false)
            {
                ContextMenu.Items.Add(returnPallet401);
            }

            // Event handler
            //MouseDown += PalletMouseDown;
            //MouseRightButtonDown += PalletShape_MouseRightButtonDown;
        }

        public string RequestDataAPI(string jsonData, string apiUrl, RequestMethod method)
        {
            string resultData = "";
            try
            {
                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create(@"http://" +
                    MapViewPallet.Properties.Settings.Default.serverIp + ":" +
                    MapViewPallet.Properties.Settings.Default.serverPort +
                    @"/robot/rest/" + apiUrl);
                request.Method = method.ToString();
                request.ContentType = @"application/json";

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
                    resultData = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
            return resultData;
        }

        private void ReturnPallet(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                if ((System.Windows.Forms.MessageBox.Show
                    (string.Format("Do you want to return the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes))
                {
                    int deviceIdToReturn = 0;
                    int bufferIdToReturn = 0;

                    List<dtPallet> palletsList = new List<dtPallet>();
                    palletsList = GetAllPallets(pallet.bufferId);

                    //Check if pallet is return able an then return it
                    if (CanPalletReturn(palletsList))
                    {
                        dynamic postApiBody = new JObject();
                        postApiBody.userName = MapViewPallet.Properties.Settings.Default["returnMainUser"].ToString();
                        postApiBody.userPassword = MapViewPallet.Properties.Settings.Default["returnMainPassword"].ToString();
                        string jsonData = JsonConvert.SerializeObject(postApiBody);
                        string contentJson = RequestDataAPI(jsonData, "user/getUserInfo", RequestMethod.POST);

                        dynamic response = JsonConvert.DeserializeObject(contentJson);
                        dtUser userInfo = response.ToObject<dtUser>();

                        //Check any device connect to account
                        foreach (dtUserDevice device in userInfo.userDevices)
                        {
                            deviceIdToReturn = device.deviceId;

                            postApiBody = new JObject();

                            postApiBody.timeWorkId = 0;
                            postApiBody.activeDate = pallet.activeDate;
                            postApiBody.deviceId = device.deviceId;
                            postApiBody.productId = pallet.productId;
                            postApiBody.productDetailId = pallet.productDetailId;
                            postApiBody.updUsrId = 1;
                            postApiBody.palletAmount = 1;

                            jsonData = JsonConvert.SerializeObject(postApiBody);
                            //contentJson = RequestDataAPI(jsonData, "plan/createPlanPallet", RequestMethod.POST);

                            //response = JsonConvert.DeserializeObject(contentJson);
                            //List<dtBuffer> listBuffer = response.ToObject<List<dtBuffer>>();
                            if (1 == 1)
                            {
                                if ((deviceIdToReturn > 0) && (bufferIdToReturn == 0))
                                {
                                    Console.WriteLine("Duoc phep Return!");
                                    dynamic postApiBody2 = new JObject();
                                    postApiBody2.userName = "WMS_Return";
                                    postApiBody2.length = 1;

                                    postApiBody2.bufferId = pallet.bufferId;
                                    postApiBody2.deviceId = pallet.deviceId;

                                    postApiBody2.bufferIdPut = bufferIdToReturn;
                                    postApiBody2.deviceIdPut = deviceIdToReturn;

                                    postApiBody2.row = pallet.row;
                                    postApiBody2.bay = pallet.bay;

                                    postApiBody2.productDetailId = pallet.productDetailId;
                                    postApiBody2.productDetailName = pallet.productDetailName;
                                    postApiBody2.productId = pallet.productId;
                                    postApiBody2.planId = pallet.planId;
                                    postApiBody2.activeDate = pallet.activeDate;
                                    postApiBody2.palletId = pallet.palletId;
                                    postApiBody2.typeReq = (int)ReturnType.ReturnAreaMain;

                                    string jsonData2 = JsonConvert.SerializeObject(postApiBody2);
                                    Console.WriteLine("Data request Return: " + jsonData2);
                                    BridgeClientRequest bridgeClientRequest = new BridgeClientRequest();
                                    bridgeClientRequest.PostCallAPI("http://" + MapViewPallet.Properties.Settings.Default.serverReturnIp + ":12000", jsonData2);

                                    //string preStatus = pallet.palletStatus;
                                    //pallet.palletStatus = "H";
                                    //string jsonDataPallet = JsonConvert.SerializeObject(pallet);
                                    //pallet.palletStatus = preStatus;

                                    //RequestDataAPI(jsonDataPallet, "pallet/updatePalletStatus", RequestMethod.POST);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void ReturnPallet401_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            if (System.Windows.Forms.MessageBox.Show
                        (
                        string.Format("Do you want to return the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                        )
            {
                int deviceIdToReturn = 0;
                int bufferIdToReturn = 0;

                List<dtPallet> palletsList = new List<dtPallet>();
                palletsList = GetAllPallets(pallet.bufferId);

                //Check if pallet is return able an then return it
                if (CanPalletReturn(palletsList))
                {
                    dynamic postApiBody = new JObject();
                    postApiBody.userName = MapViewPallet.Properties.Settings.Default["return401User"].ToString();
                    postApiBody.userPassword = MapViewPallet.Properties.Settings.Default["return401Password"].ToString();
                    string jsonData = JsonConvert.SerializeObject(postApiBody);
                    string contentJson = RequestDataAPI(jsonData, "user/getUserInfo", RequestMethod.POST);

                    dynamic response = JsonConvert.DeserializeObject(contentJson);
                    dtUser userInfo = response.ToObject<dtUser>();

                    //Check any device connect to account
                    foreach (dtUserDevice device in userInfo.userDevices)
                    {
                        deviceIdToReturn = device.deviceId;

                        postApiBody = new JObject();

                        postApiBody.timeWorkId = 0;
                        postApiBody.activeDate = pallet.activeDate;
                        postApiBody.deviceId = device.deviceId;
                        postApiBody.productId = pallet.productId;
                        postApiBody.productDetailId = pallet.productDetailId;
                        postApiBody.updUsrId = 1;
                        postApiBody.palletAmount = 1;

                        jsonData = JsonConvert.SerializeObject(postApiBody);
                        //contentJson = RequestDataAPI(jsonData, "plan/createPlanPallet", RequestMethod.POST);

                        //response = JsonConvert.DeserializeObject(contentJson);
                        //List<dtBuffer> listBuffer = response.ToObject<List<dtBuffer>>();
                        if (1 == 1)
                        {
                            if ((deviceIdToReturn > 0) && (bufferIdToReturn == 0))
                            {
                                Console.WriteLine("Duoc phep Return!");
                                dynamic postApiBody2 = new JObject();
                                postApiBody2.userName = "WMS_Return";

                                postApiBody2.bufferId = pallet.bufferId;
                                postApiBody2.deviceId = pallet.deviceId;

                                postApiBody2.bufferIdPut = bufferIdToReturn;
                                postApiBody2.deviceIdPut = deviceIdToReturn;

                                postApiBody2.row = pallet.row;
                                postApiBody2.bay = pallet.bay;

                                postApiBody2.productDetailId = pallet.productDetailId;
                                postApiBody2.productDetailName = pallet.productDetailName;
                                postApiBody2.productId = pallet.productId;
                                postApiBody2.planId = pallet.planId;
                                postApiBody2.activeDate = pallet.activeDate;
                                postApiBody2.palletId = pallet.palletId;
                                postApiBody2.typeReq = (int)ReturnType.ReturnArea401;
                                string jsonData2 = JsonConvert.SerializeObject(postApiBody2);
                                Console.WriteLine("Data request Return: " + jsonData2);
                                BridgeClientRequest bridgeClientRequest = new BridgeClientRequest();
                                bridgeClientRequest.PostCallAPI("http://" + MapViewPallet.Properties.Settings.Default.serverReturnIp + ":12000", jsonData2);

                                //string preStatus = pallet.palletStatus;
                                //pallet.palletStatus = "H";
                                //string jsonDataPallet = JsonConvert.SerializeObject(pallet);
                                //pallet.palletStatus = preStatus;
                                //RequestDataAPI(jsonDataPallet, "pallet/updatePalletStatus", RequestMethod.POST);
                            }
                        }
                    }
                }
            }
        }
        
        private void ReturnPalletGate_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                if (System.Windows.Forms.MessageBox.Show
                        (
                        string.Format("Do you want to return the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                        )
                {
                    int deviceIdToReturn = 0;
                    int bufferIdToReturn = 0;

                    List<dtPallet> palletsList = new List<dtPallet>();
                    palletsList = GetAllPallets(pallet.bufferId);

                    //Check if pallet is return able an then return it
                    if (CanPalletReturn(palletsList))
                    {
                        Console.WriteLine("Duoc phep Return!");
                        dynamic postApiBody2 = new JObject();
                        postApiBody2.userName = "WMS_Return";

                        postApiBody2.bufferId = pallet.bufferId;
                        postApiBody2.deviceId = pallet.deviceId;

                        postApiBody2.bufferIdPut = bufferIdToReturn;
                        postApiBody2.deviceIdPut = deviceIdToReturn;

                        postApiBody2.row = pallet.row;
                        postApiBody2.bay = pallet.bay;

                        postApiBody2.productDetailId = pallet.productDetailId;
                        postApiBody2.productDetailName = pallet.productDetailName;
                        postApiBody2.productId = pallet.productId;
                        postApiBody2.planId = pallet.planId;
                        postApiBody2.activeDate = pallet.activeDate;
                        postApiBody2.palletId = pallet.palletId;
                        postApiBody2.typeReq = (int)ReturnType.ReturnAreaGate;
                        string jsonData2 = JsonConvert.SerializeObject(postApiBody2);
                        BridgeClientRequest bridgeClientRequest = new BridgeClientRequest();
                        bridgeClientRequest.PostCallAPI("http://" + MapViewPallet.Properties.Settings.Default.serverReturnIp + ":12000", jsonData2);

                        //string preStatus = pallet.palletStatus;
                        //pallet.palletStatus = "H";
                        //string jsonDataPallet = JsonConvert.SerializeObject(pallet);
                        //pallet.palletStatus = preStatus;
                        //RequestDataAPI(jsonDataPallet, "pallet/updatePalletStatus", RequestMethod.POST);
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public List<dtPallet> GetAllPallets(int bufferId)
        {
            List<dtPallet> palletsList = new List<dtPallet>();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/getListPalletBufferId");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.bufferId = bufferId;
                string jsonData = JsonConvert.SerializeObject(postApiBody);
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
                    DataTable pallets = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in pallets.Rows)
                    {
                        dtPallet tempPallet = new dtPallet
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            palletId = int.Parse(dr["palletId"].ToString()),
                            deviceBufferId = int.Parse(dr["deviceBufferId"].ToString()),
                            bufferId = int.Parse(dr["bufferId"].ToString()),
                            planId = int.Parse(dr["planId"].ToString()),
                            row = int.Parse(dr["row"].ToString()),
                            bay = int.Parse(dr["bay"].ToString()),
                            dataPallet = dr["dataPallet"].ToString(),
                            palletStatus = dr["palletStatus"].ToString(),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            deviceName = dr["deviceName"].ToString(),
                            productId = int.Parse(dr["productId"].ToString()),
                            productName = dr["productName"].ToString(),
                            productDetailId = int.Parse(dr["productDetailId"].ToString()),
                            productDetailName = dr["productDetailName"].ToString(),
                        };
                        if (!ContainPallet(tempPallet, palletsList))
                        {
                            palletsList.Add(tempPallet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }

            return palletsList;
        }

        public bool CanPalletReturn(List<dtPallet> palletsList, string filter1 = "", string filter2 = "")
        {
            //Check if pallet is return able and then return it
            bool sendToReturn = true;
            if ((buffer.bufferReturn == false) &&
                (pallet.palletStatus == "W") &&
                (buffer.bufferName.Contains(filter1) || (buffer.bufferName.Contains(filter2)) || (buffer.bufferName.Contains("RETURN"))))
            {
                //Any pallet before needed send pallet need to be "Free"
                foreach (dtPallet palletItem in palletsList)
                {
                    if ((palletItem.bay == pallet.bay) && (palletItem.row < pallet.row) && (palletItem.palletStatus != "F"))
                    {
                        sendToReturn = false;
                        break;
                    }
                }
            }
            else
            {
                sendToReturn = false;
            }
            return sendToReturn;
        }

        public bool ContainPallet(dtPallet tempOpe, List<dtPallet> List)
        {
            foreach (dtPallet temp in List)
            {
                if (temp.palletId > 0)
                {
                    if (temp.palletId == tempOpe.palletId)
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

        private void FreePallet(object sender, RoutedEventArgs e)
        {
            if ((System.Windows.Forms.MessageBox.Show
                        (string.Format("Do you want to free the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes))
            {
                string preStatus = pallet.palletStatus;
                pallet.palletStatus = "F";
                string jsonData = JsonConvert.SerializeObject(pallet);
                pallet.palletStatus = preStatus;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
                request.Method = "POST";
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
                }
            }
        }

        private void LockPallet(object sender, RoutedEventArgs e)
        {
            if ((System.Windows.Forms.MessageBox.Show
                        (string.Format("Do you want to lock the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes))
            {
                string preStatus = pallet.palletStatus;
                pallet.palletStatus = "L";
                string jsonData = JsonConvert.SerializeObject(pallet);
                pallet.palletStatus = preStatus;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
                request.Method = "POST";
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
                }
            }
        }

        private void PutPallet(object sender, RoutedEventArgs e)
        {
            if ((System.Windows.Forms.MessageBox.Show
                        (string.Format("Do you want to put the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes))
            {
                string preStatus = pallet.palletStatus;
                pallet.palletStatus = "W";
                string jsonData = JsonConvert.SerializeObject(pallet);
                pallet.palletStatus = preStatus;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
                request.Method = "POST";
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
                }
            }
        }
        
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
                                   if (lbPallet3 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string bayId = palletData.bayId;
                                       lbPallet3.Text = bayId;
                                   }
                                   else
                                   {
                                       lbPallet3.Text = "";
                                   }
                                   if (lbPallet4 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string x = palletData.line.x;
                                       lbPallet4.Text = x;
                                   }
                                   else
                                   {
                                       lbPallet4.Text = "";
                                   }
                                   if (lbPallet5 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string y = palletData.line.y;
                                       lbPallet5.Text = y;
                                   }
                                   else
                                   {
                                       lbPallet5.Text = "";
                                   }
                                   if (lbPallet2 != null && pallet.productDetailName != null)
                                   {
                                       if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                       {
                                           lbPallet2.Text = pallet.productDetailName==null?"": pallet.productDetailName.Substring(0,8);
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
                                   if (lbPallet3 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string bayId = palletData.bayId;
                                       lbPallet3.Text = bayId;
                                   }
                                   else
                                   {
                                       lbPallet3.Text = "";
                                   }
                                   if (lbPallet4 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string x = palletData.line.x;
                                       lbPallet4.Text = x;
                                   }
                                   else
                                   {
                                       lbPallet4.Text = "";
                                   }
                                   if (lbPallet5 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string y = palletData.line.y;
                                       lbPallet5.Text = y;
                                   }
                                   else
                                   {
                                       lbPallet5.Text = "";
                                   }
                                   if (lbPallet2 != null && pallet.productDetailName != null)
                                   {
                                       if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                       {
                                           lbPallet2.Text = pallet.productDetailName == null ? "" : pallet.productDetailName.Substring(0, 8);
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
                                   if (lbPallet3 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string bayId = palletData.bayId;
                                       lbPallet3.Text = bayId;
                                   }
                                   else
                                   {
                                       lbPallet3.Text = "";
                                   }
                                   if (lbPallet4 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string x = palletData.line.x;
                                       lbPallet4.Text = x;
                                   }
                                   else
                                   {
                                       lbPallet4.Text = "";
                                   }
                                   if (lbPallet5 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string y = palletData.line.y;
                                       lbPallet5.Text = y;
                                   }
                                   else
                                   {
                                       lbPallet5.Text = "";
                                   }
                                   if (lbPallet2 != null && pallet.productDetailName != null)
                                   {
                                       if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                       {
                                           lbPallet2.Text = pallet.productDetailName == null ? "" : pallet.productDetailName.Substring(0, 8);
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
                                   if (lbPallet3 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string bayId = palletData.bayId;
                                       lbPallet3.Text = bayId;
                                   }
                                   else
                                   {
                                       lbPallet3.Text = "";
                                   }
                                   if (lbPallet4 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string x = palletData.line.x;
                                       lbPallet4.Text = x;
                                   }
                                   else
                                   {
                                       lbPallet4.Text = "";
                                   }
                                   if (lbPallet5 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string y = palletData.line.y;
                                       lbPallet5.Text = y;
                                   }
                                   else
                                   {
                                       lbPallet5.Text = "";
                                   }
                                   if (lbPallet2 != null && pallet.productDetailName != null)
                                   {
                                       if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                       {
                                           lbPallet2.Text = pallet.productDetailName == null ? "" : pallet.productDetailName.Substring(0, 8);
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
                                   if (lbPallet3 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string bayId = palletData.bayId;
                                       lbPallet3.Text = bayId;
                                   }
                                   else
                                   {
                                       lbPallet3.Text = "";
                                   }
                                   if (lbPallet4 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string x = palletData.line.x;
                                       lbPallet4.Text = x;
                                   }
                                   else
                                   {
                                       lbPallet4.Text = "";
                                   }
                                   if (lbPallet5 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string y = palletData.line.y;
                                       lbPallet5.Text = y;
                                   }
                                   else
                                   {
                                       lbPallet5.Text = "";
                                   }
                                   if (lbPallet2 != null && pallet.productDetailName != null)
                                   {
                                       if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                       {
                                           lbPallet2.Text = pallet.productDetailName == null ? "" : pallet.productDetailName.Substring(0, 8);
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
                           case "R":
                           {
                               if (replaceProductDetailName)
                               {
                                   if (lbPallet3 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string bayId = palletData.bayId;
                                       lbPallet3.Text = bayId;
                                   }
                                   else
                                   {
                                       lbPallet3.Text = "";
                                   }
                                   if (lbPallet4 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string x = palletData.line.x;
                                       lbPallet4.Text = x;
                                   }
                                   else
                                   {
                                       lbPallet4.Text = "";
                                   }
                                   if (lbPallet5 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string y = palletData.line.y;
                                       lbPallet5.Text = y;
                                   }
                                   else
                                   {
                                       lbPallet5.Text = "";
                                   }
                                   if (lbPallet2 != null && pallet.productDetailName != null)
                                   {
                                       if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                       {
                                           lbPallet2.Text = pallet.productDetailName == null ? "" : pallet.productDetailName.Substring(0, 8);
                                       }
                                       else
                                       {
                                           lbPallet2.Text = "";
                                       }
                                   }
                               }
                               if (replaceStatus)
                               {
                                   Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF00"));
                               }
                               break;
                           }
                           default:
                           {
                               if (replaceProductDetailName)
                               {
                                   if (lbPallet3 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string bayId = palletData.bayId;
                                       lbPallet3.Text = bayId;
                                   }
                                   else
                                   {
                                       lbPallet3.Text = "";
                                   }
                                   if (lbPallet4 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string x = palletData.line.x;
                                       lbPallet4.Text = x;
                                   }
                                   else
                                   {
                                       lbPallet4.Text = "";
                                   }
                                   if (lbPallet5 != null && pallet.dataPallet != null)
                                   {
                                       dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                                       string y = palletData.line.y;
                                       lbPallet5.Text = y;
                                   }
                                   else
                                   {
                                       lbPallet5.Text = "";
                                   }
                                   if (lbPallet2 != null && pallet.productDetailName != null)
                                   {
                                       if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                       {
                                           lbPallet2.Text = pallet.productDetailName == null ? "" : pallet.productDetailName.Substring(0, 8);
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
        
    }
}