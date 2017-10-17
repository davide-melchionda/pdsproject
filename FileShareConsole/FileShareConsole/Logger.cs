using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Logger {

    public static bool HELLO_DEBUG = true;
    public static bool JOB_SCHEDULE_DEBUG = false;
    
    private Logger() {

    }

    public static void log(bool flag, string tolog) {
        if (flag)
            Console.Write(tolog);
    }

}

