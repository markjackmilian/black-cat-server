using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace BlackHat_Server
{
    class KeepAlive
    {

        // LA Main Connection è attiva?
        public bool isAlive()
        {
            
            bool can = ST_Client.Instance.Connessione.Client.Poll(5000, System.Net.Sockets.SelectMode.SelectRead);

            if (!can)
                return true;
            else
                return false;
        }
        //---------------------------


        // LA Main Connection è attiva?
        public bool isAlivemMessageMode()
        {
            try
            {
                MsgManager mm = new MsgManager(ST_Client.Instance.Connessione.GetStream());

                bool sent = mm.SendEncryMessage("ARE YOU THERE?", 5000);

                if (sent)
                {
                    string answer = mm.WaitForEncryMessageRicorsive(10000);

                    if (answer == "I'M HERE")
                        return true;
                    else
                        return false;
                }
                else
                    return false;

            }
            catch (Exception)
            {
                
                throw;
            }
          
        }
        //---------------------------




       

    }
}
