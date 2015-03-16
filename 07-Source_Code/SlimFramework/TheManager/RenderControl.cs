using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheManager.Renderables;

namespace TheManager
{
    public partial class RenderControl : UserControl
    {
        public RenderControl()
        {
            InitializeComponent();
        }

        public void init()
        {
            DeviceManager.Instance.createDeviceAndSwapChain(this);
            RenderManager.Instance.init();

            Triangle triangle = new Triangle();
            Scene.Instance.addRenderObject(triangle);
        }

        public void shutDown()
        {
            RenderManager.Instance.shutDown();
            DeviceManager.Instance.shutDown();
        }
    }
}

