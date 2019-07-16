namespace MapViewPallet.MiniForm
{
    public class dtTimeWork : userModel
    {
        private int pTimeWorkId;
        private string pTimeWorkName;
        private string pStartTime;
        private string pEndTime;

        public int timeWorkId { get => pTimeWorkId; set { pTimeWorkId = value; RaisePropertyChanged("timeWorkId"); } }
        public string timeWorkName { get => pTimeWorkName; set { pTimeWorkName = value; RaisePropertyChanged("timeWorkName"); } }
        public string startTime { get => pStartTime; set { pStartTime = value; RaisePropertyChanged("startTime"); } }
        public string endTime { get => pEndTime; set { pEndTime = value; RaisePropertyChanged("endTime"); } }
    }
}