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


        //Check for 

        //Starter for Nircmd
        public static void ChangeSoundDevice(string sName)
        {
            //log.Debug("ChangeSoundDevice called with " + sName);


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
