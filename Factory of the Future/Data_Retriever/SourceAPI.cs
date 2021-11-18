using Newtonsoft.Json.Linq;
using System.Threading;

namespace Factory_of_the_Future
{
    public class SourceData_Process
    {
        public JObject SERVER;
        public ManualResetEvent manualResetEvent;
        public string Error;

        public SourceData_Process(JObject sERVER_CONNECTION, ManualResetEvent manualResetEvent)
        {
            this.SERVER = sERVER_CONNECTION;
            this.manualResetEvent = manualResetEvent;
        }
    }

    public class SourceAPI
    {
        internal class StartProcess
        {
            public ManualResetEvent manualEvents;

            public StartProcess(ManualResetEvent manualEvents)
            {
                this.manualEvents = manualEvents;
            }
        }
    }
}