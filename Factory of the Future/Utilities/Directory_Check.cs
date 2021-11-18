using System;
using System.IO;

namespace Factory_of_the_Future
{
    internal class Directory_Check
    {
        private readonly Object lockObj = new Object();

        internal bool DirPath(DirectoryInfo logdirpath)
        {
            lock (lockObj)
            {
                try
                {
                    string svr_active = Global.AppSettings.ContainsKey("SERVER_ACTIVE") ? Global.AppSettings.Property("SERVER_ACTIVE").Value.ToString().Trim() : "";
                    if (logdirpath != null && logdirpath.Root.Exists)
                    {
                        Global.SERVER_ACTIVE = true;
                        Global.AppSettings.Property("SERVER_ACTIVE").Value = true;
                        return true;
                    }
                    else
                    {
                        Global.SERVER_ACTIVE = false;
                        Global.AppSettings.Property("SERVER_ACTIVE").Value = false;
                        return false;
                    }
                }
                catch (Exception e)
                {
                    new ErrorLogger().ExceptionLog(e);
                    return false;
                }
            }
        }
    }
}