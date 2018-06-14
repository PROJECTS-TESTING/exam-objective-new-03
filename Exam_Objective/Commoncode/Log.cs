using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ConsoleAppLog
{
   
    public class Log
    {
        public static readonly object lockerError = new object();
        public static readonly object lockerInfo = new object();
        
        private static void Init()
        {
            string logPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Logs\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }
        private static void Init(string folderName)
        {
            string logPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\" + folderName + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }
        // --Method ที่ใช้งาน -----------------------------------------------------
        public static void Warning(string message)
        {        
            WriteWarning(message);
        }

        public static void Error(string message)
        {
            WriteError(message);
        }

        public static void Info(string message)
        {
            WriteInfo(message);
        }

        public static void More(string message, string fileName)
        {
             WriteMore(message, fileName);
        }

        public static void More(string message, string folderName, string fileName)
        {
           WriteMore(message, folderName, fileName);
        }
        //----------------------------------------------------------------------------
        private static void WriteWarning(string message)
        {
            try
            {
              
                    Init();
                    string fileName = string.Empty;
                    fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Logs\" +
                                DateTime.Now.ToString("yyyy-MM-dd") + @"\" + $"Warning-{DateTime.Now:yyyyMMdd}" + ".txt";
                    using (StreamWriter sw = new StreamWriter(fileName, true))
                    {
                        sw.Write("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now);
                        sw.WriteLine(message);
                        sw.Close();
                        sw.Dispose();
                    }
                
            }
            catch
            {

            }
        }

        private static void WriteError(string message)
        {
            try
            {
               
                    Init();
                    string fileName = string.Empty;
                    fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Logs\" +
                               DateTime.Now.ToString("yyyy-MM-dd") + @"\" + $"Error-{DateTime.Now:yyyyMMdd}" + ".txt";
                    using (StreamWriter sw = new StreamWriter(fileName, true))
                    {
                        sw.Write("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now);
                        sw.WriteLine(message);
                        sw.Close();
                        sw.Dispose();
                    }

                    //try
                    //{
                    //    string email = System.Configuration.ConfigurationManager.AppSettings["ErrorReportEmail"];
                    //    if (!string.IsNullOrEmpty(email))
                    //    {
                    //        using (var db = new DataEntities())
                    //        {
                    //            var conf = db.SettingEmail.FirstOrDefault();
                    //            if (conf != null)
                    //            {
                    //                conf.EmailTo = email;
                    //                Shared.SendEmail(conf, "เกิดข้อผิดพลาด","["+ DateTime.Now.ToString("dd/MM/yyyy HH:mm") +"] " + message);
                    //            }
                    //        }
                    //    }
                    //}catch{}
                
            }
            catch
            {

            }
        }

        private static void WriteInfo(string message)
        {
            try
            {
               
                    Init();
                    string fileName = string.Empty;
                    fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Logs\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\" + $"Info-{DateTime.Now:yyyyMMdd}" + ".txt";
                    using (StreamWriter sw = new StreamWriter(fileName, true))
                    {
                        sw.Write("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now);
                        sw.WriteLine(message);
                        sw.Close();
                        sw.Dispose();
                    }
                
            }
            catch
            {

            }
        }

        private static void WriteMore(string message, string fileName)
        {
            try
            {
                
                    Init();
                    fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Logs\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\" + fileName + ".txt";
                    using (StreamWriter sw = new StreamWriter(fileName, true))
                    {
                        sw.Write("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now);
                        sw.WriteLine(message);
                        sw.Close();
                        sw.Dispose();
                    }
                
            }
            catch
            {

            }
        }

        private static void WriteMore(string message, string folderName, string fileName)
        {
            try
            {
               
                    Init(folderName);
                    fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\" + folderName + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\" + fileName + ".txt";
                    using (StreamWriter sw = new StreamWriter(fileName, true))
                    {
                        sw.Write("{0:dd/MM/yyyy-HH:mm:ss} | ", DateTime.Now);
                        sw.WriteLine(message);
                        sw.Close();
                        sw.Dispose();
                    }
                
            }
            catch
            {

            }
        }
       

        private static void DeleteFile()
        {
            string pahtDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Logs\" +
                                DateTime.Now.ToString("yyyy-MM-dd");
            DirectoryInfo directoryInfo = new DirectoryInfo(pahtDirectory);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
                
            }

        }
        // กำหนดวันที่เก็บไฟล์ จำนวนเก็บกี่วัน
        public static void DelFolderLog(int day)
        {

            DeleteLogLimitTimes(day, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Logs\");

        }

        private static void DeleteLogLimitTimes(int day, string path)
        {
            DirectoryInfo yourRootDir = new DirectoryInfo(path);
            DirectoryInfo[] diir = yourRootDir.GetDirectories();
            int[] min;
            min = new int[diir.Length];
            if (diir.Length > day)
            {
                for (int i = 0; i < diir.Length; i++)
                {
                    string[] da = diir[i].ToString().Split('-');
                    min[i] = Int32.Parse(da[0] + da[1] + da[2]);
                }
                Array.Sort(min);
                DeleteDirectory(path + min[0].ToString().Substring(0, 4) + "-" + min[0].ToString().Substring(4, 2) + "-" + min[0].ToString().Substring(6, 2), true);
            }
        }

        private static void DeleteDirectory(string directoryName, bool checkDirectiryExist)
        {
            if (Directory.Exists(directoryName))
                Directory.Delete(directoryName, true);
            else if (checkDirectiryExist)
                throw new SystemException("Directory you want to delete is not exist");
        }
    }
}
