using System.Threading;

namespace Factory_of_the_Future
{
    public class Data_Retention
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