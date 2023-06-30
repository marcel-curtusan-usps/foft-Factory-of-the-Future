using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class MPEFullBin : IDisposable
    {
        private bool disposedValue;
        private string _mpeType = "";
        private int _mpeNumber = 0;
        private string _binFullBins = "";
        private bool saveToFile;
        private List<string> FullBins = null;
        private List<string> FullBinList = new List<string>();
        internal async Task<bool> LoadAsync(string mpeType, int mpeNumber, string binFullBins)
        {
            saveToFile = false;
            _mpeType = mpeType;
            _mpeNumber = mpeNumber;
            _binFullBins = binFullBins;
            try
            {
                FullBins = !string.IsNullOrEmpty(_binFullBins) ? _binFullBins.Split(',').Select(p => p.Trim().TrimStart('0')).ToList() : new List<string>();
                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => f.Value.Properties.ZoneType.StartsWith("Bin") &&
                    f.Value.Properties.MPEType == _mpeType &&
                    f.Value.Properties.MPENumber == _mpeNumber).Select(y => y.Value).ToList().ForEach(MPE =>
                    {
                        //MPE.Properties.MPEBins = !string.IsNullOrEmpty(_binFullBins) ? _binFullBins.Split(',').Select(p => p.Trim().TrimStart('0')).ToList() : new List<string>();
                        for (int i = 0; i < FullBins.Count; i++)
                        {
                            if (MPE.Properties.Bins.Split(',').Select(p => p.Trim()).ToList().Contains(FullBins[i]))
                            {
                                FullBinList.Add(FullBins[i]);
                            }
                        }
                        if (MPE.Properties.MPEBins.Count() != FullBinList.Count())
                        {
                           Task.Run(() => FOTFManager.Instance.BroadcastBinZoneStatus(MPE, cs.Id)).ConfigureAwait(false);
                        }
                    });
                }
                return saveToFile;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return saveToFile;
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
                _mpeType = "";
                _mpeNumber = 0;
                _binFullBins = "";
                FullBins = null;
                FullBinList = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}