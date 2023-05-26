using System;
using System.IO;

namespace Factory_of_the_Future
{
    internal class Directory_Check
    {
        private readonly Object lockObj = new Object();
        private DirectoryInfo Logdirpath { get; set; }

        internal bool DirPath(DirectoryInfo logpath)
        { 
            Logdirpath = logpath;
            lock (lockObj)
            {
                
                try
                {
                    if (Logdirpath != null && Logdirpath.Root.Exists)
                    {

                        if (Logdirpath.FullName == AppParameters.CodeBase.Parent.FullName)
                        {
                            AppParameters.ActiveServer = false;
                            AppParameters.AppSettings.SERVER_ACTIVE = false;
                            AppParameters.Logdirpath = null;
                            return false;
                        }
                        else
                        {
                            AppParameters.ActiveServer = true;
                            AppParameters.AppSettings.SERVER_ACTIVE = true;
                            AppParameters.Logdirpath = Logdirpath;
                            return true;
                        }


                    }
                    else
                    {
                        AppParameters.ActiveServer = false;
                        AppParameters.AppSettings.SERVER_ACTIVE= false;
                        return false;
                    }
                }
                catch (Exception e)
                {
                    new ErrorLogger().ExceptionLog(e);
                    return false;
                }
                finally {
                    Logdirpath = null;
                }
            }
        }
    }
}