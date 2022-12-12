using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace r11_installer.auto
{
    public class Frontend
    { 
        // Constant paths
        public const string configFile = "config.json";
        public const string logfile = "logs.txt";
        public static int InterPublic;

        public static void Main(System.String[] args)
        {
            Console.WriteLine("Welcome to the Rectify 11 Installer automatic builder! Created by Pdawg-bytes on GitHub.");
            Console.WriteLine("https://github.com/Pdawg-bytes/r11-installer-build.auto");
            Console.WriteLine("If you experience any crashes, check logs.txt inside the folder that you are running the program from.");
            Console.WriteLine("------------------------------------------------------------------------------------------------------\n");

            if (Debugger.IsAttached)
            {
                File.Delete(configFile);
            }

            if (!File.Exists(configFile))
            {
                // Backend that creates config and takes user settings
                try
                {
                    Console.WriteLine("Config file not found, creating new config file...\n");
                    File.Create(configFile);
                    Console.WriteLine("Set a time interval to pull down PRs and build (in days)");
                    int? Interval = Convert.ToInt32(Console.ReadLine());
                    InterPublic = (int)Interval;
                    Console.WriteLine("");
                    Console.WriteLine("Getting current day number of the year: Day " + DateTime.Now.DayOfYear.ToString());
                    Console.WriteLine("Calculating day of next build date");
                    Console.WriteLine("----------------------------------\n");
                    int dayYear = (int)(DateTime.Now.DayOfYear + Interval);
                    // Creates now calendar instance and calculates next build date.
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

                    Console.WriteLine("What hour would you like to trigger the check event (military hours, 1-24)?");
                    int triggerTime = Convert.ToInt32(Console.ReadLine());
                    try
                    {
                        // Checks if input hour is invalid
                        if (triggerTime > 24)
                        {
                            triggerTime = 24;
                        }
                        else if (triggerTime <= 0)
                        {
                            triggerTime = 1;
                        }
                        Console.WriteLine("");
                        Console.WriteLine("Starting check, console will update daily.");
                        Console.WriteLine("------------------------------------------\n");
                        // Creates object dailyCheck and checks if the build date is hit
                        var dailyCheck = new DailyTrigger(triggerTime);
                        var tempTrigger = new TriggerCheck();
                        tempTrigger.DailyCheckTrigger();
                        dailyCheck.OnTimeTriggered += () =>
                        {
                            var triggerclass = new TriggerCheck();
                            triggerclass.DailyCheckTrigger();
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
            else if (File.Exists(configFile))
            {
                Console.WriteLine("Config file found...reading");
                var settings = File.ReadAllTextAsync(configFile);
                Console.WriteLine("Settings read.");
            }
        }

        /*public void Save()
        {
            var config = new DataModel
            {

            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(config, options);
            Console.WriteLine("Saving JSON...");
            Console.WriteLine(config);
        }*/
    }
}
