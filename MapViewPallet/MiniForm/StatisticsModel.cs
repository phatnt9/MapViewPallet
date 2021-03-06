﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace MapViewPallet.MiniForm
{
    public class dtRobot : userModel
    {
        private string _Id;
        private string _RobotName;

        public string id { get => _Id; set { _Id = value; RaisePropertyChanged("id"); } }
        public string robotName { get => _RobotName; set { _RobotName = value; RaisePropertyChanged("robotName"); } }
    }

    public class dtProcessStatus : userModel
    {
        private string _Id;
        private string _StatusName;

        public string id { get => _Id; set { _Id = value; RaisePropertyChanged("id"); } }
        public string statusName { get => _StatusName; set { _StatusName = value; RaisePropertyChanged("statusName"); } }
    }

    public class StatisticsModel
    {
        private Statistics statistics;
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ListCollectionView GroupedRobotProcess { get; private set; }
        public ListCollectionView GroupedRobotCharge { get; private set; }

        private ObservableCollection<dtRobot> _robots;
        private ObservableCollection<dtRobot> _allRobots;
        private ObservableCollection<dtDevice> _devices;
        private ObservableCollection<dtDevice> _allDevices;
        private ObservableCollection<dtProduct> _products;
        private ObservableCollection<dtProduct> _allProducts;
        private ObservableCollection<dtBuffer> _buffers;
        private ObservableCollection<dtBuffer> _allBuffers;
        private ObservableCollection<dtProductDetail> _productDetails;
        private ObservableCollection<dtProductDetail> _allProductDetails;
        private ObservableCollection<dtOperationType> _operationTypes;
        private ObservableCollection<dtOperationType> _allOperationTypes;
        private ObservableCollection<dtTimeWork> _timeWorks;
        private ObservableCollection<dtTimeWork> _allTimeWorks;

        public List<dtRobotProcess> listRobotProcess;
        public List<dtRobotCharge> listRobotCharge;
        public List<dtOperationType> listOperationType;

        public ObservableCollection<dtRobot> Robots
        {
            get => _robots;
            set => _robots = value;
        }

        public ObservableCollection<dtRobot> AllRobots
        {
            get => _allRobots;
            set => _allRobots = value;
        }

        public ObservableCollection<dtOperationType> OperationTypes
        {
            get => _operationTypes;
            set => _operationTypes = value;
        }

        public ObservableCollection<dtOperationType> AllOperationTypes
        {
            get => _allOperationTypes;
            set => _allOperationTypes = value;
        }

        public ObservableCollection<dtDevice> Devices
        {
            get => _devices;
            set => _devices = value;
        }

        public ObservableCollection<dtDevice> AllDevices
        {
            get => _allDevices;
            set => _allDevices = value;
        }

        public ObservableCollection<dtProduct> Products
        {
            get => _products;
            set => _products = value;
        }

        public ObservableCollection<dtProduct> AllProducts
        {
            get => _allProducts;
            set => _allProducts = value;
        }

        public ObservableCollection<dtBuffer> Buffers
        {
            get => _buffers;
            set => _buffers = value;
        }

        public ObservableCollection<dtBuffer> AllBuffers
        {
            get => _allBuffers;
            set => _allBuffers = value;
        }

        public ObservableCollection<dtProductDetail> ProductDetails
        {
            get => _productDetails;
            set => _productDetails = value;
        }

        public ObservableCollection<dtProductDetail> AllProductDetails
        {
            get => _allProductDetails;
            set => _allProductDetails = value;
        }

        public ObservableCollection<dtTimeWork> TimeWorks
        {
            get => _timeWorks;
            set => _timeWorks = value;
        }

        public ObservableCollection<dtTimeWork> AllTimeWorks
        {
            get => _allTimeWorks;
            set => _allTimeWorks = value;
        }

        public StatisticsModel(Statistics statistics)
        {
            this.statistics = statistics;
            Robots = new ObservableCollection<dtRobot>();
            AllRobots = new ObservableCollection<dtRobot>();
            Devices = new ObservableCollection<dtDevice>();
            AllDevices = new ObservableCollection<dtDevice>();
            Products = new ObservableCollection<dtProduct>();
            AllProducts = new ObservableCollection<dtProduct>();
            Buffers = new ObservableCollection<dtBuffer>();
            AllBuffers = new ObservableCollection<dtBuffer>();
            ProductDetails = new ObservableCollection<dtProductDetail>();
            AllProductDetails = new ObservableCollection<dtProductDetail>();
            OperationTypes = new ObservableCollection<dtOperationType>();
            AllOperationTypes = new ObservableCollection<dtOperationType>();
            TimeWorks = new ObservableCollection<dtTimeWork>();
            AllTimeWorks = new ObservableCollection<dtTimeWork>();

            listRobotProcess = new List<dtRobotProcess>();
            listRobotCharge = new List<dtRobotCharge>();
            GroupedRobotProcess = (ListCollectionView)CollectionViewSource.GetDefaultView(listRobotProcess);
            GroupedRobotCharge = (ListCollectionView)CollectionViewSource.GetDefaultView(listRobotCharge);

            //listOperationType = new List<dtOperationType>();
        }

        public void ReloadListRobot(int tabIndex)
        {
            try
            {
                Robots.Add(new dtRobot() { id = "", robotName = "No data" });
                Robots.Add(new dtRobot() { id = "1", robotName = "Robot1" });
                Robots.Add(new dtRobot() { id = "2", robotName = "Robot2" });
                Robots.Add(new dtRobot() { id = "3", robotName = "Robot3" });

                AllRobots.Add(new dtRobot() { id = "", robotName = "No data" });
                AllRobots.Add(new dtRobot() { id = "1", robotName = "Robot1" });
                AllRobots.Add(new dtRobot() { id = "2", robotName = "Robot2" });
                AllRobots.Add(new dtRobot() { id = "3", robotName = "Robot3" });

                switch (tabIndex)
                {
                    case 0:
                    {
                        break;
                    }
                    case 1:
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

        public void ReloadListOperationType()
        {
            try
            {
                OperationTypes.Add(new dtOperationType() { idOperationType = -1, nameOperationType = "No data" });
                OperationTypes.Add(new dtOperationType() { idOperationType = 0, nameOperationType = "Buffer To Machine" });
                OperationTypes.Add(new dtOperationType() { idOperationType = 1, nameOperationType = "Forklift To Buffer" });
                OperationTypes.Add(new dtOperationType() { idOperationType = 2, nameOperationType = "Buffer To Return" });
                OperationTypes.Add(new dtOperationType() { idOperationType = 3, nameOperationType = "Machine To Return" });
                OperationTypes.Add(new dtOperationType() { idOperationType = 4, nameOperationType = "Return To Gate" });
                OperationTypes.Add(new dtOperationType() { idOperationType = 5, nameOperationType = "Robot To Ready" });
                OperationTypes.Add(new dtOperationType() { idOperationType = 6, nameOperationType = "Robot To Charge" });

                AllOperationTypes.Add(new dtOperationType() { idOperationType = -1, nameOperationType = "No data" });
                AllOperationTypes.Add(new dtOperationType() { idOperationType = 0, nameOperationType = "Buffer To Machine" });
                AllOperationTypes.Add(new dtOperationType() { idOperationType = 1, nameOperationType = "Forklift To Buffer" });
                AllOperationTypes.Add(new dtOperationType() { idOperationType = 2, nameOperationType = "Buffer To Return" });
                AllOperationTypes.Add(new dtOperationType() { idOperationType = 3, nameOperationType = "Machine To Return" });
                AllOperationTypes.Add(new dtOperationType() { idOperationType = 4, nameOperationType = "Return To Gate" });
                AllOperationTypes.Add(new dtOperationType() { idOperationType = 5, nameOperationType = "Robot To Ready" });
                AllOperationTypes.Add(new dtOperationType() { idOperationType = 6, nameOperationType = "Robot To Charge" });

                listOperationType = OperationTypes.ToList();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public void ReloadListDevice()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                Devices.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "device/getListDevice");
                request.Method = "GET";
                request.ContentType = @"application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();
                    DataTable devices = JsonConvert.DeserializeObject<DataTable>(result);
                    if (devices.Rows.Count > 0)
                    {
                        dtDevice tempDevice = new dtDevice
                        {
                            deviceId = 0,
                            deviceName = "No data"
                        };
                        if (!ContainDevice(tempDevice, Devices))
                        {
                            Devices.Add(tempDevice);
                        }
                        if (!ContainDevice(tempDevice, AllDevices))
                        {
                            AllDevices.Add(tempDevice);
                        }
                    }
                    foreach (DataRow dr in devices.Rows)
                    {
                        dtDevice tempDevice = new dtDevice
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            deviceName = dr["deviceName"].ToString(),
                            deviceNameOld = dr["deviceNameOld"].ToString(),
                            imageDeviceUrl = dr["imageDeviceUrl"].ToString(),
                            imageDeviceUrlOld = dr["imageDeviceUrlOld"].ToString(),
                            maxRow = int.Parse(dr["maxRow"].ToString()),
                            maxBay = int.Parse(dr["maxBay"].ToString()),
                            pathFile = dr["pathFile"].ToString(),
                        };
                        if (!ContainDevice(tempDevice, Devices))
                        {
                            Devices.Add(tempDevice);
                        }
                        if (!ContainDevice(tempDevice, AllDevices))
                        {
                            AllDevices.Add(tempDevice);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public void ReloadListProduct()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                if (statistics.cmbDevice.SelectedValue == null ||
                statistics.cmbDevice.SelectedValue.ToString() == "" ||
                int.Parse(statistics.cmbDevice.SelectedValue.ToString()) <= 0)
                {
                    Products.Clear();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "product/getListProduct");
                    request.Method = "GET";
                    request.ContentType = @"application/json";
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        string result = reader.ReadToEnd();
                        DataTable products = JsonConvert.DeserializeObject<DataTable>(result);
                        if (products.Rows.Count > 0)
                        {
                            dtProduct tempProduct = new dtProduct
                            {
                                productId = 0,
                                productName = "No data"
                            };
                            if (!ContainProduct(tempProduct, Products))
                            {
                                Products.Add(tempProduct);
                            }
                            if (!ContainProduct(tempProduct, AllProducts))
                            {
                                AllProducts.Add(tempProduct);
                            }
                        }
                        foreach (DataRow dr in products.Rows)
                        {
                            dtProduct tempProduct = new dtProduct
                            {
                                creUsrId = int.Parse(dr["creUsrId"].ToString()),
                                creDt = dr["creDt"].ToString(),
                                updUsrId = int.Parse(dr["updUsrId"].ToString()),
                                updDt = dr["updDt"].ToString(),
                                productId = int.Parse(dr["productId"].ToString()),
                                productName = dr["productName"].ToString(),
                                imageProductUrl = dr["imageProductUrl"].ToString(),
                                imageProductUrlOld = dr["imageProductUrlOld"].ToString(),
                                pathFile = dr["pathFile"].ToString(),
                            };
                            if (!ContainProduct(tempProduct, Products))
                            {
                                Products.Add(tempProduct);
                            }
                            if (!ContainProduct(tempProduct, AllProducts))
                            {
                                AllProducts.Add(tempProduct);
                            }
                        }
                    }
                }
                else
                {
                    Products.Clear();
                    dtDeviceProduct dtDeviceProduct = new dtDeviceProduct();
                    dtDeviceProduct.deviceId = int.Parse(statistics.cmbDevice.SelectedValue.ToString());
                    string jsonData = JsonConvert.SerializeObject(dtDeviceProduct);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "product/getListDeviceProductByDeviceId");
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
                        string result = reader.ReadToEnd();
                        DataTable products = JsonConvert.DeserializeObject<DataTable>(result);
                        if (products.Rows.Count > 0)
                        {
                            dtProduct tempProduct = new dtProduct
                            {
                                productId = 0,
                                productName = "No data"
                            };
                            if (!ContainProduct(tempProduct, Products))
                            {
                                Products.Add(tempProduct);
                            }
                            if (!ContainProduct(tempProduct, AllProducts))
                            {
                                AllProducts.Add(tempProduct);
                            }
                        }
                        foreach (DataRow dr in products.Rows)
                        {
                            dtProduct tempProduct = new dtProduct
                            {
                                creUsrId = int.Parse(dr["creUsrId"].ToString()),
                                creDt = dr["creDt"].ToString(),
                                updUsrId = int.Parse(dr["updUsrId"].ToString()),
                                updDt = dr["updDt"].ToString(),
                                productId = int.Parse(dr["productId"].ToString()),
                                productName = dr["productName"].ToString()
                            };
                            if (!ContainProduct(tempProduct, Products))
                            {
                                Products.Add(tempProduct);
                            }
                            if (!ContainProduct(tempProduct, AllProducts))
                            {
                                AllProducts.Add(tempProduct);
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

        public void ReloadListProductDetail()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                ProductDetails.Clear();
                dtProductDetail productDetail = new dtProductDetail();
                productDetail.productId = statistics.cmbProduct.SelectedValue == null ? 0 : int.Parse(statistics.cmbProduct.SelectedValue.ToString());
                string jsonData = JsonConvert.SerializeObject(productDetail);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "product/getListProductDetailByProductId");
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
                    string result = reader.ReadToEnd();
                    DataTable productDetails = JsonConvert.DeserializeObject<DataTable>(result);
                    if (productDetails.Rows.Count > 0)
                    {
                        dtProductDetail tempProductDetail = new dtProductDetail
                        {
                            productDetailId = 0,
                            productDetailName = "No data"
                        };
                        if (!ContainProductDetail(tempProductDetail, ProductDetails))
                        {
                            ProductDetails.Add(tempProductDetail);
                        }
                        if (!ContainProductDetail(tempProductDetail, AllProductDetails))
                        {
                            AllProductDetails.Add(tempProductDetail);
                        }
                    }
                    foreach (DataRow dr in productDetails.Rows)
                    {
                        dtProductDetail tempProductDetail = new dtProductDetail
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            productDetailId = int.Parse(dr["productDetailId"].ToString()),
                            productId = int.Parse(dr["productId"].ToString()),
                            productDetailName = dr["productDetailName"].ToString()
                        };
                        if (!ContainProductDetail(tempProductDetail, ProductDetails))
                        {
                            ProductDetails.Add(tempProductDetail);
                        }
                        if (!ContainProductDetail(tempProductDetail, AllProductDetails))
                        {
                            AllProductDetails.Add(tempProductDetail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public void ReloadListBuffer()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                Buffers.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "buffer/getListBuffer");
                request.Method = "GET";
                request.ContentType = @"application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();
                    DataTable buffers = JsonConvert.DeserializeObject<DataTable>(result);
                    if (buffers.Rows.Count > 0)
                    {
                        dtBuffer tempBuffer = new dtBuffer
                        {
                            bufferId = 0,
                            bufferName = "No data"
                        };
                        if (!ContainBuffer(tempBuffer, Buffers))
                        {
                            Buffers.Add(tempBuffer);
                        }
                        if (!ContainBuffer(tempBuffer, AllBuffers))
                        {
                            AllBuffers.Add(tempBuffer);
                        }
                    }
                    foreach (DataRow dr in buffers.Rows)
                    {
                        dtBuffer tempBuffer = new dtBuffer
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),

                            bufferId = int.Parse(dr["bufferId"].ToString()),
                            bufferName = dr["bufferName"].ToString(),
                            bufferNameOld = dr["bufferNameOld"].ToString(),
                            bufferCheckIn = dr["bufferCheckIn"].ToString(),
                            bufferData = dr["bufferData"].ToString(),
                            maxRow = int.Parse(dr["maxRow"].ToString()),
                            maxBay = int.Parse(dr["maxBay"].ToString()),
                            maxRowOld = int.Parse(dr["maxRowOld"].ToString()),
                            maxBayOld = int.Parse(dr["maxBayOld"].ToString()),
                            bufferReturn = bool.Parse(dr["bufferReturn"].ToString()),
                            bufferReturnOld = bool.Parse(dr["bufferReturnOld"].ToString()),
                            //pallets
                        };
                        if (!ContainBuffer(tempBuffer, Buffers))
                        {
                            Buffers.Add(tempBuffer);
                        }
                        if (!ContainBuffer(tempBuffer, AllBuffers))
                        {
                            AllBuffers.Add(tempBuffer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public void ReloadListTimeWork()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                TimeWorks.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "timeWork/getListTimeWork");
                request.Method = "GET";
                request.ContentType = @"application/json";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string result = reader.ReadToEnd();
                    DataTable timeWorks = JsonConvert.DeserializeObject<DataTable>(result);
                    if (timeWorks.Rows.Count > 0)
                    {
                        dtTimeWork tempTimeWork = new dtTimeWork
                        {
                            timeWorkId = 0,
                            timeWorkName = "No data"
                        };
                        if (!ContainTimeWork(tempTimeWork, TimeWorks))
                        {
                            TimeWorks.Add(tempTimeWork);
                        }
                        if (!ContainTimeWork(tempTimeWork, AllTimeWorks))
                        {
                            AllTimeWorks.Add(tempTimeWork);
                        }
                    }
                    foreach (DataRow dr in timeWorks.Rows)
                    {
                        dtTimeWork tempTimeWork = new dtTimeWork
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            timeWorkId = int.Parse(dr["timeWorkId"].ToString()),
                            timeWorkName = dr["timeWorkName"].ToString(),
                            startTime = dr["startTime"].ToString(),
                            endTime = dr["endTime"].ToString(),
                        };
                        if (!ContainTimeWork(tempTimeWork, TimeWorks))
                        {
                            TimeWorks.Add(tempTimeWork);
                        }
                        if (!ContainTimeWork(tempTimeWork, AllTimeWorks))
                        {
                            AllTimeWorks.Add(tempTimeWork);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public void ReloadDataGridCharge()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                dtRobotCharge robotCharge = new dtRobotCharge();
                if (statistics.cmbRobotRobotCharge.SelectedValue != null && !string.IsNullOrEmpty(statistics.cmbRobotRobotCharge.SelectedValue.ToString()))
                {
                    robotCharge.robotId = statistics.cmbRobotRobotCharge.SelectedValue.ToString();
                }

                if (statistics.cmbShiftRobotCharge.SelectedValue != null && !string.IsNullOrEmpty(statistics.cmbShiftRobotCharge.SelectedValue.ToString()) && int.Parse(statistics.cmbShiftRobotCharge.SelectedValue.ToString()) > 0)
                {
                    robotCharge.timeWorkId = int.Parse(statistics.cmbShiftRobotCharge.SelectedValue.ToString());
                }

                string jsonSend = JsonConvert.SerializeObject(robotCharge);
                this.listRobotCharge.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "reportRobot/getReportRobotCharge");
                request.Method = "POST";
                request.ContentType = "application/json";

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(jsonSend);
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
                    DataTable reportRobotCharge = JsonConvert.DeserializeObject<DataTable>(result);

                    if (reportRobotCharge.Rows.Count > 0)
                    {
                        statistics.grvReportRobotCharge.DataContext = reportRobotCharge;
                        foreach (DataRow dr in reportRobotCharge.Rows)
                        {
                            dtRobotCharge tempRobotCharage = new dtRobotCharge
                            {
                                creUsrId = int.Parse(dr["creUsrId"].ToString()),
                                creDt = dr["creDt"].ToString(),
                                updUsrId = int.Parse(dr["updUsrId"].ToString()),
                                updDt = dr["updDt"].ToString(),

                                robotChargeId = int.Parse(dr["robotChargeId"].ToString()),
                                robotTaskId = dr["robotTaskId"].ToString(),
                                chargeId = int.Parse(dr["chargeId"].ToString()),
                                timeWorkId = int.Parse(dr["timeWorkId"].ToString()),
                                rcBeginDatetime = dr["rpBeginDatetime"].ToString(),
                                rcEndDatetime = dr["rpEndDatetime"].ToString(),
                                currentBattery = dr["currentBattery"].ToString(),
                                robotChargeStatus = dr["robotChargeStatus"].ToString(),
                                robotId = dr["robotId"].ToString(),
                                procedureContent = dr["procedureContent"].ToString(),
                            };

                            if ((tempRobotCharage.rcBeginDatetime != "") && (tempRobotCharage.rcEndDatetime != ""))
                            {
                                DateTime dtBegin = DateTime.ParseExact(tempRobotCharage.rcBeginDatetime, "yyyy-MM-dd HH:mm:ss",
                                           System.Globalization.CultureInfo.InvariantCulture);

                                DateTime dtEnd = DateTime.ParseExact(tempRobotCharage.rcEndDatetime, "yyyy-MM-dd HH:mm:ss",
                                          System.Globalization.CultureInfo.InvariantCulture);

                                TimeSpan duration = dtEnd.Subtract(dtBegin);
                                tempRobotCharage.timeCharge = duration.ToString(@"hh\:mm");
                            }
                            else
                            {
                            }
                            if (!ContainRobotCharge(tempRobotCharage, listRobotCharge))
                            {
                                //tempRobotProcess.listOperationTypes = listOperationType;
                                this.listRobotCharge.Add(tempRobotCharage);
                            }
                        }
                        //foreach (DataGridViewRow dr in grvReportRobotCharge.Rows)
                        //{
                        //    if (dr.Cells["rcBeginDatetime"].Value.ToString() != "" && dr.Cells["rcEndDatetime"].Value.ToString() != "")
                        //    {
                        //        DateTime dtBegin = DateTime.ParseExact(dr.Cells["rcBeginDatetime"].Value.ToString(), "yyyy-MM-dd HH:mm:ss",
                        //                   System.Globalization.CultureInfo.InvariantCulture);

                        //        DateTime dtEnd = DateTime.ParseExact(dr.Cells["rcEndDatetime"].Value.ToString(), "yyyy-MM-dd HH:mm:ss",
                        //                  System.Globalization.CultureInfo.InvariantCulture);

                        //        TimeSpan duration = dtEnd.Subtract(dtBegin);

                        //        dr.Cells["timeCharge"].Value = duration.ToString(@"hh\:mm");
                        //    }
                        //}
                    }
                    else
                    {
                        listRobotCharge.Clear();
                    }
                }
                if (GroupedRobotCharge.IsEditingItem)
                {
                    GroupedRobotCharge.CommitEdit();
                }

                if (GroupedRobotCharge.IsAddingNew)
                {
                    GroupedRobotCharge.CommitNew();
                }

                GroupedRobotCharge.Refresh();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public void ReloadDataGridTask()
        {
            if (!Global_Object.ServerAlive())
            {
                return;
            }
            try
            {
                dtRobotProcess robotProcess = new dtRobotProcess();
                if (statistics.cmbRobot.SelectedValue != null && !string.IsNullOrEmpty(statistics.cmbRobot.SelectedValue.ToString()))
                {
                    robotProcess.robotId = statistics.cmbRobot.SelectedValue.ToString();
                }

                if (statistics.cmbDevice.SelectedValue != null && !string.IsNullOrEmpty(statistics.cmbDevice.SelectedValue.ToString()) && int.Parse(statistics.cmbDevice.SelectedValue.ToString()) > 0)
                {
                    robotProcess.deviceId = int.Parse(statistics.cmbDevice.SelectedValue.ToString());
                }

                if (statistics.cmbProduct.SelectedValue != null && !string.IsNullOrEmpty(statistics.cmbProduct.SelectedValue.ToString()) && int.Parse(statistics.cmbProduct.SelectedValue.ToString()) > 0)
                {
                    robotProcess.productId = int.Parse(statistics.cmbProduct.SelectedValue.ToString());
                }

                if (statistics.cmbProductDetail.SelectedValue != null && !string.IsNullOrEmpty(statistics.cmbProductDetail.SelectedValue.ToString()) && int.Parse(statistics.cmbProductDetail.SelectedValue.ToString()) > 0)
                {
                    robotProcess.productDetailId = int.Parse(statistics.cmbProductDetail.SelectedValue.ToString());
                }

                if (statistics.cmbOperationType.SelectedValue != null && !string.IsNullOrEmpty(statistics.cmbOperationType.SelectedValue.ToString()) && int.Parse(statistics.cmbOperationType.SelectedValue.ToString()) >= 0)
                {
                    robotProcess.operationType = int.Parse(statistics.cmbOperationType.SelectedValue.ToString());
                }
                else
                {
                    robotProcess.operationType = -1;
                }

                if (statistics.cmbBuffer.SelectedValue != null && !string.IsNullOrEmpty(statistics.cmbBuffer.SelectedValue.ToString()) && int.Parse(statistics.cmbBuffer.SelectedValue.ToString()) > 0)
                {
                    robotProcess.bufferId = int.Parse(statistics.cmbBuffer.SelectedValue.ToString());
                }

                if (statistics.cmbShift.SelectedValue != null && !string.IsNullOrEmpty(statistics.cmbShift.SelectedValue.ToString()) && int.Parse(statistics.cmbShift.SelectedValue.ToString()) > 0)
                {
                    robotProcess.timeWorkId = int.Parse(statistics.cmbShift.SelectedValue.ToString());
                    DateTime selectedDate = (DateTime)statistics.dtpActiveDate.SelectedDate;
                    string activeDate = selectedDate.Year + "-" + selectedDate.Month.ToString("00.") + "-" + selectedDate.Day.ToString("00.");
                    robotProcess.activeDate = activeDate;
                }

                string jsonSend = JsonConvert.SerializeObject(robotProcess);
                this.listRobotProcess.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://" + Properties.Settings.Default.serverIp + ":" + Properties.Settings.Default.serverPort + @"/robot/rest/" + "reportRobot/getReportRobotProcess");
                request.Method = "POST";
                request.ContentType = "application/json";

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(jsonSend);
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
                    DataTable reportRobotProcess = JsonConvert.DeserializeObject<DataTable>(result);
                    if (reportRobotProcess.Rows.Count > 0)
                    {
                        //statistics.grvReportRobotProcess.DataContext = reportRobotProcess;
                        foreach (DataRow dr in reportRobotProcess.Rows)
                        {
                            dtRobotProcess tempRobotProcess = new dtRobotProcess
                            {
                                creUsrId = int.Parse(dr["creUsrId"].ToString()),
                                creDt = dr["creDt"].ToString(),
                                updUsrId = int.Parse(dr["updUsrId"].ToString()),
                                updDt = dr["updDt"].ToString(),

                                robotProcessId = int.Parse(dr["robotProcessId"].ToString()),
                                robotTaskId = dr["robotTaskId"].ToString(),
                                gateKey = int.Parse(dr["gateKey"].ToString()),
                                planId = int.Parse(dr["planId"].ToString()),
                                deviceId = int.Parse(dr["deviceId"].ToString()),
                                productId = int.Parse(dr["productId"].ToString()),
                                productDetailId = int.Parse(dr["productDetailId"].ToString()),
                                bufferId = int.Parse(dr["bufferId"].ToString()),
                                palletId = int.Parse(dr["palletId"].ToString()),
                                operationType = int.Parse(dr["operationType"].ToString()),
                                rpBeginDatetime = dr["rpBeginDatetime"].ToString(),
                                rpEndDatetime = dr["rpEndDatetime"].ToString(),
                                robotProcessStastus = dr["robotProcessStastus"].ToString(),
                                orderContent = dr["orderContent"].ToString(),
                                robotId = dr["robotId"].ToString(),
                                timeWorkId = int.Parse(dr["timeWorkId"].ToString()),
                                activeDate = dr["activeDate"].ToString()

                                //locationBuffer = dr["locationBuffer"].ToString(),
                                //locationPallet = dr["locationPallet"].ToString(),
                                //orderContent
                            };
                            if (!ContainRobotProcess(tempRobotProcess, this.listRobotProcess))
                            {
                                //tempRobotProcess.listOperationTypes = listOperationType;
                                this.listRobotProcess.Add(tempRobotProcess);
                            }
                        }
                    }
                    else
                    {
                        this.listRobotProcess.Clear();
                        statistics.txtDetail.Text = "";
                    }
                }
                if (GroupedRobotProcess.IsEditingItem)
                {
                    GroupedRobotProcess.CommitEdit();
                }

                if (GroupedRobotProcess.IsAddingNew)
                {
                    GroupedRobotProcess.CommitNew();
                }

                GroupedRobotProcess.Refresh();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public void loadDetail()
        {
            try
            {
                if (statistics.grvReportRobotProcess.SelectedItem != null)
                {
                    dtRobotProcess row = statistics.grvReportRobotProcess.SelectedItem as dtRobotProcess;
                    //robotProcessIdSelect = int.Parse(row.Cells["robotProcessId"].Value.ToString());
                    string detail = row.orderContent.ToString();
                    Dictionary<string, string> strDetail = JsonConvert.DeserializeObject<Dictionary<string, string>>(detail);
                    statistics.txtDetail.Text = "";
                    foreach (var item in strDetail)
                    {
                        if (statistics.txtDetail.Text != "")
                        {
                            statistics.txtDetail.AppendText(Environment.NewLine);
                        }
                        statistics.txtDetail.AppendText(" - " + item.Key + ": " + item.Value);
                    }
                    statistics.txtDetail.Foreground = new SolidColorBrush(Colors.Blue);
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public bool ContainRobotProcess(dtRobotProcess tempOpe, List<dtRobotProcess> List)
        {
            foreach (dtRobotProcess temp in List)
            {
                if (temp.robotProcessId > 0)
                {
                    if (temp.robotProcessId == tempOpe.robotProcessId)
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

        public bool ContainRobotCharge(dtRobotCharge tempOpe, List<dtRobotCharge> List)
        {
            foreach (dtRobotCharge temp in List)
            {
                if (temp.robotChargeId > 0)
                {
                    if (temp.robotChargeId == tempOpe.robotChargeId)
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

        public bool ContainDevice(dtDevice tempOpe, ObservableCollection<dtDevice> List)
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

        public bool ContainProduct(dtProduct tempOpe, ObservableCollection<dtProduct> List)
        {
            foreach (dtProduct temp in List)
            {
                if (temp.productId > 0)
                {
                    if (temp.productId == tempOpe.productId)
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

        public bool ContainBuffer(dtBuffer tempOpe, ObservableCollection<dtBuffer> List)
        {
            foreach (dtBuffer temp in List)
            {
                if (temp.bufferId > 0)
                {
                    if (temp.bufferId == tempOpe.bufferId)
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

        public bool ContainProductDetail(dtProductDetail tempOpe, ObservableCollection<dtProductDetail> List)
        {
            foreach (dtProductDetail temp in List)
            {
                if (temp.productDetailId > 0)
                {
                    if (temp.productDetailId == tempOpe.productDetailId)
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

        public bool ContainTimeWork(dtTimeWork tempOpe, ObservableCollection<dtTimeWork> List)
        {
            foreach (dtTimeWork temp in List)
            {
                if (temp.timeWorkId > 0)
                {
                    if (temp.timeWorkId == tempOpe.timeWorkId)
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

        public bool ContainOperationType(dtOperationType tempOpe, ObservableCollection<dtOperationType> List)
        {
            foreach (dtOperationType temp in List)
            {
                if (temp.idOperationType > 0)
                {
                    if (temp.idOperationType == tempOpe.idOperationType)
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