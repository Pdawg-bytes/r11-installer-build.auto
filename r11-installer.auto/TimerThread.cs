using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace r11_installer.auto
{
    public class TimerThread
    {
        public static int i;
        public static void IntAdd()
        {
            while (Frontend.IsRun == false)
            {
                i++;
                Thread.Sleep(10000);
                Console.WriteLine(i.ToString());
            }
        }
    }
}
