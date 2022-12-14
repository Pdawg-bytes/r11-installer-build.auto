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
            i++;
            Task.Delay(4000);
        }
    }
}
