using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Controller.Interfaces;

namespace Controller
{
    public class Validator : IValidator
    {
        bool IValidator.verifyUnitData(string type, string ufx, string ufy, string ufz)
        {
            bool status;
            try
            {
                ushort counter = 0;
                if (type == "etabs")
                {
                    #region validate ton for etabs
                    if (ufx == "tonf")
                    {
                        counter += 1;
                    }
                    if (ufy == "tonf")
                    {
                        counter += 1;
                    }
                    if (ufz == "tonf")
                    {
                        counter += 1;
                    }
                    #endregion
                }
                if (type == "staad")
                {

                }
                #region validator
                if (counter == 3)
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR VALIDADOR UNIT DATA " + ex.Message.ToString());
                status = false;
            }
            return status;
        }
    }
}
