using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentStatusApp
{
    public static class Config
    {

        public static string Get_SQL_Connection_String()
        {
            string connection_String = ConfigurationManager.AppSettings["connection_string"];
            return connection_String;
        }
        public static int Get_Number_Of_Channels()
        {
            var number_Of_Channels = ConfigurationManager.AppSettings["Number_Of_Channels"];
            return Convert.ToInt32(number_Of_Channels);
        }

        public static TimeSpan GetTimeSpan()
        {
            return new TimeSpan(Convert.ToInt32(ConfigurationManager.AppSettings["time_interval_in_days"]),
                Convert.ToInt32(ConfigurationManager.AppSettings["time_interval_in_hours"]),
                Convert.ToInt32(ConfigurationManager.AppSettings["time_interval_in_minutes"]),
                Convert.ToInt32(ConfigurationManager.AppSettings["time_interval_in_seconds"]),
                Convert.ToInt32(ConfigurationManager.AppSettings["time_interval_in_milliseconds"])
                );
        }



    }
}
