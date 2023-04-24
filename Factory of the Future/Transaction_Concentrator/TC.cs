using System;

namespace Factory_of_the_Future
{
    internal class TC
    {
        internal static string Processor(string message)
        {
            try
            {
                return ProcessorMessage.Message(message);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }



        private static string ErrorMessage()
        {
            try
            {
                return "";
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }
    }
}