using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapViewPallet.Shape
{
    public partial class Properties : Form
    {
        dynamic myObject;
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
