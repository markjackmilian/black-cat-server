using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.IO;

namespace BlackHat_Server
{
    class UnivoqueID
    {

        /// <summary>
        /// Crea codice univoco. Seriale HardDisk + Mutex Da risorse
        /// </summary>
        /// <returns></returns>
        public string GetUnivoqueID()
        {
            //Random ran = new Random(DateTime.Now.Millisecond);
            //int r = ran.Next(10000);

            string res = "";

            string tmp = Path.GetTempPath();
            tmp = Directory.GetDirectoryRoot(tmp);
            tmp = tmp.Trim('\\');

            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + tmp + @"""");
            dsk.Get();
            res =  dsk["VolumeSerialNumber"].ToString();           
            dsk.Dispose();

            return res;
        }
        //---------------------------------------------------------

    }
}
