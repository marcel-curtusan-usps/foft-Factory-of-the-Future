using System;

namespace Factory_of_the_Future
{
    public class Utility : IDisposable
    {
        private bool disposedValue;
        public EventDtm scheduledDtm = null;
        public DateTime tripDtm = new DateTime(1, 1, 1, 0, 0, 0);
        public string Sortplan_name { get; protected set; } = "";
        internal DateTime GetSvDate(EventDtm data)
        {
            scheduledDtm = data;
            try
            {
                tripDtm = new DateTime(scheduledDtm.Year,
                                            scheduledDtm.Month + 1,
                                            scheduledDtm.DayOfMonth,
                                            scheduledDtm.HourOfDay,
                                            scheduledDtm.Minute,
                                            scheduledDtm.Second);
                return tripDtm;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return tripDtm;
            }
            finally
            {
                Dispose();
            }
        }
        internal int Get_TripMin(EventDtm data)
        {
            scheduledDtm = data;
            try
            {

                //tripDtm = new DateTime(scheduledDtm.Year,
                //                                scheduledDtm.Month + 1,
                //                                scheduledDtm.DayOfMonth,
                //                                scheduledDtm.HourOfDay,
                //                                scheduledDtm.Minute,
                //                                scheduledDtm.Second);
                tripDtm = new DateTime();
                return (int)Math.Ceiling(tripDtm.Subtract(GetDTMNow()).TotalMinutes);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return 0;
            }
            finally
            {
                Dispose();
            }
        }

        private DateTime GetDTMNow()
        {
            try
            {
                if (AppParameters.TimeZoneConvert.TryGetValue(AppParameters.AppSettings.FACILITY_TIMEZONE, out string windowsTimeZoneId))
                {
                    return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                }
                else
                {
                    return DateTime.Now;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return DateTime.Now;
            }

        }
        internal string SortPlan_Name_Trimer(string sortplan)
        {
            try
            {
                if (!string.IsNullOrEmpty(sortplan))
                {
                    int dotindex = sortplan.IndexOf(".", 1);
                    if ((dotindex == -1))
                    {
                        Sortplan_name = sortplan;
                    }
                    else
                    {
                        Sortplan_name = sortplan.Substring(0, dotindex);
                    }
                }
                return Sortplan_name;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return Sortplan_name;
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
                scheduledDtm = null;
                tripDtm = new DateTime(1, 1, 1, 0, 0, 0);
                Sortplan_name = string.Empty;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Utility()
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