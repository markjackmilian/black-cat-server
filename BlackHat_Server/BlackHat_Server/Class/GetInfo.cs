using System;

namespace BlackHat_Server.Class
{
    internal class GetInfo
    {
        /// <summary>
        ///     Nome del computer
        /// </summary>
        /// <returns></returns>
        public string MachineName()
        {
            return Environment.MachineName;
        }
        //-------------------------------------------

        /// <summary>
        ///     Nome dell'utente
        /// </summary>
        /// <returns></returns>
        public string UserName()
        {
            return Environment.UserName;
        }
        //-------------------------------------------
    }
}