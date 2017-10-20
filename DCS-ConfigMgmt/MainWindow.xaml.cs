using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace DCS_ConfigMgmt
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Prefilling all DCS directories
            textBox_dcsdir_current.Text = GetDCSRegistryPath("current release");
            textBox_dcsdir_alpha.Text = GetDCSRegistryPath("alpha release");
            textBox_dcsdir_beta.Text = GetDCSRegistryPath("beta release");

        }

        /// 
        /// Path selection buttons
        /// 
        private void Button_dcsdir_current_Click(object sender, RoutedEventArgs e)
        {
            string branch = "current release";
            textBox_dcsdir_current.Text = DirectoryPicker(branch);
        }
        private void Button_dcsdir_alpha_Click(object sender, RoutedEventArgs e)
        {
            string branch = "alpha release";
            textBox_dcsdir_alpha.Text = DirectoryPicker(branch);
        }

        private void Button_dcsdir_beta_Click(object sender, RoutedEventArgs e)
        {
            string branch = "beta release";
            textBox_dcsdir_beta.Text = DirectoryPicker(branch);
        }
        /// 
        /// Path selection function - returns string
        /// 
        private string DirectoryPicker(string branch)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.Description = "Please select your DCS folder (" + branch + ")";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.Windows.Forms.MessageBox.Show(dlg.SelectedPath);
                return dlg.SelectedPath;
            }
            else { return null; }
        }

        /// 
        /// Registry Data Get - returns string
        /// 
        private string GetDCSRegistryPath(string branch)
        {
            string sDCSpath = null;
            RegistryKey hkcu = Registry.CurrentUser; //HKCU Registry

            if (branch == "current release")
            {
                hkcu = hkcu.OpenSubKey(@"Software\Eagle Dynamics\DCS World");
                try { sDCSpath = (string)hkcu.GetValue("Path"); }
                catch { }
            }
            else if (branch == "alpha release")
            {
                hkcu = hkcu.OpenSubKey(@"Software\Eagle Dynamics\DCS World 2 OpenAlpha");
                try { sDCSpath = (string)hkcu.GetValue("Path"); }
                catch { }
            }
            else if (branch == "beta release")
            {
                hkcu = hkcu.OpenSubKey(@"Software\Eagle Dynamics\DCS World OpenBeta");
                try { sDCSpath = (string)hkcu.GetValue("Path"); }
                catch { }
            }

            return sDCSpath;
        }
    }
}
