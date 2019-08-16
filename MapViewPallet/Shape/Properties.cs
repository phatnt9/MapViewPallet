using System;
using System.Windows.Forms;

namespace MapViewPallet.Shape
{
    public partial class Properties : Form
    {
        private dynamic myObject;

        public Properties(dynamic Path)
        {
            InitializeComponent();
            myObject = Path;
        }

        private void PathProperties_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = myObject.props;
        }
    }
}