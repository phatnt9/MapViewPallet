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
            _shift1_Operations = _shift2_Operations = _shift3_Operations = new List<Operation>
            {
                //##############
                new Operation { Machine = temp[0], PMs = PackingMaterials.Bottle, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[0], PMs = PackingMaterials.Cap, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[0], PMs = PackingMaterials.Pouch, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[0], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[0], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[1], PMs = PackingMaterials.Bottle, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[1], PMs = PackingMaterials.Cap, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[1], PMs = PackingMaterials.Pouch, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[1], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[1], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[2], PMs = PackingMaterials.Bottle, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[2], PMs = PackingMaterials.Cap, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[2], PMs = PackingMaterials.Pouch, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[2], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[2], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[3], PMs = PackingMaterials.Bottle, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[3], PMs = PackingMaterials.Cap, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[3], PMs = PackingMaterials.Pouch, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[3], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[3], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[4], PMs = PackingMaterials.Bottle, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[4], PMs = PackingMaterials.Cap, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[4], PMs = PackingMaterials.Pouch, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[4], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[4], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[5], PMs = PackingMaterials.Bottle, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[5], PMs = PackingMaterials.Cap, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[5], PMs = PackingMaterials.Pouch, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[5], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[5], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[6], PMs = PackingMaterials.Bottle, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[6], PMs = PackingMaterials.Cap, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[6], PMs = PackingMaterials.Pouch, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[6], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[6], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[7], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[7], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[8], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[8], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[9], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[9], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[10], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[10], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[11], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[11], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[12], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[12], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                //##############
                new Operation { Machine = temp[13], PMs = PackingMaterials.Bottle, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[13], PMs = PackingMaterials.Cap, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[13], PMs = PackingMaterials.Pouch, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[13], PMs = PackingMaterials.Outer, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
                new Operation { Machine = temp[13], PMs = PackingMaterials.Divider, Quantity = 0, Serial = Codes.Empty, Buffered = 0 },
            };
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
