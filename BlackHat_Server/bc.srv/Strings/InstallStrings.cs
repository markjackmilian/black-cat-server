namespace bc.srv.Strings
{
    public static class RegistryStrings
    {
        /// <summary>
        ///  "Software\\Microsoft\\Windows\\CurrentVersion\\Run" 
        /// </summary>
        /// <returns></returns>
        public static string RegistryRun()
        {
            var registryRun = "\uBFD2\uAC95\uE82D\uD1CD\uED3D\uDB9D\uD5ED\uC4DD" +
                              "\uE04D\uCB5D\u881D\uF4FD\u8DED\u7CBD\uF5FD\uE4BD" +
                              "\u002D\uE9CD\u284D\u133D\u301D\u1CAD\u18CD\u04BD" +
                              "\u1D3D\u09FD\uC84D\u32FD\uCDDD\uB9ED\u35ED\u24DD" +
                              "\u40AD\u29CD\u672A\u54DA\u6DEA\u59FA\u581A\u44BA" +
                              "\u60AA\u4C4A\u07ED\u71DA\u10AD";

            for (int iUJoC = 0, jWzqX = 0; iUJoC < 45; iUJoC++)
            {
                jWzqX = registryRun[iUJoC];
                jWzqX += 0x4630;
                jWzqX = ~jWzqX;
                jWzqX += 0x7F7A;
                jWzqX = ~jWzqX;
                jWzqX += 0x004C;
                jWzqX ^= 0x05BE;
                jWzqX -= 0x8336;
                jWzqX = (((jWzqX & 0xFFFF) >> 8) | (jWzqX << 8)) & 0xFFFF;
                jWzqX = ~jWzqX;
                jWzqX++;
                jWzqX = ((jWzqX << 11) | ((jWzqX & 0xFFFF) >> 5)) & 0xFFFF;
                jWzqX ^= 0xD345;
                jWzqX++;
                jWzqX = (((jWzqX & 0xFFFF) >> 13) | (jWzqX << 3)) & 0xFFFF;
                jWzqX -= 0xDC2C;
                jWzqX -= iUJoC;
                jWzqX++;
                jWzqX += iUJoC;
                jWzqX ^= 0x810A;
                jWzqX += iUJoC;
                jWzqX = (((jWzqX & 0xFFFF) >> 10) | (jWzqX << 6)) & 0xFFFF;
                registryRun = registryRun.Substring(0, iUJoC) + (char) (jWzqX & 0xFFFF) +
                              registryRun.Substring(iUJoC + 1);
            }

            return registryRun;
        }
    }
}