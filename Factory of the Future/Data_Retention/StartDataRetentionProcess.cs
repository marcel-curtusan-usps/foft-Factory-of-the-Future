using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Factory_of_the_Future
{
    internal class StartDataRetentionProcess
    {
        private static int SPworkItemCount = 0;

        private StartDataRetentionProcess()
        { }

        internal static void StartProcess(object state)
        {
            int SPworkItemNumber = SPworkItemCount;
            Interlocked.Increment(ref SPworkItemCount);

            Data_Retention.StartProcess _SP = (Data_Retention.StartProcess)state;

            try
            {
                //add file data retention check here.
                if (Global.Logdirpath.Exists)
                {
                    DirectoryInfo maindir = new DirectoryInfo(@"" + Global.Logdirpath.FullName + "\\" + "LOG" + "\\");
                    if (maindir.Exists)
                    {
                        List<FileInfo> files = maindir.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(d => d.LastWriteTime.Year).ThenBy(d => d.LastWriteTime.Month).ThenBy(d => d.LastWriteTime.Day).Select(x => x).ToList();
                        int days = 60;
                        long target_size = 1073741824;
                        if (Global.AppSettings.ContainsKey("FILE_RETENTION"))
                        {
                            string tempint = (string)Global.AppSettings.Property("FILE_RETENTION").Value.ToString();
                            int.TryParse(tempint, out int tempdays);

                            if (tempdays != days)
                            {
                                if (tempdays > 0)
                                {
                                    days = tempdays;
                                }
                            }
                        }
                        if (Global.AppSettings.ContainsKey("MAX_FILE_SIZE"))
                        {
                            string tempint = (string)Global.AppSettings.Property("MAX_FILE_SIZE").Value.ToString();
                            int.TryParse(tempint, out int temptarget_size);

                            if (temptarget_size != target_size)
                            {
                                if (temptarget_size > 625000)
                                {
                                    target_size = temptarget_size;
                                }
                            }
                        }

                        //this to check file date last write time
                        if (days > 0)
                        {
                            if (files.Count > 0)
                            {
                                foreach (FileInfo file in files)
                                {
                                    //calculate the number of days between two dates
                                    double NumberofDay = (DateTime.Now - file.LastWriteTime).TotalDays;
                                    if (Math.Round(NumberofDay) >= days)
                                    {
                                        try
                                        {
                                            file.Delete();
                                            string data = string.Concat("FileName : ", file.FullName
                                                , " | FileSize : " + FormatBytes(file.Length)
                                                , " | Number Of Day Old : ", Math.Round(NumberofDay)
                                                , " | Delete Date/Time : ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                                                , " | File was deleted because file was older then ", Math.Round(NumberofDay) + " days old\n");
                                            new ErrorLogger().CustomLog(data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "Deletelogs"));
                                        }
                                        catch (Exception ex)
                                        {
                                            new ErrorLogger().ExceptionLog(ex);
                                        }
                                    }
                                }
                            }
                        }
                        //check if file size is greater then 1 gig
                        files = maindir.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(d => d.LastWriteTime.Year).ThenBy(d => d.LastWriteTime.Month).ThenBy(d => d.LastWriteTime.Day).Select(x => x).ToList();
                        if (target_size > 0)
                        {
                            if (files.Count > 0)
                            {
                                foreach (FileInfo file in files)
                                {
                                    if (file.Length > target_size)
                                    {
                                        try
                                        {
                                            file.Delete();
                                            string data = string.Concat("FileName : " + file.FullName
                                                , " | FileSize : " + FormatBytes(file.Length)
                                                , " | Delete Date/Time : ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                                                , " | File was deleted because file exceed file size ", FormatBytes(target_size));
                                            new ErrorLogger().CustomLog(data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "Deletelogs"));
                                        }
                                        catch (Exception ex)
                                        {
                                            new ErrorLogger().ExceptionLog(ex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                _SP.manualEvents.Set();
            }
            finally
            {
                _SP.manualEvents.Set();
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
    }
}