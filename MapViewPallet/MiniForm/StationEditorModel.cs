using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MapViewPallet.MiniForm
{
    class StationEditorModel : NotifyUIBase
    {

        StationEditor stationEditor;

        public ListCollectionView GroupedPallets { get; private set; }

        public List<dtPallet> palletsList;


        public StationEditorModel (StationEditor stationEditor)
        {
            this.stationEditor = stationEditor;
            palletsList = new List<dtPallet>();
            GroupedPallets = (ListCollectionView)CollectionViewSource.GetDefaultView(palletsList);
        }

        public void ReloadListPallets(int bufferId)
        {
            //try
            {
                palletsList.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Global_Object.url + "pallet/getListPalletBufferId");
                request.Method = "POST";
                request.ContentType = @"application/json";
                dynamic postApiBody = new JObject();
                postApiBody.bufferId = bufferId;
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

                    DataTable pallets = JsonConvert.DeserializeObject<DataTable>(result);
                    foreach (DataRow dr in pallets.Rows)
                    {
                        dtPallet tempPallet = new dtPallet
                        {
                            creUsrId = int.Parse(dr["creUsrId"].ToString()),
                            creDt = dr["creDt"].ToString(),
                            updUsrId = int.Parse(dr["updUsrId"].ToString()),
                            updDt = dr["updDt"].ToString(),
                            palletId = int.Parse(dr["palletId"].ToString()),
                            deviceBufferId = int.Parse(dr["deviceBufferId"].ToString()),
                            bufferId = int.Parse(dr["bufferId"].ToString()),
                            planId = int.Parse(dr["planId"].ToString()),
                            row = int.Parse(dr["row"].ToString()),
                            bay = int.Parse(dr["bay"].ToString()),
                            dataPallet = dr["dataPallet"].ToString(),
                            palletStatus = dr["palletStatus"].ToString(),
                            deviceId = int.Parse(dr["deviceId"].ToString()),
                            deviceName = dr["deviceName"].ToString(),
                            productId = int.Parse(dr["productId"].ToString()),
                            productName = dr["productName"].ToString(),
                            productDetailId = int.Parse(dr["productDetailId"].ToString()),
                            productDetailName = dr["productDetailName"].ToString(),
                        };
                        if (!ContainPallet(tempPallet, palletsList))
                        {
                            palletsList.Add(tempPallet);
                        }
                    }
                }
                if (GroupedPallets.IsEditingItem)
                    GroupedPallets.CommitEdit();
                if (GroupedPallets.IsAddingNew)
                    GroupedPallets.CommitNew();
                GroupedPallets.Refresh();
                if (stationEditor.PalletsListDg.HasItems)
                {
                    stationEditor.PalletsListDg.SelectedItem = stationEditor.PalletsListDg.Items[0];
                    //devicesManagement.PalletsListDg.ScrollIntoView(devicesManagement.PalletsListDg.SelectedItem);
                }
            }
            //catch
            {
            }
        }

        public bool ContainPallet(dtPallet tempOpe, List<dtPallet> List)
        {
            foreach (dtPallet temp in List)
            {
                if (temp.palletId > 0)
                {
                    if (temp.palletId == tempOpe.palletId)
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
