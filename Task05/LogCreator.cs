using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class LogCreator
    {
        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;


        public void CreateLog(string logDir, string backupDir)
        {
            object obj = new object();
            lock (obj)
            {
                if (!Directory.Exists(backupDir))
                {
                    logDir = Path.Combine(backupDir, "log.txt");
                    Directory.CreateDirectory(backupDir);
                    File.Create(logDir).Close();
                }
            }
        }

        public void WriteLog(string textLog, string logDir, string backupDir)
        {
            object obj = new object();
            lock (obj)
            {
                if (!Directory.Exists(backupDir))
            {
                CreateLog(logDir, backupDir);
            }
            else
            {
                    IsFileLocked("C:\\Backuper\\log.txt");
                    using (var fileStream = File.Open("C:\\Backuper\\log.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (StreamWriter log = new StreamWriter(fileStream))
                        {
                            log.WriteLine(textLog);
                        }
                    }
                }
            }
        }

        
        private bool IsFileLocked(string file)
        {
            if (File.Exists(file) == true)
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex2)
                {
                    int errorCode = Marshal.GetHRForException(ex2) & ((1 << 16) - 1);
                    if ((ex2 is IOException) && (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
                    {
                        return true;
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return false;
        }
    }
}
