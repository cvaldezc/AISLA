using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controller.Interfaces
{
    public interface IReadFileStaadProable
    {

        /// <summary>
        /// Procedimientos para leer archivo de entrada de Staad PRO
        /// </summary>
        void ReadStaadPro();

        void close();

    }
}
