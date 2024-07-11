using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace QA.API.Shared
{
    public class LogWriter
    {
        public LogWriter() { }

        public void WriteLog(string sLog, string sPathAPI)
        {
            string path = HttpContext.Current.Server.MapPath("~/LogFiles") + "\\Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.WriteLine("File Log Created !");
                    tw.Close();
                }
            }

            if (File.Exists(path))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Environment.NewLine + DateTime.Now.ToString() + " : " + sPathAPI + Environment.NewLine);
                sb.Append(sLog);
                File.AppendAllText(path, sb.ToString());
                sb.Clear();
            }
        }

    }
}