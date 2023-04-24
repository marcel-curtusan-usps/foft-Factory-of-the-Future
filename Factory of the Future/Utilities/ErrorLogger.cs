using System;
using System.Text;

namespace Factory_of_the_Future
{
    public class ErrorLogger
    {
        internal void ExceptionLog(Exception e)
        {
            if (AppParameters.Logdirpath != null && new Directory_Check().DirPath(AppParameters.Logdirpath))
            {
                if (AppParameters.AppSettings.ContainsKey("APPLICATION_NAME"))
                {
                    StringBuilder errorBuilder = new StringBuilder(AppParameters.AppSettings.Property("APPLICATION_NAME").Value.ToString() + " Application Error");
                    errorBuilder.Append("Exception:DateTime = " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    errorBuilder.Append("Exception:Data = " + e.Data);
                    errorBuilder.Append("Exception:InnerException = " + e.InnerException);
                    errorBuilder.Append("Exception:Message = " + e.Message);
                    errorBuilder.Append("Exception:StackTrace = " + e.StackTrace);
                    errorBuilder.Append("Exception:TargetSite = " + e.TargetSite);
                    errorBuilder.Append("Exception:Source = " + e.Source);

                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.LogFloder, "\\"), (string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value + "_Applogs_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", errorBuilder.ToString());
                }
            }
        }

        internal void CustomLog(string Data, string type)
        {

            if (AppParameters.Logdirpath != null && new Directory_Check().DirPath(AppParameters.Logdirpath))
            {
                if (AppParameters.AppSettings.ContainsKey("APPLICATION_NAME"))
                {
                    StringBuilder errorBuilder = new StringBuilder(AppParameters.AppSettings.Property("APPLICATION_NAME").Value.ToString() + " " + type + " Info ");
                    errorBuilder.Append("DateTime = " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    errorBuilder.Append(" Data = " + Data);
                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.LogFloder, "\\"), type + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", errorBuilder.ToString());
                }
            }

        }
    }
}