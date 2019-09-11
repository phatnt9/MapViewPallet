using MapViewPallet.MiniForm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MapViewPallet.Model;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using MapViewPallet.MiniForm.MicsWpfForm;
using System.Windows.Forms;

namespace MapViewPallet.View
{
    /// <summary>
    /// Interaction logic for MVVM_PlanManagement.xaml
    /// </summary>
    public partial class MVVM_PlanManagement : Window
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum AppStatus
        {
            Ready,
            Exporting,
            Completed,
            Finished,
            Cancelled,
            Error,
        }
        
        MVVM_PlanManagementModel model;
        public MVVM_PlanManagement()
        {
            InitializeComponent();
            dp_planManagement.SelectedDate = DateTime.Now;
            Loaded += MVVM_PlanManagement_Loaded;
            model = new MVVM_PlanManagementModel();
            DataContext = model;
        }

        private void MVVM_PlanManagement_Loaded(object sender, RoutedEventArgs e)
        {
            model.ReloadListPlan((DateTime)dp_planManagement.SelectedDate);
        }

        private void Dp_planManagement_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                model.ReloadListPlan((DateTime)dp_planManagement.SelectedDate);
            }
            catch
            {

            }
        }

        private void Btn_createPlanPallet_Click(object sender, RoutedEventArgs e)
        {
            string result = model.CreatePlanPallet(DataGridPlan.SelectedItem as dtPlan);
            if (result != "")
            {
                rtb_log.AppendText(result);
                rtb_log.AppendText("\u2028");
                rtb_log.ScrollToEnd();
            }
        }

        private void Tb_LocalSearch_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                FilterLocal_Click(sender, e);
                if (e.Key == Key.Enter)
                {
                    FilterLocal_Click(sender, e);
                    return;
                }
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void FilterLocal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //mainModel.ReloadListProfileRFDGV(tb_nameSearch.Text, tb_pinSearch.Text, tb_adnoSearch.Text);
                DataGridPlan.Items.Filter = (obj) =>
                (
                (((dtPlan)obj).deviceName.ToLower().Contains(tb_search.Text.ToString().ToLower())) ||
                (((dtPlan)obj).productName.ToLower().Contains(tb_search.Text.ToString().ToLower())) ||
                (((dtPlan)obj).productDetailName.ToLower().Contains(tb_search.Text.ToString().ToLower()))
                );
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Btn_importPlan_Click(object sender, RoutedEventArgs e)
        {
            ImportPlanForm importPlanForm = new ImportPlanForm();
            importPlanForm.ShowDialog();
            model.ReloadListPlan((DateTime)dp_planManagement.SelectedDate);
        }

        private void Btn_deletePlanPallet_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show
                        (
                        String.Format(Global_Object.messageDeleteConfirm, "Plans"),
                        Global_Object.messageTitileWarning, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes
                        )
            {
                try
                {
                    List<dtTempPlan> listToDetele = new List<dtTempPlan>();
                    if (DataGridPlan.SelectedItems.Count > 0)
                    {
                        foreach (dtPlan plan in DataGridPlan.SelectedItems)
                        {
                            listToDetele.Add(new dtTempPlan(plan));
                        }
                        string jsonData = JsonConvert.SerializeObject(listToDetele);
                        string contentJson = Global_Object.RequestDataAPI(jsonData, "plan/deleteListPlan", Global_Object.RequestMethod.DELETE);
                        dynamic response = JsonConvert.DeserializeObject(contentJson);

                        if (response != null)
                        {
                            if (response == 1)
                            {
                                System.Windows.MessageBox.Show("Plans deleted successfully!");
                                model.ReloadListPlan((DateTime)dp_planManagement.SelectedDate);
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Cannot Delete!");
                            }
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Nothing to delete!");
                    }
                }
                catch (Exception ex)
                {
                    logFile.Error(ex.Message);
                }
            }
        }
    }
}
