using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
            List<string> arrReturn = null;
            String sContent = null;
            RegistryKey hklm = Registry.LocalMachine; //HKLM Registry
            RegistryKey hklmOutput = null;
            RegistryKey hklmInput = null;

            if (sType == "output")
            {
                //Grab existing audio output device paths
                hklmInput = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Render");
                arrDevicesOutput = hklmInput.GetSubKeyNames();

                //Get Devicename from each one
                foreach (string sDevice in arrDevicesOutput)
                {
                    //System.Windows.Forms.MessageBox.Show(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Render\" + sDevice + @"\Properties");
                    RegistryKey deviceregpath = hklmInput.OpenSubKey(sDevice + @"\Properties");

                    //System.Windows.Forms.MessageBox.Show(deviceregpath.GetValueNames()[0]);
                    sContent = sContent + deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2") + " @ " + deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},6") + " \n";
                    arrReturn.Add(deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2") + " @ " + deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},6"));
                }
            }
            else if (sType == "input")
            {
                //Grab existing audio output device paths

                hklmOutput = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Capture");
                arrDevicesInput = hklmOutput.GetSubKeyNames();

                //Get Devicename from each one
                foreach (string sDevice in arrDevicesInput)
                {
                    //System.Windows.Forms.MessageBox.Show(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Render\" + sDevice + @"\Properties");
                    RegistryKey deviceregpath = hklmOutput.OpenSubKey(sDevice + @"\Properties");

                    //System.Windows.Forms.MessageBox.Show(deviceregpath.GetValueNames()[0]);
                    sContent = deviceregpath.GetValue("{a45c254e-df1c-4efd-8020-67d146a850e0},2") + " @ " + deviceregpath.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},6");
                    //arrReturn.Add("sContent.ToString()");
                    this.arrReturn.Add("something");
                }
            }

            //System.Windows.Forms.MessageBox.Show((content.Length).ToString() + " " + content[0]);
            System.Windows.Forms.MessageBox.Show(sContent);

            return arrReturn;
        }
    }
}
