using Factory_of_the_Future.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Security.Policy;

namespace Factory_of_the_Future
{
    internal class LoadContainers
    {
        internal void LoadAsync(SVtripLoadContainers _containers, string siteId)
        {
            List<LoadedCtrHuDetail> containers = _containers.LoadedCtrHuDetails;
            try
            {
              
                foreach (LoadedCtrHuDetail d in containers)
                {
                  
                    if (!AppParameters.Containers.ContainsKey(d.Barcode) && AppParameters.Containers.TryGetValue(d.Barcode, out Container CurrentContatiner))
                    {
                        //
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally  
            {
                _containers = null;
                containers = null;
                siteId = null;
            }
        }
    }
}