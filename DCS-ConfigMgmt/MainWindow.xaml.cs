/// 
/// Todo:
/// - Saving current branch, read and enable radiobutton+other visual, use for unlink button, enable unlink button only when set
/// - do the symlink thing for link and unlink
/// 
/// 
/// 
/// 
/// 
/// 
/// 
/// 

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
using System.IO;
using System.Runtime.InteropServices;

namespace DCS_ConfigMgmt
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(
        string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        public MainWindow(string sStartOption)
        {
            InitializeComponent();

            //// Prefilling all DCS directories
            // Get DCS directories if settings are empty
            if (Properties.Settings.Default.sPathCurrent == "")
            {
                Properties.Settings.Default.sPathCurrent = GetDCSSavePath("current");
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.sPathAlpha == "")
            {
                Properties.Settings.Default.sPathAlpha = GetDCSSavePath("alpha");
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.sPathBeta == "")
            {
                Properties.Settings.Default.sPathBeta = GetDCSSavePath("beta");
                Properties.Settings.Default.Save();
            }

            //Write textboxes
            if (GetDCSRegistryPath("current") != null)
            {
                textBox_dcsdir_current.Text = Properties.Settings.Default.sPathCurrent;
            }
            else
            {
                textBox_dcsdir_current.Text = "Not found.";
            }

            if (GetDCSRegistryPath("alpha") != null)
            {
                    textBox_dcsdir_alpha.Text = Properties.Settings.Default.sPathAlpha;
            }
            else
            {
                textBox_dcsdir_alpha.Text = "Not found.";
            }

            if (GetDCSRegistryPath("beta") != null)
            {
                textBox_dcsdir_beta.Text = Properties.Settings.Default.sPathBeta;
            }
            else
            {
                textBox_dcsdir_beta.Text = "Not found.";
            }

            //check for current branch
            if(Properties.Settings.Default.sMasterBranch != "")
            {
                if (Properties.Settings.Default.sMasterBranch == "Current") { radioButton_dcsdir_current.IsChecked = true; }
                else if (Properties.Settings.Default.sMasterBranch == "Alpha") { radioButton_dcsdir_alpha.IsChecked = true; }
                else if (Properties.Settings.Default.sMasterBranch == "Beta") { radioButton_dcsdir_beta.IsChecked = true; }

                radioButton_dcsdir_current.IsEnabled = false;
                radioButton_dcsdir_alpha.IsEnabled = false;
                radioButton_dcsdir_beta.IsEnabled = false;

                button_unlinkcontrols.IsEnabled = true;
                button_linkcontrols.IsEnabled = false;
            }

            //Check for first run
            if (!Properties.Settings.Default.bSawConfigWarning)
            {
                System.Windows.Forms.MessageBox.Show("Hey there!\n\n" +
                    "As this is your first time using this tool, please check the automatically detected directories and determine if your current config has VR enabled or not.\n\n" +
                    "Click 'Load VR Settings' if VR is currently -disabled-.\n" +
                    "Click 'Load nonVR Settings' if VR is currently -enabled-.\n\nThis will create a copy of the current configuration. Then you may load the other one to do your VR/nonVR settings. After that, you can just switch between them. " +
                    "Please do this for every version installed. If you don't use VR, just ignore it.", "First use information");

                Properties.Settings.Default.bSawConfigWarning = true;
                Properties.Settings.Default.Save();
            }

            //Check configs for VR/nonVR and disable buttons accordingly
            if (!Properties.Settings.Default.bFirstUseVRCurrent)
            {
                if (Properties.Settings.Default.bVRConfActiveCurrent)
                {
                    button_load_vr_current.IsEnabled = false;
                    button_load_nonvr_current.IsEnabled = true;
                }
                else
                {
                    button_load_vr_current.IsEnabled = true;
                    button_load_nonvr_current.IsEnabled = false;
                }
            }
            if (!Properties.Settings.Default.bFirstUseVRBeta)
            {
                if (Properties.Settings.Default.bVRConfActiveBeta)
                {
                    button_load_vr_beta.IsEnabled = false;
                    button_load_nonvr_beta.IsEnabled = true;
                }
                else
                {
                    button_load_vr_beta.IsEnabled = true;
                    button_load_nonvr_beta.IsEnabled = false;
                }
            }
            if (!Properties.Settings.Default.bFirstUseVRAlpha)
            {
                if (Properties.Settings.Default.bVRConfActiveAlpha)
                {
                    button_load_vr_alpha.IsEnabled = false;
                    button_load_nonvr_alpha.IsEnabled = true;
                }
                else
                {
                    button_load_vr_alpha.IsEnabled = true;
                    button_load_nonvr_alpha.IsEnabled = false;
                }
            }

            //
            // Automated startup
            //
            if (sStartOption != "")
            {
                //Debug
                //System.Windows.Forms.MessageBox.Show(sStartOption);

                //Start the handler
                StartupHandler(sStartOption);

                //We're done, exit.
                System.Windows.Application.Current.Shutdown();
            }
        }

        #region Paths and SymLinks

        /// 
        /// Path selection buttons
        /// 
        private void Button_dcsdir_current_Click(object sender, RoutedEventArgs e)
        {
            string branch = "current";
            textBox_dcsdir_current.Text = DirectoryPicker(branch);
        }
        private void Button_dcsdir_alpha_Click(object sender, RoutedEventArgs e)
        {
            string branch = "alpha";
            textBox_dcsdir_alpha.Text = DirectoryPicker(branch);
        }

        private void Button_dcsdir_beta_Click(object sender, RoutedEventArgs e)
        {
            string branch = "beta";
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

            if (branch == "current")
            {
                hkcu = hkcu.OpenSubKey(@"Software\Eagle Dynamics\DCS World");
                try { sDCSpath = (string)hkcu.GetValue("Path"); }
                catch { }
            }
            else if (branch == "alpha")
            {
                hkcu = hkcu.OpenSubKey(@"Software\Eagle Dynamics\DCS World 2 OpenAlpha");
                try { sDCSpath = (string)hkcu.GetValue("Path"); }
                catch { }
            }
            else if (branch == "beta")
            {
                hkcu = hkcu.OpenSubKey(@"Software\Eagle Dynamics\DCS World OpenBeta");
                try { sDCSpath = (string)hkcu.GetValue("Path"); }
                catch { }
            }

            return sDCSpath;
        }

        /// 
        ///Get DCS Savegame folder - returns string
        /// 
        private string GetDCSSavePath(string branch)
        {
            string sDCSpath = null;
            string sUserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (branch == "current")
            {
                sDCSpath = sUserProfile + "\\Saved Games\\DCS";
            }
            else if (branch == "alpha")
            {
                sDCSpath = sUserProfile + "\\Saved Games\\DCS.openalpha";
            }
            else if (branch == "beta")
            {
                sDCSpath = sUserProfile + "\\Saved Games\\DCS.openbeta";
            }
            //System.Windows.Forms.MessageBox.Show(sDCSpath);
            return sDCSpath;
        }

        /// 
        ///Savegame folder radio button actions
        /// 
        private void RadioButton_dcsdir_current_Checked(object sender, RoutedEventArgs e)
        {
            button_linkcontrols.IsEnabled = true;
        }

        private void RadioButton_dcsdir_alpha_Checked(object sender, RoutedEventArgs e)
        {
            button_linkcontrols.IsEnabled = true;
        }

        private void RadioButton_dcsdir_beta_Checked(object sender, RoutedEventArgs e)
        {
            button_linkcontrols.IsEnabled = true;
        }

        private void Button_linkcontrols_Click(object sender, RoutedEventArgs e)
        {
            string sMasterBranch = "";
            if (radioButton_dcsdir_current.IsChecked.Value) { sMasterBranch = "Current"; }
            else if (radioButton_dcsdir_alpha.IsChecked.Value) { sMasterBranch = "Alpha"; }
            else if (radioButton_dcsdir_beta.IsChecked.Value) { sMasterBranch = "Beta"; }

            Properties.Settings.Default.sMasterBranch = sMasterBranch;
            Properties.Settings.Default.Save();
            LinkToBranch(sMasterBranch, false);

            button_unlinkcontrols.IsEnabled = true;
            button_linkcontrols.IsEnabled = false;

            radioButton_dcsdir_current.IsEnabled = false;
            radioButton_dcsdir_alpha.IsEnabled = false;
            radioButton_dcsdir_beta.IsEnabled = false;

            Properties.Settings.Default.sPathCurrent = textBox_dcsdir_current.Text;
            Properties.Settings.Default.sPathAlpha = textBox_dcsdir_alpha.Text;
            Properties.Settings.Default.sPathBeta = textBox_dcsdir_beta.Text;
            Properties.Settings.Default.Save();
        }

        private void Button_unlinkcontrols_Click(object sender, RoutedEventArgs e)
        {
            string sMasterBranch = "";
            if (radioButton_dcsdir_current.IsChecked.Value) { sMasterBranch = "Current"; }
            else if (radioButton_dcsdir_alpha.IsChecked.Value) { sMasterBranch = "Alpha"; }
            else if (radioButton_dcsdir_beta.IsChecked.Value) { sMasterBranch = "Beta"; }

            Properties.Settings.Default.sMasterBranch = "";
            Properties.Settings.Default.Save();

            radioButton_dcsdir_current.IsChecked = false;
            radioButton_dcsdir_alpha.IsChecked = false;
            radioButton_dcsdir_beta.IsChecked = false;

            radioButton_dcsdir_current.IsEnabled = true;
            radioButton_dcsdir_alpha.IsEnabled = true;
            radioButton_dcsdir_beta.IsEnabled = true;

            button_unlinkcontrols.IsEnabled = false;

            LinkToBranch(sMasterBranch, true);
        }
        /// 
        ///Get DCS Savegame folder - returns string
        /// 
        private void LinkToBranch(string sMasterBranch, bool bUnlink)
        {
            //Prepare alternative paths
            string sPathCurrent = Properties.Settings.Default.sPathCurrent + "\\config\\input";
            string sPathCurrentBackup = Properties.Settings.Default.sPathCurrent + "\\config\\input.bak";
            string sPathAlpha = Properties.Settings.Default.sPathAlpha + "\\config\\input";
            string sPathAlphaBackup = Properties.Settings.Default.sPathAlpha + "\\config\\input.bak";
            string sPathBeta = Properties.Settings.Default.sPathBeta + "\\config\\input";
            string sPathBetaBackup = Properties.Settings.Default.sPathBeta + "\\config\\input.bak";

            //Remove symlink on unlink

            //Rename current directories
            if (bUnlink == false)
            {
                if (sMasterBranch == "Current")
                {
                    if (textBox_dcsdir_alpha.Text != "Not found.")
                    {
                        MoveDirectory(sPathAlpha, sPathAlphaBackup);
                        CreateSymLink(sPathCurrent, sPathAlpha);
                    }
                    if (textBox_dcsdir_beta.Text != "Not found.")
                    {
                        MoveDirectory(sPathBeta, sPathBetaBackup);
                        CreateSymLink(sPathCurrent, sPathBeta);
                    }
                }
                else if (sMasterBranch == "Alpha")
                {
                    if (textBox_dcsdir_current.Text != "Not found.")
                    {
                        MoveDirectory(sPathCurrent, sPathCurrentBackup);
                        CreateSymLink(sPathAlpha, sPathCurrent);
                    }
                    if (textBox_dcsdir_beta.Text != "Not found.")
                    {
                        MoveDirectory(sPathBeta, sPathBetaBackup);
                        CreateSymLink(sPathAlpha, sPathBeta);
                    }
                }
                else if (sMasterBranch == "Beta")
                {
                    if (textBox_dcsdir_alpha.Text != "Not found.")
                    {
                        MoveDirectory(sPathAlpha, sPathAlphaBackup);
                        CreateSymLink(sPathBeta, sPathAlpha);
                    }
                    if (textBox_dcsdir_current.Text != "Not found.")
                    {
                        MoveDirectory(sPathCurrent, sPathCurrentBackup);
                        CreateSymLink(sPathBeta, sPathCurrent);
                    }
                }
            }
            else if (bUnlink == true)
            {
                if (sMasterBranch == "Current")
                {
                    if (textBox_dcsdir_alpha.Text != "Not found.")
                    {
                        DeleteSymLink(sPathAlpha);
                        MoveDirectory(sPathAlphaBackup, sPathAlpha);
                    }
                    if (textBox_dcsdir_beta.Text != "Not found.")
                    {
                        DeleteSymLink(sPathBeta);
                        MoveDirectory(sPathBetaBackup, sPathBeta);
                    }
                }
                else if (sMasterBranch == "Alpha")
                {
                    if (textBox_dcsdir_current.Text != "Not found.")
                    {
                        DeleteSymLink(sPathCurrent);
                        MoveDirectory(sPathCurrentBackup, sPathCurrent);
                    }
                    if (textBox_dcsdir_beta.Text != "Not found.")
                    {
                        DeleteSymLink(sPathBeta);
                        MoveDirectory(sPathBetaBackup, sPathBeta);
                    }
                }
                else if (sMasterBranch == "Beta")
                {
                    if (textBox_dcsdir_alpha.Text != "Not found.")
                    {
                        DeleteSymLink(sPathAlpha);
                        MoveDirectory(sPathAlphaBackup, sPathAlpha);
                    }
                    if (textBox_dcsdir_current.Text != "Not found.")
                    {
                        DeleteSymLink(sPathCurrent);
                        MoveDirectory(sPathCurrentBackup, sPathCurrent);
                    }
                }
            }
            
            //System.Windows.Forms.MessageBox.Show(sPathCurrent);
        }
        
        /// 
        ///Move a folder - returns void
        /// 
        static void MoveDirectory(string sSource, string sTarget)
        {
            string sourceDirectory = @sSource;
            string destinationDirectory = @sTarget;

            try
            {
                Directory.Move(sourceDirectory, destinationDirectory);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + "\nHow about you don't screw with the path?\nOkay, maybe it's my fault... JUST MAYBE\nBe so kind and open an issue on github via the support link.\nThanks!");
            }
        }

        /// 
        ///Create a symlink - returns void
        /// 
        static void CreateSymLink(string sSource, string sTarget)
        {             
            JunctionPoint.Create(sTarget, sSource, false /*don't overwrite*/);
        }
        /// 
        ///Delete a symlink - returns void
        /// 
        static void DeleteSymLink(string sTarget)
        {
            JunctionPoint.Delete(sTarget);
        }

        #endregion

        #region VR config

        //
        // CURRENT
        //
        private void Button_load_nonvr_current_Click(object sender, RoutedEventArgs e)
        {
            SwitchVRConfig("current", false);
            button_load_nonvr_current.IsEnabled = false;
            button_load_vr_current.IsEnabled = true;
        }

        private void Button_load_vr_current_Click(object sender, RoutedEventArgs e)
        {
            SwitchVRConfig("current", true);
            button_load_nonvr_current.IsEnabled = true;
            button_load_vr_current.IsEnabled = false;
        }

        //
        // ALPHA
        //
        private void Button_load_nonvr_alpha_Click(object sender, RoutedEventArgs e)
        {
            SwitchVRConfig("alpha", false);
            button_load_nonvr_alpha.IsEnabled = false;
            button_load_vr_alpha.IsEnabled = true;
        }

        private void Button_load_vr_alpha_Click(object sender, RoutedEventArgs e)
        {
            SwitchVRConfig("alpha", true);
            button_load_nonvr_alpha.IsEnabled = true;
            button_load_vr_alpha.IsEnabled = false;
        }

        //
        // BETA
        //
        private void Button_load_nonvr_beta_Click(object sender, RoutedEventArgs e)
        {
            SwitchVRConfig("beta", false);
            button_load_nonvr_beta.IsEnabled = false;
            button_load_vr_beta.IsEnabled = true;
        }

        private void Button_load_vr_beta_Click(object sender, RoutedEventArgs e)
        {
            SwitchVRConfig("beta", true);
            button_load_nonvr_beta.IsEnabled = true;
            button_load_vr_beta.IsEnabled = false;
        }

        //
        // Switch VR configuration
        //
        private void SwitchVRConfig(string sBranch, bool bVR)
        {
            //Prepare alternative paths
            string sPathCurrent = Properties.Settings.Default.sPathCurrent + "\\config\\options.lua";
            string sPathCurrentnonVR = Properties.Settings.Default.sPathCurrent + "\\config\\options.lua.nonvr";
            string sPathCurrentVR = Properties.Settings.Default.sPathCurrent + "\\config\\options.lua.vr";
            string sPathAlpha = Properties.Settings.Default.sPathAlpha + "\\config\\options.lua";
            string sPathAlphanonVR = Properties.Settings.Default.sPathAlpha + "\\config\\options.lua.nonvr";
            string sPathAlphaVR = Properties.Settings.Default.sPathAlpha + "\\config\\options.lua.vr";
            string sPathBeta = Properties.Settings.Default.sPathBeta + "\\config\\options.lua";
            string sPathBetanonVR = Properties.Settings.Default.sPathBeta + "\\config\\options.lua.nonvr";
            string sPathBetaVR = Properties.Settings.Default.sPathBeta + "\\config\\options.lua.vr";

            if(!bVR)
            {
                if(sBranch == "current")
                {
                    MoveFile(sPathCurrent, sPathCurrentVR, sBranch);
                    MoveFile(sPathCurrentnonVR, sPathCurrent, sBranch);
                    Properties.Settings.Default.bVRConfActiveCurrent = false;
                    Properties.Settings.Default.bFirstUseVRCurrent = false;
                }
                else if(sBranch == "alpha")
                {
                    MoveFile(sPathAlpha, sPathAlphaVR, sBranch);
                    MoveFile(sPathAlphanonVR, sPathAlpha, sBranch);
                    Properties.Settings.Default.bVRConfActiveAlpha = false;
                    Properties.Settings.Default.bFirstUseVRAlpha = false;
                }
                else if (sBranch == "beta")
                {
                    MoveFile(sPathBeta, sPathBetaVR, sBranch);
                    MoveFile(sPathBetanonVR, sPathBeta, sBranch);
                    Properties.Settings.Default.bVRConfActiveBeta = false;
                    Properties.Settings.Default.bFirstUseVRBeta = false;
                }
            }
            else if(bVR)
            {
                if (sBranch == "current")
                {
                    MoveFile(sPathCurrent, sPathCurrentnonVR, sBranch);
                    MoveFile(sPathCurrentVR, sPathCurrent, sBranch);
                    Properties.Settings.Default.bVRConfActiveCurrent = true;
                    Properties.Settings.Default.bFirstUseVRCurrent = false;
                }
                else if (sBranch == "alpha")
                {
                    MoveFile(sPathAlpha, sPathAlphanonVR, sBranch);
                    MoveFile(sPathAlphaVR, sPathAlpha, sBranch);
                    Properties.Settings.Default.bVRConfActiveAlpha = true;
                    Properties.Settings.Default.bFirstUseVRAlpha = false;
                }
                else if (sBranch == "beta")
                {
                    MoveFile(sPathBeta, sPathBetanonVR, sBranch);
                    MoveFile(sPathBetaVR, sPathBeta, sBranch);
                    Properties.Settings.Default.bVRConfActiveBeta = true;
                    Properties.Settings.Default.bFirstUseVRBeta = false;
                }
            }
            else { System.Windows.Forms.MessageBox.Show("WTF happened here?! Please tell the coder!", "Watch EKRAN"); }

            Properties.Settings.Default.Save();
        }


        //
        // Move file
        //
        private void MoveFile(string sSource, string sTarget, string sBranch)
        {
            try
            {
                if ((sBranch.Contains("current") & Properties.Settings.Default.bFirstUseVRCurrent & !sSource.Contains("vr")) | (sBranch.Contains("alpha") & Properties.Settings.Default.bFirstUseVRAlpha & !sSource.Contains("vr")) | (sBranch.Contains("beta") & Properties.Settings.Default.bFirstUseVRBeta & !sSource.Contains("vr")))
                {
                    File.Copy(sSource, sTarget);
                }
                else
                {
                    File.Move(sSource, sTarget);
                }                
            }
            catch (Exception e)
            {
                if((sBranch.Contains("current") & Properties.Settings.Default.bFirstUseVRCurrent) | (sBranch.Contains("alpha") & Properties.Settings.Default.bFirstUseVRAlpha) | (sBranch.Contains("beta") & Properties.Settings.Default.bFirstUseVRBeta))
                {
                    //hmm
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(e.Message + "\nHow about you don't screw with the path?\nOkay, maybe it's my fault... JUST MAYBE\nBe so kind and open an issue on github via the support link.\nThanks!");
                }
                
            }
     
        }
        #endregion

        //
        // Startup option handler
        //
        private void StartupHandler(string sStartOption)
        {
            //Possible option:
            //current, currentvr, alpha, alphavr, beta, betavr

            if (sStartOption == "current")
            {
                if (Properties.Settings.Default.bVRConfActiveCurrent)
                {
                    SwitchVRConfig("current", false);
                }
                DCSStarter("current");
            }
            else if (sStartOption == "alpha")
            {
                if (Properties.Settings.Default.bVRConfActiveAlpha)
                {
                    SwitchVRConfig("alpha", false);
                }
                DCSStarter("alpha");
            }
            else if (sStartOption == "beta")
            {
                if (Properties.Settings.Default.bVRConfActiveBeta)
                {
                    SwitchVRConfig("beta", false);
                }
                DCSStarter("beta");
            }
            else if (sStartOption == "currentvr")
            {
                if (!Properties.Settings.Default.bVRConfActiveCurrent)
                {
                    SwitchVRConfig("current", true);
                }
                DCSStarter("current");
            }
            else if (sStartOption == "alphavr")
            {
                if (!Properties.Settings.Default.bVRConfActiveAlpha)
                {
                    SwitchVRConfig("alpha", true);
                }
                DCSStarter("alpha");
            }
            else if (sStartOption == "betavr")
            {
                if (!Properties.Settings.Default.bVRConfActiveBeta)
                {
                    SwitchVRConfig("beta", true);
                }
                DCSStarter("beta");
            }
        }

        private void DCSStarter(string branch)
        {
            //Get path to game files
            if(branch == "current")
            {
                System.Windows.Forms.MessageBox.Show(GetDCSRegistryPath(branch));
            }
        }

        //
        // System message
        //

        //
        // Create shortcuts
        //

    }
}
