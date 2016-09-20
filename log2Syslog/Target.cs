using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel; 
using System.Threading;
using System.IO;
using System.Diagnostics;


namespace Log2Syslog
{
    public class Target : INotifyPropertyChanged 
    {
        /* variables */
        public String Name{ get; set; }
        public String FullPath{ get; set; }
        public String Action{ get; set; }
        public String Active{ get; set; }
        public Syslog Syslog { get; set; }
        public String Status{ get; set; }
        public String SyslogServer { get; set; }

        public String DirName;
        public String FileName;

        private Watcher watcher = null;

        private Boolean _running;
        public Boolean Running
        {
            get
            {
                return this._running;
            }

            set
            {
                this._running = value;
                if (value == true)
                {
                    this.Status = " ";
                }
                else
                {
                    this.Status = "S";
                }

                OnPropertyChanged("Status");
            }

        }


        /* handler */
        public event PropertyChangedEventHandler PropertyChanged; 


        /* methods */
     
        
        public Target(Boolean running, String fullPath, String name, Syslog syslog)
        {
            this.Name = name;
            this.FullPath = fullPath;
            this.Running = running;
            this.Syslog = syslog;
            this.Syslog.Name = name;
            this.SyslogServer = this.Syslog.Receiver;
            }

        public void Init()
        {
            this.DirName = System.IO.Path.GetDirectoryName(this.FullPath);
            this.FileName = System.IO.Path.GetFileName(this.FullPath);

            if (this.watcher != null)
                Stop();

            this.watcher = new Watcher(this);

        }


        public void Start()
        {
            this.watcher = new Watcher(this);
            this.Running = this.watcher.Start();
        }

        public void Stop()
        {
            this.Running = false;
            this.watcher.Stop();
            this.watcher.Dispose();
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        } 
    }
}
