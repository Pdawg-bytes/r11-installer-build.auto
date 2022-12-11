﻿using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace r11_installer.auto
{
    public class Frontend
    { 
        public const string configFile = "config.json";
        public const string logfile = "logs.txt";
        public static void Main(System.String[] args)
        {
            Console.WriteLine("Welcome to the Rectify 11 Installer automatic builder! Created by Pdawg-bytes on GitHub.");
            Console.WriteLine("https://github.com/Pdawg-bytes/r11-installer-build.auto");
            Console.WriteLine("If you experience any crashes, check logs.txt inside the folder that you are running the program from.");
            Console.WriteLine("------------------------------------------------------------------------------------------------------\n");

            // Imports user32 for bringing window to focus
            try
            {
                [DllImport("User32.dll")]
                static extern Int32 SetForegroundWindow(int hWnd);
            }
            catch (Exception ExDLL)
            {
                using (StreamWriter sw = File.CreateText(logfile))
                sw.WriteLine("CRASH! Trace: " + ExDLL.Message);
            }

            if (!File.Exists(configFile))
            {
                try
                {
                    Console.WriteLine("Config file not found, creating new config file...\n");
                    File.Create(configFile);
                    Console.WriteLine("Set a time interval to pull down PRs (in days)");
                    int? Inverval = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Getting current day number of the year: Day " + DateTime.Now.DayOfYear.ToString());
                }
                catch (Exception Ex)
                {
                    using (StreamWriter sw = File.CreateText(logfile))
                    sw.WriteLine("CRASH! Trace: " + Ex.Message);
                }
            }
            else if (File.Exists(configFile))
            {
                Console.WriteLine("Config file found...reading");
                var settings = File.ReadAllTextAsync(configFile);
                Console.WriteLine("Settings read.");
            }
        }

        public void Save()
        {
            var config = new DataModel
            {

            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(config, options);
            Console.WriteLine("Saving JSON...");
            Console.WriteLine(config);
        }
    }
}