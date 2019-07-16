using System.Collections.Generic;

namespace MapViewPallet.MiniForm
{
    public class Plan : dtPlan
    {
        private List<dtProductDetail> pListProductDetails;
        public List<dtProductDetail> listProductDetails { get => pListProductDetails; set { pListProductDetails = value; RaisePropertyChanged("listProductDetails"); } }

        public Plan()
        {
            listProductDetails = new List<dtProductDetail>();
        }

        public Plan(dynamic jsonPlan)
        {
            listProductDetails = new List<dtProductDetail>();
            creUsrId = (int)jsonPlan.creUsrId;
            creDt = (string)jsonPlan.creDt;
            updUsrId = (int)jsonPlan.updUsrId;
            updDt = (string)jsonPlan.updDt;

            planId = (int)jsonPlan.planId;
            deviceProductId = (int)jsonPlan.deviceProductId;
            timeWorkId = (int)jsonPlan.timeWorkId;
            productDetailId = (int)jsonPlan.productDetailId;

            palletAmount = (int)jsonPlan.palletAmount;
            palletUse = (int)jsonPlan.palletUse;
            palletMiss = (int)jsonPlan.palletMiss;
            activeDate = (string)jsonPlan.activeDate;

            deviceId = (int)jsonPlan.deviceId;
            productId = (int)jsonPlan.productId;
            deviceName = (string)jsonPlan.deviceName;
            imageDeviceUrl = (string)jsonPlan.imageDeviceUrl;
            productName = (string)jsonPlan.productName;
            imageProductUrl = (string)jsonPlan.imageProductUrl;
            productDetailName = (string)jsonPlan.productDetailName;
            listProductDetails.Add(new dtProductDetail
            {
                productDetailId = (int)jsonPlan.productDetailId,
                productDetailName = (string)jsonPlan.productDetailName,
                productId = (int)jsonPlan.productId
            });
        }
    }
}