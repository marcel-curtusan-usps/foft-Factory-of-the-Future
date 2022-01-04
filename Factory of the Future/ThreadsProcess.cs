using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace Factory_of_the_Future
{
    internal class ThreadsProcess
    {
        private static JObject connection = new JObject();
        internal static void Start_processor(JObject o)
        {
            try
            {
                connection = o;

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
    }
}