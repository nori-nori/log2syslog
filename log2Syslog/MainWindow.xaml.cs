using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;


namespace Log2Syslog
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public static String Progname = "log2syslog";
        public Boolean debug = false;


        private static String ConfigFile = @"config.xml";
        private static Boolean Modified = false;
    
        public MainWindow()
        {
            String[] args = Environment.GetCommandLineArgs();

            InitializeComponent();

            // For Event log
            EventLog.Init(Progname);

            /* option */
            foreach (String s in args)
            {
                if (s.Equals("/d"))
                {
                    this.debug = true;
                    this.Title = Progname + "(Debug)";
                }
            }

            /* read config */
            ReadAppConfig();

        }



        private void subMenu_New_Click(object sender, RoutedEventArgs e)
        {
            NewDiag newDiag = new NewDiag();
            newDiag.Closed += new EventHandler(subMenu_Closed_Handler);
            newDiag.Show();
        }

        private void subMenu_Edit_Click(object sender, RoutedEventArgs e)
        {
            int index = this.L_MainList.SelectedIndex;
            NewDiag newDiag = new NewDiag();
            newDiag.SetTargetParam(this.L_MainList.Items[index] as Target, index);
            newDiag.Closed += new EventHandler(subMenu_Closed_Handler);
            newDiag.Show();            
        }


        private void subMenu_Closed_Handler(object sender, EventArgs e)
        {
            NewDiag newDiag = sender as NewDiag;

            if (newDiag.Result == true)
                {
                Target t = new Target(false,
                        newDiag.T_FileTextBox.Text,
                        newDiag.T_NameTextBox.Text,
                        new Syslog() {
                            Priority = (newDiag.C_Syslog_Priority.SelectedItem as Priority).Id,
                            Facility = (newDiag.C_Syslog_Facility.SelectedItem as Facility).Id,
                           
                            Receiver = newDiag.T_Syslog_IP.Text,
                            AddDate = (newDiag.C_AddDate.IsChecked == true) ? true : false,
                        }
                );

                if (newDiag.EditMode == false) 
                {
                    this.L_MainList.Items.Add(t);
                } else {
                    this.L_MainList.Items[newDiag.Index] = t;
                }

                t.Init();
                Modified = true;
            }

        }


        private void subMenu_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Modified == false) return;
            WriteAppConfig();
            Modified = false;
        }

        private void subMenu_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void subMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = this.L_MainList.SelectedIndex;

            if (index == -1) return;

            MessageBoxResult result = MessageBox.Show(
                    "Delete ?", "Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;

            this.L_MainList.Items.RemoveAt(index);
            Modified = true;
        }


        private void subMenu_Start_Click(object sender, RoutedEventArgs e)
        {
            int index = this.L_MainList.SelectedIndex;

            if (index != -1)
            {
                Target target = (Target)this.L_MainList.Items[index];
                target.Start();
            }
            Modified = true;
        }




        private void subMenu_Stop_Click(object sender, RoutedEventArgs e)
        {
            Target target = (Target)this.L_MainList.SelectedItem;

            if (target.Running == false
)
                return;

            MessageBoxResult result = MessageBox.Show(
                "Stop?", "Stop",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;

            target.Stop();
            Modified = true;
        }


        private void subMenuList_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                    "Exit?", "Exit",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            if (Modified == true)
            {
                result = MessageBox.Show(
                        "Save?", "Save",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                    WriteAppConfig();
            }
        }



        /* config read/write */

        private Boolean WriteAppConfig()
        {
            if (Modified == false)
                return true;
           
            XDocument xml = new XDocument(
                new XDeclaration( "1.0" , "us-ascii" , "true" ));
            XElement el = new XElement(Progname);

            foreach (object item in this.L_MainList.Items)
            {
                el.Add(new XElement("Target",
                    new XElement("Name", (item as Target).Name),
                    new XElement("FullPath", (item as Target).FullPath),
                    new XElement("Syslog",
                        new XElement("Receiver", (item as Target).Syslog.Receiver),
                        new XElement("Priority", ((item as Target).Syslog.Priority)),
                        new XElement("Facility", ((item as Target).Syslog.Facility)),
                        new XElement("Date"), 
                           ((((Target)item).Syslog.AddDate) == true) ? "0" : "1"),
                    new XElement("Running", ((item as Target).Running) ? "0" : "1"))
               );
            }

            xml.Add(el);
            xml.Save(ConfigFile);

            return true;
        }

        private Boolean ReadAppConfig()
        {
            XElement xml;
            try
            {
                xml = XElement.Load(ConfigFile);
            }
            catch (SystemException)
            {
                EventLog.Write("config load error",
                    System.Diagnostics.EventLogEntryType.Warning,
                    1
                    );
                return false;
            }

            try {
                IEnumerable<XElement> target = xml.Elements("Target");
                foreach (XElement el in target)
                {
                    Target t = new Target(
                        ((el.Element("Running").Value.Equals("0")) ? true : false),
                        el.Element("FullPath").Value,
                        el.Element("Name").Value,
                        new Syslog() {
                            Priority = int.Parse(el.Element("Syslog").Element("Priority").Value),
                            Facility = int.Parse(el.Element("Syslog").Element("Facility").Value),
                            Receiver = el.Element("Syslog").Element("Receiver").Value,
                            AddDate = 
                              (el.Element("Syslog").Element("Date").Value.ToString().Equals("0")) ? true : false
                       }
                    );
                    t.Init();
                    this.L_MainList.Items.Add(t);
                    if (t.Running == true) t.Start();
                }
            }

            catch (SystemException) {
                    EventLog.Write("Invalid Config, set to default",
                        System.Diagnostics.EventLogEntryType.Error,
                        1);
            }

            return true;
        }


    }
}
