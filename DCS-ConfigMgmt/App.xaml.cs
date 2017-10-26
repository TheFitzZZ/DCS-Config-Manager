using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DCS_ConfigMgmt
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private const int MINIMUM_SPLASH_TIME = 3000; // Miliseconds  

        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            string sStartOption = "";
            
            if (e.Args.Length != 0)
            {
                sStartOption = e.Args[0];
            }

            if (sStartOption == "")
            {
                MainWindow mainWindow = new MainWindow(sStartOption);
                mainWindow.Show();
                //splash.Close();
            }
            else
            {
                Splash splash = new Splash();

                if (sStartOption == "current")
                {
                    splash.label_SplashWindow.Content = "Starting DCS";
                    splash.image_SplashWindow.Source = new BitmapImage(new Uri(@"Resources/DCS_Icon_Current.png", UriKind.Relative));
                }
                else if (sStartOption == "alpha")
                {
                    splash.label_SplashWindow.Content = "Starting DCS Alpha";
                    splash.image_SplashWindow.Source = new BitmapImage(new Uri(@"Resources/DCS_Icon_Alpha.png", UriKind.Relative));
                }
                else if (sStartOption == "beta")
                {
                    splash.label_SplashWindow.Content = "Starting DCS Beta";
                    splash.image_SplashWindow.Source = new BitmapImage(new Uri(@"Resources/DCS_Icon_Beta.png", UriKind.Relative));
                }
                else if (sStartOption == "currentvr")
                {
                    splash.label_SplashWindow.Content = "Starting DCS VR";
                    splash.image_SplashWindow.Source = new BitmapImage(new Uri(@"Resources/DCS_Icon_Current_VR.png", UriKind.Relative));
                }
                else if (sStartOption == "alphavr")
                {
                    splash.label_SplashWindow.Content = "Starting DCS Alpha VR";
                    splash.image_SplashWindow.Source = new BitmapImage(new Uri(@"Resources/DCS_Icon_Alpha_VR.png", UriKind.Relative));
                }
                else if (sStartOption == "betavr")
                {
                    splash.label_SplashWindow.Content = "Starting DCS Beta VR";
                    splash.image_SplashWindow.Source = new BitmapImage(new Uri(@"Resources/DCS_Icon_Beta_VR.png", UriKind.Relative));
                }

                splash.Show();
                MainWindow mainWindow = new MainWindow(sStartOption);
                System.Threading.Thread.Sleep(4000);
                splash.Close();
            }
        }
    }
}
