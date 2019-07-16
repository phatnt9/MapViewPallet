namespace MapViewPallet.MiniForm
{
    public class structExcel
    {
        private int _deviceId;
        private string _deviceName;
        private int _productId;
        private string _productName;
        private int _productDetailId;
        private string _productDetailName;
        private int _palletAmount;
        private string _imageProductUrl;

        public string deviceName { get => _deviceName; set => _deviceName = value; }
        public string productName { get => _productName; set => _productName = value; }
        public string productDetailName { get => _productDetailName; set => _productDetailName = value; }
        public int palletAmount { get => _palletAmount; set => _palletAmount = value; }
        public int deviceId { get => _deviceId; set => _deviceId = value; }
        public int productId { get => _productId; set => _productId = value; }
        public int productDetailId { get => _productDetailId; set => _productDetailId = value; }
        public string imageProductUrl { get => _imageProductUrl; set => _imageProductUrl = value; }
    }
}