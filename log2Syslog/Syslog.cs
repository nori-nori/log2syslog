using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;



namespace Log2Syslog
{
    /// <summary>
    /// Syslog Class
    /// </summary>

    class KeyValue
    {
        public String Name;
        public int Id;
        public override string ToString()
        {
            return Name;
        }
    }

    class Facility : KeyValue
    {
    }


    static class SyslogFacilityList
    {
        public static Facility[] List = new Facility[]{
                new Facility(){Name = "KERNEL", Id = 0},
                new Facility(){Name = "USER"  , Id = 1},
                new Facility(){Name = "MAIL"  , Id = 2},
                new Facility(){Name = "DAEMON", Id = 3},
                new Facility(){Name = "AUTH"  , Id = 4},
                new Facility(){Name = "SYSLOG", Id = 5},
                new Facility(){Name = "LPR"   , Id = 6},
                new Facility(){Name = "NEWS"  , Id = 7},
                new Facility(){Name = "UUCP"  , Id = 8},
                new Facility(){Name = "CRON"  , Id = 9},
                new Facility(){Name = "AUTHPRIV", Id = 10},
                new Facility(){Name = "FTP"   , Id = 11},
                new Facility(){Name = "NTP"   , Id = 12},
                new Facility(){Name = "SECURITY", Id = 13},
                new Facility(){Name = "CONSOLE", Id = 14},
                new Facility(){Name = "LOCAL0", Id = 16},
                new Facility(){Name = "LOCAL1", Id = 17},
                new Facility(){Name = "LOCAL2", Id = 18},
                new Facility(){Name = "LOCAL3", Id = 19},
                new Facility(){Name = "LOCAL4", Id = 20},
                new Facility(){Name = "LOCAL5", Id = 21},
                new Facility(){Name = "LOCAL6", Id = 22},
                new Facility(){Name = "LOCAL7", Id = 23}};

        public static String FacilityName(int id)
        {
            foreach (Facility f in SyslogFacilityList.List)
            {
                if (f.Id == id) return f.Name;
            }
            return "";
        }

        public static int Id2Index(int id)
        {
            int i = 0;
            foreach (Facility f in SyslogFacilityList.List)
            {
                if (f.Id == id) return i;
                i++;
            }
            return 0;
        }

    }

    class Priority : KeyValue
    {
    }

    static class SyslogPriorityList
    {
        public static Priority[] List = new Priority[]{
                new Priority(){Name = "EMERGENCY", Id = 0},
                new Priority(){Name = "ALERT"    , Id = 1},
                new Priority(){Name = "CRITICAL" , Id = 2},
                new Priority(){Name = "ERROR"    , Id = 3},
                new Priority(){Name = "WARNING"  , Id = 4},
                new Priority(){Name = "NOTICE"   , Id = 5},
                new Priority(){Name = "INFO"     , Id = 6},
                new Priority(){Name = "DEBUG"    , Id = 7}
        };

        public static String PriorityName(int id)
        {
            foreach (Priority f in SyslogPriorityList.List)
            {
                if (f.Id == id) return f.Name;
            }
            return "";
        }

        public static int Id2Index(int id)
        {
            int i = 0;
            foreach (Priority p in SyslogPriorityList.List)
            {
                if (p.Id == id) return i;
                i++;
            }
            return 0;
        }
    }


    public class Syslog
    {
        public int Priority = 0;
        public int Facility = 0;
        public Boolean AddDate = false;
        public String Receiver = "";
        public String HostName = "";
        public String Name = "";
        
      
        public void Send(String msg)
        {
            UdpClient uc = new UdpClient();
            Byte[] body = Encoding.ASCII.GetBytes(BuildBody(msg));
            Console.WriteLine(msg);
            try
            {
                uc.Send(body,
                    (body.Length > 512) ? 512 : body.Length, this.Receiver, 514);
                uc.Close();
            }
            catch (SystemException ex)
            {
                EventLog.Write(
                    "Can not send syslog message to " + this.Receiver + ":" + ex.ToString(),
                     System.Diagnostics.EventLogEntryType.Error, 1);

            }

        }

        private String BuildBody(String msg)
        {
            String body;
            IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
            String date;
            String name = "";


            if (this.AddDate == true)
            {
                date = DateTime.Now.ToString(" yyyy/MM/dd HH:mm:ss ");
            }
            else
            {
                date = String.Empty;
            }

            if (this.Name != "")
            {
                name = String.Format("[{0}] ", this.Name);
            }
            
            body = String.Format("<{0}>{1}{2}{3}",
                (this.Priority | (this.Facility << 3)),
                date,
                name,
                msg);
            return body;
        }
    }
}
