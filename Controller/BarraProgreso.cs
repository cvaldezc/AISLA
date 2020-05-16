using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Controller
{
    public class BarraProgreso
    {
        public volatile static bool parar;

        public static int status = -1;

        public static Thread tarea1;

        public static Thread tarea2;

        //public static void cambiosdebarra()
        //{
        //    while (parar)
        //    {
        //        if (true)
        //        {
                    
        //        }
        //        Thread.Sleep(3600);
        //    }
        //}

        public static void StopTarea1()
        {
            parar = true;
        }

    }
}
