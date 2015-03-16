using System.Windows.Forms;
using Resource = SlimDX.Direct3D11.Resource;
using Device = SlimDX.Direct3D11.Device;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Threading;

namespace TheManager
{
    public class RenderManager
    {
        private static RenderManager instance = null;
        public static RenderManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RenderManager(DeviceManager.Instance);
                }
                return instance;
            }
        }
        Thread renderThread;
        DeviceManager dm;
        public RenderManager(DeviceManager dm)
        {
            this.dm = dm;
        }

        public void renderScene()
        {
            while (true)
            {
                dm.context.ClearRenderTargetView(dm.renderTarget, new Color4(0.25f, 0.75f, 0.25f));
                Scene.Instance.render();
                dm.swapChain.Present(0, PresentFlags.None);
            }
        }

        public void init()
        {
            renderThread = new Thread(new ThreadStart(renderScene));
            renderThread.Start();
        }
        public void shutDown()
        {
            renderThread.Abort();
        }
    }

}
