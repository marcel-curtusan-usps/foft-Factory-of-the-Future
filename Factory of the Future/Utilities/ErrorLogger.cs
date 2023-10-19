using System;
using System.Text;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    public class ErrorLogger
    {
        internal void ExceptionLog(Exception e)
        {
            if (AppParameters.Logdirpath != null && new Directory_Check().DirPath(AppParameters.Logdirpath))
            {
                
                    StringBuilder errorBuilder = new StringBuilder(AppParameters.AppSettings.APPLICATION_NAME + " Application Error");
                _ = errorBuilder.Append("Exception:DateTime = " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                _ = errorBuilder.Append("Exception:Data = " + e.Data);
                _ = errorBuilder.Append("Exception:InnerException = " + e.InnerException);
                _ = errorBuilder.Append("Exception:Message = " + e.Message);
                _ = errorBuilder.Append("Exception:StackTrace = " + e.StackTrace);
                _ = errorBuilder.Append("Exception:TargetSite = " + e.TargetSite);
                _ = errorBuilder.Append("Exception:Source = " + e.Source);

                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.LogFloder, "\\"), (string)AppParameters.AppSettings.APPLICATION_NAME + "_Applogs_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", errorBuilder.ToString());
                
            }
        }

        internal async Task CustomLog(string Data, string type)
        {

            if (AppParameters.Logdirpath != null && new Directory_Check().DirPath(AppParameters.Logdirpath))
            {
                    StringBuilder errorBuilder = new StringBuilder(AppParameters.AppSettings.APPLICATION_NAME + " " + type + " Info ");
                _ = errorBuilder.Append("DateTime = " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                _ = errorBuilder.Append(" Data = " + Data);
                    await Task.Run(() => new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.LogFloder, "\\"), type + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", errorBuilder.ToString())).ConfigureAwait(false);
                
            }

        }
    }
}