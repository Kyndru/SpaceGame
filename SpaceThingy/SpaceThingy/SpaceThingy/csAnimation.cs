using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SpaceThingy
{
    class csAnimation
    {
        private static bool zoomOutEnd;
        private static bool zoomInEnd;

        public enum animationType { ZoomIn, ZoomOut };

        public static void execute(animationType at)
        {
            switch (at)
            {
                case animationType.ZoomOut:
                    zoomOutEnd = false;
                    Game1.zoomScaling = 1f;
                    Thread threadOut = new Thread(()=>{
                        while (!zoomOutEnd)
                       {
                           Thread.Sleep(10);
                           zoomOut();
                       }});

                    threadOut.IsBackground = true;
                    threadOut.Start();
                    break;
                case animationType.ZoomIn:
                    zoomInEnd = false;
                    Game1.zoomScaling = 0.5f;
                    Thread threadIn = new Thread(()=>{
                        while (!zoomInEnd)
                       {
                           Thread.Sleep(10);
                           zoomIn();
                       }});

                    threadIn.IsBackground = true;
                    threadIn.Start();
                    break;
            }
        }

        static void zoomIn()
        {
            Game1.zoomScaling += 0.01f;

            if (Game1.zoomScaling + 0.01f >= 1f)
                zoomInEnd = true;
        }

        static void zoomOut()
        {
            Game1.zoomScaling -= 0.01f;

            if (Game1.zoomScaling - 0.01f <= 0.5f)
                zoomOutEnd = true;
        }
    }
}
