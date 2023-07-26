using System;
using System.Reactive.Disposables;
using System.Text.RegularExpressions;

namespace Factory_of_the_Future
{
    internal class GetTagType : IDisposable
    {
        private bool disposedValue;

        internal string Get(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_AGV, RegexOptions.IgnoreCase))
                    {
                        return "Autonomous Vehicle";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_PIV, RegexOptions.IgnoreCase))
                    {
                        return "Vehicle";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_PERSON, RegexOptions.IgnoreCase))
                    {
                        return "Person";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_HVI, RegexOptions.IgnoreCase))
                    {
                        return "HVI";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_LOCATOR, RegexOptions.IgnoreCase))
                    {
                        return "Locator";
                    }
                    else
                    {
                        return name;
                    }
                }
                else
                {
                    return "Person";
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "Error_Unknown_Tag";
            }
            finally
            {
                Dispose();
            }
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
        // ~GetTagType()
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