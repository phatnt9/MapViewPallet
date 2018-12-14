﻿
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Data;
using System.Threading;

namespace MapViewPallet.MiniForm
{

    public class PlanModel 
    {
        public int currentPlan = 1;
        //***************DATA*********************
        public ICollectionView GroupedDevices_S1 { get; private set; }
        public ICollectionView GroupedDevices_S2 { get; private set; }
        public ICollectionView GroupedDevices_S3 { get; private set; }

        //***************VARIABLES*********************

        public PlanControl planControl;
        public DeviceList deviceList;
        public List<Plan> basePlans1 { get; set; }
        public List<Plan> basePlans2 { get; set; }
        public List<Plan> basePlans3 { get; set; }
        //***************NOTIFY*********************
        //private string _productdetailid = "";
        //public string ProductDetailId
        //{
        //    get { return _productdetailid; }
        //    set
        //    {
        //        if (_productdetailid != value)
        //        {
        //            _productdetailid = value;
        //            NotifyPropertyChanged("ProductDetailId");
        //        }
        //    }
        //}
        //***************METHOD*********************

        public PlanModel(PlanControl planControl)
        {
            this.planControl = planControl;
            deviceList = new DeviceList();
            basePlans1 = new List<Plan>();
            basePlans2 = new List<Plan>();
            basePlans3 = new List<Plan>();
            //###############################
            //GroupedDevices_S1 = CollectionViewSource.GetDefaultView(basePlans1);
            //GroupedDevices_S2 = CollectionViewSource.GetDefaultView(basePlans2);
            //GroupedDevices_S3 = CollectionViewSource.GetDefaultView(basePlans3);

            GroupedDevices_S1 = new ListCollectionView(basePlans1);
            GroupedDevices_S2 = new ListCollectionView(basePlans2);
            GroupedDevices_S3 = new ListCollectionView(basePlans3);

            //RefreshData();
            //GroupedDevices_S1.CollectionChanged += GroupedDevices_S1_Change;
            //GroupedDevices_S2.CollectionChanged += GroupedDevices_S2_Change;
            //GroupedDevices_S3.CollectionChanged += GroupedDevices_S3_Change;
            //_shift1_Operations = _shift2_Operations = _shift3_Operations = basePlans;
            //#######
            //GroupedDevices_S1.GroupDescriptions.Add(new PropertyGroupDescription("deviceName"));
            //GroupedDevices_S2.GroupDescriptions.Add(new PropertyGroupDescription("deviceName"));
            //GroupedDevices_S3.GroupDescriptions.Add(new PropertyGroupDescription("deviceName"));


        }

        private void GroupedDevices_S1_Change(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Console.WriteLine("GroupedDevices_S1_Change");
        }
        private void GroupedDevices_S2_Change(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Console.WriteLine("GroupedDevices_S2_Change");
        }
        private void GroupedDevices_S3_Change(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Console.WriteLine("GroupedDevices_S3_Change");
        }

