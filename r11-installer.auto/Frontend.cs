using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows.Input;

namespace r11_installer.auto
{
    public class Frontend
    { 
        public const string configFile = "config.json";
        public const string logfile = "logs.txt";

        public static int? Interval;
        public static int triggerTime;


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
                sw.WriteLine("CRASH (While importing user32.dll)! Trace: " + ExDLL.Message);
            }

            if (!File.Exists(configFile))
            {
                Console.WriteLine("Config file not found, creating new config file...\n");

                Console.WriteLine("Set a time interval to pull down PRs and build (in days)");
                Interval = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("What hour would you like to trigger the check event (military hours, 1-24)?");
                triggerTime = Convert.ToInt32(Console.ReadLine());

                Save();
            }
            else
            {
                Console.WriteLine("Config file found... reading");
                string settings = File.ReadAllText(configFile);
                DataModel? dataModel = JsonSerializer.Deserialize<DataModel>(settings);
                Console.WriteLine($"Interval: {dataModel?.Interval}");
                Console.WriteLine($"TriggerTime: {dataModel?.TriggerTime}");

                Interval = dataModel?.Interval;
                triggerTime = (int) dataModel?.TriggerTime;
            }




            try
            {
                Console.WriteLine("");
                Console.WriteLine("Getting current day number of the year: Day " + DateTime.Now.DayOfYear.ToString());
                Console.WriteLine("Calculating day of next build date");
                Console.WriteLine("----------------------------------\n");

                int dayYear = (int)(DateTime.Now.DayOfYear + Interval);
                Calendar calendar = new GregorianCalendar();
                if (dayYear > calendar.GetDaysInYear(DateTime.Now.Year))
                {
                    Console.WriteLine("Date rolls over into next year, happy new year!");
                    int yearOver = DateTime.Now.Year;
                    DateTime buildDateOver = new DateTime(yearOver, 1, 1).AddDays(dayYear - 1);
                    Console.WriteLine("Next build date: " + buildDateOver.ToString("M/d/yyyy\n"));
                }
                else
                {
                    int year = DateTime.Now.Year;
                    DateTime buildDate = new DateTime(year, 1, 1).AddDays(dayYear - 1);
                    Console.WriteLine("Next build date: " + buildDate.ToString("M/d/yyyy\n"));
                }

                try
                {
                    if (triggerTime > 24)
                    {
                        triggerTime = 24;
                    }
                    else if (triggerTime <= 0)
                    {
                        triggerTime = 1;
                    }
                    Console.WriteLine("Starting check, console will update daily.");
                    Console.WriteLine("------------------------------------------\n");
                    var dailyCheck = new DailyTrigger(triggerTime);
                    dailyCheck.OnTimeTriggered += () =>
                    {
                        int yearCheck = DateTime.Now.Year;
                        DateTime buildDateCheck = new DateTime(yearCheck, 1, 1).AddDays(dayYear - 1);
                        string now = DateTime.Now.ToString("D");
                        string check = buildDateCheck.ToString("D");
                        int? daysLeft = Interval;
                        if (now == check)
                        {
                            Console.WriteLine("Build date hit!");
                            Console.WriteLine("Preparing for Git pull...");

                            if (!Directory.Exists("Git")) Directory.CreateDirectory("Git");
                            if (Directory.Exists("Build")) Directory.Move("Build", "Build.bak");
                            Directory.CreateDirectory("Build");

                            if (!Directory.Exists("Git\\.git")) Console.WriteLine("test");
                            // Process.Start(@"git", @"pull");



                            // TODO: If build succeeds, delete Build.bak
                            Directory.Delete("Build.bak", true);
                        }
                        else
                        {
                            Console.WriteLine("Waiting for the day...when it's finally time to build!");
                            Console.WriteLine("Days left: " + daysLeft.ToString());
                            Console.WriteLine("");
                        }
                    };
                }
                catch (Exception ExHour)
                {
                    using (StreamWriter sw = File.CreateText(logfile))
                        sw.WriteLine("CRASH! Trace: " + ExHour.Message);
                }
            }
                catch (Exception Ex)
            {
                using (StreamWriter sw = File.CreateText(logfile))
                    sw.WriteLine("CRASH! Trace: " + Ex.Message);
            }
        }

        public static void Save()
        {
            var config = new DataModel
            {
                Interval = Interval,
                TriggerTime = triggerTime
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(config, options);
            File.WriteAllText(configFile, jsonString);
            Console.WriteLine("Saving JSON...");
        }
    }
}