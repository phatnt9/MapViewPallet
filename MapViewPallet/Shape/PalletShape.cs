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
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static MapViewPallet.Global_Object;

namespace MapViewPallet.Shape
{
    public class PalletShape : Border
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private double palletMargin = 0.1; //metters

        private dtPallet pPallet;
        public dtPallet pallet { get => pPallet; set => pPallet = value; }


        private dtBuffer pBuffer;
        public dtBuffer buffer { get => pBuffer; set => pBuffer = value; }

        public string name = "";
        public TextBlock lbPallet;
        public TextBlock lbPallet2;

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

        public PalletShape(dtBuffer buffer,string name)
        {
            this.buffer = buffer;
            pallet = new dtPallet();
            this.name = name;
            Name = name;
            // Specific Size of Pallet
            Margin = new Thickness
                (
                (palletMargin / Global_Object.resolution)+0,
                (palletMargin / Global_Object.resolution)+0,
                (palletMargin / Global_Object.resolution)+0,
                (palletMargin / Global_Object.resolution)+0
                );
            //Padding = new Thickness(5,0,5,0);
            // Style Pallet Border
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new Thickness(0.3);
            CornerRadius = new CornerRadius(1.3);
            // Background for Pallet
            //Bitmap bmp = (Bitmap)MapViewPallet.Properties.Resources.ResourceManager.GetObject(typePallet);
            //ImageBrush img = new ImageBrush();
            //img.ImageSource = ImageSourceForBitmap(bmp);
            //Background = img;
            //=============================
            StatusChanged(new dtPallet());

            lbPallet = new TextBlock();
            lbPallet.TextWrapping = TextWrapping.Wrap;
            lbPallet.Width = this.Width;
            lbPallet.FontSize = 2;
            lbPallet.Text = this.name.Split('x')[1] +"-"+ this.name.Split('x')[2];

            lbPallet2 = new TextBlock();
            lbPallet2.TextWrapping = TextWrapping.Wrap;
            lbPallet2.Width = this.Width;
            lbPallet2.FontSize = 1.5;
            lbPallet2.Text = pallet.productDetailName;

            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            rectangle.Width = 2;
            rectangle.Height = 2;
            rectangle.Fill = new SolidColorBrush(Colors.Red);


            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;
            stackPanel.Children.Add(lbPallet);
            stackPanel.Children.Add(lbPallet2);

            Child = stackPanel;


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
            returnPallet.Header = "Return";
            returnPallet.Click += ReturnPallet;

            MenuItem returnPalletGate = new MenuItem();
            returnPalletGate.Header = "Return Gate";
            returnPalletGate.Click += ReturnPalletGate_Click; ;

            MenuItem returnPallet401 = new MenuItem();
            returnPallet401.Header = "Return 401";
            returnPallet401.Click += ReturnPallet401_Click; ;


            ContextMenu.Items.Add(putPallet);
            ContextMenu.Items.Add(freePallet);
            ContextMenu.Items.Add(lockPallet);
            ContextMenu.Items.Add(returnPallet);
            ContextMenu.Items.Add(returnPalletGate);
            ContextMenu.Items.Add(returnPallet401);

            // Event handler
            //MouseDown += PalletMouseDown;
            //MouseRightButtonDown += PalletShape_MouseRightButtonDown;

        }

