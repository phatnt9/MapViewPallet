using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewPallet.MiniForm
{
    public class dtOperationType : userModel
    {
        private int _IdOperationType;
        private string _OperationTypeName;

        public int idOperationType { get => _IdOperationType; set { _IdOperationType = value; RaisePropertyChanged("idOperationType"); } }
        public string nameOperationType { get => _OperationTypeName; set { _OperationTypeName = value; RaisePropertyChanged("nameOperationType"); } }

    }
}
