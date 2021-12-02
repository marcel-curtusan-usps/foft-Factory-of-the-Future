using System;
using System.Text;

namespace Factory_of_the_Future
{
    public class ErrorLogger
    {
        internal void ExceptionLog(Exception e)
        {
            if (new Directory_Check().DirPath(Global.Logdirpath))
            {
                if (Global.AppSettings.ContainsKey("APPLICATION_NAME"))
                {
                    StringBuilder errorBuilder = new StringBuilder(Global.AppSettings.Property("APPLICATION_NAME").Value.ToString() + " Application Error");
                    errorBuilder.Append("Exception:DateTime = " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    errorBuilder.Append("Exception:Data = " + e.Data);
                    errorBuilder.Append("Exception:InnerException = " + e.InnerException);
                    errorBuilder.Append("Exception:Message = " + e.Message);
                    errorBuilder.Append("Exception:StackTrace = " + e.StackTrace);
                    errorBuilder.Append("Exception:TargetSite = " + e.TargetSite);
                    errorBuilder.Append("Exception:Source = " + e.Source);

                    new FileIO().Write(string.Concat(Global.Logdirpath, Global.LogFloder, "\\"), (string)Global.AppSettings.Property("APPLICATION_NAME").Value + "_Applogs_" + DateTime.Now.ToString("yyyy-MM-dd") + ".text", errorBuilder.ToString());
                }
            }
            else
            {
                if (new Directory_Check().DirPath(Global.CodeBase.Parent))
                {
                    if (Global.AppSettings.ContainsKey("APPLICATION_NAME"))
                    {
                        StringBuilder errorBuilder = new StringBuilder("FOTF Application Error");
                        errorBuilder.Append("Exception:DateTime = " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        errorBuilder.Append("Exception:Data = " + e.Data);
                        errorBuilder.Append("Exception:InnerException = " + e.InnerException);
                        errorBuilder.Append("Exception:Message = " + e.Message);
                        errorBuilder.Append("Exception:StackTrace = " + e.StackTrace);
                        errorBuilder.Append("Exception:TargetSite = " + e.TargetSite);
                        errorBuilder.Append("Exception:Source = " + e.Source);

                        new FileIO().Write(string.Concat(Global.CodeBase.Parent.FullName.ToString()), (string)Global.AppSettings.Property("APPLICATION_NAME").Value + "_Applogs_" + DateTime.Now.ToString("yyyy-MM-dd") + ".text", errorBuilder.ToString());
                    }
                }
            }
        }

        internal void CustomLog(string Data, string type)
        {
            if (new Directory_Check().DirPath(Global.Logdirpath))
            {
                if (Global.AppSettings.ContainsKey("APPLICATION_NAME"))
                {
                    StringBuilder errorBuilder = new StringBuilder(Global.AppSettings.Property("APPLICATION_NAME").Value.ToString() + " " + type + " Info ");
                    errorBuilder.Append("DateTime = " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    errorBuilder.Append(" Data = " + Data);
                    new FileIO().Write(string.Concat(Global.Logdirpath, Global.LogFloder, "\\"), type + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".text", errorBuilder.ToString());
                }
            }
            else
            {
                if (new Directory_Check().DirPath(Global.CodeBase.Parent))
                {
                    if (Global.AppSettings.ContainsKey("APPLICATION_NAME"))
                    {
                        StringBuilder errorBuilder = new StringBuilder(Global.AppSettings.Property("APPLICATION_NAME").Value.ToString() + " " + type + " Info ");
                        errorBuilder.Append("DateTime = " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        errorBuilder.Append(" Data = " + Data);

                        new FileIO().Write(string.Concat(Global.CodeBase.Parent.FullName.ToString()), (string)Global.AppSettings.Property("APPLICATION_NAME").Value + "_Applogs_" + DateTime.Now.ToString("yyyy-MM-dd") + ".text", errorBuilder.ToString());
                    }
                }
            }
        }
    }
}