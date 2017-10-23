using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DCS_ConfigMgmt
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            string sStartOption = "";

            //for (int i = 0; i != e.Args.Length; ++i)
            //{
            //    if (e.Args[i] == "/StartMinimized")
            //    {
                    
            //    }
            //}

            if(e.Args.Length != 0)
            {
                sStartOption = e.Args[0];
            }

            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow(sStartOption);
            if (sStartOption == "")
            {
                mainWindow.Show();
            }
            else {
                //System.Windows.Forms.MessageBox.Show(sStartOption);
                //System.Windows.Application.Current.Shutdown();
            }


            //mainWindow.Show();
            //System.Windows.Forms.MessageBox.Show(e.Args[0]);

            
        }
    }
}
