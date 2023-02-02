using System;

namespace Factory_of_the_Future.AGV.Models
{
    internal class ProcessMessage
    {

        internal string CancelRequet(EITS_Messages.CancelMission Mission)
        {
            try
            {
                if (!string.IsNullOrEmpty(Mission.REQUESTID))
                {

                    return (0).ToString();
                }
                else
                {
                    return (-99).ToString();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return (-99).ToString();

            }
        }

        internal string LocationStatus(EITS_Messages.LocationStatus locationStatus)
        {
            try
            {
                if (!string.IsNullOrEmpty(locationStatus.LOCATION))
                {

                    return (0).ToString();
                }
                else
                {
                    return (-99).ToString();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return (-99).ToString();

            }
        }
    }
}