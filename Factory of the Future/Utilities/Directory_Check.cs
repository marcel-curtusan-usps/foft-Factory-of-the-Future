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
                     
                        if (logdirpath.FullName == AppParameters.CodeBase.Parent.FullName)
                        {
                            AppParameters.ActiveServer = false;
                            AppParameters.AppSettings.Property("SERVER_ACTIVE").Value = false;
                            AppParameters.Logdirpath = null;
                            return false;
                        }
                        else
                        {
                            AppParameters.ActiveServer = true;
                            AppParameters.AppSettings.Property("SERVER_ACTIVE").Value = true;
                            AppParameters.Logdirpath = logdirpath;
                            return true;
                        }
                      
                      
                    }
                    else
                    {
                        AppParameters.ActiveServer = false;
                        AppParameters.AppSettings.Property("SERVER_ACTIVE").Value = false;
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