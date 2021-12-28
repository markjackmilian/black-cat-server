using System;
using System.Collections.Generic;
using System.Text;

namespace BlackHat_Server
{
    class GetInfo
    {

        /// <summary>
        /// Nome del computer
        /// </summary>
        /// <returns></returns>
        public string MachineName()
        {
            return System.Environment.MachineName;
        }
        //-------------------------------------------

        /// <summary>
        /// Nome dell'utente
        /// </summary>
        /// <returns></returns>
        public string UserName()
        {
            return System.Environment.UserName;
        }
        //-------------------------------------------

    }
}
