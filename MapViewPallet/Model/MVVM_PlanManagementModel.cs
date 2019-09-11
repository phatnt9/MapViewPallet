using MapViewPallet.MiniForm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Net;
using System.IO;
using System.Windows.Data;
using System.Globalization;

namespace MapViewPallet.Model
{
   
    class MVVM_PlanManagementModel:ViewModelBase
    {
        public enum AppStatus
        {
            Ready,
            Exporting,
            Completed,
            Finished,
            Cancelled,
            Loading,
            Error,
        }

        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BackgroundWorker worker;

        private ObservableCollection<dtPlan> _plans = new ObservableCollection<dtPlan>();
        public ObservableCollection<dtPlan> Plans => _plans;

        private AppStatus _pgbStatus;
        public AppStatus PgbStatus { get => _pgbStatus; set { _pgbStatus = value; RaisePropertyChanged("PgbStatus"); } }

        public MVVM_PlanManagementModel()
        {
        }

        public void ReloadListPlan(DateTime selectedDate)
        {

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync(argument: selectedDate);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PgbStatus = AppStatus.Loading;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // handle the error
                PgbStatus = AppStatus.Error;
            }
            else if (e.Cancelled)
            {
                // handle cancellation
                PgbStatus = AppStatus.Cancelled;
            }
            else
            {
                PgbStatus = AppStatus.Completed;
                List<dtPlan> listPlan = (List<dtPlan>)e.Result;
                if (listPlan.Count != 0)
                {
                    Plans.Clear();
                    foreach (dtPlan item in listPlan)
                    {
                        Plans.Add(item);
                    }
                }
            }
            // general cleanup code, runs when there was an error or not.
            //mainW.pbStatus.Value = 0;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime selectedDate = (DateTime)e.Argument;
            dynamic postApiBody = new JObject();
            string activeDate = selectedDate.Year + "-" + selectedDate.Month.ToString("00.") + "-" + selectedDate.Day.ToString("00.");
            postApiBody.activeDate = activeDate;
            postApiBody.timeWorkId = 1;
            string jsonData = JsonConvert.SerializeObject(postApiBody);
            string contentJson = Global_Object.RequestDataAPI(jsonData, "plan/getPlanByShift", Global_Object.RequestMethod.POST);
            dynamic response = JsonConvert.DeserializeObject(contentJson);
            List<dtPlan> listPlan = response.ToObject<List<dtPlan>>();
            e.Result = listPlan;
        }

        public string CreatePlanPallet(dtPlan selectedPlan)
        {
            try
            {
                dynamic postApiBody = new JObject();
                postApiBody.timeWorkId = 0;
                postApiBody.activeDate = selectedPlan.activeDate;
                postApiBody.deviceId = selectedPlan.deviceId;
                postApiBody.productId = selectedPlan.productId;
                postApiBody.productDetailId = selectedPlan.productDetailId;
                postApiBody.updUsrId = 1;
                postApiBody.palletAmount = 1;
                string jsonData = JsonConvert.SerializeObject(postApiBody);
                //Console.WriteLine(jsonData);

                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create(@"http://" +
                    Properties.Settings.Default.serverIp + ":" +
                    Properties.Settings.Default.serverPort +
                    @"/robot/rest/" + "plan/createPlanPallet");

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
                    return ("Plan Succeed: " + result);
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
                return ("Error create plan Pallet");
            }
        }
    }
}