        public string RequestDataAPI(string jsonData, string apiUrl,RequestMethod method)
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
                if (System.Windows.Forms.MessageBox.Show
                        (
                        string.Format("Do you want to return the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                        )
                {
                    List<dtPallet> palletsList = new List<dtPallet>();
                    palletsList = GetAllPallets(pallet.bufferId);


                    //Check if pallet is return able an then return it
                    if (CanPalletReturn(palletsList))
                    {
                        Console.WriteLine("Duoc phep Return!");
                        dynamic postApiBody2 = new JObject();
                        postApiBody2.userName = "WMS_Return";
                        postApiBody2.bufferId = pallet.bufferId;
                        postApiBody2.productDetailId = pallet.productDetailId;
                        postApiBody2.productDetailName = pallet.productDetailName;
                        postApiBody2.productId = pallet.productId;
                        //postApiBody2.planId = pallet.planId;
                        postApiBody2.deviceId = pallet.deviceId;
                        postApiBody2.typeReq = (int)ReturnType.ReturnAreaMain;
                        string jsonData2 = JsonConvert.SerializeObject(postApiBody2);
                        BridgeClientRequest bridgeClientRequest = new BridgeClientRequest();
                        bridgeClientRequest.PostCallAPI("http://" + MapViewPallet.Properties.Settings.Default.serverReturnIp + ":12000", jsonData2);

                        string preStatus = pallet.palletStatus;
                        pallet.palletStatus = "R";
                        string jsonDataPallet = JsonConvert.SerializeObject(pallet);
                        pallet.palletStatus = preStatus;

                        HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
                        request2.Method = "POST";
                        request2.ContentType = "application/json";

                        System.Text.UTF8Encoding encoding2 = new System.Text.UTF8Encoding();
                        Byte[] byteArray2 = encoding2.GetBytes(jsonDataPallet);
                        request2.ContentLength = byteArray2.Length;
                        using (Stream dataStream = request2.GetRequestStream())
                        {
                            dataStream.Write(byteArray2, 0, byteArray2.Length);
                            dataStream.Flush();
                        }

                        HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse;
                        using (Stream responseStream = response2.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            int result = 0;
                            int.TryParse(reader.ReadToEnd(), out result);
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
            if (System.Windows.Forms.MessageBox.Show
                        (
                        string.Format("Do you want to return the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                        )
            {
                int deviceIdToReturn = 0;
                int bufferIdToReturn = 0;

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
                    postApiBody.deviceId = device.deviceId;
                    jsonData = JsonConvert.SerializeObject(postApiBody);
                    contentJson = RequestDataAPI(jsonData, "buffer/getListDeviceBufferByDeviceId", RequestMethod.POST);

                    response = JsonConvert.DeserializeObject(contentJson);
                    List<dtBuffer> listBuffer = response.ToObject<List<dtBuffer>>();

                    //Check number of free pallet
                    foreach (dtBuffer buffer in listBuffer)
                    {
                        postApiBody = new JObject();
                        postApiBody.bufferId = buffer.bufferId;
                        postApiBody.palletStatus = "F";
                        jsonData = JsonConvert.SerializeObject(postApiBody);
                        contentJson = RequestDataAPI(jsonData, "pallet/getListPalletBufferId", RequestMethod.POST);

                        response = JsonConvert.DeserializeObject(contentJson);
                        List<dtPallet> listPallet = response.ToObject<List<dtPallet>>();
                        if (listPallet.Count > 0)
                        {
                            bufferIdToReturn = buffer.bufferId;
                            break;
                        }
                    }
                }


                if ((deviceIdToReturn > 0) && (bufferIdToReturn > 0))
                {
                    List<dtPallet> palletsList = new List<dtPallet>();
                    palletsList = GetAllPallets(pallet.bufferId);


                    //Check if pallet is return able an then return it
                    if (CanPalletReturn(palletsList,"CAP","BOTTLE"))
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
                        //postApiBody2.planId = pallet.planId;
                        postApiBody2.typeReq = (int)ReturnType.ReturnAreaMain;
                        string jsonData2 = JsonConvert.SerializeObject(postApiBody2);
                        BridgeClientRequest bridgeClientRequest = new BridgeClientRequest();
                        bridgeClientRequest.PostCallAPI("http://" + MapViewPallet.Properties.Settings.Default.serverReturnIp + ":12000", jsonData2);

                        string preStatus = pallet.palletStatus;
                        pallet.palletStatus = "R";
                        string jsonDataPallet = JsonConvert.SerializeObject(pallet);
                        pallet.palletStatus = preStatus;

                        HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
                        request2.Method = "POST";
                        request2.ContentType = "application/json";

                        System.Text.UTF8Encoding encoding2 = new System.Text.UTF8Encoding();
                        Byte[] byteArray2 = encoding2.GetBytes(jsonDataPallet);
                        request2.ContentLength = byteArray2.Length;
                        using (Stream dataStream = request2.GetRequestStream())
                        {
                            dataStream.Write(byteArray2, 0, byteArray2.Length);
                            dataStream.Flush();
                        }

                        HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse;
                        using (Stream responseStream = response2.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            int result = 0;
                            int.TryParse(reader.ReadToEnd(), out result);
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
                    List<dtPallet> palletsList = new List<dtPallet>();
                    palletsList = GetAllPallets(pallet.bufferId);


                    //Check if pallet is return able an then return it
                    if (CanPalletReturn(palletsList))
                    {
                        Console.WriteLine("Duoc phep Return!");
                        dynamic postApiBody2 = new JObject();
                        postApiBody2.userName = "WMS_Return";
                        postApiBody2.bufferId = pallet.bufferId;
                        postApiBody2.productDetailId = pallet.productDetailId;
                        postApiBody2.productDetailName = pallet.productDetailName;
                        postApiBody2.productId = pallet.productId;
                        //postApiBody2.planId = pallet.planId;
                        postApiBody2.deviceId = pallet.deviceId;
                        postApiBody2.typeReq = (int)ReturnType.ReturnAreaGate;
                        string jsonData2 = JsonConvert.SerializeObject(postApiBody2);
                        BridgeClientRequest bridgeClientRequest = new BridgeClientRequest();
                        bridgeClientRequest.PostCallAPI("http://" + MapViewPallet.Properties.Settings.Default.serverReturnIp + ":12000", jsonData2);

                        string preStatus = pallet.palletStatus;
                        pallet.palletStatus = "R";
                        string jsonDataPallet = JsonConvert.SerializeObject(pallet);
                        pallet.palletStatus = preStatus;

                        HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(@"http://" + MapViewPallet.Properties.Settings.Default.serverIp + ":" + MapViewPallet.Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
                        request2.Method = "POST";
                        request2.ContentType = "application/json";

                        System.Text.UTF8Encoding encoding2 = new System.Text.UTF8Encoding();
                        Byte[] byteArray2 = encoding2.GetBytes(jsonDataPallet);
                        request2.ContentLength = byteArray2.Length;
                        using (Stream dataStream = request2.GetRequestStream())
                        {
                            dataStream.Write(byteArray2, 0, byteArray2.Length);
                            dataStream.Flush();
                        }

                        HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse;
                        using (Stream responseStream = response2.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            int result = 0;
                            int.TryParse(reader.ReadToEnd(), out result);
                        }
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

        public bool CanPalletReturn(List<dtPallet> palletsList, string filter1="", string filter2 = "", string filter3 = "")
        {
            //Check if pallet is return able and then return it
            bool sendToReturn = true;
            if ((buffer.bufferReturn == false) && 
                (pallet.palletStatus == "W") && 
                (buffer.bufferName.Contains(filter1)||(buffer.bufferName.Contains(filter2)) ||(buffer.bufferName.Contains(filter3))))
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
            if (System.Windows.Forms.MessageBox.Show
                        (string.Format("Do you want to free the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
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
            if (System.Windows.Forms.MessageBox.Show
                        (string.Format("Do you want to lock the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
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
            if (System.Windows.Forms.MessageBox.Show
                        (string.Format("Do you want to put the selected {0}?", "Pallet"),
                        Global_Object.messageTitileWarning, System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
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

        private void PalletShape_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(pallet);
            MessageBox.Show("" + name);
        }
        
        public void StatusChanged(dtPallet pPallet)
        {
            if (this.pallet != null)
            {
                bool replaceStatus = (this.pallet.palletStatus != pPallet.palletStatus) ? true : false;
                //bool replaceStatus = true;
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
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
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
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
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
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
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
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
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
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
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
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
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
                                       if (lbPallet2 != null && pallet.productDetailName != null)
                                       {
                                           if ((pallet.productDetailName.ToString().Trim() != "") && (pallet.palletStatus != "F"))
                                           {
                                               lbPallet2.Text = pallet.productDetailName;
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

        private void PalletMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(""+name);
        }
        
        private ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { }
        }

        private void ChangeToolTipContent(object sender, ToolTipEventArgs e)
        {
            ToolTip = "Name: " + 
                "\n Start: " +
                " \n End: " +
                " \n Rotate: ";
        }
    }
}
