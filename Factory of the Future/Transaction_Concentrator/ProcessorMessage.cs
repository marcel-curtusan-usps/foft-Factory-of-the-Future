using Factory_of_the_Future.Transaction_Concentrator;
using System.Text.RegularExpressions;

namespace Factory_of_the_Future
{
    internal class ProcessorMessage
    {

        public static string Message(string data)
        {
            char[] message = data.ToCharArray();
            try
            {

                if (Regex.IsMatch(message[0].ToString(), "(b|B)", RegexOptions.ExplicitCapture))
                {
                    return new InitializationMessageResponse(new Initialization_Message(data)).Message_format;
                }
                else if (Regex.IsMatch(message[0].ToString(), "(W)", RegexOptions.ExplicitCapture))
                {
                    return new Host_Status_Response(new Host_Status(data)).Message_format;
                }
                else
                {
                    return "";
                }
            }
            catch (System.Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }

    }
}