using System.Collections.Generic;

namespace MapViewPallet.MiniForm
{
    public class clImportPlan : userModel
    {
        private int _flgDeleteInsert;
        private string _actveDate;
        private List<structExcel> _structExcels;

        public int flgDeleteInsert { get => _flgDeleteInsert; set => _flgDeleteInsert = value; }
        public string actveDate { get => _actveDate; set => _actveDate = value; }
        public List<structExcel> structExcels { get => _structExcels; set => _structExcels = value; }
    }
}