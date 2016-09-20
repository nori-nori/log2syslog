using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Log2Syslog
{
    class Watcher : FileSystemWatcher
    {
        public FileInfo fi;
        public FileStream fs;
        public StreamReader sr;

        private String FullPath;
        private Syslog syslog;
        

        
        public Watcher(Target t)
        {
            this.syslog = t.Syslog;
            this.FullPath = t.FullPath;
            this.Path = t.DirName;
            this.Filter = t.FileName;
            this.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                              | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            this.Changed += new FileSystemEventHandler(OnChanged);
            this.Created += new FileSystemEventHandler(OnChanged);
            this.Deleted += new FileSystemEventHandler(OnDeleted);
            this.Renamed += new RenamedEventHandler(OnRenamed);
            this.FileOpen(t.FullPath);
        }


        public Boolean Start()
        {
            try
            {
                this.EnableRaisingEvents = true;
                return true;
            }
            catch (ArgumentException)
            {
                EventLog.Write(
                    "cannot start " + this.FullPath,
                    System.Diagnostics.EventLogEntryType.FailureAudit,
                    1);
                return false;

            }

            catch (SystemException)
            {
                return false;
            }
        }

        public void Stop()
        {
            this.EnableRaisingEvents = false;
        }


        private void FileOpen(String fullPath)
        {
            this.fi = new FileInfo(fullPath);
            if (this.fi.Exists)
            {
                this.fs = this.fi.Open(FileMode.Open, FileAccess.Read,
                                       FileShare.ReadWrite | FileShare.Delete);
                this.fs.Seek(0, SeekOrigin.End);
                this.sr = new StreamReader(this.fs);
            }
            else
            {
                this.fs = null;
            }
        }



        /* handler */

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            String line;

            if (this.fi == null)
            {
                this.FileOpen(this.FullPath);
                Thread.Sleep(100);
            }

            while ((line = this.sr.ReadLine()) != null)
            {
                Console.WriteLine(line);
                    this.syslog.Send(line);

            }

        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            OnDeletedOrRenamed();
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            OnDeletedOrRenamed();
        }

        private void OnDeletedOrRenamed()
        {
            this.fs.Close();
            this.fi = null;

        }
    }
}
