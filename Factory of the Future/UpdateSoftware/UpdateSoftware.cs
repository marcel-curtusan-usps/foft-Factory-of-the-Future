using Factory_of_the_Future.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    internal class UpdateSoftware : IDisposable
    {
        private bool disposedValue;

        internal void StartProcess(UpdateParam param)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                string FilePath = string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), @"\UpdateSoftware\UpdateSoftware.ps1");
                string paramForamt = " -Version \"{0}\" -AppName \"{1}\" -DrivePath \"{2}\" -UserName \"{3}\" -Token \"{4}\" ";
                string Param = string.Format(paramForamt, param.Version, param.AppName, param.DrivePath, param.UserName, param.Token);

                startInfo.FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
                startInfo.Arguments = string.Concat(@"Set-ExecutionPolicy RemoteSigned -File ", string.Concat("\"", FilePath, "\""), Param);
                startInfo.Verb = "runas";
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;

                using (Process process = Process.Start(startInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        _ = new ErrorLogger().CustomLog(reader.ToString(), "UpdateSoftware");
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally { Dispose(); }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UpdateSoftware()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}