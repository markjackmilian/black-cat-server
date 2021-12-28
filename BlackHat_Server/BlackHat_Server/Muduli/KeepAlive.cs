using System.Net.Sockets;

namespace BlackHat_Server
{
    internal class KeepAlive
    {
        // LA Main Connection è attiva?
        public bool isAlive()
        {
            var can = ST_Client.Instance.Connessione.Client.Poll(5000, SelectMode.SelectRead);

            if (!can)
                return true;
            return false;
        }
        //---------------------------


        // LA Main Connection è attiva?
        public bool isAlivemMessageMode()
        {
            var mm = new MsgManager(ST_Client.Instance.Connessione.GetStream());

            var sent = mm.SendEncryMessage("ARE YOU THERE?", 5000);

            if (sent)
            {
                var answer = mm.WaitForEncryMessageRicorsive(10000);

                if (answer == "I'M HERE")
                    return true;
                return false;
            }

            return false;
        }
        //---------------------------
    }
}