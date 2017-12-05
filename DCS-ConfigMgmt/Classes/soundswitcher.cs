using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Session;

namespace DCS_ConfigMgmt
{
    class Soundswitcher
    {
        public static List<string> GetAudioDevices(string sType)
        {
            List<string> arrReturn = new List<string> { };
            CoreAudioController Controller = new CoreAudioController();

            if (sType == "output")
            {
                var devices = Controller.GetPlaybackDevices(DeviceState.Active);

                foreach (var d in devices.OrderBy(x => x.Name))
                {
                    arrReturn.Add(d.FullName);
                }
            }
            else if (sType == "input")
            {
                var devices = Controller.GetCaptureDevices(DeviceState.Active);

                foreach (var d in devices.OrderBy(x => x.Name))
                {
                    arrReturn.Add(d.FullName);
                }
            }
            return arrReturn;
        }

        public static void Initialize()
        {
            //Test();
        }
 
        public static CoreAudioDevice GetStandardSoundDevice(string sType)
        {
            CoreAudioDevice device = null;
            CoreAudioController Controller = new CoreAudioController();

            if (sType == "output")
            {
                device = Controller.DefaultPlaybackDevice;
            }
            else if (sType == "input")
            {
                device = Controller.DefaultCaptureDevice;
            }

            return device;
        }

        public static void ChangeStandardSoundDevice(CoreAudioDevice device)
        {
            CoreAudioController Controller = new CoreAudioController();
            Controller.SetDefaultDevice(device);
            Controller.SetDefaultCommunicationsDevice(device);
        }

        public static void FindSoundDeviceByName(string sType, string sName)
        {
            CoreAudioController Controller = new CoreAudioController();
            CoreAudioDevice device = null;

            if(sType == "input")
            {
                var devices = Controller.GetCaptureDevices(DeviceState.Active);
                foreach (var d in devices)
                {
                    if (d.FullName == sName)
                    {
                        device = d;
                    }
                }
            }
            else
            {
                var devices = Controller.GetDevices(DeviceState.Active);
                foreach (var d in devices)
                {
                    if (d.FullName == sName)
                    {
                        device = d;
                    }
                }
            }
            if(device != null)
            {
                ChangeStandardSoundDevice(device);
            }
        }

        public static void Test()
        {
            string test = "Mikrofon (USB Audio Device)";

            CoreAudioController Controller = new CoreAudioController();
            var devices = Controller.GetCaptureDevices(DeviceState.Active);
            
            foreach (var d in devices)
            {
                if (d.FullName == test)
                {
                    System.Windows.Forms.MessageBox.Show("Yeah");
                }
            }

            return;
            
        }
        
    }
}
