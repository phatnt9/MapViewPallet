using MapViewPallet.MiniForm.Database;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        StatisticsModel statisticsModel;

        public Statistics()
        {
            InitializeComponent();
            Loaded += Statistics_Loaded;
            statisticsModel = new StatisticsModel(this);
            DataContext = statisticsModel;
        }

        private void Statistics_Loaded(object sender, RoutedEventArgs e)
        {
            statisticsModel.ReloadListProduct();
            statisticsModel.ReloadListProductDetail();
            statisticsModel.ReloadListOperationType();
            statisticsModel.ReloadListRobot();
            statisticsModel.ReloadListDevice();
            statisticsModel.ReloadListBuffer();
            statisticsModel.ReloadListTimeWork();
        }
        

        private void CmbDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            statisticsModel.ReloadListProduct();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach(dtRobotProcess item in statisticsModel.listRobotProcess)
            {
                Console.WriteLine(item.operationType);
            }
        }

        private void CmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int productDetail = -1;
            if (cmbProductDetail.SelectedValue != null && cmbProductDetail.SelectedValue.ToString() != "")
            {
                productDetail = int.Parse(cmbProductDetail.SelectedValue.ToString());
            }
            statisticsModel.ReloadListProductDetail();
            if (productDetail != -1)
            {
                cmbProductDetail.SelectedValue = productDetail;
            }
        }

        private void CmbDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            int product = -1;
            if (cmbProduct.SelectedValue != null && cmbProduct.SelectedValue.ToString() != "")
            {
                product = int.Parse(cmbProduct.SelectedValue.ToString());
            }
            statisticsModel.ReloadListProduct();
            if (product != -1)
            {
                cmbProduct.SelectedValue = product;
            }
        }

        private void CmbShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbShift.SelectedValue != null && int.Parse(cmbShift.SelectedValue.ToString()) > 0)
            {
                dtpActiveDate.IsEnabled = true;
            }
            else
            {
                dtpActiveDate.IsEnabled = false;
            }
        }
        

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            statisticsModel.ReloadDataGridTask();
        }
    }
}
