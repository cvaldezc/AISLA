using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controller.Interfaces
{
    public interface IWriteEtabs
    {

        void processe2kAislado(String story, Double cm);

        void processe2kNoAislado(String story, Double cm);

    }
}
