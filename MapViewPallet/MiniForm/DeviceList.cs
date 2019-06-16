using Newtonsoft.Json;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MapViewPallet.MiniForm
{
    public class DeviceList
    {
        public List<dtDevice> listDevices;
        public BackgroundWorker workerLoadDevice;
        PlanControl planControl;

        public DeviceList()
        {
            listDevices = new List<dtDevice>();
        }

        public bool GetDevicesList(PlanControl planControl)
        {
            this.planControl = planControl;
            if (!Global_Object.ServerAlive())
            {
                return false;
            }
            try
            {
                workerLoadDevice = new BackgroundWorker();
                workerLoadDevice.WorkerSupportsCancellation = true;
                workerLoadDevice.WorkerReportsProgress = true;
                workerLoadDevice.DoWork += WorkerLoadDevice_DoWork;
                workerLoadDevice.ProgressChanged += WorkerLoadDevice_ProgressChanged;
                workerLoadDevice.RunWorkerCompleted += WorkerLoadDevice_RunWorkerCompleted;
                workerLoadDevice.RunWorkerAsync();

                
                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
        }

        private void WorkerLoadDevice_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check to see if an error occurred in the
            // background process.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }

            // Check to see if the background process was cancelled.
            if (e.Cancelled)
            {
                MessageBox.Show("Processing cancelled.");
                return;
            }
            planControl.operation_model.LoadListPlanToDataGrid();
            // Everything completed normally.
            // process the response using e.Result
            //MessageBox.Show("Processing is complete.");
            this.planControl.pbStatus.Value = 0;
            planControl.btn_importPlan.IsEnabled = true;
            planControl.Btn_Accept.IsEnabled = true;
            planControl.RefreshBtn.IsEnabled = true;
        }

        private void WorkerLoadDevice_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.planControl.pbStatus.Value = e.ProgressPercentage;
        }

        private void WorkerLoadDevice_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                planControl.btn_importPlan.IsEnabled = false;
                planControl.Btn_Accept.IsEnabled = false;
                planControl.RefreshBtn.IsEnabled = false;
            }));
            Stopwatch stopwatch = Stopwatch.StartNew();
            listDevices.Clear();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "device/getListDevice");
            request.Method = "GET";
            request.ContentType = @"application/json";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string result = reader.ReadToEnd();

                DataTable devices = JsonConvert.DeserializeObject<DataTable>(result);
                for (int i=0;i< devices.Rows.Count;i++)
                {
                    dtDevice tempDevice = new dtDevice
                    {
                        creUsrId = int.Parse(devices.Rows[i]["creUsrId"].ToString()),
                        creDt = devices.Rows[i]["creDt"].ToString(),
                        updUsrId = int.Parse(devices.Rows[i]["updUsrId"].ToString()),
                        updDt = devices.Rows[i]["updDt"].ToString(),

                        deviceId = int.Parse(devices.Rows[i]["deviceId"].ToString()),
                        deviceName = devices.Rows[i]["deviceName"].ToString()
                    };
                    if (AddDevice(tempDevice))
                    {
                        tempDevice.GetDeviceProductsList();
                    }
                    (sender as BackgroundWorker).ReportProgress((i * 100) / devices.Rows.Count);
                }
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        public bool AddDevice(dtDevice checkDevice)
        {
            foreach (dtDevice item in listDevices)
            {
                if (item.deviceId == checkDevice.deviceId)
                {
                    return false;
                }
            }
            listDevices.Add(checkDevice);
            return true;
        }
        
        public int GetProductIdByDeviceProductId(int deviceProductId)
        {
            foreach (dtDevice device in listDevices)
            {
                foreach (dtDeviceProduct deviceProduct in device.deviceProducts)
                {
                    if (deviceProduct.deviceProductId == deviceProductId)
                    {
                        return deviceProduct.productId;
                    }
                }
            }
            return -1;
        }

        public int GetDeviceIdByDeviceProductId(int deviceProductId)
        {
            foreach (dtDevice device in listDevices)
            {
                foreach (dtDeviceProduct deviceProduct in device.deviceProducts)
                {
                    if (deviceProduct.deviceProductId == deviceProductId)
                    {
                        return deviceProduct.deviceId;
                    }
                }
            }
            return -1;
        }
    }
}
