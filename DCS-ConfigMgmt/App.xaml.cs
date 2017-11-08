using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
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
                                                      
        //NLog integration
        public static Logger log = LogManager.GetCurrentClassLogger();

        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            log.Info("Application started");


            //Detect if another instance is running
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                log.Fatal("Application already running. Shutting down.");
                MessageBox.Show("Application already running. Only one instance of this application is allowed! Please close the app before using the shortcuts!", "WATCH EKRAN!");
                System.Windows.Application.Current.Shutdown();
            }


            // Process command line args
            string sStartOption = "";
            
            if (e.Args.Length != 0)
            {
                sStartOption = e.Args[0];
                log.Info("Used paramter = " + sStartOption);
            }
            else
            {
                log.Info("No parameter given.");
            }

            
            //Debug
            //sStartOption = "current";

            if (sStartOption == "")
            {
                CopyApp();
                log.Debug("Showing main window");
                MainWindow mainWindow = new MainWindow(sStartOption);
                mainWindow.Show();
                //splash.Close();
            }
            else
            {
                log.Debug("Showing splash window");
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
                log.Debug("Sleeping for 4 seconds");
                System.Threading.Thread.Sleep(4000);
                splash.Close();
            }
        }


        //
        // Dirty workaround to overcome issues introduced by ClickOnce updating - copy the currently used assembly into a well known directory so that shortcuts always point to the same target
        //
        void CopyApp()
        {
            log.Debug("Starting clone copy");

            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr\\";
            string appDir = AppDomain.CurrentDomain.BaseDirectory;

            log.Info("Clone directory = " + deskDir);
            log.Info("App directory = " + appDir);

            try
            {
                log.Debug("Trying clone copy");
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DCSConfMgr");

                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(appDir, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(appDir, deskDir));
                
                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(appDir, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(appDir, deskDir), true);
            }
            catch (Exception e)
            {
                log.Fatal("Failed clone copy: " + e.Message);
                System.Windows.Forms.MessageBox.Show(e.Message + "\nWatch EKRAN!\n Couldn't copy program data to AppData. Please report on my GitHub page, thanks!");
            }
        }
    }
}
