using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapViewPallet.Shape
{
    public class StationDataService:BridgeClientRequest
    {
        public StationDataService()
        {
        }

        public override void ReceiveResponseHandler(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
