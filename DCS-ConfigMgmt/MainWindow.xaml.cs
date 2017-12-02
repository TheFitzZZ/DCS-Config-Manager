using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MahApps.Metro.Controls;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using System.Configuration;
using NLog;

namespace DCS_ConfigMgmt
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : MetroWindow //Window
    {
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [DllImport("kernel32.dll")]

        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        //NLog integration
        public static Logger log = LogManager.GetCurrentClassLogger();

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        public MainWindow(string sStartOption)
        {
            InitializeComponent();

            log.Debug("Main windows initializing and loading config");

            //Get global config if available
            CopyConfig("load");
            Properties.Settings.Default.Reload();

            //Logging out current config
            log.Info("PathCurrent = " + Properties.Settings.Default.sPathCurrent);
            log.Info("PathAlpha = " + Properties.Settings.Default.sPathAlpha);
            log.Info("PathBeta = " + Properties.Settings.Default.sPathBeta);
            log.Info("sMasterBranch = " + Properties.Settings.Default.sMasterBranch); 
            log.Info("bSawConfigWarning = " + Properties.Settings.Default.bSawConfigWarning);
            log.Info("bVRConfActiveCurrent = " + Properties.Settings.Default.bVRConfActiveCurrent);
            log.Info("bVRConfActiveAlpha = " + Properties.Settings.Default.bVRConfActiveAlpha);
            log.Info("bVRConfActiveBeta = " + Properties.Settings.Default.bVRConfActiveBeta);
            log.Info("bFirstUseVRCurrent = " + Properties.Settings.Default.bFirstUseVRCurrent);
            log.Info("bFirstUseVRAlpha = " + Properties.Settings.Default.bFirstUseVRAlpha);
            log.Info("bFirstUseVRBeta = " + Properties.Settings.Default.bFirstUseVRBeta);
            log.Info("bManualPathCurrent = " + Properties.Settings.Default.bManualPathCurrent);
            log.Info("bManualPathAlpha = " + Properties.Settings.Default.bManualPathAlpha);
            log.Info("bManualPathBeta = " + Properties.Settings.Default.bManualPathBeta);
            log.Info("bManualPathBeta = " + Properties.Settings.Default.sSoundInput);
            log.Info("bManualPathBeta = " + Properties.Settings.Default.sSoundInputRevert);
            log.Info("bManualPathBeta = " + Properties.Settings.Default.sSoundOutput);
            log.Info("bManualPathBeta = " + Properties.Settings.Default.sSoundOutputRevert);

            //// Prefilling all DCS directories
            // Get DCS directories if settings are empty
            if (Properties.Settings.Default.sPathCurrent == "" | Properties.Settings.Default.sPathCurrent == "Not found.")
            {
                Properties.Settings.Default.sPathCurrent = GetDCSSavePath("current");
                log.Info("sPathCurrent is now = " + Properties.Settings.Default.sPathCurrent);
            }

            if (Properties.Settings.Default.sPathAlpha == "" | Properties.Settings.Default.sPathAlpha == "Not found.")
            {
                Properties.Settings.Default.sPathAlpha = GetDCSSavePath("alpha");
                log.Info("sPathAlpha is now = " + Properties.Settings.Default.sPathAlpha);
            }

            if (Properties.Settings.Default.sPathBeta == "" | Properties.Settings.Default.sPathBeta == "Not found.")
            {
                Properties.Settings.Default.sPathBeta = GetDCSSavePath("beta");
                log.Info("sPathBeta is now = " + Properties.Settings.Default.sPathBeta);
            }
            Properties.Settings.Default.Save();
            CopyConfig("save");

            //Write textboxes
            if (GetDCSRegistryPath("current") != null | Properties.Settings.Default.bManualPathCurrent)
            {
                textBox_dcsdir_current.Text = Properties.Settings.Default.sPathCurrent;
            }
            else
            {
                textBox_dcsdir_current.Text = "Not found.";
                radioButton_dcsdir_current.IsEnabled = false;
                button_load_nonvr_current.IsEnabled = false;
                button_load_vr_current.IsEnabled = false;
                buttonCreate_Shortcut_Current.IsEnabled = false;
            }

            if (GetDCSRegistryPath("alpha") != null | Properties.Settings.Default.bManualPathAlpha)
            {
                    textBox_dcsdir_alpha.Text = Properties.Settings.Default.sPathAlpha;
            }
            else
            {
                textBox_dcsdir_alpha.Text = "Not found.";
                radioButton_dcsdir_alpha.IsEnabled = false;
                button_load_nonvr_alpha.IsEnabled = false;
                button_load_vr_alpha.IsEnabled = false;
                buttonCreate_Shortcut_Alpha.IsEnabled = false;
            }

            if (GetDCSRegistryPath("beta") != null | Properties.Settings.Default.bManualPathBeta)
            {
                textBox_dcsdir_beta.Text = Properties.Settings.Default.sPathBeta;
            }
            else
            {
                textBox_dcsdir_beta.Text = "Not found.";
                radioButton_dcsdir_beta.IsEnabled = false;
                button_load_nonvr_beta.IsEnabled = false;
                button_load_vr_beta.IsEnabled = false;
                buttonCreate_Shortcut_Beta.IsEnabled = false;
            }

            //check for current branch for linking
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
            else
            {
                //Should only do stuff when app got reinstalled / reset
                
                //Paths to check
                string sPathCurrent = Properties.Settings.Default.sPathCurrent + "\\config\\input";
                string sPathAlpha = Properties.Settings.Default.sPathAlpha + "\\config\\input";
                string sPathBeta = Properties.Settings.Default.sPathBeta + "\\config\\input";

                string sMasterBranch = "";

                log.Info("Checking for junctions...");

                if (JunctionPoint.Exists(sPathAlpha) & JunctionPoint.Exists(sPathBeta))
                {
                    sMasterBranch = "Current";
                    radioButton_dcsdir_current.IsChecked = true;
                    log.Info("Junction found at " + sMasterBranch);
                }
                else if (JunctionPoint.Exists(sPathBeta) & JunctionPoint.Exists(sPathCurrent))
                {
                    sMasterBranch = "Alpha";
                    radioButton_dcsdir_alpha.IsChecked = true;
                    log.Info("Junction found at " + sMasterBranch);
                }
                else if (JunctionPoint.Exists(sPathAlpha) & JunctionPoint.Exists(sPathCurrent))
                {
                    sMasterBranch = "Beta";
                    radioButton_dcsdir_beta.IsChecked = true;
                    log.Info("Junction found at " + sMasterBranch);
                }

                //If nothing got detected, go to square one
                if (sMasterBranch != "")
                {
                    Properties.Settings.Default.sMasterBranch = sMasterBranch;
                    Properties.Settings.Default.Save();
                    log.Info("Saving new junction information");
                    CopyConfig("save");

                    button_unlinkcontrols.IsEnabled = true;
                    button_linkcontrols.IsEnabled = false;
                    radioButton_dcsdir_current.IsEnabled = false;
                    radioButton_dcsdir_alpha.IsEnabled = false;
                    radioButton_dcsdir_beta.IsEnabled = false;
                }
            }

            //Check configs for VR/nonVR and disable buttons accordingly
            if (!Properties.Settings.Default.bFirstUseVRCurrent & Properties.Settings.Default.sPathCurrent != "Not found.")
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
            else if(Properties.Settings.Default.bFirstUseVRCurrent & Properties.Settings.Default.sPathCurrent != "Not found.")
            {
                CheckOptionsLua("current");
            }

            if (!Properties.Settings.Default.bFirstUseVRBeta & Properties.Settings.Default.sPathBeta != "Not found.")
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
            else if (Properties.Settings.Default.bFirstUseVRBeta & Properties.Settings.Default.sPathBeta != "Not found.")
            {
                CheckOptionsLua("beta");
            }

            if (!Properties.Settings.Default.bFirstUseVRAlpha & Properties.Settings.Default.sPathAlpha != "Not found.")
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
            else if (Properties.Settings.Default.bFirstUseVRAlpha & Properties.Settings.Default.sPathAlpha != "Not found.")
            {
                CheckOptionsLua("alpha");
            }

            //Initialize soundswitcher
            Soundswitcher.Initialize();
            ListboxInput.ItemsSource = Soundswitcher.GetAudioDevices("input");
            ListboxOutput.ItemsSource = Soundswitcher.GetAudioDevices("output");
            textCurrentInput.Text = Properties.Settings.Default.sSoundInput;
            textCurrentOutput.Text = Properties.Settings.Default.sSoundOutput;

            //Debug
            //System.Windows.Forms.MessageBox.Show(Soundswitcher.GetStandardSoundDevice("input"));
            //System.Windows.Forms.MessageBox.Show(Soundswitcher.GetStandardSoundDevice("output"));
            //Soundswitcher.ChangeSoundDevice(Properties.Settings.Default.sSoundOutput);

            //Get current standard devices
            Properties.Settings.Default.sSoundInputRevert = Soundswitcher.GetStandardSoundDevice("input");
            Properties.Settings.Default.sSoundOutputRevert = Soundswitcher.GetStandardSoundDevice("output");


            //
            // Automated startup
            //
            if (sStartOption != "")
            {
                //Debug
                //System.Windows.Forms.MessageBox.Show(sStartOption);

                //Start the handler
                log.Debug("Calling startup handler");
                if (Properties.Settings.Default.sSoundOutput != "" | Properties.Settings.Default.sSoundInput != "")
                {
                    Soundswitcher.ChangeSoundDevice(Properties.Settings.Default.sSoundOutput);
                    Soundswitcher.ChangeSoundDevice(Properties.Settings.Default.sSoundInput);
                }
                StartupHandler(sStartOption);
                if (Properties.Settings.Default.sSoundOutput != "" | Properties.Settings.Default.sSoundInput != "")
                {
                    Soundswitcher.ChangeSoundDevice(Properties.Settings.Default.sSoundOutputRevert);
                    Soundswitcher.ChangeSoundDevice(Properties.Settings.Default.sSoundInputRevert);
                }

                //We're done, exit.
                log.Info("Exiting after DCS end");
                System.Windows.Application.Current.Shutdown();
            }


            //Debug calls
            //CheckOptionsLua("current");
        }

        #region Paths and SymLinks

        /// 
        /// Path selection buttons
        /// 
        private void Button_dcsdir_current_Click(object sender, RoutedEventArgs e)
        {
            log.Debug("Directory chooser clicked");
            string sTempDir = DirectoryPicker("current");
            log.Info("New directory current : " + sTempDir);
            if (sTempDir != null)
            {
                textBox_dcsdir_current.Text = sTempDir;
            }
            radioButton_dcsdir_current.IsEnabled = true;
            button_load_nonvr_current.IsEnabled = true;
            button_load_vr_current.IsEnabled = true;
            buttonCreate_Shortcut_Current.IsEnabled = true;
            Properties.Settings.Default.sPathCurrent = textBox_dcsdir_current.Text;
            Properties.Settings.Default.bManualPathCurrent = true;
            Properties.Settings.Default.Save();
            CopyConfig("save");
        }
        private void Button_dcsdir_alpha_Click(object sender, RoutedEventArgs e)
        {
            log.Debug("Directory chooser clicked");
            string sTempDir = DirectoryPicker("alpha");
            log.Info("New directory alpha : " + sTempDir);
            if (sTempDir != null)
            {
                textBox_dcsdir_alpha.Text = sTempDir;
            }
            radioButton_dcsdir_alpha.IsEnabled = true;
            button_load_nonvr_alpha.IsEnabled = true;
            button_load_vr_alpha.IsEnabled = true;
            buttonCreate_Shortcut_Alpha.IsEnabled = true;
            Properties.Settings.Default.sPathAlpha = textBox_dcsdir_alpha.Text;
            Properties.Settings.Default.bManualPathAlpha = true;
            Properties.Settings.Default.Save();
            CopyConfig("save");
        }
        private void Button_dcsdir_beta_Click(object sender, RoutedEventArgs e)
        {
            log.Debug("Directory chooser clicked");
            string sTempDir = DirectoryPicker("beta");
            log.Info("New directory beta : " + sTempDir);
            if (sTempDir != null)
            {
                textBox_dcsdir_beta.Text = sTempDir;
            }
            radioButton_dcsdir_beta.IsEnabled = true;
            button_load_nonvr_beta.IsEnabled = true;
            button_load_vr_beta.IsEnabled = true;
            buttonCreate_Shortcut_Beta.IsEnabled = true;
            Properties.Settings.Default.sPathBeta = textBox_dcsdir_beta.Text;
            Properties.Settings.Default.bManualPathBeta = true;
            Properties.Settings.Default.Save();
            CopyConfig("save");
        }


        //Remove buttons
        private void Button_dcsdir_current_remove_Click(object sender, RoutedEventArgs e)
        {
            log.Debug("Directory remover clicked current");
            textBox_dcsdir_current.Text = "Not found.";
            radioButton_dcsdir_current.IsEnabled = false;
            button_load_nonvr_current.IsEnabled = false;
            button_load_vr_current.IsEnabled = false;
            buttonCreate_Shortcut_Current.IsEnabled = false;
            Properties.Settings.Default.sPathCurrent = textBox_dcsdir_current.Text;
            Properties.Settings.Default.bManualPathCurrent = false;
            Properties.Settings.Default.bVRConfActiveCurrent = false;
            Properties.Settings.Default.Save();
            CopyConfig("save");
        }
        private void Button_dcsdir_alpha_remove_Click(object sender, RoutedEventArgs e)
        {
            log.Debug("Directory remover clicked alpha");
            textBox_dcsdir_alpha.Text = "Not found.";
            radioButton_dcsdir_alpha.IsEnabled = false;
            button_load_nonvr_alpha.IsEnabled = false;
            button_load_vr_alpha.IsEnabled = false;
            buttonCreate_Shortcut_Alpha.IsEnabled = false;
            Properties.Settings.Default.sPathAlpha = textBox_dcsdir_alpha.Text;
            Properties.Settings.Default.bManualPathAlpha = false;
            Properties.Settings.Default.bVRConfActiveAlpha = false;
            Properties.Settings.Default.Save();
            CopyConfig("save");
        }
        private void Button_dcsdir_beta_remove_Click(object sender, RoutedEventArgs e)
        {
            log.Debug("Directory remover clicked beta");
            textBox_dcsdir_beta.Text = "Not found.";
            radioButton_dcsdir_beta.IsEnabled = false;
            button_load_nonvr_beta.IsEnabled = false;
            button_load_vr_beta.IsEnabled = false;
            buttonCreate_Shortcut_Beta.IsEnabled = false;
            Properties.Settings.Default.sPathBeta = textBox_dcsdir_beta.Text;
            Properties.Settings.Default.bManualPathBeta = false;
            Properties.Settings.Default.bVRConfActiveBeta = false;
            Properties.Settings.Default.Save();
            CopyConfig("save");
        }

        
        /// 
        /// Path selection function - returns string
        /// 
        private string DirectoryPicker(string branch)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Please select your DCS folder (" + branch + ")"
            };
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
                catch (Exception e) { log.Warn("Request for DCS Registry Path for " + branch + " failed. " + e.Message); }
            }
            else if (branch == "alpha")
            {
                hkcu = hkcu.OpenSubKey(@"Software\Eagle Dynamics\DCS World 2 OpenAlpha");
                try { sDCSpath = (string)hkcu.GetValue("Path"); }
                catch (Exception e) { log.Warn("Request for DCS Registry Path for " + branch + " failed. " + e.Message); }
            }
            else if (branch == "beta")
            {
                hkcu = hkcu.OpenSubKey(@"Software\Eagle Dynamics\DCS World OpenBeta");
                try { sDCSpath = (string)hkcu.GetValue("Path"); }
                catch (Exception e) { log.Warn("Request for DCS Registry Path for " + branch + " failed. " + e.Message); }
            }

            log.Info("Requested DCS Registry Path for " + branch + " : " + sDCSpath);
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

            log.Info("Requested DCS Save Path for " + branch + " : " + sDCSpath);

            if (!Directory.Exists(sDCSpath))
            {
                sDCSpath = "Not found.";
                log.Warn("Requested DCS Registry Path for " + branch + " does not exist!");
            }

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
            log.Debug("Link controls clicked");

            string sMasterBranch = "";
            if (radioButton_dcsdir_current.IsChecked.Value) { sMasterBranch = "Current"; }
            else if (radioButton_dcsdir_alpha.IsChecked.Value) { sMasterBranch = "Alpha"; }
            else if (radioButton_dcsdir_beta.IsChecked.Value) { sMasterBranch = "Beta"; }

            Properties.Settings.Default.sMasterBranch = sMasterBranch;
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
            CopyConfig("save");
        }
        private void Button_unlinkcontrols_Click(object sender, RoutedEventArgs e)
        {
            log.Debug("Unlink controls clicked");

            string sMasterBranch = "";
            if (radioButton_dcsdir_current.IsChecked.Value) { sMasterBranch = "Current"; }
            else if (radioButton_dcsdir_alpha.IsChecked.Value) { sMasterBranch = "Alpha"; }
            else if (radioButton_dcsdir_beta.IsChecked.Value) { sMasterBranch = "Beta"; }

            Properties.Settings.Default.sMasterBranch = "";
            Properties.Settings.Default.Save();
            CopyConfig("save");

            radioButton_dcsdir_current.IsChecked = false;
            radioButton_dcsdir_alpha.IsChecked = false;
            radioButton_dcsdir_beta.IsChecked = false;

            if (GetDCSRegistryPath("current") != null | Properties.Settings.Default.bManualPathCurrent)
            {
                radioButton_dcsdir_current.IsEnabled = true;
            }
            else
            {
                radioButton_dcsdir_current.IsEnabled = false;                
            }

            if (GetDCSRegistryPath("alpha") != null | Properties.Settings.Default.bManualPathAlpha)
            {
                radioButton_dcsdir_alpha.IsEnabled = true;
            }
            else
            {
                radioButton_dcsdir_alpha.IsEnabled = false;
            }

            if (GetDCSRegistryPath("beta") != null | Properties.Settings.Default.bManualPathBeta)
            {
                radioButton_dcsdir_beta.IsEnabled = true;
            }
            else
            {
                radioButton_dcsdir_beta.IsEnabled = false;
            }
            
            
            button_unlinkcontrols.IsEnabled = false;

            LinkToBranch(sMasterBranch, true);
        }
        
        
        /// 
        ///Get DCS Savegame folder - returns string
        /// 
        private void LinkToBranch(string sMasterBranch, bool bUnlink)
        {
            log.Debug("Linker called for branch/command: " + sMasterBranch + " " + bUnlink.ToString());
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
            log.Debug("Move Directory called. Source: " + sSource + " Target: " + sTarget);
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
            log.Debug("Create junction point called. Source: " + sSource + " Target: " + sTarget);
            if (!JunctionPoint.Exists(sTarget))
            {
                log.Info("Creating the junctions...");
                JunctionPoint.Create(sTarget, sSource, false /*don't overwrite*/);
            }
        }
        /// 
        ///Delete a symlink - returns void
        /// 
        static void DeleteSymLink(string sTarget)
        {
            log.Debug("Delete junction point called. Target: " + sTarget);
            if (JunctionPoint.Exists(sTarget))
            {
                log.Info("Creating the junctions...");
                JunctionPoint.Delete(sTarget);
            }
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
            log.Debug("Switch VR config called with " + sBranch + " " + bVR);

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
            CopyConfig("save");
        }


        //
        // Move file
        //
        private void MoveFile(string sSource, string sTarget, string sBranch)
        {
            log.Debug("Movefile called with " + sSource + " - " + sTarget + " - " + sBranch);
            try
            {
                if ((sBranch.Contains("current") & Properties.Settings.Default.bFirstUseVRCurrent & !sSource.Contains("vr")) | (sBranch.Contains("alpha") & Properties.Settings.Default.bFirstUseVRAlpha & !sSource.Contains("vr")) | (sBranch.Contains("beta") & Properties.Settings.Default.bFirstUseVRBeta & !sSource.Contains("vr")))
                {
                    log.Debug("Copying file " + sSource + " > " + sTarget);
                    File.Copy(sSource, sTarget, true);
                }
                else
                {
                    if(File.Exists(sTarget) & !sSource.Contains("vr"))
                    {
                        log.Debug("Deleting file " + sTarget);
                        File.Delete(sTarget);

                        log.Debug("Moving file " + sSource + " > " + sTarget);
                        File.Move(sSource, sTarget);
                    }
                    else
                    {
                        log.Debug("Moving file " + sSource + " > " + sTarget);
                        File.Move(sSource, sTarget);
                    }
                }                
            }
            catch (Exception e)
            {
                if((sBranch.Contains("current") & Properties.Settings.Default.bFirstUseVRCurrent) | (sBranch.Contains("alpha") & Properties.Settings.Default.bFirstUseVRAlpha) | (sBranch.Contains("beta") & Properties.Settings.Default.bFirstUseVRBeta))
                {
                    log.Warn("Moving/copy file failed - might be expected though! " + e.Message);
                }
                else
                {
                    log.Fatal("Moving/copy file failed unexpectedly! " + e.Message + " || " + sBranch + " || " + sSource + " -> " + sTarget);
                    System.Windows.Forms.MessageBox.Show(e.Message + "\n" + sBranch + " || " + sSource + " -> " + sTarget + "\n\nHow about you don't screw with the path?\nOkay, maybe it's my fault... JUST MAYBE\nBe so kind and open an issue on github via the support link.\nThanks!");
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
            log.Debug("Startup handler called with " + sStartOption);

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

        //
        // Start DCS binary by branch
        //
        private void DCSStarter(string branch)
        {
            log.Debug("DCS starter called with " + branch);

            try
            {
                log.Info("Starting DCS with " + GetDCSRegistryPath(branch) + "\\bin\\dcs_updater.exe");

                var process = Process.Start(GetDCSRegistryPath(branch) + "\\bin\\dcs_updater.exe");
                if (Properties.Settings.Default.sSoundOutput != "" | Properties.Settings.Default.sSoundInput != "")
                {
                    process.WaitForExit();
                }
            }
            catch { }
        }

        //
        // Hyperlink opener
        //
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        //
        // Create shortcuts
        //
        private void AppShortcutToDesktop(string branch)
        {
            log.Debug("AppShortcutToDesktop called with " + branch);

            branch = branch.ToLower();

            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string appDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\DCS Configuration Manger.exe";

            log.Debug("deskDir " + deskDir);
            log.Debug("appDir " + appDir);

            string linkDesc = "Changes your DCS " + branch + " configuration to non VR and starts it";
            string linkDescVR = "Changes your DCS " + branch + " configuration to VR and starts it";

            string iconDirCommon = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\icons\\DCS_Icon_Current.ico";
            string iconDirCommonVR = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\icons\\DCS_Icon_Current_vr.ico";
            string iconDirAlpha = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\icons\\DCS_Icon_Alpha.ico";
            string iconDirAlphaVR = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\icons\\DCS_Icon_Alpha_vr.ico";
            string iconDirBeta = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\icons\\DCS_Icon_Beta.ico";
            string iconDirBetaVR = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\icons\\DCS_Icon_Beta_vr.ico";

            //string iconPath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\DCS_Icon_" + branch + ".ico";
            //string iconPathVR = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\DCS_Icon_" + branch + "_VR.ico";
            string iconPath = null;
            string iconPathVR = null;

            Int16 iconIndex = 0;
            Int16 iconIndexVR = 0;

            if (branch == "current")
            {
                //iconIndex = 6;
                //iconIndexVR = 7;
                iconPath = iconDirCommon;
                iconPathVR = iconDirCommonVR;
            }
            else if(branch == "alpha")
            {
                //iconIndex = 2;
                //iconIndexVR = 3;
                iconPath = iconDirAlpha;
                iconPathVR = iconDirAlphaVR;
            }
            else if (branch == "beta")
            {
                //iconIndex = 4;
                //iconIndexVR = 5;
                iconPath = iconDirBeta;
                iconPathVR = iconDirBetaVR;
            }

            
            string linkName = "DCS " + branch + ".lnk";
            string linkNameVR = "DCS " + branch + " VR.lnk";


            IShellLink link = (IShellLink)new ShellLink();

            // setup shortcut information nonvr
            link.SetDescription(linkDesc);
            link.SetPath(appDir);
            link.SetArguments(branch);
            link.SetIconLocation(iconPath, iconIndex);
            //link.SetIconLocation(null, iconIndex);

            // save it
            IPersistFile file = (IPersistFile)link;
            file.Save(Path.Combine(deskDir, linkName), false);

            // setup shortcut information vr
            link.SetDescription(linkDescVR);
            link.SetPath(appDir);
            link.SetArguments((branch + "vr"));
            link.SetIconLocation(iconPathVR, iconIndexVR);

            // save it
            IPersistFile fileVR = (IPersistFile)link;
            fileVR.Save(Path.Combine(deskDir, linkNameVR), false);
        }
        private void ButtonCreate_Shortcut_Current_Click(object sender, RoutedEventArgs e)
        {
            AppShortcutToDesktop("current");
        }
        private void ButtonCreate_Shortcut_Alpha_Click(object sender, RoutedEventArgs e)
        {
            AppShortcutToDesktop("alpha");
        }
        private void ButtonCreate_Shortcut_Beta_Click(object sender, RoutedEventArgs e)
        {
            AppShortcutToDesktop("beta");
        }

        //
        // Settings Tab focus event - warning for users
        //
        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            //Check for first run
            if (!Properties.Settings.Default.bSawConfigWarning)
                {
                    System.Windows.Forms.MessageBox.Show("Hey there!\n\n" +
                        "As this is your first time using this tool, please beware that the current and automatically detected setting will be fine for 99% of all players.\n\nThese settings point to the Saved Games folder -  NOT the install dir of your DCS instances!!\n\nOnly change this if you know what you are doing or risk mayor config fuckups.\n\nConcider yourself warned, pilot!", "WATCH EKRAN");

                    Properties.Settings.Default.bSawConfigWarning = true;
                    Properties.Settings.Default.Save();
                    CopyConfig("save");
            }
        }

        //
        // Stuff to do when the main window closes
        //
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Copy the settings to a known location for later pickup
            CopyConfig("save");
        }

        //
        // Function to copy the user.config between instances to preserve settings after updates and for the "clone"
        //
        private void CopyConfig(string action)
        {
            log.Debug("Copyconfig called with " + action);

            var currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            string globalConfig = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\user.config";

            log.Debug("currentconfig at " + currentConfig.FilePath);
            log.Debug("globalCOnfig at " + globalConfig);

            try
            {
                if(action == "save")
                {
                    log.Debug("Saving - Copying current config to global.");
                    File.Copy(currentConfig.FilePath, globalConfig, true);
                }
                else
                {
                    //Lazy hack lol
                    if (File.Exists(globalConfig))
                    {
                        log.Debug("Loading - Global config exists.");
                        if (!Directory.Exists(currentConfig.FilePath) & !File.Exists(currentConfig.FilePath))
                        {
                            log.Debug("App config doesn't exist - creating path.");
                            Directory.CreateDirectory(currentConfig.FilePath);
                            Directory.Delete(currentConfig.FilePath);
                        }
                        log.Debug("Copying global config to app.");
                        File.Copy(globalConfig, currentConfig.FilePath, true);
                    }
                    else { log.Debug("Global config doesn't exist - skipping."); }
                }
                
            }
            catch (Exception reeeeeeeeeeeee)
            {
                log.Fatal("Copy config failed. " + action + " || " + reeeeeeeeeeeee.Message);
                System.Windows.Forms.MessageBox.Show(reeeeeeeeeeeee.Message + "\n\nWatch EKRAN!\n\nCouldn't copy program settings from/to AppData (Action: " + action + "). Please report on my GitHub page, thanks!");
            }
        }

        //
        // LUA Interaction
        //
        private void CheckOptionsLua(string branch)
        {
            log.Debug("CheckOptionLUA Called. " + branch);
            //Prepare stuff 
            object objNull = null;
            RoutedEventArgs reeNull = null;
            int iLineToWrite = 0;
            string sLineToWrite = null;

            //Get directory
            string sLuaPath = "";
            if(branch == "current")
            {
                sLuaPath = Properties.Settings.Default.sPathCurrent + "\\config\\options.lua";
            }
            else if(branch == "alpha")
            {
                sLuaPath = Properties.Settings.Default.sPathAlpha + "\\config\\options.lua";
            }
            else if (branch == "beta")
            {
                sLuaPath = Properties.Settings.Default.sPathBeta + "\\config\\options.lua";
            }
            log.Info("sLuaPath = " + sLuaPath);

            if (File.Exists(sLuaPath))
            {
                log.Debug("Options.lua exists, going forward...");
                //Read the content
                string[] sContent = null;
                try
                {
                    sContent = File.ReadAllLines(sLuaPath);
                }
                catch (Exception e)
                {
                    log.Fatal("Reading options.lua failed");
                    PanicLUA("Something is wrong with your options.lua - " + branch + " Readline part\n\n" + e.Message, branch);
                }

                //Search for VR setting in file

                string sVRoff = "		[\"enable\"] = false,";
                string sVRon = "		[\"enable\"] = true,";

                int iLineOn = 0;
                int iLineOff = 0;

                if (sContent != null)
                {
                    log.Debug("Content exists, searching for VR options");
                    //This is a shit implementation but I am too lazy to integrate a Lua reader
                    try
                    {
                        iLineOn = Array.IndexOf(sContent, sVRon);
                        iLineOff = Array.IndexOf(sContent, sVRoff);
                        log.Debug("Line on = " + iLineOn);
                        log.Debug("Line off = " + iLineOff);
                    }
                    catch (Exception e)
                    {
                        log.Fatal("Array IndexOf crashed the ting.");
                        PanicLUA("Something is wrong with your options.lua - " + branch + " Array Index part\n\n" + e.Message, branch);
                        return;
                    }

                    if (iLineOff > iLineOn)
                    {
                        log.Info("VR was off, switching through.");
                        //Simulate button press to initiate the copying of the current config
                        switch (branch)
                        {
                            case "current":
                                Button_load_vr_current_Click(objNull, reeNull);
                                break;
                            case "alpha":
                                Button_load_vr_alpha_Click(objNull, reeNull);
                                break;
                            case "beta":
                                Button_load_vr_beta_Click(objNull, reeNull);
                                break;
                        }

                        //Set variables for writing the new option
                        iLineToWrite = iLineOff;
                        sLineToWrite = sVRon;
                    }
                    else if (iLineOff < iLineOn)
                    {
                        log.Info("VR was on, switching through.");
                        //Simulate button press to initiate the copying of the current config
                        switch (branch)
                        {
                            case "current":
                                Button_load_nonvr_current_Click(objNull, reeNull);
                                break;
                            case "alpha":
                                Button_load_nonvr_alpha_Click(objNull, reeNull);
                                break;
                            case "beta":
                                Button_load_nonvr_beta_Click(objNull, reeNull);
                                break;
                        }

                        //Set variables for writing the new option
                        iLineToWrite = iLineOn;
                        sLineToWrite = sVRoff;
                    }

                    //Write the file back with the opposite configuration

                    if (iLineToWrite != -1 & iLineToWrite != 0)
                    {
                        try
                        {
                            log.Debug("Trying to write options.lua");
                            sContent[iLineToWrite] = sLineToWrite;
                            File.WriteAllLines(sLuaPath, sContent);
                        }
                        catch (Exception e)
                        {
                            log.Fatal("Trying to write options.lua failed (writefile) || " + e.Message);
                            PanicLUA("Something is wrong with your options.lua - " + branch + " Writefile part\n\n" + e.Message, branch);
                            return;
                        }
                    }
                    else
                    {
                        log.Fatal("Trying to write options.lua failed (writeback) || " + branch);
                        PanicLUA("Something is wrong with your options.lua - " + branch + " Writeback part", branch);
                        return;
                    }


                    //Switch back to original config to avoid confusion (especially for nonVR users)
                    if (iLineOff > iLineOn)
                    {
                        log.Debug("Switching back to nonVR for " + branch);
                        switch (branch)
                        {
                            case "current":
                                Button_load_nonvr_current_Click(objNull, reeNull);
                                break;
                            case "alpha":
                                Button_load_nonvr_alpha_Click(objNull, reeNull);
                                break;
                            case "beta":
                                Button_load_nonvr_beta_Click(objNull, reeNull);
                                break;
                        }
                    }
                    else if (iLineOff < iLineOn)
                    {
                        log.Debug("Switching back to VR for " + branch);
                        switch (branch)
                        {
                            case "current":
                                Button_load_vr_current_Click(objNull, reeNull);
                                break;
                            case "alpha":
                                Button_load_vr_alpha_Click(objNull, reeNull);
                                break;
                            case "beta":
                                Button_load_vr_beta_Click(objNull, reeNull);
                                break;
                        }
                    }
                }
                else
                {
                    //log.Fatal("Options.lua not found || " + branch);
                    //Panic("Something went wrong! Your options.lua was not found! - " + branch);
                    log.Fatal("Options.lua not found but we did not panic. Resetting that directory. || " + branch);
                    PanicLUA("Options.lua not found! - " + branch + " Writeback part", branch);
                    return;
                }
            }
            else
            {
                //File wasn't found, so we think the branch isn't installed
                log.Fatal("Options.lua not found but we did not panic. Resetting that directory. || " + branch);
                PanicLUA("Options.lua not found! - " + branch + " Writeback part", branch);
                return;
            }
        }

        //
        // Reset for all user.config settings - used for debugging and whatever
        //
        private void LabelResetSettings_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Don't know why you touched this - but okay. Wish granted!", "Watch EKRAN!");
            log.Info("Reset called");
            //Delete settings & save to clone
            Properties.Settings.Default.Reset();
            CopyConfig("save");
            System.Windows.Application.Current.Shutdown();
        }

        //
        //Panic button
        //
        private void PanicLUA(string message, string branch)
        {
            log.Fatal("Panic called! " + message);

            //Prepare stuff 
            object objNull = null;
            RoutedEventArgs reeNull = null;

            //string basemessage = "PULL UP! PULL UP!\n\nWe ran into a problem here, I'm sorry for that. Please be so kind and create an issue on GitHub or reach out to me in other forms.\nI'll shut down now and reset my settings. What happened? This:\n\n";

            //System.Windows.Forms.MessageBox.Show(basemessage + message,"Watch EKRAN!");
            ////Delete settings & save to clone
            //Properties.Settings.Default.Reset();
            //CopyConfig("save");
            //Process.GetCurrentProcess().Kill();

            log.Fatal("Panic: " + message + " || " + branch);
            switch (branch)
            {
                case "current":
                    Button_dcsdir_current_remove_Click(objNull, reeNull);
                    break;
                case "alpha":
                    Button_dcsdir_alpha_remove_Click(objNull, reeNull);
                    break;
                case "beta":
                    Button_dcsdir_beta_remove_Click(objNull, reeNull);
                    break;
            }
        }


        //
        // Change active item to not confuse user
        //
        private void TabItem_GotFocus_SharedControls(object sender, RoutedEventArgs e)
        {
            textBox.Focus();
        }

        //
        //Open Lofgile
        //
        private void LabelLogfile_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            log.Debug("Open log called");

            try
            {
                string globalLog = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\DCSCM.log";
                Process.Start(globalLog);
            }
            catch { }
        }

        private void ListboxOutput_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            log.Debug("Sound Switcher selection changed for output: " + ListboxOutput.SelectedValue.ToString());
            textCurrentOutput.Text = ListboxOutput.SelectedValue.ToString();
            Properties.Settings.Default.sSoundOutput = ListboxOutput.SelectedValue.ToString();
            Properties.Settings.Default.Save();
            CopyConfig("save");
        }

        private void ListboxInput_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            log.Debug("Sound Switcher selection changed for input: " + ListboxInput.SelectedValue.ToString());
            textCurrentInput.Text = ListboxInput.SelectedValue.ToString();
            Properties.Settings.Default.sSoundInput = ListboxInput.SelectedValue.ToString();
            Properties.Settings.Default.Save();
            CopyConfig("save");
        }

        private void ButtonResetSettingsSoundSwitcher_Click(object sender, RoutedEventArgs e)
        {
            log.Debug("Sound Switcher selection reset called");
            textCurrentOutput.Text = "";
            textCurrentInput.Text = "";

            Properties.Settings.Default.sSoundInput = "";
            Properties.Settings.Default.sSoundOutput = "";
            Properties.Settings.Default.Save();
            CopyConfig("save");
        }
    }
}
