using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Factory_of_the_Future
{
    internal class GetItineraryTrip : IDisposable
    {
        private string _Itineraryitem { get; set; }
        private string _routetripid { get; set; }    
        public List<Leg> Legs { get; set; } = new List<Leg>();
        private JToken Itinerary { get; set; } 
        private JToken legs { get; set; }
        private bool disposedValue;

        public List<Leg> Get_ItineraryTrip(string Itineraryitem, string routetripid)
        {
            this._Itineraryitem = Itineraryitem;
            this._routetripid = routetripid;
            try
            {
                if (!string.IsNullOrEmpty(_Itineraryitem))
                {
                    Itinerary = JToken.Parse(_Itineraryitem);
                    if (Itinerary.HasValues)
                    {
                        legs = Itinerary[0].SelectToken("legs");
                        if (legs.HasValues)
                        {
                            Legs = legs.ToObject<List<Leg>>();
                        }
                    }
                }
                return Legs;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
            finally { this.Dispose(); }

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
        // ~GetItineraryTrip()
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