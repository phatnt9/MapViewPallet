using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapViewPallet.MiniForm
{
    /// <summary>
    /// Interaction logic for OperationControl.xaml
    /// </summary>
    ///
    public partial class OperationControl : Window
    {
        //=================VARIABLE==================
        OperationModel operation_model;

        public OperationControl()
        {
            InitializeComponent();
            //===============DataGridView1========
            Shift1Dgv.SelectionMode = Shift2Dgv.SelectionMode = Shift3Dgv.SelectionMode = DataGridSelectionMode.Single;
            Shift1Dgv.SelectionUnit = Shift2Dgv.SelectionUnit = Shift3Dgv.SelectionUnit = DataGridSelectionUnit.FullRow;
            //Shift1Dgv.SelectedCellsChanged += Shift1Dgv_SelectedCellsChanged;
            //Shift1Dgv.SelectionChanged += Shift1Dgv_SelectionChanged;
            //Shift1Dgv.GotFocus += Shift1Dgv_GotFocus;
            //Shift1Dgv.LostFocus += Shift1Dgv_LostFocus;
            operation_model = new OperationModel();
            DataContext = operation_model;
        }
    }
    public class MachineList : List<string>
    {
        public MachineList()
        {
            this.Add("JUJENG");
            this.Add("POSIMAT1");
            this.Add("POSIMAT2");
            this.Add("POSIMAT3");
            this.Add("SANGTAO2");
            this.Add("SANGTAO3");
            this.Add("SANGTAO4");

            this.Add("MESPACK1");
            this.Add("MESPACK2");
            this.Add("AKASH1");
            this.Add("AKASH2");
            this.Add("AKASH3");
            this.Add("VOLPACK");
            this.Add("LEEPACK");
        }
    }
}
