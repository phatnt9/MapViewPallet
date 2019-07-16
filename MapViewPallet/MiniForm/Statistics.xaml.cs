using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Excel = Microsoft.Office.Interop.Excel;

namespace MapViewPallet.MiniForm
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        private static readonly log4net.ILog logFile = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private StatisticsModel statisticsModel;

        public Statistics(string cultureName = null)
        {
            InitializeComponent();
            ApplyLanguage(cultureName);
            Loaded += Statistics_Loaded;
            StatisticsTabControl.SelectionChanged += StatisticsTabControl_SelectionChanged;
            statisticsModel = new StatisticsModel(this);
            DataContext = statisticsModel;
        }

        private void StatisticsTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TabControl)
            {
                switch (((e.Source as System.Windows.Controls.TabControl).SelectedIndex))
                {
                    case 0:
                    {
                        statisticsModel.ReloadListProduct();
                        statisticsModel.ReloadListProductDetail();
                        statisticsModel.ReloadListOperationType();
                        statisticsModel.ReloadListRobot((e.Source as System.Windows.Controls.TabControl).SelectedIndex);
                        statisticsModel.ReloadListDevice();
                        statisticsModel.ReloadListBuffer();
                        statisticsModel.ReloadListTimeWork();
                        break;
                    }
                    case 1:
                    {
                        statisticsModel.ReloadListRobot(((e.Source as System.Windows.Controls.TabControl).SelectedIndex));
                        break;
                    }
                }
            }
        }

        public void ApplyLanguage(string cultureName = null)
        {
            if (cultureName != null)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            }

            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "vi-VN":
                    dict.Source = new Uri("..\\Lang\\Vietnamese.xaml", UriKind.Relative);
                    break;
                // ...
                default:
                    dict.Source = new Uri("..\\Lang\\English.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void Statistics_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                statisticsModel.ReloadListProduct();
                statisticsModel.ReloadListProductDetail();
                statisticsModel.ReloadListOperationType();
                statisticsModel.ReloadListRobot(0);
                statisticsModel.ReloadListDevice();
                statisticsModel.ReloadListBuffer();
                statisticsModel.ReloadListTimeWork();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void CmbDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            statisticsModel.ReloadListProduct();
        }

        private void CmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void CmbDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void CmbShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            statisticsModel.ReloadDataGridTask();
        }

        private void BtnSearchRobotCharge_Click(object sender, RoutedEventArgs e)
        {
            statisticsModel.ReloadDataGridCharge();
        }

        private void BtxExportRobotCharge_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Excel.Worksheet worksheet = null;

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 2;
                if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.Cursor = System.Windows.Input.Cursors.Wait;

                    excel.DisplayAlerts = false;
                    worksheet = workbook.ActiveSheet;
                    //worksheet.Name = "Report Robot Process";

                    worksheet.Cells[1, 1] = "Report Robot Process".ToUpper();
                    worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 12]].Merge();
                    worksheet.Cells[1, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Cells[1, 1].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    worksheet.Cells[1, 1].Font.Bold = true;
                    worksheet.Cells[1, 1].Font.Size = 24;
                    worksheet.Cells[1, 1].RowHeight = 70;

                    worksheet.Cells[2, 1].RowHeight = 20;

                    worksheet.Cells[3, 1] = "Robot";
                    worksheet.Cells[3, 2] = "Robot Task Id";
                    worksheet.Cells[3, 3] = "Gate Key";
                    worksheet.Cells[3, 4] = "Device";
                    worksheet.Cells[3, 5] = "Product";
                    worksheet.Cells[3, 6] = "Product Detail";
                    worksheet.Cells[3, 7] = "Buffer";
                    worksheet.Cells[3, 8] = "Operation Type";
                    worksheet.Cells[3, 9] = "Status";
                    worksheet.Cells[3, 10] = "Shift";
                    worksheet.Cells[3, 11] = "Active Date";
                    worksheet.Cells[3, 12] = "Detail";

                    worksheet.Range[worksheet.Cells[3, 1], worksheet.Cells[3, 12]].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    worksheet.Range[worksheet.Cells[3, 1], worksheet.Cells[3, 12]].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    worksheet.Range[worksheet.Cells[3, 1], worksheet.Cells[3, 12]].Font.Bold = true;
                    worksheet.Range[worksheet.Cells[3, 1], worksheet.Cells[3, 12]].Font.Size = 15;
                    worksheet.Range[worksheet.Cells[3, 1], worksheet.Cells[3, 12]].RowHeight = 35;

                    int cellRowIndex = 4;
                    int cellColumnIndex = 1;

                    for (int i = 0; i < grvReportRobotProcess.Items.Count; i++)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (j == 0)//Robot
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }
                            if (j == 1) //Robot Task Id
                            {
                                //worksheet.Cells[cellRowIndex, cellColumnIndex] = grvReportRobotProcess.Rows[i].Cells["robotTaskId"].Value.ToString();
                                //worksheet.Cells[cellRowIndex, cellColumnIndex] = statisticsModel.listRobotProcess[i].robotTaskId.ToString();
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }
                            if (j == 2) //gateKey
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }

                            if (j == 3)//Device
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }

                            if (j == 4)//Product
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }

                            if (j == 5)//Product Detail
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }

                            if (j == 6)//Buffer
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }

                            if (j == 7)//Operation Typ
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }

                            if (j == 8)//Status
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }

                            if (j == 9) //Shift
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }

                            if (j == 10) //Active Date
                            {
                                worksheet.Cells[cellRowIndex, cellColumnIndex] = GetCellValue(grvReportRobotProcess, i, j);
                            }

                            if (j == 11) //Detail
                            {
                                Dictionary<string, string> strDetail = JsonConvert.DeserializeObject<Dictionary<string, string>>(statisticsModel.listRobotProcess[i].orderContent.ToString());
                                string contentDetail = "";
                                foreach (var item in strDetail)
                                {
                                    if (contentDetail != "")
                                    {
                                        contentDetail += Environment.NewLine;
                                    }
                                    contentDetail += " - " + item.Key + ": " + item.Value;
                                }

                                worksheet.Cells[cellRowIndex, cellColumnIndex] = contentDetail;
                                //worksheet.Range[worksheet.Cells[cellRowIndex, cellColumnIndex], worksheet.Cells[cellRowIndex, cellColumnIndex]].WrapText = false;
                            }
                            worksheet.Cells[cellRowIndex, cellColumnIndex].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            worksheet.Cells[cellRowIndex, cellColumnIndex].RowHeight = 30;
                            cellColumnIndex++;
                        }

                        cellColumnIndex = 1;
                        cellRowIndex++;
                    }

                    worksheet.Columns.AutoFit();

                    workbook.SaveAs(saveDialog.FileName, Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                    System.Windows.Forms.MessageBox.Show("Export Successful");
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                logFile.Error(ex.Message);
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
                this.Cursor = System.Windows.Input.Cursors.Arrow;
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            string test = GetCellValue(grvReportRobotProcess, 0, 0);
            string test2 = GetCellValue(grvReportRobotProcess, 1, 0);
            string test3 = GetCellValue(grvReportRobotProcess, 2, 0);
        }

        public IEnumerable<DataGridRow> GetDataGridRows(System.Windows.Controls.DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource)
            {
                yield return null;
            }

            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row)
                {
                    yield return row;
                }
            }
        }

        private void GrvReportRobotProcess_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                txtDetail.Text = "";
                Console.WriteLine(e.ToString());
                statisticsModel.loadDetail();
            }
            catch (Exception ex)
            {
                logFile.Error(ex.Message);
            }
        }

        public string GetCellValue(System.Windows.Controls.DataGrid datagrid, int row, int column)
        {
            var cellInfo = new DataGridCellInfo(
                datagrid.Items[row], datagrid.Columns[column]);

            System.Windows.Controls.DataGridCell cell = null;
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);
            if (cellContent != null)
            {
                cell = (System.Windows.Controls.DataGridCell)cellContent.Parent;
            }

            if (cell == null)
            {
                return string.Empty;
            }

            // if DataGridTextColumn / DataGridComboBoxColumn is used
            // or AutoGeneratedColumns is True
            if (cell.Content is TextBlock)
            {
                return ((TextBlock)cell.Content).Text;
            }
            else if (cell.Content is System.Windows.Controls.ComboBox)
            {
                return ((System.Windows.Controls.ComboBox)cell.Content).Text;
            }

            // if DataGridTemplateColumn is used
            // assuming cells are either TextBox, TextBlock or ComboBox. Other Types could be handled the same way.
            else
            {
                var txtPresenter = FindVisualChild<System.Windows.Controls.TextBox>((ContentPresenter)cell.Content);
                if (txtPresenter != null)
                {
                    return txtPresenter.Text;
                }

                var txbPresenter = FindVisualChild<TextBlock>((ContentPresenter)cell.Content);
                if (txbPresenter != null)
                {
                    return txbPresenter.Text;
                }

                var cmbPresenter = FindVisualChild<System.Windows.Controls.ComboBox>((ContentPresenter)cell.Content);
                if (cmbPresenter != null)
                {
                    return cmbPresenter.Text;
                }
            }
            return string.Empty;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}