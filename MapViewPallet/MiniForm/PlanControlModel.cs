using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Data;
using System.Windows.Forms;

namespace MapViewPallet.MiniForm
{
    public class PlanControlModel : NotifyUIBase
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ListCollectionView GroupedDevices_S1 { get; private set; }

        public List<Plan> BasePlans1 { get; set; }

        public PlanControl planControl;
        public DeviceList deviceList;

        public PlanControlModel(PlanControl planControl)
        {
            this.planControl = planControl;

            deviceList = new DeviceList();

            BasePlans1 = new List<Plan>();
            GroupedDevices_S1 = (ListCollectionView)CollectionViewSource.GetDefaultView(BasePlans1);
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

        public void CreateListPlansFromShift(DateTime selectedDate, int selectedShift)
        {
            try
            {
                if (deviceList.GetDevicesList(planControl))
                {
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public void LoadListPlanToDataGrid()
        {
            try
            {
                DateTime selectedDate = (DateTime)planControl.pCalendar.SelectedDate;
                int selectedShift = planControl.TabControlShift.SelectedIndex + 1;
                string date = selectedDate.Year + "-" + selectedDate.Month.ToString("00.") + "-" + selectedDate.Day.ToString("00.");
                List<Plan> plansTemp = new List<Plan>();

                List<Plan> checkPlansList = CheckPlans(selectedShift, selectedDate);

                switch (DateTime.Now.Date.CompareTo(selectedDate.Date))
                {
                    case -1:
                    {
                        //Console.WriteLine("Ngay hien tai nho hon ngay duoc chon_Aka ngay hom sau");
                        goto case 0;
                    }
                    case 0:
                    {
                        //Console.WriteLine("Ngay hien tai bang ngay duoc chon_Aka ngay hom nay");
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
                                tempOpe.palletAmount = 0;
                                tempOpe.deviceProductId = product.deviceProductId;
                                tempOpe.timeWorkId = selectedShift;
                                tempOpe.activeDate = date;
                                tempOpe.deviceId = device.deviceId;
                                tempOpe.deviceName = device.deviceName;
                                tempOpe.productId = product.productId;
                                tempOpe.productName = product.productName;
                                tempOpe.listProductDetails = product.productDetails;
                                tempOpe.productDetailName = "";
                                if (tempOpe.listProductDetails.Count != 0)
                                {
                                    for (int i = 0; i < tempOpe.listProductDetails.Count; i++)
                                    {
                                        if (!(string.IsNullOrEmpty(tempOpe.listProductDetails[i].productDetailName)) && !(tempOpe.listProductDetails[i].productDetailName.Trim() == ""))
                                        {
                                            tempOpe.productDetailName = tempOpe.listProductDetails[i].productDetailName;
                                            break;
                                        }
                                    }
                                    tempOpe.productDetailName = tempOpe.listProductDetails[0].productDetailName;
                                }
                                if (product.productDetails.Count > 0)
                                {
                                    tempOpe.productDetailId = product.productDetails.First().productDetailId;
                                }

                                if (checkPlansList.Count != 0)
                                {
                                    bool isPlaned = false;
                                    foreach (Plan tempPlan in checkPlansList)
                                    {
                                        if (EqualPlan(tempOpe, tempPlan))
                                        {
                                            //tempOpe.creUsrId = tempPlan.creUsrId;
                                            //tempOpe.creDt = tempPlan.creDt;
                                            //tempOpe.updUsrId = tempPlan.updUsrId;
                                            //tempOpe.updDt = tempPlan.updDt;

                                            tempOpe.planId = tempPlan.planId;
                                            tempOpe.productDetailId = tempPlan.productDetailId;
                                            tempOpe.productDetailName = tempPlan.productDetailName;
                                            tempOpe.palletAmount = tempPlan.palletAmount;
                                            tempOpe.palletUse = tempPlan.palletUse;
                                            tempOpe.palletMiss = tempPlan.palletMiss;

                                            tempOpe.deviceProductId = tempPlan.deviceProductId;
                                            tempOpe.timeWorkId = tempPlan.timeWorkId;
                                            tempOpe.activeDate = tempPlan.activeDate;
                                            tempOpe.deviceId = tempPlan.deviceId;
                                            tempOpe.productId = tempPlan.productId;
                                            tempOpe.imageDeviceUrl = tempPlan.imageDeviceUrl;
                                            tempOpe.imageProductUrl = tempPlan.imageProductUrl;
                                            ////**************************************
                                            isPlaned = true;
                                            //break;
                                        }
                                    }
                                    if (isPlaned)
                                    {
                                        if (tempOpe.deviceName.Contains("RETURN"))
                                        {
                                            if (planControl.cbShowReturnPlan.IsChecked == true)
                                            {
                                                plansTemp.Add(tempOpe);
                                            }
                                        }
                                        else
                                        {
                                            plansTemp.Add(tempOpe);
                                        }
                                    }
                                    else if (product.checkStatus)
                                    {
                                        if (planControl.cbShowManualPlan.IsChecked == true)
                                        {
                                            if (tempOpe.deviceName.Contains("RETURN"))
                                            {
                                                if (planControl.cbShowReturnPlan.IsChecked == true)
                                                {
                                                    plansTemp.Add(tempOpe);
                                                }
                                            }
                                            else
                                            {
                                                plansTemp.Add(tempOpe);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (product.checkStatus)
                                    {
                                        if (planControl.cbShowManualPlan.IsChecked == true)
                                        {
                                            if (tempOpe.deviceName.Contains("RETURN"))
                                            {
                                                if (planControl.cbShowReturnPlan.IsChecked == true)
                                                {
                                                    plansTemp.Add(tempOpe);
                                                }
                                            }
                                            else
                                            {
                                                plansTemp.Add(tempOpe);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        //Console.WriteLine("Ngay hien tai lon hon ngay duoc chon_Aka ngay hom truoc");
                        foreach (Plan item in checkPlansList)
                        {
                            plansTemp.Add(item);
                        }
                        break;
                    }
                    default:
                    {
                        //Console.WriteLine("-100");
                        break;
                    }
                }

                switch (selectedShift)
                {
                    /*
                     * Có lỗi ở đây sau khi nhập palletAmount bằng chữ và nhấn save, crash, kết quả không được lưu vào
                     * */
                    case 1:
                    {
                        BasePlans1.Clear();
                        AddPlans(plansTemp, BasePlans1);
                        if (GroupedDevices_S1.IsEditingItem)
                        {
                            GroupedDevices_S1.CommitEdit();
                        }

                        if (GroupedDevices_S1.IsAddingNew)
                        {
                            GroupedDevices_S1.CommitNew();
                        }

                        GroupedDevices_S1.Refresh();
                        break;
                    }
                    case 2:
                    {
                        break;
                    }
                    case 3:
                    {
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public bool EqualPlan(Plan pl0, Plan pl1)
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
        
        public List<Plan> CheckPlans(int timeWorkId, DateTime selectedDate)
        {
            if (!Global_Object.ServerAlive())
            {
                return new List<Plan>();
            }
            try
            {
                List<Plan> returnList = new List<Plan>();
                string activeDate = selectedDate.Year + "-" + selectedDate.Month.ToString("00.") + "-" + selectedDate.Day.ToString("00.");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "plan/getPlanByShift");
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
                        Plan tempPlan = new Plan(item);
                        AddPlan(tempPlan, returnList);
                    }
                    return returnList;
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
                return null;
            }
        }

        public void UpdateAllCurrentPlansToDb()
        {
            try
            {
                if ((planControl.pCalendar.SelectedDate != null) &&
                                (planControl.TabControlShift.SelectedIndex >= 0) &&
                                (planControl.TabControlShift.IsLoaded))
                {
                    switch (planControl.TabControlShift.SelectedIndex)
                    {
                        case 0:
                        { UpdatePlansToDb(BasePlans1); break; }
                        //case 1: { UpdatePlansToDb(BasePlans2); break; }
                        //case 2: { UpdatePlansToDb(BasePlans3); break; }
                        default:
                        { break; }
                    }
                    System.Windows.Forms.MessageBox.Show(
                        String.Format(Global_Object.messageSaveSucced),
                        Global_Object.messageTitileInformation,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public void UpdatePlansToDb(List<Plan> listPlan)
        {
            try
            {
                List<Plan> listPlanTemp = new List<Plan>();
                foreach (Plan item in listPlan)
                {
                    if (item.planId != 0 || item.palletAmount != 0)
                    {
                        listPlanTemp.Add(item);
                    }
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "plan/insertUpdatePlan");
                request.Method = "POST";
                request.ContentType = @"application/json";

                string jsonData = JsonConvert.SerializeObject(listPlanTemp);

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
                logFile.Error(ex.Message);
            }
        }

    }
}