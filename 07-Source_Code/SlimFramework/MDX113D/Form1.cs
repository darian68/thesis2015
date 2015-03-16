using System.Windows.Forms;
using Resource = SlimDX.Direct3D11.Resource;
using Device = SlimDX.Direct3D11.Device;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Threading;
using TheManager;
using TheManager.Renderables;

namespace MDX113D
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderControl1.shutDown();
        }

        private void renderControl1_Load(object sender, System.EventArgs e)
        {
            renderControl1.init();
        }
    }
}
