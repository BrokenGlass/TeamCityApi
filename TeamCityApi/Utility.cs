using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;

namespace TeamCity
{
    public class TimeOutWebClient : WebClient
    {
        public TimeSpan Timeout { get; set; }

        public TimeOutWebClient() : this(60000) { }
        public TimeOutWebClient(int seconds) : this(TimeSpan.FromSeconds(seconds))
        {
        }

        public TimeOutWebClient(TimeSpan t) 
        {
            Timeout = t;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = (int)Timeout.TotalSeconds;
            }
            return request;
        }
    }

    public static class TeamCityUtils
    {
        public static DateTime ParseTime(string s)
        {
            DateTime time;
			DateTime.TryParseExact(s, "yyyyMMdd'T'HHmmsszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
            return time;
        }
		public static string FormatTime(DateTime time)
		{
			string result = time.ToString ("yyyyMMdd'T'HHmmsszzz");
			return result;
		}
    }


	public static class DateTimeHelper
	{
		public static string ToTeamCityTime(this DateTime time)
		{
			string result = time.ToString ("yyyyMMdd'T'HHmmsszzz");
			return result;
		}
	}

}
