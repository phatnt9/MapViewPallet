
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MapViewPallet.MiniForm
{

    public class PlanControlModel 
    {

        public ICollectionView GroupedDevices_S1 { get; private set; }
        public ICollectionView GroupedDevices_S2 { get; private set; }
        public ICollectionView GroupedDevices_S3 { get; private set; }

        public List<Plan> BasePlans1 { get; set; }
        public List<Plan> BasePlans2 { get; set; }
        public List<Plan> BasePlans3 { get; set; }

        public PlanControl planControl;
        public DeviceList deviceList;


        public PlanControlModel(PlanControl planControl)
        {
            this.planControl = planControl;

            deviceList = new DeviceList();

            BasePlans1 = new List<Plan>();
            BasePlans2 = new List<Plan>();
            BasePlans3 = new List<Plan>();
            GroupedDevices_S1 = new ListCollectionView(BasePlans1);
            GroupedDevices_S2 = new ListCollectionView(BasePlans2);
            GroupedDevices_S3 = new ListCollectionView(BasePlans3);
        }

        public bool ContainPlan(Plan tempOpe, List<Plan> List)
        {
            if (tempOpe.planId != 0)
            {
                foreach (Plan temp in List)
                {
                    if (temp.planId == tempOpe.planId)
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (Plan temp in List)
                {
                    if ((temp.deviceProductId == tempOpe.deviceProductId) &&
                        (temp.productDetailId == tempOpe.productDetailId) &&
                        (temp.timeWorkId == tempOpe.timeWorkId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddPlans(List<Plan> tempOpe, List<Plan> List)
        {
            foreach (Plan item in tempOpe)
            {
                if (List.Count != 0)
                {
                    if (!ContainPlan(item, List))
                    {
                        List.Add(item);
                    }
                }
                else
                {
                    List.Add(item);
                }
            }
            
        }

        public void AddPlan(Plan tempOpe, List<Plan> List)
        {
            if (List.Count != 0)
            {
                if (!ContainPlan(tempOpe, List))
                {
                    List.Add(tempOpe);
                }
            }
            else
            {
                List.Add(tempOpe);
            }
        }

        public void CreateListPlansFromShift(DateTime selectedDate,int selectedShift)
        {
            if (deviceList.GetDevicesList())
            {
                string date = selectedDate.Year + "-" + selectedDate.Month + "-" + selectedDate.Day.ToString("00.");
                List<Plan> plansTemp = new List<Plan>();
                List<Plan> checkPlansList = CheckPlans(selectedShift, selectedDate);

                foreach (dtDevice device in deviceList.listDevices)
                {
                    foreach (dtDeviceProduct product in device.deviceProducts)
                    {
                        Plan tempOpe = new Plan();

                        tempOpe.creUsrId = Global_Object.userLogin;
                        tempOpe.creDt = "";
                        tempOpe.updUsrId = Global_Object.userLogin;
                        tempOpe.updDt = "";

                        tempOpe.planId = 0;
                        if (product.productDetails.Count > 0)
                        {
                            tempOpe.productDetailId = product.productDetails.First().productDetailId;
                        }
                        tempOpe.palletAmount = 0;
                        //tempOpe.palletUse = 20;
                        //tempOpe.palletMiss = 11;

                        tempOpe.deviceProductId = product.deviceProductId;
                        tempOpe.timeWorkId = selectedShift;
                        tempOpe.activeDate = date;
                        tempOpe.deviceId = device.deviceId;
                        tempOpe.deviceName = device.deviceName;
                        tempOpe.productId = product.productId;
                        tempOpe.productName = product.productName;
                        tempOpe.listProductDetails = product.productDetails;

                        if (checkPlansList.Count != 0)
                        {
                            foreach (Plan tempPlan in checkPlansList)
                            {
                                if (EqualPlan(tempOpe, tempPlan))
                                {
                                    //tempOpe.creUsrId = tempPlan.creUsrId;
                                    tempOpe.creDt = tempPlan.creDt;
                                    //tempOpe.updUsrId = tempPlan.updUsrId;
                                    tempOpe.updDt = tempPlan.updDt;

                                    tempOpe.planId = tempPlan.planId;
                                    tempOpe.productDetailId = tempPlan.productDetailId;
                                    tempOpe.palletAmount = tempPlan.palletAmount;
                                    tempOpe.palletUse = tempPlan.palletUse;
                                    tempOpe.palletMiss = tempPlan.palletMiss;
                                    
                                    tempOpe.deviceProductId = tempPlan.deviceProductId;
                                    tempOpe.timeWorkId = tempPlan.timeWorkId;
                                    tempOpe.activeDate = tempPlan.activeDate;
                                    tempOpe.deviceId = tempPlan.deviceId;
                                    tempOpe.productId = tempPlan.productId;
                                    //**************************************
                                }
                            }
                            plansTemp.Add(tempOpe);
                        }
                        else
                        {
                            plansTemp.Add(tempOpe);
                        }
                    }

                }
                switch (selectedShift)
                {
                    case 1: { BasePlans1.Clear(); AddPlans(plansTemp, BasePlans1); GroupedDevices_S1.Refresh(); break; }
                    case 2: { BasePlans2.Clear(); AddPlans(plansTemp, BasePlans2); GroupedDevices_S2.Refresh(); break; }
                    case 3: { BasePlans3.Clear(); AddPlans(plansTemp, BasePlans3); GroupedDevices_S3.Refresh(); break; }
                    default: { break; }
                }
            }
        }

        public bool EqualPlan (Plan pl0, Plan pl1)
        {
            if ((pl0.deviceProductId == pl1.deviceProductId) &&
                        //(pl0.productDetailId == pl1.productDetailId) &&
                        (pl0.timeWorkId == pl1.timeWorkId) &&
                        (pl0.activeDate == pl1.activeDate))
            {
                return true;
            }
            return false;
        }

        public void RefreshData ()
        {
            GroupedDevices_S1.Refresh();
            GroupedDevices_S2.Refresh();
            GroupedDevices_S3.Refresh();
        }


        public List<Plan> CheckPlans(int timeWorkId, DateTime selectedDate)
        {
            try
            {
                List<Plan> returnList = new List<Plan>();
                string activeDate = selectedDate.Year + "-" + selectedDate.Month + "-" + selectedDate.Day.ToString("00.");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "plan/getPlanByShift");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.activeDate = activeDate;
                postApiBody.timeWorkId = timeWorkId;
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

                    dynamic listplan = JsonConvert.DeserializeObject(result);
                    foreach (dynamic item in listplan)
                    {
                        Plan tempPlan = new Plan()
                        {
                            creUsrId = (int)item.creUsrId,
                            creDt = (string)item.creDt,
                            updUsrId = (int)item.updUsrId,
                            updDt = (string)item.updDt,

                            planId = (int)item.planId,
                            deviceProductId = (int)item.deviceProductId,
                            timeWorkId = (int)item.timeWorkId,
                            productDetailId = (int)item.productDetailId,

                            palletAmount = (int)item.palletAmount,
                            palletUse = (int)item.palletUse,
                            palletMiss = (int)item.palletMiss,
                            activeDate = (string)item.activeDate,

                            deviceId = (int)item.deviceId,
                            productId = (int)item.productId
                            //deviceId = deviceList.GetDeviceIdByDeviceProductId((int)item.deviceProductId),
                            //productId = deviceList.GetProductIdByDeviceProductId((int)item.deviceProductId)
                        };
                        AddPlan(tempPlan, returnList);
                    }
                    return returnList;
                }
            }
            catch 
            {
                return null;
            }
        }

        public void UpdateAllCurrentPlansToDb()
        {
            if ((planControl.pCalendar.SelectedDate != null) &&
                (planControl.TabControlShift.SelectedIndex >= 0) &&
                (planControl.TabControlShift.IsLoaded))
            {
                switch(planControl.TabControlShift.SelectedIndex)
                {
                    case 0: { UpdatePlansToDb(BasePlans1); break; }
                    case 1: { UpdatePlansToDb(BasePlans2); break; }
                    case 2: { UpdatePlansToDb(BasePlans3); break; }
                    default: {  break; }
                }
                System.Windows.Forms.MessageBox.Show(
                    String.Format(Global_Object.messageSaveSucced), 
                    Global_Object.messageTitileInformation, 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
        }

        public void UpdatePlansToDb (List<Plan> listPlan)
        {
            foreach (Plan item in listPlan)
            {
                if(item.palletAmount != 0)
                {
                    SendPlanToDb(item);
                }
            }
        }

        public void SendPlanToDb (Plan plan)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "plan/insertUpdatePlan");
                request.Method = "POST";
                request.ContentType = @"application/json";

                //********************************
                dynamic postApiBody = new JObject();
                postApiBody.planId = plan.planId;
                postApiBody.deviceProductId = plan.deviceProductId;
                postApiBody.timeWorkId = plan.timeWorkId;
                postApiBody.productDetailId = plan.productDetailId;
                postApiBody.palletAmount = plan.palletAmount;
                postApiBody.activeDate = plan.activeDate;
                postApiBody.deviceId = plan.deviceId;
                postApiBody.productId = plan.productId;
                postApiBody.creUsrId = Global_Object.userLogin;
                postApiBody.updUsrId = Global_Object.userLogin;

                //********************************
                string jsonData = "["+JsonConvert.SerializeObject(postApiBody)+"]";
                int result = 0;
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
                    int.TryParse(reader.ReadToEnd(), out result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        
    }
}
