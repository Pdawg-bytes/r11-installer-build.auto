using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace r11_installer.auto
{
    public class TriggerCheck
    {
        // Imports user32 for window handling
        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(int hWnd);

        public void DailyCheckTrigger()
        {
            int dayYear = (int)(DateTime.Now.DayOfYear + Frontend.InterPublic);
            int yearCheck = DateTime.Now.Year;
            DateTime buildDateCheck = new DateTime(yearCheck, 1, 1).AddDays(dayYear - 1);
            // Truncates exact time from date
            string now = DateTime.Now.ToString("D");
            string check = buildDateCheck.ToString("D");
            int? daysLeft = Frontend.InterPublic;
            if (now == check)
            {
                // Prep for build
                Console.WriteLine("Build date hit!");
                Console.WriteLine("Preparing for Git pull...");
                Frontend.IsRun = true;

                // Build process

            }
            else
            {
                // Updates status
                Frontend.IsRun = false;
                daysLeft--;
                Console.WriteLine("Waiting for the day...when it's finally time to build!");
                Console.WriteLine("Days left: " + daysLeft.ToString());
                Console.WriteLine("------------------------------------------------------\n");
            }
        }
    }
}
