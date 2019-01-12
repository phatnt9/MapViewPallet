namespace MapViewPallet.MiniForm
{
    public class dtProductDetail : userModel
    {
        private int pProductDetailId;
        private int pProductId;
        private string pProductDetailName;

        public int productDetailId { get => pProductDetailId; set { pProductDetailId = value; RaisePropertyChanged("productDetailId"); } }
        public int productId { get => pProductId; set { pProductId = value; RaisePropertyChanged("productId"); } }
        public string productDetailName { get => pProductDetailName; set { pProductDetailName = value; RaisePropertyChanged("productDetailName"); } }
    }
}
