using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsExporter
{
    public class DateUtils
    {
        private static DateTime unixTimestampBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimestamp(DateTime time)
        {
            if (time.Kind != DateTimeKind.Utc)
                time = time.ToUniversalTime();
            return Convert.ToInt64((time - unixTimestampBaseTime).TotalSeconds);
        }

        public static DateTime ToDateTime(long unixTimestamp)
        {
            return unixTimestampBaseTime.AddMilliseconds(unixTimestamp).ToLocalTime();
        }
    }
}
