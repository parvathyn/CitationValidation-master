using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public static class ExceptionLogging
    {

        private static String ErrorlineNo, Errormsg, extype, exurl, hostIp, ErrorLocation, HostAdd;

        public static void SendErrorToText(Exception ex, string data,string filepath)
        {
            var line = Environment.NewLine + Environment.NewLine;

            ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            exurl = data;
            ErrorLocation = ex.Message.ToString();

            try
            {
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + " Error Page Url:" + " " + exurl + line + "User Host IP:" + " " + hostIp + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine(ex.StackTrace);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }

            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        public  static void SendErrorToText2(Exception ex, string data, string filepath)
        {
 
            try
            {
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }


                var line = Environment.NewLine + Environment.NewLine;

                ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
                Errormsg = ex.GetType().Name.ToString();
                extype = ex.GetType().ToString();
             
                ErrorLocation = ex.Message.ToString();


                StringBuilder sb = new StringBuilder();
                sb.Append(Environment.NewLine);
                sb.AppendFormat("UTC Time : {0} ", DateTime.UtcNow.ToString("O")).Append(Environment.NewLine);
                sb.AppendFormat("Server Time : {0} ", DateTime.Now.ToString("O")).Append(Environment.NewLine);
                sb.Append("Exurl : ").AppendFormat("{0}",data).Append(Environment.NewLine);
                if(ex != null)
                {
                    sb.Append("ErrorMessage : ").AppendFormat("{0}", ex.Message).Append(Environment.NewLine);
                    if(!String.IsNullOrWhiteSpace(ex.StackTrace))
                    {
                        sb.Append("Trace : ").AppendFormat("{0}", ex.StackTrace).Append(Environment.NewLine);
                    }
                }
                sb.Append("----------------------------------------------------------------------------------------------------------");
                using (StreamWriter sw = File.AppendText(filepath))
                {
                  if(sb != null)
                  {
                       sw.WriteAsync(sb.ToString());
                  }
                }

            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        
    }  
}
