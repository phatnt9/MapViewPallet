namespace MapViewPallet.MiniForm
{
    public class userModel: NotifyUIBase//INotifyPropertyChanged
    {
        private int pCreUsrId;
        private string pCreDt;
        private int pUpdUsrId;
        private string pUpdDt;

        public int creUsrId { get => pCreUsrId; set => pCreUsrId = value; }
        public string creDt { get => pCreDt; set => pCreDt = value; }
        public int updUsrId { get => pUpdUsrId; set => pUpdUsrId = value; }
        public string updDt { get => pUpdDt; set => pUpdDt = value; }
    }
}
