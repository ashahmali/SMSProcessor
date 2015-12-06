using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace AMSMSService{
    public static class Logger {
        // Calculate the log file's name.
        
        private static string LogFile = "SmSServiceLog.txt";
        static EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES");
        

        // Write the current date and time plus
        // a line of text into the log file.
        public static void WriteLineToFile(string txt) {
            try {
                waitHandle.WaitOne();
                StreamWriter w = File.AppendText(LogFile);
                Log(txt, w);
                w.Close();
                waitHandle.Set();
            } catch(Exception Ex) {
                Console.WriteLine(Ex.Message);
                return;
            } 
        }

        public static void Log(string logMessage, TextWriter w) {
            w.Write("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.Write("  : {0}", logMessage);
            w.WriteLine();
        }

        // Delete the log file.
        public static void DeleteLog() {
            File.Delete(LogFile);
        }
    }
}