        public bool ContainPlan(Plan tempOpe, List<Plan> Base)
        {
            if (tempOpe.planId != 0)
            {
                foreach (Plan temp in Base)
                {
                    if (temp.planId == tempOpe.planId)
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (Plan temp in Base)
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

        public void AddPlans(List<Plan> tempOpe, List<Plan> Base)
        {
            foreach (Plan item in tempOpe)
            {
                if (Base.Count != 0)
                {
                    if (!ContainPlan(item, Base))
                    {
                        Base.Add(item);
                    }
                }
                else
                {
                    Base.Add(item);
                }
            }
            
        }

        public void AddPlan(Plan tempOpe, List<Plan> Base)
        {
            if (Base.Count != 0)
            {
                if (!ContainPlan(tempOpe, Base))
                {
                    Base.Add(tempOpe);
                }
            }
            else
            {
                Base.Add(tempOpe);
            }
        }

        public void CreateListPlansFromDeviceList(DateTime selectedDate)
        {
            string date = selectedDate.Year + "-" + selectedDate.Month + "-" + selectedDate.Day.ToString("00.");
            List<Plan> jsonShift1 = CheckPlans(1, selectedDate);
            List<Plan> jsonShift2 = CheckPlans(2, selectedDate);
            List<Plan> jsonShift3 = CheckPlans(3, selectedDate);
            
            for (int i = 1; i < 4; i++)
            {
                List<Plan> plansTemp = new List<Plan>();
                List<Plan> checkPlanList = (i == 1) ? jsonShift1 : ((i == 2) ? jsonShift2 : jsonShift3);
                foreach (Device device in deviceList.listDevices)
                {
                    foreach (DeviceProduct product in device.listDeviceProduct)
                    {
                        Plan tempOpe = new Plan()
                        {
                            planId = 0,
                            deviceProductId = product.deviceProductId,
                            timeWorkId = i,
                            productDetailId = product.listProductDetails.First().productDetailId,
                            listProductDetails = product.listProductDetails,
                            palletAmount = 0,
                            palletUse = 0,
                            palletMiss = 0,
                            activeDate = date,
                            deviceId = device.deviceId,
                            deviceName = device.deviceName,
                            productId = product.productId,
                            productName = product.productName
                        };
                        if (checkPlanList.Count != 0)
                        {
                            foreach (Plan tempPlan in checkPlanList)
                            {
                                if (ComparePlan(tempOpe,tempPlan))
                                {
                                    tempOpe.creUsrId = tempPlan.creUsrId;
                                    tempOpe.creDt = tempPlan.creDt;
                                    tempOpe.updUsrId = tempPlan.updUsrId;
                                    tempOpe.updDt = tempPlan.updDt;

                                    tempOpe.planId = tempPlan.planId;
                                    tempOpe.productDetailId = tempPlan.productDetailId;
                                    tempOpe.palletAmount = tempPlan.palletAmount;
                                    tempOpe.palletUse = tempPlan.palletUse;
                                    tempOpe.palletMiss = tempPlan.palletMiss;

                                    //**************************************
                                    //tempOpe.deviceProductId = tempPlan.deviceProductId;
                                    //tempOpe.timeWorkId = tempPlan.timeWorkId;
                                    //tempOpe.activeDate = tempPlan.activeDate;
                                    tempOpe.deviceId = tempPlan.deviceId;
                                    tempOpe.productId = tempPlan.productId;
                                    //tempOpe.buffers = pl.buffers;
                                    //tempOpe.buffers = pl.buffers;
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
                switch (i)
                {
                    case 1: { basePlans1.Clear(); AddPlans(plansTemp, basePlans1); break; }
                    case 2: { basePlans2.Clear(); AddPlans(plansTemp, basePlans2); break; }
                    case 3: { basePlans3.Clear(); AddPlans(plansTemp, basePlans3); break; }
                    default: { break; }
                }
            }
            //RefreshData();
            //GroupedDevices_S1 = new ListCollectionView(basePlans);
            //GroupedDevices_S2 = new ListCollectionView(basePlans);
            //GroupedDevices_S3 = new ListCollectionView(basePlans);
            //Console.WriteLine("Add 123");
            //RefreshData();


        }

        public bool ComparePlan (Plan pl0, Plan pl1)
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
            //Console.WriteLine("RefreshData 123");
            GroupedDevices_S1.Refresh();
            GroupedDevices_S2.Refresh();
            GroupedDevices_S3.Refresh();
        }


        public List<Plan> CheckPlans(int timeWorkId, DateTime selectedDate)
        {
            //DataTable planByShift;
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
                    //Console.WriteLine(result);
                    //Console.WriteLine("---------------------");
                    dynamic listplan = JsonConvert.DeserializeObject(result);
                    foreach (dynamic item in listplan)
                    {
                        //Console.WriteLine(item);
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

                            deviceId = deviceList.GetDeviceIdByDeviceProductId((int)item.deviceProductId),
                            productId = deviceList.GetProductIdByDeviceProductId((int)item.deviceProductId)
                        };
                        AddPlan(tempPlan, returnList);
                    }
                    return returnList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void UpdateAllCurrentPlansToDb()
        {
            UpdatePlansToDb(basePlans1);
            UpdatePlansToDb(basePlans2);
            UpdatePlansToDb(basePlans3);
            Console.WriteLine("---Done---");
        }

        public void UpdatePlansToDb (List<Plan> listPlan)
        {
            foreach (Plan item in listPlan)
            {
                SendPlanToDb(item);
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
                postApiBody.creUsrId = plan.creUsrId;
                postApiBody.updUsrId =  plan.updUsrId;
                //postApiBody.deviceId =  plan.deviceId;
                //postApiBody.productId =  plan.productId;

                //********************************
                string jsonData = "["+JsonConvert.SerializeObject(postApiBody)+"]";
                //Console.WriteLine(jsonData);
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
                    //Console.WriteLine("Insert or Update plan"+result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        
    }
}
