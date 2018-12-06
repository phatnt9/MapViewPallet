
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MapViewPallet.MiniForm
{


    public class Machine : BridgeClientRequest
    {
        public int deviceId;
        public string deviceName;
        public string bufferArea = "";
        public List<string> activePMs; //Cac loai hang dang chay Cap/Bottle/Pouch/Divider/Outer
        //########-----METHOD-----#######
        public Machine(int deviceId,string deviceName)
        {
            this.deviceId = deviceId;
            this.deviceName = deviceName;
            activePMs = new List<string>();
            dynamic postApiBody = new JObject();
            postApiBody.deviceId = deviceId;
            PostCallAPI("http://localhost:8081/robot/rest/product/getListDeviceProductByDeviceId", JsonConvert.SerializeObject(postApiBody));
        }

        public override void ReceiveResponseHandler(string msg)
        {
            dynamic stuff = JsonConvert.DeserializeObject(msg);
            int pmsCount = stuff.Count;
            foreach (dynamic item in stuff)
            {
                string productName = item.productName;
                activePMs.Add(productName);
            }
        }

    }
    public class MachineList : List<Machine>
    {

        public MachineList()
        {

        }
        void Setdata()
        {
            //Add(new Machine("JUJENG", "Buffer_A", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            //Add(new Machine("POSIMAT1", "Buffer_B", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            //Add(new Machine("POSIMAT2", "Buffer_C", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            //Add(new Machine("POSIMAT3", "Buffer_D", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            //Add(new Machine("SANGTAO2", "Buffer_E", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            //Add(new Machine("SANGTAO3", "Buffer_F", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            //Add(new Machine("SANGTAO4", "Buffer_G", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            //Add(new Machine("MESPACK1", "Buffer_H", new List<string>(new string[] { "Divider", "Outer" })));
            //Add(new Machine("MESPACK2", "Buffer_I", new List<string>(new string[] { "Divider", "Outer" })));
            //Add(new Machine("AKASH1", "Buffer_J", new List<string>(new string[] { "Divider", "Outer" })));
            //Add(new Machine("AKASH2", "Buffer_K", new List<string>(new string[] { "Divider", "Outer" })));
            //Add(new Machine("AKASH3", "Buffer_L", new List<string>(new string[] { "Divider", "Outer" })));
            //Add(new Machine("VOLPACK", "Buffer_M", new List<string>(new string[] { "Divider", "Outer" })));
            //Add(new Machine("LEEPACK", "Buffer_N", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
        }
    }

    public class OperationModelDataService : BridgeClientRequest
    {
        public MachineList machineList;
        public OperationModelDataService()
        {
            machineList = new MachineList();
            UpdateMachineList();
        }
        public void UpdateMachineList()
        {
            Console.WriteLine("Call API");
            GetCallAPI("http://localhost:8081/robot/rest/device/getListDevice");
        }
        public override void ReceiveResponseHandler(string msg)
        {
            dynamic stuff = JsonConvert.DeserializeObject(msg);
            foreach (dynamic item in stuff)
            {
                machineList.Add(new Machine((int)item.deviceId, (string)item.deviceName));
            }
        }
    }

    public class OperationModel : OperationModelDataService
    {
        public ICollectionView Shift1Data { get; private set; }
        public ICollectionView Shift2Data { get; private set; }
        public ICollectionView Shift3Data { get; private set; }

        public ICollectionView GroupedMachines_S1 { get; private set; }
        public ICollectionView GroupedMachines_S2 { get; private set; }
        public ICollectionView GroupedMachines_S3 { get; private set; }

        //***************VARIABLES*********************
        List<Operation> listItem;

        public List<Operation> _shift1_Operations;
        public List<Operation> _shift2_Operations;
        public List<Operation> _shift3_Operations;

        public OperationModel()
        {
            listItem = new List<Operation>();
            _shift1_Operations = _shift2_Operations = _shift3_Operations = listItem;
            //#######
            GroupedMachines_S1 = new ListCollectionView(_shift1_Operations);
            GroupedMachines_S1.GroupDescriptions.Add(new PropertyGroupDescription("Machine"));
            //#######
            GroupedMachines_S2 = new ListCollectionView(_shift2_Operations);
            GroupedMachines_S2.GroupDescriptions.Add(new PropertyGroupDescription("Machine"));
            //#######
            GroupedMachines_S3 = new ListCollectionView(_shift3_Operations);
            GroupedMachines_S3.GroupDescriptions.Add(new PropertyGroupDescription("Machine"));
        }
        

        public void RefreshData ()
        {
            UpdateMachineList();
            foreach (Machine item in machineList)
            {
                foreach (string PMs in item.activePMs)
                {
                    Operation tempOp = new Operation
                    {
                        Machine = item.deviceName,
                        PMs = PMs,
                        Quantity = 0,
                        Serial = Codes.Empty,
                        Area = item.bufferArea,
                        Buffered = 0
                    };
                    if (!listItem.Contains(tempOp))
                    {
                        listItem.Add(new Operation
                        {
                            Machine = item.deviceName,
                            PMs = PMs,
                            Quantity = 0,
                            Serial = Codes.Empty,
                            Area = item.bufferArea,
                            Buffered = 0
                        });
                    }
                }
            }
            GroupedMachines_S1.Refresh();
            GroupedMachines_S2.Refresh();
            GroupedMachines_S3.Refresh();
        }
        

    }
}
