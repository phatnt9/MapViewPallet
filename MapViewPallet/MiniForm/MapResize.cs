using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapViewPallet.MiniForm
{
    public partial class MapResize : Form
    {
        public event Action<double,double> ResizeHandle;
        double mapWidth;
        double mapHeight;

        public MapResize(double W, double H)
        {
            InitializeComponent();
            tb_Width.Maximum = 10000;
            tb_Height.Maximum = 10000;
            mapWidth = W;
            mapHeight = H;
        }
        

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            try
            {
                double W = double.Parse(tb_Width.Text);
                double H = double.Parse(tb_Height.Text);
                ResizeHandle(W, H);
                Close();
            }
            catch { }

        }

        private void MapResize_Load(object sender, EventArgs e)
        {
            tb_Width.Value = (int)mapWidth;
            tb_Height.Value = (int)mapHeight;

        }
    }
}
