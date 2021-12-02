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
                    if (logdirpath != null && logdirpath.Root.Exists)
                    {
                     
                        if (logdirpath.FullName == Global.CodeBase.Parent.FullName)
                        {
                            Global.SERVER_ACTIVE = false;
                            Global.AppSettings.Property("SERVER_ACTIVE").Value = false;
                            Global.Logdirpath = null;
                            return false;
                        }
                        else
                        {
                            Global.SERVER_ACTIVE = true;
                            Global.AppSettings.Property("SERVER_ACTIVE").Value = true;
                            Global.Logdirpath = logdirpath;
                            return true;
                        }
                      
                      
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