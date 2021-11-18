using Newtonsoft.Json.Linq;

namespace Factory_of_the_Future
{
    internal class HandleAsyncProcess
    {
        private readonly ProcessType parallel;
        private readonly bool v;
        private readonly JObject item;

        public HandleAsyncProcess(ProcessType parallel, bool v, JObject item)
        {
            this.parallel = parallel;
            this.v = v;
            this.item = item;
        }
    }
}