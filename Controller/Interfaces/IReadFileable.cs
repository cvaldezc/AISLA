using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controller.Interfaces
{
    public interface IReadFileable
    {

        /// <summary>
        /// procedimiento que se encarga leer el formato de excel
        /// y llenar los datos a los dataset
        /// </summary>
        /// 
        void selectSheet(String name);

        void readETabs(String story);

        /// <summary>
        ///     funciones reautilizables
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="cm"></param>
        /// <param name="story"></param>

        void calcFormulas(Double delta, Double cm, String story);

        void close();

    }
}
