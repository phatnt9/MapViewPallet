using System.Collections.Generic;

namespace MapViewPallet.MiniForm
{
    public class dtRobotProcess : userModel
    {
        private int _robotProcessId;
        private string _robotTaskId;
        private int _gateKey;
        private int _planId;
        private int _deviceId;
        private int _productId;
        private int _productDetailId;
        private int _bufferId;
        private int _palletId;
        private int _operationType;
        private string _rpBeginDatetime;
        private string _rpEndDatetime;
        private string _orderContent;
        private string _robotProcessStastus;

        private string _robotId;
        private int _timeWorkId;
        private string _activeDate;

        public int robotProcessId { get => _robotProcessId; set => _robotProcessId = value; }
        public string robotTaskId { get => _robotTaskId; set => _robotTaskId = value; }
        public int gateKey { get => _gateKey; set => _gateKey = value; }
        public int planId { get => _planId; set => _planId = value; }
        public int deviceId { get => _deviceId; set => _deviceId = value; }
        public int productId { get => _productId; set => _productId = value; }
        public int productDetailId { get => _productDetailId; set => _productDetailId = value; }
        public int bufferId { get => _bufferId; set => _bufferId = value; }
        public int palletId { get => _palletId; set => _palletId = value; }
        public int operationType { get => _operationType; set => _operationType = value; }
        public string rpBeginDatetime { get => _rpBeginDatetime; set => _rpBeginDatetime = value; }
        public string rpEndDatetime { get => _rpEndDatetime; set => _rpEndDatetime = value; }
        public string orderContent { get => _orderContent; set => _orderContent = value; }
        public string robotProcessStastus { get => _robotProcessStastus; set => _robotProcessStastus = value; }
        public string robotId { get => _robotId; set => _robotId = value; }
        public int timeWorkId { get => _timeWorkId; set => _timeWorkId = value; }
        public string activeDate { get => _activeDate; set => _activeDate = value; }

        private List<dtOperationType> pListOperationTypes;
        public List<dtOperationType> listOperationTypes { get => pListOperationTypes; set { pListOperationTypes = value; RaisePropertyChanged("listOperationTypes"); } }

        public dtRobotProcess()
        {
            listOperationTypes = new List<dtOperationType>();
        }
    }
}