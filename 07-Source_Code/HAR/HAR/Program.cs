using SlimDX.Windows;

namespace HAR
{
    static class Program
    {
        static void Main()
        {
            var form = new RenderForm("Human Activity Recognition");
            MessagePump.Run(form, () => { });
        }
    }
}
