using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controller.Interfaces
{
    public interface IExportar
    {

        void LlenarDatos();

        void Etabs();

        void releaseObject(object obj);

        void closeExcel();

    }
}
