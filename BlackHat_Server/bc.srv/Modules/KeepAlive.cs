using System.Net.Sockets;
using bc.srv.Classes.Comunicator;

namespace bc.srv.Modules
{
    internal class KeepAlive
    {
        // LA Main Connection è attiva?
        public bool IsAlive()
        {
            var can = SrvData.Instance.Connessione.Client.Poll(5000, SelectMode.SelectRead);

            if (!can)
                return true;
            return false;
        }
        //---------------------------


        // LA Main Connection è attiva?
        public bool IsAlivemMessageMode()
        {
            var mm = new MsgManager(SrvData.Instance.Connessione.GetStream());

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