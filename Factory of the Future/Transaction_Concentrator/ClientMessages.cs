using System;

namespace Factory_of_the_Future.Transaction_Concentrator
{
    public class Initialization_Message
    {
        public string Request_Code;
        public string Sequence_Number;
        public string MPE_ZIP_Code;
        public string TC_ZIP_Code;
        public string MPE_Name;
        public string NGTC_Server_Name;
        public string Message_format;
        public string Error;

        public Initialization_Message(string message)
        {
            Request_Code = message.Substring(0, 1);
            Message_format += Request_Code;
            Sequence_Number = message.Substring(Message_format.Length, 4);
            Message_format += Sequence_Number;
            MPE_ZIP_Code = message.Substring(Message_format.Length, 5);
            Message_format += MPE_ZIP_Code;
            TC_ZIP_Code = message.Substring(Message_format.Length, 5);
            Message_format += TC_ZIP_Code;
            MPE_Name = message.Substring(Message_format.Length, 30);
            Message_format += MPE_Name;
            NGTC_Server_Name = message.Substring(Message_format.Length, 30);
            Message_format += NGTC_Server_Name;
            if (Message_format.Length != message.Length)
            {
                Error = "01";
            }
        }
    }
    public class InitializationMessageResponse
    {
        public string Request_Code;
        public string Sequence_Number;
        public string TC_System_Month_and_Day;
        public string TC_System_Year;
        public string TC_System_Hour_and_Minute;
        public string MPE_ID;
        public string MPE_Name;
        public string Message_format;

        public InitializationMessageResponse(Initialization_Message message)
        {
            Request_Code = "B";
            Sequence_Number = message.Sequence_Number.ToString().PadLeft(4, '0');
            TC_System_Month_and_Day = DateTime.Now.ToString("MMdd").PadLeft(4, ' ');
            TC_System_Year = DateTime.Now.ToString("yyyy").PadLeft(4, ' ');
            TC_System_Hour_and_Minute = DateTime.Now.ToString("HHmm").PadLeft(4, ' ');
            MPE_ID = "".PadLeft(2, '0');
            MPE_Name = message.MPE_Name;
            Message_format = Sequence_Number + Request_Code + TC_System_Month_and_Day + TC_System_Year + TC_System_Hour_and_Minute + MPE_ID + MPE_Name;
        }
    }
    public class Host_Status
    {
        public string Request_Code;
        public string Sequence_Number;
        public string Message_format;
        public string Error;
        public Host_Status(string message)
        {
            Request_Code = message.Substring(0, 1);
            Message_format += Request_Code;
            Sequence_Number = message.Substring(Message_format.Length, 4);
            Message_format += Sequence_Number;
            if (Message_format.Length != message.Length)
            {
                Error = "01";
            }
        }
    }
    public class Host_Status_Response
    {
        public string Sequence_Number;
        public string Response_Code;
        public string SAMS_Sommunication_Status;
        public string MIDAS_Communication_Status;
        public string Message_format;
        public Host_Status_Response(Host_Status message)
        {
            Sequence_Number = message.Sequence_Number;
            Message_format += Sequence_Number;
            Response_Code = "I";
            Message_format += Response_Code;
            SAMS_Sommunication_Status = "0";
            Message_format += SAMS_Sommunication_Status;
            MIDAS_Communication_Status = "0";
            Message_format += MIDAS_Communication_Status;
        }
    }
}