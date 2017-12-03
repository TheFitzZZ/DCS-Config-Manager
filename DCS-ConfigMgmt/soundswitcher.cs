using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCS_ConfigMgmt
{
    class Soundswitcher
    {
        public static List<string> GetAudioDevices(string sType)
        {
            String[] arrDevicesOutput;
            String[] arrDevicesInput;
            List<string> arrReturn = new List<string> { };
            RegistryKey hklm = Registry.LocalMachine; //HKLM Registry
            RegistryKey hklmOutput = null;
            RegistryKey hklmInput = null;
            if (sType == "output")
            {
                //Grab existing audio output device paths
                hklmOutput = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Render");
                arrDevicesOutput = hklmOutput.GetSubKeyNames();

                //Get Devicename from each one
                foreach (string sDevice in arrDevicesOutput)
                {
                    RegistryKey deviceregpath = hklmOutput.OpenSubKey(sDevice + @"\Properties");

                    string isVisible = null;
                    string isDeactivated = null;

                    try
                    {
                        isVisible = deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},0").ToString();
                        isDeactivated = deviceregpath.GetValue("{9c119480-ddc2-4954-a150-5bd240d454ad},1").ToString();
                    }
                    catch { }

                    if (isVisible == "1" & isDeactivated != null)
                    {
                        arrReturn.Add(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2") + " @ " + deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},6"));
                    }

                }
            }
            else if (sType == "input")
            {
                //Grab existing audio output device paths

                hklmInput = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Capture");
                arrDevicesInput = hklmInput.GetSubKeyNames();

                //Get Devicename from each one
                foreach (string sDevice in arrDevicesInput)
                {
                    RegistryKey deviceregpath = hklmInput.OpenSubKey(sDevice + @"\Properties");

                    string isVisible = null;
                    string isDeactivated = null;

                    try
                    {
                        isVisible = deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},0").ToString();
                        isDeactivated = deviceregpath.GetValue("{9c119480-ddc2-4954-a150-5bd240d454ad},1").ToString();
                    }
                    catch { }

                    if (isVisible == "1" & isDeactivated != null)
                    {
                        arrReturn.Add(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2") + " @ " + deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},6"));
                    }
                }
            }
            return arrReturn;
        }

        public static void Initialize()
        {
            
        }


        //Check for identical named soud devices and number them to allow proper change
        public static void FixSoundNames()
        {
            String[] arrDevicesOutput;
            String[] arrDevicesInput;
            List<string> arrNames = new List<string> { };
            RegistryKey hklm = Registry.LocalMachine; //HKLM Registry
            RegistryKey hklmOutput = null;
            RegistryKey hklmInput = null;
            
            //Grab existing audio output device paths
            hklmOutput = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Render");
            arrDevicesOutput = hklmOutput.GetSubKeyNames();

            //Get Devicename from each one
            foreach (string sDevice in arrDevicesOutput)
            {
                RegistryKey deviceregpath = hklmOutput.OpenSubKey(sDevice + @"\Properties");

                string isVisible = null;
                string isDeactivated = null;

                try
                {
                    isVisible = deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},0").ToString();
                    isDeactivated = deviceregpath.GetValue("{9c119480-ddc2-4954-a150-5bd240d454ad},1").ToString();
                }
                catch { }

                if (isVisible == "1" & isDeactivated != null)
                {
                    if (!arrNames.Contains(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString())) {
                        //Name is fine, add it to the list
                        //System.Windows.Forms.MessageBox.Show(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString() + " added to list");
                        arrNames.Add(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString());
                    }
                    else
                    {
                        //Name is duplicated, we change it and add the new one to the list
                        //deviceregpath.SetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2", "{a45c254e-df1c-4efd-8020-67d146a850e0},2" + "-");
                        //arrNames.Add(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString() + "-");
                        System.Windows.Forms.MessageBox.Show("Duplicate sound device name detected. Please rename it yourself.\nJust right-click the speaker icon in the traybar and select either playback or recording devices. Doubleclick the device in question and give it a new name.\n" + deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString());

                    }

                }

            }
            
            
            //Grab existing audio output device paths

            hklmInput = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Capture");
            arrDevicesInput = hklmInput.GetSubKeyNames();

            //Get Devicename from each one
            foreach (string sDevice in arrDevicesInput)
            {
                RegistryKey deviceregpath = hklmInput.OpenSubKey(sDevice + @"\Properties");

                string isVisible = null;
                string isDeactivated = null;

                try
                {
                    isVisible = deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},0").ToString();
                    isDeactivated = deviceregpath.GetValue("{9c119480-ddc2-4954-a150-5bd240d454ad},1").ToString();
                }
                catch { }

                if (isVisible == "1" & isDeactivated != null)
                {
                    if (!arrNames.Contains(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString()))
                    {
                        //Name is fine, add it to the list
                        //System.Windows.Forms.MessageBox.Show(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString() + " added to list");
                        arrNames.Add(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString());
                    }
                    else
                    {
                        //Name is duplicated, we change it and add the new one to the list
                        //deviceregpath.SetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2", "{a45c254e-df1c-4efd-8020-67d146a850e0},2" + "-");
                        //arrNames.Add(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString() + "-");
                        System.Windows.Forms.MessageBox.Show("Duplicate sound device name detected. Please rename it yourself.\n\nJust right-click the speaker icon in the traybar and select either playback or recording devices. Doubleclick the device in question and give it a new name.\n\nDuplicate name:" + deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2").ToString());
                        System.Windows.Forms.MessageBox.Show("Sorry for nagging - but also restart the app after renaming them, thanks :-)");
                    }

                }
            }
        }


        //Starter for Nircmd
        public static void ChangeSoundDevice(string sName)
        {
            //log.Debug("ChangeSoundDevice called with " + sName);

            if (sName != "" & sName != null)
            {


                //Strip name to use (e.g. USB Audio (HTC Vive) @ USB Audio Device)
                sName = sName.Substring(0, sName.IndexOf("@"));
                sName = sName.TrimEnd();


                string sNirDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\ExternalTools";
                try
                {
                    //log.Info("Starting DCS with " + GetDCSRegistryPath(branch) + "\\bin\\dcs_updater.exe");
                    //System.Windows.Forms.MessageBox.Show(sNirDir + "\\nircmdc.exe" + @"setdefaultsounddevice """ + sName + @"""");
                    Process.Start(sNirDir + "\\nircmdc.exe", @"setdefaultsounddevice """ + sName + @"""");
                }
                catch { }
            }
        }

        public static string GetStandardSoundDevice(string sType)
        {
            string sStandardDevice = null;
            int iDeviceNoIn = 0;
            int iDeviceNoOut = 0;
            String[] arrDevicesOutput;
            String[] arrDevicesInput;
            RegistryKey hklm = Registry.LocalMachine; //HKLM Registry
            RegistryKey hklmOutput = null;
            RegistryKey hklmInput = null;
            if (sType == "output")
            {
                //Grab existing audio output device paths
                hklmOutput = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Render");
                arrDevicesOutput = hklmOutput.GetSubKeyNames();

                //Get Devicename from each one
                foreach (string sDevice in arrDevicesOutput)
                {
                    RegistryKey deviceregpath = hklmOutput.OpenSubKey(sDevice);
                    try
                    {
                        if (Int32.Parse(deviceregpath.GetValue("Level:2").ToString()) > iDeviceNoOut)
                        {
                            iDeviceNoOut = Int32.Parse(deviceregpath.GetValue("Level:2").ToString());
                            deviceregpath = hklmOutput.OpenSubKey(sDevice + @"\Properties");
                            sStandardDevice = deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2") + " @ " + deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},6");
                        }
                    }
                    catch { }
                }
            }
            else if (sType == "input")
            {
                //Grab existing audio output device paths
                hklmInput = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Capture");
                arrDevicesInput = hklmInput.GetSubKeyNames();

                //Get Devicename from each one
                foreach (string sDevice in arrDevicesInput)
                {
                    RegistryKey deviceregpath = hklmInput.OpenSubKey(sDevice);
                    try
                    {
                        if (Int32.Parse(deviceregpath.GetValue("Level:2").ToString()) > iDeviceNoIn)
                        {
                            iDeviceNoIn = Int32.Parse(deviceregpath.GetValue("Level:2").ToString());
                            deviceregpath = hklmInput.OpenSubKey(sDevice + @"\Properties");
                            sStandardDevice = deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2") + " @ " + deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},6");
                        }
                    }
                    catch { }
                }
            }

            return sStandardDevice;
        }
    }
}
