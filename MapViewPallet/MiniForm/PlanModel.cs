
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
using System.Windows.Data;

namespace MapViewPallet.MiniForm
{
    class Owner
    {
        int ID { get; }
        string Name { get; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    class House
    {
        int ID { get; }
        Owner HouseOwner { get; set; }
    }

    class ViewModel
    {
        ObservableCollection<Owner> Owners;
        ObservableCollection<House> Houses;
    }

    public class PlanModel : BridgeClientRequest
    {

        public ICollectionView Shift1Data { get; private set; }
        public ICollectionView Shift2Data { get; private set; }
        public ICollectionView Shift3Data { get; private set; }

        public ICollectionView GroupedDevices_S1 { get; private set; }
        public ICollectionView GroupedDevices_S2 { get; private set; }
        public ICollectionView GroupedDevices_S3 { get; private set; }

        //***************VARIABLES*********************
        public DeviceList deviceList;
        List<Plan> listPlan;

        public List<Plan> _shift1_Operations;
        public List<Plan> _shift2_Operations;
        public List<Plan> _shift3_Operations;
        public bool Contain(Plan tempOpe)
        {
            foreach (Plan temp in listPlan)
            {
                if (temp.DeviceName == tempOpe.DeviceName)
                {
                    if (temp.ProductName == tempOpe.ProductName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void AddPlan(Plan tempOpe)
        {
            if (listPlan.Count != 0)
            {
                //Console.WriteLine(Contain(tempOpe));
                if (!Contain(tempOpe))
                {
                    //Console.WriteLine("Add: " + tempOpe.Machine + "-" + tempOpe.PackingMaterial);
                    listPlan.Add(tempOpe);
                }
                else
                {
                    //Console.WriteLine("Trùng" + tempOpe.Machine + "-" + tempOpe.PackingMaterial);
                }
            }
            else
            {
                //Console.WriteLine("Add Operation: " + tempOpe.Machine + "-" + tempOpe.PackingMaterial + "");
                listPlan.Add(tempOpe);
            }
        }
        public PlanModel()
        {
            deviceList = new DeviceList();
            //###############################
            listPlan = new List<Plan>();
            _shift1_Operations = _shift2_Operations = _shift3_Operations = listPlan;
            //#######
            GroupedDevices_S1 = new ListCollectionView(_shift1_Operations);
            GroupedDevices_S1.GroupDescriptions.Add(new PropertyGroupDescription("DeviceName"));
            //#######
            GroupedDevices_S2 = new ListCollectionView(_shift2_Operations);
            GroupedDevices_S2.GroupDescriptions.Add(new PropertyGroupDescription("DeviceName"));
            //#######
            GroupedDevices_S3 = new ListCollectionView(_shift3_Operations);
            GroupedDevices_S3.GroupDescriptions.Add(new PropertyGroupDescription("DeviceName"));
        }
        public void RefreshData()
        {
            deviceList.UpdateDevicesList();
            foreach (Device device in deviceList.listDevices)
            {
                foreach (Product product in device.listProducts)
                {
                    Plan tempOpe = new Plan
                    {
                        DeviceId = device.Id,
                        DeviceName = device.Name,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 0,
                        Serials = product.listSerials,
                        Buffered = 0
                    };
                    AddPlan(tempOpe);
                }
            }
            GroupedDevices_S1.Refresh();
            GroupedDevices_S2.Refresh();
            GroupedDevices_S3.Refresh();
        }


    }
}
