using System;
using System.IO;
using System.Net;
using System.Text;

namespace bc.srv
{
    public static class Extensions
    {
        /// <summary>
        /// Write exception to file, only in debug mode
        /// </summary>
        /// <param name="ex"></param>
        public static void DumpDebugException(this Exception ex)
        {
#if DEBUG
            var builder = new StringBuilder();
            builder.Append(ex.Message);
            builder.AppendLine();
            builder.AppendLine(ex.StackTrace);
            File.WriteAllText($"{Guid.NewGuid():N}.txt",builder.ToString());
#endif
        }
        
        /// <summary>
        /// Write info
        /// </summary>
        /// <param name="str"></param>
        public static void DebugLog(this string str)
        {
#if DEBUG
            File.AppendAllText("debug.txt",$"{Environment.NewLine}{str}");
#endif
        }
    }
}