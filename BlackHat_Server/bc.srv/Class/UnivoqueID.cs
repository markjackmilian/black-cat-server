using System.IO;
using System.Management;

namespace bc.srv.Class
{
    internal class UnivoqueID
    {
        /// <summary>
        ///     Crea codice univoco. Seriale HardDisk + Mutex Da risorse
        /// </summary>
        /// <returns></returns>
        public string GetUnivoqueID()
        {
            //Random ran = new Random(DateTime.Now.Millisecond);
            //int r = ran.Next(10000);

            var res = "";

            var tmp = Path.GetTempPath();
            tmp = Directory.GetDirectoryRoot(tmp);
            tmp = tmp.Trim('\\');

            var dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + tmp + @"""");
            dsk.Get();
            res = dsk["VolumeSerialNumber"].ToString();
            dsk.Dispose();

            return res;
        }
        //---------------------------------------------------------
    }
}