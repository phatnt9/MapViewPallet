using MapViewPallet.Shape;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for StationEditor.xaml
    /// </summary>
    /// 
    public partial class StationEditor : Window
    {
        StationShape stationShape;
        StationEditorModel stationEditorModel;
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StationEditor()
        {
            InitializeComponent();
        }

        public StationEditor(StationShape stationShape, string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            this.stationShape = stationShape;
            Loaded += StationEditor_Loaded;
            stationEditorModel = new StationEditorModel(this);
            DataContext = stationEditorModel;
            PalletsListDg.SelectedCellsChanged += PalletsListDg_SelectedCellsChanged;
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

        private void PalletsListDg_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (PalletsListDg.SelectedItem != null)
                {
                    dtPallet pallet = PalletsListDg.SelectedItem as dtPallet;
                    dynamic palletData = JsonConvert.DeserializeObject(pallet.dataPallet);
                    palletX.Text = (palletData.line.x != null) ? (((double)palletData.line.x).ToString()) : "0";
                    palletY.Text = (palletData.line.y != null) ? (((double)palletData.line.y).ToString()) : "0";
                    palletA.Text = (palletData.line.angle != null) ? (((double)palletData.line.angle).ToString()) : "0";
                    //palletR.Text = (palletData != null) ? (((double)palletData.pallet.row).ToString()) : "0";
                    palletR.Text = pallet.row.ToString();
                    //{"line":{"x":-4.19,"y":-13.69,"angle":0.0},"pallet":{"row":0,"bay":0,"dir_main":1,"dir_sub":1,"dir_out":2,"line_ord":0,"hasSubLine":"yes"}}
                    //palletR.Text = (palletData.pallet.row != null) ? (((int)palletData.pallet.row).ToString()) : pallet.row.ToString();
                    palletB.Text = pallet.bay.ToString();
                    //palletB.Text = (palletData.pallet.bay != null) ? (((int)palletData.pallet.bay).ToString()) : pallet.bay.ToString();
                    palletRowlb.Content = pallet.row;
                    palletBaylb.Content = pallet.bay;
                    palletD_Main.Text = (palletData.pallet.dir_main != null) ? ((palletData.pallet.dir_main).ToString()) : "0";
                    palletD_Sub.Text = (palletData.pallet.dir_sub != null) ? ((palletData.pallet.dir_sub).ToString()) : "0";
                    palletD_Out.Text = (palletData.pallet.dir_out != null) ? ((palletData.pallet.dir_out).ToString()) : "0";
                    palletL_Ord.Text = (palletData.pallet.line_ord != null) ? ((palletData.pallet.line_ord).ToString()) : "0";
                    palletHasSubLine.Text = (palletData.pallet.hasSubLine != null) ? ((palletData.pallet.hasSubLine).ToString()) : "no";
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }

        }

        private void StationEditor_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bufferNamelb.Content = stationShape.props.bufferDb.bufferName;
                bufferMaxRowlb.Content = stationShape.props.bufferDb.maxRow;
                bufferMaxBaylb.Content = stationShape.props.bufferDb.maxBay;
                //bufferTypeNamelb.Content = (stationShape.props.bufferDb.bufferReturn == true) ? "StationEditor_BufferType_Return" : "StationEditor_BufferType_Buffer";
                bufferTypeNamelb.SetResourceReference(System.Windows.Controls.Label.ContentProperty, (stationShape.props.bufferDb.bufferReturn == true) ? "StationEditor_BufferType_Return" : "StationEditor_BufferType_Buffer");

                dynamic buffercheckin = JsonConvert.DeserializeObject(stationShape.props.bufferDb.bufferCheckIn);
                bufferX.Text = (buffercheckin.checkin != null) ? (((double)buffercheckin.checkin.x).ToString()) : "0";
                bufferY.Text = (buffercheckin.checkin != null) ? (((double)buffercheckin.checkin.y).ToString()) : "0";
                bufferA.Text = (buffercheckin.checkin != null) ? (((double)buffercheckin.checkin.angle).ToString()) : "0";

                bufferHeadPointX.Text = (buffercheckin.headpoint != null) ? (((double)buffercheckin.headpoint.x).ToString()) : "0";
                bufferHeadPointY.Text = (buffercheckin.headpoint != null) ? (((double)buffercheckin.headpoint.y).ToString()) : "0";
                bufferHeadPointA.Text = (buffercheckin.headpoint != null) ? (((double)buffercheckin.headpoint.angle).ToString()) : "0";

                dynamic bufferdata = JsonConvert.DeserializeObject(stationShape.props.bufferDb.bufferData);
                bufferPosX.Text = (bufferdata != null) ? (((double)bufferdata.x).ToString()) : "0";
                bufferPosY.Text = (bufferdata != null) ? (((double)bufferdata.y).ToString()) : "0";
                bufferPosA.Text = (bufferdata != null) ? (((double)bufferdata.angle).ToString()) : "0";
                bufferArr.Text = (bufferdata != null) ? ((bufferdata.arrange).ToString()) : "bigEndian";
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    stationEditorModel.ReloadListPallets(this.stationShape.props.bufferDb.bufferId);
                }));
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }

        }

        private void Btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Refresh_Pallet_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                stationEditorModel.ReloadListPallets(this.stationShape.props.bufferDb.bufferId);
            }));
        }

        private void Btn_SetBufferData_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                dtBuffer buffer = stationShape.props.bufferDb;
                List<dtBuffer> buffers = new List<dtBuffer>();

                dynamic postApiBody = new JObject();
                dynamic checkin = new JObject();
                dynamic headpoint = new JObject();

                checkin.x = Math.Round((double.Parse((bufferX.Text != "") ? bufferX.Text : "0")), 2);
                checkin.y = Math.Round((double.Parse((bufferY.Text != "") ? bufferY.Text : "0")), 2);
                checkin.angle = Math.Round((double.Parse((bufferA.Text != "") ? bufferA.Text : "0")), 2);

                headpoint.x = Math.Round((double.Parse((bufferHeadPointX.Text != "") ? bufferHeadPointX.Text : "0")), 2);
                headpoint.y = Math.Round((double.Parse((bufferHeadPointY.Text != "") ? bufferHeadPointY.Text : "0")), 2);
                headpoint.angle = Math.Round((double.Parse((bufferHeadPointA.Text != "") ? bufferHeadPointA.Text : "0")), 2);

                postApiBody.checkin = checkin;
                postApiBody.headpoint = headpoint;
                string jsonBufferData = JsonConvert.SerializeObject(postApiBody);
                buffer.bufferCheckIn = jsonBufferData;

                buffers.Add(buffer);

                if (buffers.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string jsonData = JsonConvert.SerializeObject(buffers);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "buffer/updateListBuffer");
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
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    int result = 0;
                    int.TryParse(reader.ReadToEnd(), out result);
                    if (result == 1)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -2)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Buffers Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Btn_SetPalletDataPallet_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                //dtPallet temp = (sender as System.Windows.Controls.DataGrid).SelectedItem as dtPallet;
                //temp.updUsrId = Global_Object.userLogin;
                //string palletCellEdit = ((e.EditingElement as System.Windows.Controls.TextBox).Text.Trim());
                List<dtPallet> pallets = new List<dtPallet>();
                //switch (e.Column.DisplayIndex)
                //{
                //    case 4:
                //        {
                //            temp.dataPallet = palletCellEdit;
                //            break;
                //        }
                //}

                dtPallet pallet = PalletsListDg.SelectedItem as dtPallet;

                dynamic palletData = new JObject();
                dynamic palletLine = new JObject();
                dynamic palletPallet = new JObject();

                palletLine.x = double.Parse((palletX.Text != "") ? palletX.Text : "0");
                palletLine.y = double.Parse((palletY.Text != "") ? palletY.Text : "0");
                palletLine.angle = double.Parse((palletA.Text != "") ? palletA.Text : "0");

                palletPallet.row = int.Parse((palletR.Text != "") ? palletR.Text : "0");
                palletPallet.bay = int.Parse((palletB.Text != "") ? palletB.Text : "0");
                palletPallet.dir_main = int.Parse((palletD_Main.Text != "") ? palletD_Main.Text : "0");
                palletPallet.dir_sub = int.Parse((palletD_Sub.Text != "") ? palletD_Sub.Text : "0");
                palletPallet.dir_out = int.Parse((palletD_Out.Text != "") ? palletD_Out.Text : "0");
                palletPallet.line_ord = int.Parse((palletL_Ord.Text != "") ? palletL_Ord.Text : "0");
                palletPallet.hasSubLine = (palletHasSubLine.Text != "") ? palletHasSubLine.Text : "no";

                palletData.line = palletLine;
                palletData.pallet = palletPallet;

                string jsonBufferData = JsonConvert.SerializeObject(palletData);
                pallet.dataPallet = jsonBufferData;


                pallets.Add(pallet);

                if (pallets.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string jsonData = JsonConvert.SerializeObject(pallets);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updateListPalletData");
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
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    int result = 0;
                    int.TryParse(reader.ReadToEnd(), out result);
                    if (result == 1)
                    {
                        //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -2)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Pallets Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void myPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }





        private void Btn_SetBufferPosData_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                dtBuffer buffer = stationShape.props.bufferDb;
                List<dtBuffer> buffers = new List<dtBuffer>();

                dynamic postApiBody = new JObject();
                postApiBody.x = Math.Round((double.Parse((bufferPosX.Text != "") ? bufferPosX.Text : "0")), 2);
                postApiBody.y = Math.Round((double.Parse((bufferPosY.Text != "") ? bufferPosY.Text : "0")), 2);
                postApiBody.angle = Math.Round((double.Parse((bufferPosA.Text != "") ? bufferPosA.Text : "0")), 2);
                postApiBody.arrange = (bufferArr.Text != "") ? bufferArr.Text : "bigEndian";
                string jsonBufferData = JsonConvert.SerializeObject(postApiBody);
                buffer.bufferData = jsonBufferData;

                buffers.Add(buffer);

                if (buffers.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageNoDataSave), Global_Object.messageTitileWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string jsonData = JsonConvert.SerializeObject(buffers);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "buffer/updateListBuffer");
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
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    int result = 0;
                    int.TryParse(reader.ReadToEnd(), out result);
                    if (result == 1)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -2)
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Buffers Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                foreach (dtPallet pallet in stationEditorModel.palletsList)
                {
                    pallet.palletStatus = "F";
                    string jsonData = JsonConvert.SerializeObject(pallet);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
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
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        int result = 0;
                        int.TryParse(reader.ReadToEnd(), out result);
                        if (result == 1)
                        {
                            //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (result == -2)
                        {
                            // System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Pallets Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    stationEditorModel.ReloadListPallets(this.stationShape.props.bufferDb.bufferId);
                }));
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                dtPallet pallet = (sender as System.Windows.Controls.Button).DataContext as dtPallet;
                //if (pallet.palletStatus == "F")
                //{
                //    return;
                //}
                pallet.palletStatus = "W";
                string jsonData = JsonConvert.SerializeObject(pallet);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
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
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    int result = 0;
                    int.TryParse(reader.ReadToEnd(), out result);
                    if (result == 1)
                    {
                        //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -2)
                    {
                        // System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Pallets Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    stationEditorModel.ReloadListPallets(this.stationShape.props.bufferDb.bufferId);
                }));
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                dtPallet pallet = (sender as System.Windows.Controls.Button).DataContext as dtPallet;
                pallet.palletStatus = "F";
                string jsonData = JsonConvert.SerializeObject(pallet);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
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
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    int result = 0;
                    int.TryParse(reader.ReadToEnd(), out result);
                    if (result == 1)
                    {
                        //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -2)
                    {
                        // System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Pallets Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    stationEditorModel.ReloadListPallets(this.stationShape.props.bufferDb.bufferId);
                }));
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                dtPallet pallet = (sender as System.Windows.Controls.Button).DataContext as dtPallet;
                pallet.palletStatus = "L";
                string jsonData = JsonConvert.SerializeObject(pallet);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
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
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    int result = 0;
                    int.TryParse(reader.ReadToEnd(), out result);
                    if (result == 1)
                    {
                        //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == -2)
                    {
                        // System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Pallets Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    stationEditorModel.ReloadListPallets(this.stationShape.props.bufferDb.bufferId);
                }));
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                dtPallet pallet = (sender as System.Windows.Controls.Button).DataContext as dtPallet;
                if ((stationShape.props.bufferDb.bufferReturn == false) && (pallet.palletStatus == "W"))
                {
                    bool sendToReturn = true;
                    //Any pallet before needed send pallet need to be "Free"
                    foreach (dtPallet palletItem in stationShape.props.bufferDb.pallets)
                    {
                        if ((palletItem.bay == pallet.bay) && (palletItem.row < pallet.row))
                        {
                            if (palletItem.palletStatus != "F")
                            {
                                sendToReturn = false;
                                Console.WriteLine("Khong cho phep Return!");
                                break;
                            }
                        }
                    }
                    if (sendToReturn)
                    {
                        Console.WriteLine("Duoc phep Return!");
                        dynamic postApiBody = new JObject();
                        postApiBody.userName = "WMS_Return";
                        postApiBody.bufferId = pallet.bufferId;
                        postApiBody.productDetailId = pallet.productDetailId;
                        postApiBody.productDetailName = pallet.productDetailName;
                        postApiBody.productDetailName = pallet.productDetailName;
                        postApiBody.productId = pallet.productId;
                        //postApiBody.planId = pallet.planId;
                        postApiBody.deviceId = pallet.deviceId;
                        postApiBody.typeReq = 13;
                        string jsonData = JsonConvert.SerializeObject(postApiBody);
                        BridgeClientRequest bridgeClientRequest = new BridgeClientRequest();
                        bridgeClientRequest.PostCallAPI("http://" + Properties.Settings.Default.serverReturnIp + ":12000", jsonData);

                        pallet.palletStatus = "R";
                        string jsonDataPallet = JsonConvert.SerializeObject(pallet);

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "pallet/updatePalletStatus");
                        request.Method = "POST";
                        request.ContentType = "application/json";

                        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                        Byte[] byteArray = encoding.GetBytes(jsonDataPallet);
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
                                //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveSucced), Global_Object.messageTitileInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (result == -2)
                            {
                                // System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageDuplicated, "Pallets Name"), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                //System.Windows.Forms.MessageBox.Show(String.Format(Global_Object.messageSaveFail), Global_Object.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        Dispatcher.BeginInvoke(new ThreadStart(() =>
                        {
                            stationEditorModel.ReloadListPallets(this.stationShape.props.bufferDb.bufferId);
                        }));
                    }
                }
                else
                {
                    Console.WriteLine("Khong cho phep Return!");
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        
    }
}
