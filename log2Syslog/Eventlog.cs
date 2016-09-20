using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Log2Syslog
{
    class EventLog
    {
        private static String Progname = "";
        private static Boolean available = false;

        public static Boolean Init(String Progname)
        {
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists(Progname))
                {
                    System.Diagnostics.EventLog.CreateEventSource(Progname, "");
                }

                EventLog.Progname = Progname;
                return (available = true);
            }
            catch
            {
                return (available = false);
            }
        }

        public static void Write(String Message, EventLogEntryType Type, int EventId)
        {
            if (available == false) return;

            System.Diagnostics.EventLog.WriteEntry(
                EventLog.Progname,
                Message,
                Type,
                EventId);

        }


    }
}
