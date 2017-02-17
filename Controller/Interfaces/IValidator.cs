using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controller.Interfaces
{
    public interface IValidator
    {

        bool verifyUnitData(string type, string ufx, string ufy, string ufz);
    }
}
