using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Logger {

    public static bool HELLO_DEBUG = false;
    public static bool JOB_SCHEDULE_DEBUG = false;
    public static bool ZIP_DEBUG = true;
    public static bool EXCEPTION_LOGGING = false;
    
    private Logger() {

    }

    public static void log(bool flag, string tolog) {
        if (flag)
            Console.Write(tolog);
    }

}

