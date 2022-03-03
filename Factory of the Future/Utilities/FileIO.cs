using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Factory_of_the_Future
{
    public class FileIO
    {
        private int tryattp = 0;
        private TimeSpan interval = new TimeSpan(0, 0, 10);

        internal string Read(string DirectoryPath, string FileName)
        {
            string filecontect = string.Empty;
            try
            {
                DirectoryInfo maindir = new DirectoryInfo(DirectoryPath);
                if (!maindir.Exists)
                {
                    return filecontect;
                }
                else
                {
                    List<FileInfo> filesList = maindir.GetFiles(FileName, SearchOption.TopDirectoryOnly).Select(x => x).ToList();

                    if (filesList.Count > 0)
                    {
                        foreach (FileInfo _file in filesList)
                        {
                            if (FileName.ToUpper() == _file.Name.ToUpper())
                            {
                                if (tryattp < 10)
                                {
                                    if (!IsFileLocked(_file))
                                    {
                                        tryattp = 0;
                                        FileStream fileStream = null;
                                        try
                                        {
                                            MemoryStream ms = new MemoryStream();
                                            using (fileStream = new FileStream(_file.FullName.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                                fileStream.CopyTo(ms);
                                            ms.Position = 0;
                                            StreamReader msr = new StreamReader(ms);
                                            filecontect = msr.ReadToEnd().ToString();
                                            msr.Close();
                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            new ErrorLogger().ExceptionLog(e);
                                        }
                                        finally
                                        {
                                            fileStream?.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        tryattp++;
                                        //sleep of 10 seconds and try again
                                        Thread.Sleep(interval);
                                        Read(DirectoryPath, FileName);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return filecontect;
                    }
                    return filecontect;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return filecontect;
            }
        }

        internal void Write(string DirectoryPath, string FileName, string data)
        {
            try
            {
                DirectoryInfo maindir = new DirectoryInfo(DirectoryPath);
                if (!maindir.Exists)
                {
                    Directory.CreateDirectory(maindir.FullName);
                    maindir = new DirectoryInfo(maindir.FullName);
                }
                if (maindir.Exists)
                {
                    List<FileInfo> filesList = maindir.GetFiles(FileName, SearchOption.TopDirectoryOnly).Select(x => x).ToList();
                    if (filesList.Count > 0)
                    {
                        foreach (FileInfo _file in filesList)
                        {
                            if (FileName.ToUpper() == _file.Name.ToUpper())
                            {
                                if (Path.GetFileName(_file.Name).Contains("txt"))
                                {
                                    FileStream file = new FileStream(_file.FullName.ToString(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                    using (StreamWriter sr = new StreamWriter(file, Encoding.UTF8))
                                    {
                                        file = null;
                                        sr.WriteLine(data);
                                    }
                                }
                                else
                                {
                                    FileStream file = new FileStream(maindir.FullName.ToString() + "\\" + FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                                    using (StreamWriter sr = new StreamWriter(file, Encoding.UTF8))
                                    {
                                        file = null;
                                        sr.WriteLine(data);
                                    }
                                }
                            }
                        }
                    }
                    else if (filesList.Count == 0)
                    {
                        FileStream file = new FileStream(maindir.FullName.ToString() + "\\" + FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        using (StreamWriter sr = new StreamWriter(file, Encoding.UTF8))
                        {
                            file = null;
                            sr.WriteLine(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailabel because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}