using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Net;

namespace Log2Syslog
{
    /// <summary>
    /// NewDiag.xaml の相互作用ロジック
    /// </summary>
    public partial class NewDiag : Window
    {
        public Boolean Result;
        public Boolean EditMode;
        public int Index;
       
        public NewDiag()
        {
            InitializeComponent();
            this.Result = false;
            this.EditMode = false;
        }

        public void SetTargetParam(Target target, int Index)
        {
            this.T_FileTextBox.Text = target.FullPath;
            this.T_Syslog_IP.Text = target.Syslog.Receiver;
            this.T_NameTextBox.Text = target.Name;
            this.C_Syslog_Facility.SelectedIndex = 
                SyslogFacilityList.Id2Index(target.Syslog.Facility);
            this.C_Syslog_Priority.SelectedIndex = 
                SyslogPriorityList.Id2Index(target.Syslog.Priority);
            this.C_AddDate.IsChecked = target.Syslog.AddDate;
            this.Index = Index;
            this.EditMode = true;
        }


        private void NewDiagOK_Click(object sender, RoutedEventArgs e)
        {
            /* validation */

            /* Filename and Path */
            if (this.T_FileTextBox.Text.Equals(String.Empty))
            {
                MessageBox.Show("No such target file");
                return;
            }

            /* syslog server */
            if (this.T_Syslog_IP.Text.Equals(String.Empty))
            {
                MessageBox.Show("Invalid Syslog Server Empty");
                return;
            }

            this.Result = true;
            this.Close();
        }

        private void NewDiagCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Result = false;
            this.Close();
        }

        private void OpenComDiag_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.DefaultExt = "*.*";
            if (ofd.ShowDialog() == true)
            {
                this.T_FileTextBox.Text = ofd.FileName;
            }
        }
    }
}
