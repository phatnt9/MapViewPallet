using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MapViewPallet.MiniForm
{
    public class Machine
    {
        public string Name;
        public string BufferArea;
        public List<string> ActivePMs; //Cac loai hang dang chay Cap/Bottle/Pouch/Divider/Outer
        public Machine(string name, string bufferArea, List<string> activePMs)
        {
            Name = name;
            BufferArea = bufferArea;
            ActivePMs = activePMs;
        }

    }


    public class MachineList : List<Machine>
    {
        public MachineList()
        {
            Add(new Machine("JUJENG", "Buffer_A", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            Add(new Machine("POSIMAT1", "Buffer_B", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            Add(new Machine("POSIMAT2", "Buffer_C", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            Add(new Machine("POSIMAT3", "Buffer_D", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            Add(new Machine("SANGTAO2", "Buffer_E", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            Add(new Machine("SANGTAO3", "Buffer_F", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            Add(new Machine("SANGTAO4", "Buffer_G", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
            Add(new Machine("MESPACK1", "Buffer_H", new List<string>(new string[] { "Divider", "Outer" })));
            Add(new Machine("MESPACK2", "Buffer_I", new List<string>(new string[] { "Divider", "Outer" })));
            Add(new Machine("AKASH1", "Buffer_J", new List<string>(new string[] { "Divider", "Outer" })));
            Add(new Machine("AKASH2", "Buffer_K", new List<string>(new string[] { "Divider", "Outer" })));
            Add(new Machine("AKASH3", "Buffer_L", new List<string>(new string[] { "Divider", "Outer" })));
            Add(new Machine("VOLPACK", "Buffer_M", new List<string>(new string[] { "Divider", "Outer" })));
            Add(new Machine("LEEPACK", "Buffer_N", new List<string>(new string[] { "Cap", "Bottle", "Pouch", "Divider", "Outer" })));
        }
    }

    public class OperationModel
    {
        public ICollectionView Shift1Data { get; private set; }
        public ICollectionView Shift2Data { get; private set; }
        public ICollectionView Shift3Data { get; private set; }

        public ICollectionView GroupedMachines_S1 { get; private set; }
        public ICollectionView GroupedMachines_S2 { get; private set; }
        public ICollectionView GroupedMachines_S3 { get; private set; }

        //***************VARIABLES*********************
        public List<Operation> _shift1_Operations;
        public List<Operation> _shift2_Operations;
        public List<Operation> _shift3_Operations;

        public OperationModel()
        {
            MachineList temp = new MachineList();
            /*
             * Connect to DB and Add Machine Item to this MachineList
             */

            List<Operation> initialItem = new List<Operation>();

            foreach (Machine item in temp)
            {
                foreach (string PMs in item.ActivePMs)
                {
                    initialItem.Add(new Operation
                    {
                        Machine = item.Name,
                        PMs = PMs,
                        Quantity = 0,
                        Serial = Codes.Empty,
                        Area = item.BufferArea,
                        Buffered = 0
                    });
                }
            }

            _shift1_Operations = _shift2_Operations = _shift3_Operations = initialItem;
            //#######
            Shift1Data = CollectionViewSource.GetDefaultView(_shift1_Operations);
            Shift2Data = CollectionViewSource.GetDefaultView(_shift2_Operations);
            Shift3Data = CollectionViewSource.GetDefaultView(_shift3_Operations);

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

        public void AddOperationShift1(Operation x)
        {
            _shift1_Operations.Add(x);
            Shift1Data.Refresh();
            //######
            _shift2_Operations.Add(x);
            Shift2Data.Refresh();
            //######
            _shift3_Operations.Add(x);
            Shift3Data.Refresh();
        }

    }
}
