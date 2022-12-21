namespace Factory_of_the_Future
{
    internal class ProcessorMessage
    {
        private string requestType;
        private string message1;
        private string message2;

        public static string Message(string data)
        {
            char[] message = data.ToCharArray();
            try
            {

                if (message[0] == 'B')
                {
                    return "";
                }
                else
                {
                    return "";
                }
            }
            catch (System.Exception e)
            {
                return "";
            }
        }

    }
}