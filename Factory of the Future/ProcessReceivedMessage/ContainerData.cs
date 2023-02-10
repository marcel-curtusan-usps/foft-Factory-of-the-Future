using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class ContainerData : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public List<Container> Containers { get; protected set; }
        public Container _container = null;
        private bool saveToFile;

        internal async Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (_data != null)
                {
                    Containers = JsonConvert.DeserializeObject<List<Container>>(_data);
                    if (Containers.Count > 0)
                    {
                        string siteId = (string)AppParameters.AppSettings["FACILITY_NASS_CODE"];

                        foreach (Container d in Containers)
                        {
                            if (d.PlacardBarcode.StartsWith("99M"))
                            {
                                d.MailClass = "99M";
                                d.MailClassDisplay = "Mailer";
                                d.OriginName = "Mailer";
                                d.Origin = "";
                                d.Dest = "";
                                d.DestinationName = "";
                            }
                            int sortindex = 0;
                            foreach (ContainerHistory scan in d.ContainerHistory.OrderBy(o => o.EventDtmfmt))
                            {
                                if (d.EventDtm.Year == 1)
                                {
                                    d.EventDtm = scan.EventDtmfmt;
                                }
                                sortindex++;
                                scan.sortind = sortindex;
                                d.BinDisplay = scan.Event == "PASG" ? scan.BinName : "";
                                if (scan.SiteId == siteId)
                                {

                                    if (scan.Event == "PASG")
                                    {
                                        d.hasAssignScans = true;
                                    }
                                    if ((scan.Event == "CLOS" || scan.Event == "BCLS"))
                                    {
                                        d.hasCloseScans = true;
                                    }
                                    if (scan.Event == "LOAD")
                                    {
                                        d.hasLoadScans = true;
                                        d.Trailer = scan.Trailer;
                                    }
                                    if (scan.Event == "UNLD")
                                    {
                                        d.hasUnloadScans = true;
                                        d.Trailer = scan.Trailer;
                                    }
                                    if (scan.Event == "PRINT")
                                    {
                                        d.hasPrintScans = true;
                                    }
                                    if (scan.Event == "TERM")
                                    {
                                        d.containerTerminate = true;
                                    }
                                    if (!string.IsNullOrEmpty(scan.Location) && scan.SiteType == "Origin" && scan.Event == "PASG" && scan.Location != d.Location)
                                    {
                                        d.Location = scan.Location;
                                    }

                                }
                                if (scan.Event == "TERM" || scan.Event == "47")
                                {
                                    d.containerTerminate = true;
                                }
                                if (scan.Event == "UNLD" && scan.RedirectInd == "Y" && d.Dest == siteId)
                                {
                                    d.containerRedirectedDest = true;
                                }
                                if (scan.Event == "UNLD" && scan.SiteType == "Destination")
                                {
                                    if (scan.SiteId == d.Dest)
                                    {
                                        d.containerAtDest = true;
                                    }
                                }
                                if (scan.Event == "UNLD" && scan.SiteType == "Via")
                                {
                                    if (scan.SiteId != d.Dest)
                                    {
                                        d.containerRedirectedDest = true;
                                        d.hasUnloadScans = true;
                                        d.Location = "X-Dock";
                                    }
                                }
                            }
                            if (AppParameters.Containers.ContainsKey(d.PlacardBarcode) && AppParameters.Containers.TryGetValue(d.PlacardBarcode, out _container))
                            {
                                foreach (PropertyInfo prop in _container.GetType().GetProperties())
                                {
                                    if (!new Regex("^(BinDisplay|ContainerHistory|BinName)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                    {
                                        if (prop.GetValue(d, null).ToString() != prop.GetValue(_container, null).ToString())
                                        {
                                            prop.SetValue(_container, prop.GetValue(d, null));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (AppParameters.Containers.TryAdd(d.PlacardBarcode, d))
                                {
                                    //
                                }
                            }

                            // Global.DockDoor_List.AddOrUpdate(vr_door_item.DoorNumber, vr_door_item, (key, existingVal) =>
                            // {
                            //AppParameters.Containers.AddOrUpdate(d.PlacardBarcode, d, (Key, exisitingContainer) =>
                            //{
                            //    foreach (PropertyInfo prop in exisitingContainer.GetType().GetProperties())
                            //    {
                            //        if (!new Regex("^(BinDisplay|ContainerHistory|BinName)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                            //        {
                            //            if (prop.GetValue(d, null).ToString() != prop.GetValue(exisitingContainer, null).ToString())
                            //            {
                            //                prop.SetValue(exisitingContainer, prop.GetValue(d, null));
                            //            }
                            //        }
                            //    }
                            //    exisitingContainer.ContainerHistory = d.ContainerHistory;
                            //    return exisitingContainer;
                            //});
                            //{

                            //    exisitingContainer.hasAssignScans = d.hasAssignScans;
                            //    exisitingContainer.hasCloseScans = d.hasCloseScans;
                            //    exisitingContainer.hasLoadScans = d.hasLoadScans;
                            //    exisitingContainer.hasUnloadScans = d.hasUnloadScans;
                            //    exisitingContainer.hasPrintScans = d.hasPrintScans;
                            //    exisitingContainer.containerTerminate = d.containerTerminate;
                            //    exisitingContainer.Location = d.Location;
                            //    exisitingContainer.containerTerminate = d.containerTerminate;
                            //    exisitingContainer.containerRedirectedDest = d.containerRedirectedDest;
                            //    exisitingContainer.containerAtDest = d.containerAtDest;
                            //    exisitingContainer.containerRedirectedDest = d.containerRedirectedDest;
                            //    exisitingContainer.ContainerHistory = d.ContainerHistory;
                            //}
                            //else
                            //{
                            //    AppParameters.Containers.TryAdd(d.PlacardBarcode, d);
                            //}

                        }


                    }
                }
               await Task.Run(() => RemoveContainers()).ConfigureAwait(false);
              
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

        private void RemoveContainers()
        {
            try
            {
                if (AppParameters.Containers.Count > 0)
                {
                    foreach (string m in AppParameters.Containers.Where(r => DateTime.Now.Subtract(r.Value.EventDtm).TotalDays >= 3).Select(y => y.Key))
                    {
                        AppParameters.Containers.TryRemove(m, out _container);
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
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
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
                Containers = null;
                _container = null;
               
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProjectData()
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