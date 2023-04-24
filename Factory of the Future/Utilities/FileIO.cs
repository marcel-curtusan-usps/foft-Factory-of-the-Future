using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Factory_of_the_Future
{
    public class FileIO : IDisposable
    {
        private bool disposedValue;
        public string filecontect { get; protected set; } = "";
        public List<FileInfo> filesList = null;
        public DirectoryInfo maindir = null;
        public FileStream file = null;
        internal string Read(string DirectoryPath, string FileName)
        {

            try
            {
                maindir = new DirectoryInfo(DirectoryPath);
                if (!maindir.Exists)
                {
                    return filecontect;
                }
                else
                {
                    filesList = maindir.GetFiles(FileName, SearchOption.TopDirectoryOnly).Select(x => x).ToList();

                    if (filesList.Count > 0)
                    {
                        foreach (FileInfo _file in filesList)
                        {
                            if (FileName.ToUpper() == _file.Name.ToUpper())
                            {
                                try
                                {
                                    using (FileStream fileStream = new FileStream(_file.FullName.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    using (StreamReader msr = new StreamReader(fileStream))
                                    {
                                        filecontect = msr.ReadToEnd().ToString();
                                    }
                                }
                                catch (Exception e)
                                {
                                    new ErrorLogger().ExceptionLog(e);
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
            finally
            {
                Dispose();
            }
        }

        internal void Write(string DirectoryPath, string FileName, string data)
        {
            filecontect = data;
            try
            {
                maindir = new DirectoryInfo(DirectoryPath);
                if (!maindir.Exists)
                {
                    Directory.CreateDirectory(maindir.FullName);
                    maindir = new DirectoryInfo(maindir.FullName);
                }
                if (maindir.Exists)
                {
                    filesList = maindir.GetFiles(FileName, SearchOption.TopDirectoryOnly).Select(x => x).ToList();
                    if (filesList.Count > 0)
                    {
                        foreach (FileInfo _file in filesList)
                        {
                            if (FileName.ToUpper() == _file.Name.ToUpper())
                            {
                                if (Path.GetFileName(_file.Name).Contains("txt"))
                                {
                                    file = new FileStream(_file.FullName.ToString(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                    using (StreamWriter sr = new StreamWriter(file, Encoding.UTF8))
                                    {
                                        file = null;
                                        sr.WriteLine(filecontect);
                                    }
                                }
                                else
                                {
                                    file = new FileStream(maindir.FullName.ToString() + "\\" + FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                                    using (StreamWriter sr = new StreamWriter(file, Encoding.UTF8))
                                    {
                                        file = null;
                                        sr.WriteLine(filecontect);
                                    }
                                }
                            }
                        }
                    }
                    else if (filesList.Count == 0)
                    {
                        file = new FileStream(maindir.FullName.ToString() + "\\" + FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)

                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
                filecontect = string.Empty;
                filesList = null;
                maindir = null;
                file = null;

            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FileIO()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}